using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;

public class Arena : Page
{
	public static Arena instance;

	public MapData mapData;

	public List<ActivePlayer> players = new List<ActivePlayer>();

	public List<Vill> vills = new List<Vill>();
	public List<Human> humans = new List<Human>();
	public List<Wolf> wolves = new List<Wolf>();

	public EntityArea entityArea;

	public DayManager dayManager;

	public FContainer frontContainer;

	public FSprite colorOverlay;

	public bool isGameOver = false;

	public Arena ()
	{
		instance = this;	

		mapData = MapGenerator.Generate();

		FSprite bgSprite = new FSprite("Arena/BG_1");
		AddChild(bgSprite);

		AddChild(entityArea = new EntityArea());
		float inset = 13;
		entityArea.bounds.x = -Config.WIDTH/2 + inset;
		entityArea.bounds.y = -Config.HEIGHT/2 + inset;
		entityArea.bounds.width = Config.WIDTH - inset*2;
		entityArea.bounds.height = Config.HEIGHT - inset*2;

		colorOverlay = new FSprite("WhiteBox");
		colorOverlay.color = new Color(1,1,1,0);
		colorOverlay.width = Config.WIDTH;
		colorOverlay.height = Config.HEIGHT;
		entityArea.overlayContainer.AddChild(colorOverlay);

		AddChild(frontContainer = new FContainer());

		dayManager = new DayManager();

		CreateBuildings();
		SetupPlayers();

		ListenForUpdate(Update);
	}

	public void CheckGameOver()
	{
		if(isGameOver) return; //already game over!

		int wolfCount = 0;
		int villCount = 0;

		foreach(var player in players)
		{
			if(player is VillagerActivePlayer)
			{
				if(!player.isDead) villCount++;
			}
			else if(player is WolfActivePlayer)
			{
				if(!player.isDead) wolfCount++;
			}
		}

		Debug.Log("check game over wolf " + wolfCount + " vill " + villCount);

		if(wolfCount == 0 || villCount == 0)
		{
			bool didWolfWin = villCount == 0;
			ShowGameOver(didWolfWin);
		}
	}

	void ShowGameOver(bool didWolfWin)
	{
		isGameOver = true;
		GameOverScreen goScreen = new GameOverScreen(didWolfWin);

		Debug.Log("Game over! wolf won?" + didWolfWin);

		if(didWolfWin)
		{
			FXPlayer.WolfWin();
		}
		else 
		{
			FXPlayer.VillWin();
		}

		AddChild(goScreen);
		goScreen.alpha = 0.0f;
		goScreen.scale = 0.4f;
		Go.to(goScreen,0.5f, new TweenConfig().alpha(1.0f).setDelay(0.5f));
		Go.to(goScreen,0.5f, new TweenConfig().scaleXY(1.0f).setDelay(0.5f).backOut());
	}
	
	override public void Start()
	{
		this.alpha = 0.0f;
		Go.to(this,1.0f,new TweenConfig().alpha(1.0f));
	}

	void CreateBuildings()
	{
		foreach(var item in mapData.items)
		{
			if(item is House1MI)
			{
				var houseMI = item as House1MI;

				House house = new House(houseMI.x,houseMI.y,entityArea);
				house.AddToArea();
			}
			else if(item is FenceHMI)
			{
				var fenceMI = item as FenceHMI;
				
				FenceH fence = new FenceH(fenceMI.x,fenceMI.y,entityArea);
				fence.AddToArea();
			}
			else if(item is FenceVMI)
			{
				var fenceMI = item as FenceVMI;
				
				FenceV fence = new FenceV(fenceMI.x,fenceMI.y,entityArea);
				fence.AddToArea();
			}
		}
	}

	void SetupPlayers()
	{
		players.Clear();

		int numWolf = 0;
		int numVill = 0;

		foreach(var player in Core.playerManager.players)
		{
			if(player.team == PlayerManager.Team_Wolves)
			{
				players.Add(new WolfActivePlayer(player));
				numWolf++;
			}
			else if(player.team == PlayerManager.Team_Villagers)
			{
				players.Add(new VillagerActivePlayer(player));
				numVill++;
			}
		}

		int totalVill = numWolf * Config.VILLAGERS_PER_WOLF;
		int villPerPlayer = totalVill/Math.Max(1,numVill);

		foreach(var player in players)
		{
			if(player is VillagerActivePlayer)
			{
				(player as VillagerActivePlayer).initialVillCount = villPerPlayer;
			}
		}

		foreach(var player in players)
		{
			player.Start();
		}
	}

	public void HitVillagersInRect(Wolf wolf, TORect rect, float hitAngle)
	{
		if(Config.SHOULD_DEBUG_BLOCKING_RECTS)
		{
			var debugSprite = entityArea.CreateDebugSprite(rect, Color.blue);
			Go.to(debugSprite, 0.5f, new TweenConfig().alpha(0.0f).removeWhenComplete());
		}

		for(int v = vills.Count-1; v>= 0; v--) //reverse for easy removal
		{
			var vill = vills[v];

			if(rect.Contains(vill.x,vill.y))
			{
				vill.Hit(wolf, hitAngle);
			}
		}
	}

	public void HitHumansInRect(Vill vill, TORect rect, float hitAngle)
	{
		if(Config.SHOULD_DEBUG_BLOCKING_RECTS)
		{
			var debugSprite = entityArea.CreateDebugSprite(rect, Color.green);
			Go.to(debugSprite, 0.5f, new TweenConfig().alpha(0.0f).removeWhenComplete());
		}
		
		for(int h = humans.Count-1; h>= 0; h--) //reverse for easy removal
		{
			var human = humans[h];
			
			if(rect.CheckIntersect(human.attackableRect))
			{
				human.Hit(vill, hitAngle);
			}
		}
	}

	void Update()
	{
		foreach(var player in players)
		{
			player.Update();
		}

		dayManager.Update();

//		foreach(var player in players)
//		{
//			if(player.player.device.MenuWasPressed)
//			{
//				Core.instance.Restart();
//			}
//		}

		foreach(var device in InputManager.Devices)
		{
			if(device.MenuWasPressed)
			{
				Core.instance.Restart();
			}
		}
	}
	
	override public void Destroy()
	{
		instance = null;
	}

	public void ShowDeadMessage(ActivePlayer player)
	{
		FContainer callout = new FContainer();
		callout.scale = 0.5f;

		MultiDualLabel fullLabel = new MultiDualLabel();
		callout.AddChild(fullLabel);
		
		DualLabel colorLabel = new DualLabel(TOFonts.MEDIUM_BOLD,player.player.color.name.ToUpper());
		colorLabel.mainLabel.color = player.player.color.color;
		colorLabel.scale = 4.0f;
		
		DualLabel messageLabel = new DualLabel(TOFonts.MEDIUM_BOLD," WAS ELIMINATED!");
		callout.AddChild(messageLabel);
		messageLabel.mainLabel.color = Color.white;
		messageLabel.scale = 4.0f;

		//smaller shadow
		colorLabel.mainLabel.y += 0.5f;
		messageLabel.mainLabel.y += 0.5f;

		fullLabel.AddLabel(colorLabel);
		fullLabel.AddLabel(messageLabel);
		fullLabel.Align(0.5f);
		
		Arena.instance.frontContainer.AddChild(callout);
		
		fullLabel.alpha = 0.0f;
		
		Go.to(fullLabel, 0.5f, new TweenConfig().alpha(1.0f));

		callout.y = Config.HEIGHT/2 - 26.0f;
		
		Go.to(callout, 3.0f, new TweenConfig().y(callout.y+5.0f).removeWhenComplete());
		Go.to(callout,0.8f,new TweenConfig().setDelay(2.2f).alpha(0.0f));
	}
}

public class ActivePlayer
{
	public Player player;
	public Arena arena;
	public bool isDead = false;

	public ActivePlayer(Player player)
	{
		this.player = player;
		arena = Arena.instance;
	}

	virtual public void Start()
	{
		
	}

	virtual public void Update()
	{

	}
}

public class WolfActivePlayer : ActivePlayer
{
	public int health;
	public Wolf wolf;
	public Human human;
	public bool isFacingLeft = true;

	public WolfActivePlayer(Player player) : base(player)
	{
		health = Config.HUMAN_MAX_HEALTH;
	}
	
	override public void Start()
	{
		var startPos = RXRandom.GetRandomItem(arena.mapData.startPoses) as StartPosMI;
		arena.mapData.startPoses.Remove(startPos);
		
		Vector2 center = new Vector2(startPos.x,startPos.y);

		human = new Human(this,arena.entityArea,false);
		human.SetPosition(center);
		human.AddToArea();
		arena.humans.Add(human);
	}

	public void DestroyHumanAndCreateWolf(Human human)
	{
		arena.humans.Remove(human);
		human.RemoveFromArea();
		this.human = null;

		wolf = new Wolf(this,arena.entityArea);
		wolf.SetPosition(human.x,human.y);
		wolf.AddToArea();
		arena.wolves.Add(wolf);
		wolf.Update();
	}

	public void DestroyWolfAndCreateHuman(Wolf wolf)
	{
		arena.wolves.Remove(wolf);
		wolf.RemoveFromArea();
		this.wolf = null;
		
		human = new Human(this,arena.entityArea,true);
		human.SetPosition(wolf.x,wolf.y);
		human.AddToArea();
		arena.humans.Add(human);
		human.Update();
	}
	
	override public void Update()
	{
		if(wolf != null)
		{
			wolf.inputDirection = player.device.LeftStick;

			if(player.device.Action1.WasPressed || player.device.Action2.WasPressed || player.device.Action3.WasPressed || player.device.Action4.WasPressed)
			{
				wolf.DoAttack();
			}
		}
		else if(human != null)
		{
			human.inputDirection = player.device.LeftStick;
			
			if(player.device.Action1.WasPressed || player.device.Action2.WasPressed || player.device.Action3.WasPressed || player.device.Action4.WasPressed)
			{
				human.DoAttack();
			}
		}
	}

	public void OnHumanDeath(Human human)
	{
		this.human = null;
		arena.humans.Remove(human);
		var cloud = new HumanDeathCloud(human);
		cloud.AddToArea();

		human.RemoveFromArea();
	
		DoDead();
	}

	void DoDead()
	{
		if(isDead) return;
		isDead = true;

		arena.ShowDeadMessage(this);

		arena.CheckGameOver();
	}
}

public class VillagerActivePlayer : ActivePlayer
{
	public int initialVillCount; //set in SetupPlayers
	public List<Vill> vills = new List<Vill>();

	public VillagerActivePlayer(Player player) : base(player)
	{

	}

	override public void Start()
	{
		var startPos = RXRandom.GetRandomItem(arena.mapData.startPoses) as StartPosMI;
		arena.mapData.startPoses.Remove(startPos);

		Vector2 center = new Vector2(startPos.x,startPos.y);
		float radius = 40;
		int failsafe = 0;

		while(vills.Count < initialVillCount && failsafe++ < 1000)
		{
			Vector2 checkPos = center + (RXRandom.Vector2Normalized() * radius * RXRandom.Float());
			if(!arena.entityArea.CheckPointHit(checkPos.x,checkPos.y))
			{
				Vill vill = new Vill(this,arena.entityArea);
				vill.SetPosition(checkPos);
				vill.AddToArea();
				vills.Add(vill);
				arena.vills.Add(vill);
			}
		}
	}

	override public void Update()
	{
		int villCount = vills.Count;

		Vector2 moveDir = player.device.LeftStick;

		bool shouldSpread = true;

		bool isHoldingButton = player.device.Action1.IsPressed || player.device.Action2.IsPressed  || player.device.Action3.IsPressed || player.device.Action4.IsPressed;

		bool shouldSpreadOut = isHoldingButton ? true : false;

		float spreadModifier = shouldSpreadOut ? 1.0f : -0.25f;

		for(int a = 0; a<villCount; a++)
		{
			Vill villA = vills[a];
			villA.hopper.inputDirection = moveDir;

			if(shouldSpread)
			{
				Vill closeVill = null;
				float closeDist = shouldSpreadOut ? float.MaxValue : float.MinValue;

				for(int b = 0; b<villCount; b++)
				{
					Vill villB = vills[b];
					if(villB == villA) continue; //can't be close to self

					float dx = villA.x - villB.x;
					float dy = villA.y - villB.y;
					float dist = dx*dx + dy*dy;

					if(shouldSpreadOut)
					{
						if(dist < closeDist)
						{
							closeDist = dist;
							closeVill = villB;
						}
					}
					else 
					{
						if(dist > closeDist)
						{
							closeDist = dist;
							closeVill = villB;
						}
					}
				}

				if(closeVill != null)
				{
					Vector2 delta = new Vector2(villA.x - closeVill.x,villA.y - closeVill.y);

					float force = 1.0f;

					if(shouldSpreadOut)
					{
						if(delta.magnitude == 0)
						{
							force = 1.0f;
						}
						else 
						{
							force = Mathf.Clamp01(7.0f / delta.magnitude);
						}
					}
					else 
					{
						force = 1.0f - Mathf.Clamp01(17.0f / delta.magnitude);
					}

					force *= force;

					villA.pushSpeed += delta.normalized * force * 0.3f * spreadModifier;
				}
			}
		}
	}

	public void OnVillDeath(Vill vill)
	{
		vills.Remove(vill);
		arena.vills.Remove(vill);
		var cloud = new VillDeathCloud(vill);
		cloud.AddToArea();

		if(vills.Count == 0)
		{
			DoDead();
		}
	}
	
	void DoDead()
	{
		if(isDead) return;
		isDead = true;

		arena.ShowDeadMessage(this);

		arena.CheckGameOver();
	}
}



