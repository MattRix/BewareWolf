using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;


public class Vill : Entity
{
	public VillagerActivePlayer player;

	public FContainer body;
	public FSprite bodySprite;
	public FSprite colorSprite;
	public FContainer weapon;
	public FSprite shadowSprite;
	public Hopper hopper;

	public float offsetY;

	public int artIndex;

	public Vector2 pushSpeed = new Vector2();

	public int health = Config.VILLAGER_MAX_HEALTH;

	public bool isDead = false;

	public float attackCooldown = 0.0f;

	public Vill(VillagerActivePlayer player, EntityArea entityArea) : base(entityArea)
	{
		this.player = player;

		attackCooldown = RXRandom.Float(); //random cooldown to start so they're not synced

		offsetY = 6f;

		int numArts = 8;
		artIndex = RXRandom.Range(0,numArts)+1;

		body = new FContainer();
		bodySprite = new FSprite("Arena/Vill"+artIndex+"_body");
		body.AddChild(bodySprite);

		colorSprite = new FSprite("Arena/Vill"+artIndex+"_color");
		colorSprite.color = player.player.color.color;
		body.AddChild(colorSprite);

		weapon = new FContainer();
		body.AddChild(weapon);

		string weaponName = RXRandom.GetRandomString("RollingPin","Torch","Pitchfork","","","",""); //Rake, FryingPan

		if(weaponName != "" && artIndex != 8)
		{
			if(weaponName == "Torch")
			{
				var torch = new VillTorch();
				weapon.AddChild(torch);
			}
			else 
			{
				FSprite weaponSprite = new FSprite("Arena/"+weaponName+"_1");
				weapon.AddChild(weaponSprite);
			}
		}


		shadowSprite = new FSprite("Arena/VillShadow");
		shadowSprite.alpha = 0.2f;
		shadowSprite.scaleX = 0.7f;
		shadowSprite.scaleY = 0.5f;

		hopper = new Hopper(this);
		hopper.config.jumpDist = RXRandom.Range(18f,19f);
		hopper.config.jumpDuration = RXRandom.Range(0.2f,0.24f);
		hopper.config.jumpHeight = RXRandom.Range(3f,4f);
	}
	
	override public void HandleAdded()
	{
		base.HandleAdded();
		entityArea.sortYContainer.AddChild(body);
		entityArea.shadowContainer.AddChild(shadowSprite);
	}
	
	override public void HandleRemoved()
	{
		base.HandleRemoved();
		body.RemoveFromContainer();
		shadowSprite.RemoveFromContainer();
		player.OnVillDeath(this);
	}

	public void Hit(Wolf wolf, float hitAngle)
	{
		if(isDead) return;

//		Vector2 offset = new Vector2(wolf.x - x,wolf.y + 10 - y).normalized * -1f;

		float rads = -hitAngle * RXMath.DTOR + Mathf.PI;
		Vector2 offset = new Vector2(Mathf.Cos(rads),Mathf.Sin(rads));

		pushSpeed += offset * 5.0f;

		hopper.jumpTime = 0.001f; //fly into the air?

		health--;

		if(health <= 0)
		{
			Die();
		}
	}

	void Die()
	{
		if(isDead) return;

		isDead = true;

		FXPlayer.VillDeath();

		RemoveFromArea();
	}
	
	override public void Update()
	{
		hopper.Update();

		attackCooldown -= Time.deltaTime;

		if(attackCooldown < 0)
		{
			attackCooldown = RXRandom.Range(0.9f,1.0f)*Config.VILLAGER_ATTACK_COOLDOWN;

			if(Arena.instance != null && Arena.instance.dayManager.isDay)
			{
				CheckForAttack();
			}
		}

		float scaleXMultiplier = body.scaleX < 0 ? -1f : 1f;
		float scaleMultiplier = 1.0f;

		if(hopper.jumpTime > 0)
		{
			scaleXMultiplier = (hopper.speedX < 0) ? -1.0f : 1.0f;
			scaleMultiplier = 0.75f + 0.3f*hopper.jumpY;
		}

		body.scaleX = scaleXMultiplier/scaleMultiplier;
		body.scaleY = scaleMultiplier;

		bodySprite.y = offsetY+hopper.jumpY*hopper.config.jumpHeight;
		colorSprite.y = bodySprite.y;
		weapon.x = bodySprite.x+4;
		weapon.y = bodySprite.y+4;

		pushSpeed *= 0.93f;

		if(pushSpeed.sqrMagnitude > 0.05f)
		{
			float newX = x + pushSpeed.x;
			float newY = y + pushSpeed.y * Config.ISO_RATIO;

			if(!entityArea.CheckVillPointHit(newX,newY))
			{
				x = newX;
				y = newY;
			}
			else if(!entityArea.CheckVillPointHit(x,newY))
			{
				pushSpeed.x = -pushSpeed.x * 0.5f;
				y = newY;
			}
			else if(!entityArea.CheckVillPointHit(newX,y))
			{
				x = newX;
				pushSpeed.y = -pushSpeed.y * 0.5f;
			}
		}

		body.SetPosition(x,y);
		shadowSprite.SetPosition(x,y);
	}

	void CheckForAttack()
	{
		List<Human>humans = Arena.instance.humans;
		int humanCount = humans.Count;

		for(int h = humanCount-1; h>=0; h--)
		{
			var human = humans[h];

			if(human.isDead || human.isTransformingToWolf || human.isTransformingFromWolf) continue; //can't attack em

			if(human.attackableRect.Contains(x,y))
			{
				DoAttack(human);
			}
		}
		
	}

	void DoAttack(Human human)
	{
		float dx = x - human.x;
		float dy = y - human.y;

		float attackBeamAngle = Mathf.Atan2(dy,dx) * RXMath.RTOD;

		pushSpeed -= new Vector2(dx,dy).normalized; //move towards human

		var attackBeam = new VillAttackBeam(this,attackBeamAngle);
		
		float beamDist = 5;
		float rads = -attackBeamAngle * RXMath.DTOR + Mathf.PI;
		Vector2 offset = new Vector2(Mathf.Cos(rads),Mathf.Sin(rads))*beamDist;
		attackBeam.x = x+offset.x;
		attackBeam.y = y+6+offset.y;
		
		attackBeam.AddToArea();
		attackBeam.Update();

		FXPlayer.VillAttack();
	}
}

public class VillTorch : FContainer
{
	public FSprite sprite;
	public FAtlasElement[] frames;

	public int frameTime = 0;
	public int frameToShow = 0;

	public VillTorch()
	{
		frameTime = RXRandom.Range(0,100);

		frames = new FAtlasElement[]
		{
			Futile.atlasManager.GetElementWithName("Arena/Torch_1"),
			Futile.atlasManager.GetElementWithName("Arena/Torch_2"),
			Futile.atlasManager.GetElementWithName("Arena/Torch_3"),
			Futile.atlasManager.GetElementWithName("Arena/Torch_4"),
			Futile.atlasManager.GetElementWithName("Arena/Torch_5"),
		};

		frameToShow = RXRandom.Range(0,frames.Length);

		sprite = new FSprite(frames[frameToShow]);
		AddChild(sprite);
		ListenForUpdate(Update);
	}

	void Update()
	{
		frameTime++;

		if(frameTime % 5 == 0)
		{
			frameToShow = (frameToShow+1)%frames.Length;
			sprite.element = frames[frameToShow];
		}
	}
}






