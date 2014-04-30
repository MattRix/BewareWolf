using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;

public class PlayerSelectPage : Page
{
	public List<TeamCol> cols = new List<TeamCol>();

	public List<PlayerRow> rows = new List<PlayerRow>();

	public DualLabel statusLabel;
	public bool isReady = false;

	public FSprite bg;

	public FSprite logo;

	public FContainer rowHolder;

	public PlayerSelectPage ()
	{
		bg = new FSprite("Arena/BG_1");
		bg.color = new Color(0.5f,0.5f,0.5f,1.0f);
		AddChild(bg);

		statusLabel = new DualLabel(TOFonts.MEDIUM_BOLD,"");
		statusLabel.scale = 1f;
		statusLabel.SetPosition(0,-Config.HEIGHT/2+27);
		AddChild(statusLabel);

		float spreadControlX = Config.WIDTH/2 - 74.0f;

		DualLabel wolfLabel = new DualLabel(TOFonts.MEDIUM_BOLD,"WOLF (night):\n\nTAP ANY BUTTON\nTO ATTACK\n\n\nHUMAN (day):\n\nRUN FOR\nYOUR LIFE");
		wolfLabel.mainLabel.color = new Color(1.0f,0.7f,0.7f);
		wolfLabel.x = -spreadControlX;
		AddChild(wolfLabel);

		DualLabel villLabel = new DualLabel(TOFonts.MEDIUM_BOLD,"VILLAGERS:\n\nRUN AWAY\nAT NIGHT\n\nHOLD ANY BUTTON\nTO SPREAD OUT\n\nCAN HOP\nOVER FENCES");
		villLabel.mainLabel.color = new Color(0.8f,0.8f,1.0f);
		villLabel.x = spreadControlX;
		AddChild(villLabel);

		logo = new FSprite("Arena/Logo");
		AddChild(logo);

		logo.y = Config.HEIGHT/2 - 43.0f;

		ListenForUpdate(Update);

		UpdateStatus();
	}
	
	override public void Start()
	{
		float spreadX = 125;
		//float startY = -TeamCol.HEIGHT/2-200;

		float colY = -20.0f;

		TeamCol col = new TeamCol(PlayerManager.Team_None);
		cols.Add(col);
		AddChild(col);
		col.y = colY;

		col = new TeamCol(PlayerManager.Team_Wolves);
		col.x = -spreadX;
		cols.Add(col);
		AddChild(col);
		col.y = colY;

		col = new TeamCol(PlayerManager.Team_Villagers);
		col.x = spreadX;
		cols.Add(col);
		AddChild(col);
		col.y = colY;

		rowHolder = new FContainer();
		rowHolder.y = colY;
		AddChild(rowHolder);

		foreach(var player in Core.playerManager.players)
		{
			HandleOnPlayerAdded(player);
		}
		


		Core.playerManager.OnPlayerAdded += HandleOnPlayerAdded;
		Core.playerManager.OnPlayerRemoved += HandleOnPlayerRemoved;

		if(Config.SHOULD_AUTO_SELECT_TEAMS || Config.SHOULD_SKIP_PLAYER_SELECT)
		{
			for(int r = 0; r<rows.Count;r++)
			{
				rows[r].player.team = r % 2 == 0 ? PlayerManager.Team_Wolves : PlayerManager.Team_Villagers;
			}
		}

		for(int r = 0; r<rows.Count;r++)
		{
			rows[r].player.team = PlayerManager.Team_None;
			rows[r].inner.x = GetColForTeam(rows[r].player.team).x;
		}

		CheckStatus();

		if(Config.SHOULD_SKIP_PLAYER_SELECT)
		{
			StartGame();
		}
	}

	void HandleOnPlayerAdded (Player player)
	{
		PlayerRow row = new PlayerRow(player);
		rowHolder.AddChild(row);
		rows.Add(row);

		UpdateRowPositions();
	}

	void HandleOnPlayerRemoved (Player player)
	{
		foreach(var row in rows)
		{
			if(row.player == player)
			{
				rows.Remove(row);
				row.RemoveFromContainer();
				break;
			}
		}

		UpdateRowPositions();
	}
	
	void UpdateRowPositions()
	{
		if(rows.Count == 0) return;

		float spreadY = 46;
		float totalHeight = spreadY * (rows.Count-1);

		for(int r = 0; r<rows.Count; r++)
		{
			float targetY = -totalHeight/2 + r*spreadY;
			Go.killAllTweensWithTarget(rows[r]);
			Go.to(rows[r], 0.4f, new TweenConfig().y(targetY).expoOut());
		}

		rowHolder.scale = Mathf.Clamp01(5.0f / (float)(rows.Count));
	}

	void Update()
	{
		bool didTeamChange = false;
		bool didPlayerPressStart = false;

		foreach(var row in rows)
		{
			var player = row.player;
			Team newTeam = null;

			if(player.team.teamToTheLeft != null && player.device.LeftStick.Left.WasPressed)
			{
				newTeam = player.team.teamToTheLeft;
			}
			else if(player.team.teamToTheRight != null && player.device.LeftStick.Right.WasPressed)
			{
				newTeam = player.team.teamToTheRight;
			}

			if(newTeam != null)
			{
				player.team = newTeam;
				TeamCol col = GetColForTeam(newTeam);
				Go.killAllTweensWithTarget(row.inner);
				Go.to(row.inner,0.3f, new TweenConfig().x(col.x).expoOut());
				didTeamChange = true;

				if(newTeam == PlayerManager.Team_Villagers)
				{
					FXPlayer.VillAttack();
				}
				else if(newTeam == PlayerManager.Team_Wolves)
				{
					FXPlayer.WolfAttack();
				}
			}

			if(player.device.MenuWasPressed)
			{
				didPlayerPressStart = true;
			}
		}

		if(didTeamChange)
		{
			CheckStatus();
		}

		if(isReady && didPlayerPressStart)
		{
			StartGame();
		}

	}

	void StartGame()
	{
		Core.instance.StartGame();
	}

	void CheckStatus()
	{
		int copCount = 0;
		int robberCount = 0;
		
		foreach(var row in rows)
		{
			if(row.player.team == PlayerManager.Team_Wolves)
			{
				copCount++;
			}
			else if(row.player.team == PlayerManager.Team_Villagers)
			{
				robberCount++;
			}
		}
		
		bool isNowReady = robberCount >= Config.MIN_PLAYERS_PER_TEAM && copCount >= Config.MIN_PLAYERS_PER_TEAM;
		
		if(!isReady && isNowReady)
		{
			isReady = true;
			UpdateStatus();
		}
		else if(isReady && !isNowReady)
		{
			isReady = false;
			UpdateStatus();
		}
	}

	void UpdateStatus()
	{
		if(isReady)
		{
			statusLabel.text = "press START to play!";
			statusLabel.mainLabel.color = Color.white;
		}
		else 
		{
			statusLabel.text = "you need villagers and werewolves...";
			statusLabel.mainLabel.color = Color.grey;
		}

		statusLabel.scale = 0.8f;
		Go.to(statusLabel,0.3f, new TweenConfig().scaleXY(1f).backOut());
	}

	public TeamCol GetColForTeam(Team team)
	{
		foreach(var col in cols)
		{
			if(col.team == team)
			{
				return col;
			}
		}
		return null;
	}
	
	override public void Destroy()
	{
		Core.playerManager.OnPlayerAdded -= HandleOnPlayerAdded;
		Core.playerManager.OnPlayerRemoved -= HandleOnPlayerRemoved;
	}
}

public class PlayerRow : FContainer
{
	public Player player;

	public FContainer inner;

	public FContainer iconContainer;

	public FSprite icon;

	public PlayerRow(Player player)
	{
		this.player = player;

		AddChild(inner = new FContainer());

		inner.AddChild(iconContainer = new FContainer());

		if(player.device.Name == "TOKeyboardProfileA")
		{
			icon = new FSprite("UI/WASDIcon");
		}
		else if(player.device.Name == "TOKeyboardProfileB")
		{
			icon = new FSprite("UI/ArrowsIcon");
		}
		else 
		{
			icon = new FSprite("UI/ControllerIcon");
		}


		icon.color = player.color.color;
		iconContainer.AddChild(icon);

		iconContainer.scale = 0;
		Go.to(iconContainer, 0.4f, new TweenConfig().scaleXY(1.0f).backOut());

		ListenForUpdate(Update);
	}

	void Update()
	{
		if(player.device.Action1.WasPressed)
		{
			icon.scale = 0.9f; 
			Go.to(icon,0.3f, new TweenConfig().scaleXY(1.0f).backOut());
		}
	}
}

public class TeamCol : FContainer
{
	static public float WIDTH = 125;
	static public float HEIGHT = 250;

	public Team team;
	public DualLabel label;
	public RoundedRect bgRect;

	public TeamCol(Team team)
	{
		this.team = team;

		label = new DualLabel(TOFonts.MEDIUM_BOLD,team.name.ToUpper());
		label.alpha = 0.5f;
		label.scale = 1.5f;
		AddChild(label);

		bool isNone = team == PlayerManager.Team_None;

		bgRect = new RoundedRect(WIDTH,HEIGHT,!isNone);
		AddChild(bgRect);

		if(isNone)
		{
			bgRect.width -= 25;
			bgRect.alpha = 0.2f;
		}

		label.mainLabel.color = team.color;
		bgRect.color = team.color;
	}
}

