using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;

public class PlayerManager
{
	public static Team Team_None;
	public static Team Team_Wolves;
	public static Team Team_Villagers;
	
	public List<Player> players = new List<Player>();

	public List<PlayerColor> availableColors = new List<PlayerColor>();
	
	public List<Team> teams = new List<Team>();

	public event Action<Player> OnPlayerAdded;
	public event Action<Player> OnPlayerRemoved;
	
	public PlayerManager()
	{

	}

	public void Setup()
	{
		teams.Add(Team_None = new Team("", new Color(1,1,1,0.5f)));
		teams.Add(Team_Wolves = new Team("Werewolves", new Color(1,0,0,1)));
		teams.Add(Team_Villagers = new Team("Villagers", new Color(0,0.2f,1,1)));

		Team_None.teamToTheLeft = Team_Wolves;
		Team_None.teamToTheRight = Team_Villagers;

		Team_Wolves.teamToTheRight = Team_None;

		Team_Villagers.teamToTheLeft = Team_None;
		
		availableColors.Add(new PlayerColor("Red",0xFF0022));
		availableColors.Add(new PlayerColor("Blue",0x0033FF));
		availableColors.Add(new PlayerColor("Green",0x33FF00));
		availableColors.Add(new PlayerColor("Yellow",0xFFEE00));
		availableColors.Add(new PlayerColor("Purple",0xDD00FF));
		availableColors.Add(new PlayerColor("Orange",0xFF8800));
		availableColors.Add(new PlayerColor("Cyan",0x00FFFF));
		availableColors.Add(new PlayerColor("Pink",0xFF66DD));
		
		InputManager.Setup();

		if(Config.SHOULD_ADD_KEYBOARD_PLAYER)
		{
			InputManager.AttachDevice( new UnityInputDevice( new TOKeyboardProfileA()) );
			InputManager.AttachDevice( new UnityInputDevice( new TOKeyboardProfileB()) );
		}
	
		foreach(var device in InputManager.Devices)
		{
			HandleOnDeviceAttached(device);
		}


		InputManager.OnDeviceAttached += HandleOnDeviceAttached;
		InputManager.OnDeviceDetached += HandleOnDeviceDetached;
	}
	
	void HandleOnDeviceAttached (InputDevice device)
	{
		bool wasDeviceAlreadyUsed = false;
		foreach(var player in players)
		{
			if(player.device == device)
			{
				wasDeviceAlreadyUsed = true;
				break;
			}
		}
		
		if(!wasDeviceAlreadyUsed)
		{
			AddPlayer(device);
		}
	}
	
	void HandleOnDeviceDetached (InputDevice device)
	{
		foreach(var player in players)
		{
			if(player.device == device)
			{
				RemovePlayer(player);
				break;
			}
		}
	}
	
	void AddPlayer(InputDevice device)
	{
		if(players.Count >= Config.MAX_PLAYERS) return;

		Player player = new Player();
		player.team = Team_None;
		player.device = device;
		player.color = availableColors.Shift();

		players.Add(player);

		player.HandleAdded();

		if(OnPlayerAdded != null) OnPlayerAdded(player);
	}
	
	void RemovePlayer(Player player)
	{
		players.Remove(player);
		availableColors.Unshift(player.color);
		player.HandleRemoved();

		if(OnPlayerRemoved != null) OnPlayerRemoved(player);
	}
	
	public void Update()
	{
		InputManager.Update();
	}
}


public class Player
{
	public InputDevice device;
	public Team team;
	public PlayerColor color;

	public Player ()
	{
		
	}

	public void HandleAdded()
	{
		Debug.Log("added player: " + color.name + " # " + device.Meta);
	}

	public void HandleRemoved()
	{
		Debug.Log("removed player " + color.name + " # " + device.Meta);
	}
}

public class PlayerColor
{
	public string name;
	public Color color;

	public PlayerColor(string name, uint hex)
	{
		this.name = name;
		this.color = RXColor.GetColorFromHex(hex);
	}
}

public class Team
{
	public string name;
	public Color color;

	public Team teamToTheLeft = null;
	public Team teamToTheRight = null;

	public Team(string name, Color color)
	{
		this.name = name;
		this.color = color;
	}
}

public class TOKeyboardProfileA : UnityInputDeviceProfile
{
	public TOKeyboardProfileA()
	{
		Name = "TOKeyboardProfileA";
		Meta = "Keyboard ControlsA";
		
		// This profile only works on desktops.
		SupportedPlatforms = new[]
		{
			"Windows",
			"Mac",
			"Linux"
		};
		
		Sensitivity = 1.0f;
		LowerDeadZone = 0.0f;
		UpperDeadZone = 1.0f;
		
		ButtonMappings = new[]
		{
			new InputControlMapping
			{
				Handle = "Fire - Keyboard",
				Target = InputControlType.Action1,
				Source = KeyCodeButton( KeyCode.LeftShift )
			},
			new InputControlMapping
			{
				Handle = "Menu",
				Target = InputControlType.Menu,
				Source = KeyCodeButton( KeyCode.Return )
			}
		};
		
		AnalogMappings = new[]
		{
			new InputControlMapping
			{
				Handle = "Move X",
				Target = InputControlType.LeftStickX,
				Source = KeyCodeAxis( KeyCode.A, KeyCode.D )
			},
			new InputControlMapping
			{
				Handle = "Move Y",
				Target = InputControlType.LeftStickY,
				Source = KeyCodeAxis( KeyCode.S, KeyCode.W )
			}
		};
	}
}

public class TOKeyboardProfileB : UnityInputDeviceProfile
{
	public TOKeyboardProfileB()
	{
		Name = "TOKeyboardProfileB";
		Meta = "Keyboard ControlsB";
		
		// This profile only works on desktops.
		SupportedPlatforms = new[]
		{
			"Windows",
			"Mac",
			"Linux"
		};
		
		Sensitivity = 1.0f;
		LowerDeadZone = 0.0f;
		UpperDeadZone = 1.0f;
		
		ButtonMappings = new[]
		{
			new InputControlMapping
			{
				Handle = "Fire - Keyboard",
				Target = InputControlType.Action1,
				Source = KeyCodeButton( KeyCode.Space )
			},
			new InputControlMapping
			{
				Handle = "Menu",
				Target = InputControlType.Menu,
				Source = KeyCodeButton( KeyCode.Return )
			}
		};
		
		AnalogMappings = new[]
		{
			new InputControlMapping
			{
				Handle = "Move X",
				Target = InputControlType.LeftStickX,
				Source = KeyCodeAxis( KeyCode.LeftArrow, KeyCode.RightArrow )
			},
			new InputControlMapping
			{
				Handle = "Move Y",
				Target = InputControlType.LeftStickY,
				Source = KeyCodeAxis( KeyCode.DownArrow, KeyCode.UpArrow )
			}
		};
	}
}

