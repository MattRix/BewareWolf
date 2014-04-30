using UnityEngine;
using System;
using System.Collections.Generic;

public class SimpleButton : FContainer
{
	protected SimpleButtonColor _color;

	public FSliceButton button;

	public DualLabel mainLabel;
	public DualLabel subLabel;

	public Action<SimpleButton> SignalTap;
	public Action<SimpleButton> SignalDisabledTap;

	public EffectManager effectManager;

	FSliceSprite _overHighlight;
	bool _isEnabled = true;

	Func<bool> _enableCheck;

	bool _isPressTap;

	bool _canTapWhileDisabled = false;

	bool _shouldBlink = false;

	public bool shouldAppearSelected = false;

	public bool shouldDoTapEffect = true;

	public SimpleButton(string title, float width, float height, SimpleButtonColor color, bool isPressTap) : this(title,width,height,color,isPressTap,TOFonts.MEDIUM_BOLD)
	{

	}

	public SimpleButton(string title, float width, float height, SimpleButtonColor color, bool isPressTap,string fontName)
	{
		_isPressTap = isPressTap;
		this.effectManager = Core.topEffectManager;

		_color = color;
		AddChild(button = new FSliceButton(width,height,_color.path,_color.path));

		//button.SetPosition(0.25f,0.33f);//pixel perfect?

		AddChild(mainLabel = new DualLabel(fontName,title,Color.white,TOColors.TEXT_SHADOW));
		mainLabel.y = 2.5f;

		if(_isPressTap)
		{
			button.SignalPress += (b) => DoTap(); 
		}
		else 
		{
			button.SignalRelease += (b) => DoTap();
		}

		_overHighlight = new FSliceSprite("UI/ButtonHighlight",width,height,12,12,12,12);
		_overHighlight.alpha = 0.45f;


		ListenForUpdate(HandleUpdate);

		UpdateEnabled();
	}

	public DualLabel AddSubLabel(DualLabel subLabel, float mainOffsetY, float subOffsetY)
	{
		this.subLabel = subLabel;
		AddChild(subLabel);

		mainLabel.y += mainOffsetY;
		subLabel.y += subOffsetY;

		UpdateEnabled();

		return subLabel;
	}

	public void SetEnableCheck(Func<bool> enableCheck) //can be used to enable/disable the button
	{
		_enableCheck = enableCheck;
	}

	void HandleUpdate()
	{
		if(_enableCheck != null)
		{ 
			this.isEnabled = _enableCheck();
		}

		if(shouldAppearSelected)
		{
			if(_overHighlight.container == null)
			{
				AddChild(_overHighlight);
				_overHighlight.alpha = 0.9f;
			}
		}
		else 
		{
			if(!_isPressTap)
			{
				if(button.isTouchOver && button.isTouchable)
				{
					if(_overHighlight.container == null)
					{
						AddChild(_overHighlight);
					}
				}
				else
				{
					_overHighlight.RemoveFromContainer();
				}
			}

			if(shouldBlink)
			{
				if(Time.time % 0.25f < 0.12f && _isEnabled)
				{
					if(_overHighlight.container == null)
					{
						AddChild(_overHighlight);
					}
				}
				else 
				{
					_overHighlight.RemoveFromContainer();
				}
			}
		}
	}

	public void DoTap()
	{
		if(!_isEnabled && _canTapWhileDisabled)
		{
			if(shouldDoTapEffect) this.effectManager.ShowButtonHighlight(this,Vector2.zero,button.width,button.height,_color.color);
			if(SignalDisabledTap != null) SignalDisabledTap(this);
		}
		else if(!_isEnabled)
		{
			FXPlayer.DisabledButtonTap();
		}
		else
		{
			if(shouldDoTapEffect) this.effectManager.ShowButtonHighlight(this,Vector2.zero,button.width,button.height,_color.color);
			if(SignalTap != null) SignalTap(this);
		}

	}

	void UpdateEnabled()
	{
		if(_canTapWhileDisabled || !_isEnabled)
		{
			button.isEnabled = true;
			if(_isEnabled)
			{
				mainLabel.alpha = 1.0f;
				if(subLabel != null) subLabel.alpha = 1.0f;
				button.SetColors(Color.white,Color.white);
			}
			else 
			{
				mainLabel.alpha = 0.5f;
				if(subLabel != null) subLabel.alpha = 0.5f;
				button.SetColors(new Color(0.6f,0.6f,0.6f),new Color(0.7f,0.7f,0.7f));
			}
		}
		else 
		{
			if(_isEnabled)
			{
				mainLabel.alpha = 1.0f;
				if(subLabel != null) subLabel.alpha = 1.0f;
				//button.isEnabled = true;
			}
			else 
			{
				mainLabel.alpha = 0.5f;
				if(subLabel != null) subLabel.alpha = 0.5f;
				//button.isEnabled = false;
			}
		}

	}

	void UpdateColor()
	{
		button.SetElements(_color.path,_color.path);
	}

	void UpdateShouldBlink()
	{
		if(!_shouldBlink)
		{
			_overHighlight.RemoveFromContainer();
		}
	}

	public SimpleButtonColor color
	{
		get {return _color;}
		set {if(_color != value) {_color = value; UpdateColor();}}
	}

	public bool isEnabled
	{
		get {return _isEnabled;}
		set {if(_isEnabled != value){_isEnabled = value; UpdateEnabled();}}
	}

	public bool canTapWhileDisabled
	{
		get {return _canTapWhileDisabled;}
		set {if(_canTapWhileDisabled != value) {_canTapWhileDisabled = value; UpdateEnabled();}}
	}

	public float width
	{
		get {return button.width;}
		set {if(button.width != value) {button.width = value; _overHighlight.width = value;}}
	}

	public float height
	{
		get {return button.height;}
		set {if(button.height != value) {button.height = value; _overHighlight.height = value;}}
	}

	public bool shouldBlink
	{
		get {return _shouldBlink;}
		set {if(_shouldBlink != value) {_shouldBlink = value; UpdateShouldBlink();}}
	}

}

public class SimpleButtonColor
{
	public static SimpleButtonColor Blue = 			new SimpleButtonColor("Blue", new Color(0.0f,0.2f,1.0f));
	public static SimpleButtonColor Red = 			new SimpleButtonColor("Red", new Color(1.0f,0.1f,0.1f));
	public static SimpleButtonColor Cyan = 			new SimpleButtonColor("Cyan", new Color(0.1f,0.9f,1.0f));
	public static SimpleButtonColor Green = 		new SimpleButtonColor("Green", new Color(0.3f,1.0f,0.1f));
	public static SimpleButtonColor Orange = 		new SimpleButtonColor("Orange", new Color(1.0f,0.6f,0.1f));
	public static SimpleButtonColor Pink = 			new SimpleButtonColor("Pink", new Color(1.0f,0.7f,0.8f));
	public static SimpleButtonColor Purple = 		new SimpleButtonColor("Purple", new Color(1.0f,0.1f,0.9f));
	public static SimpleButtonColor Yellow = 		new SimpleButtonColor("Yellow", new Color(1.0f,0.9f,0.1f));
	public static SimpleButtonColor MenuOrange = 	new SimpleButtonColor("MenuOrange", new Color(1.0f,0.7f,0.1f));

	public string name;
	public Color color;
	public string path;

	public SimpleButtonColor(string name, Color color)
	{
		this.name = name;
		this.color = color;
		this.path = "UI/SimpleButtons/SimpleButton_"+name;
	}

	public SimpleButtonColor(string name, Color color, string path)
	{
		this.name = name;
		this.color = color;
		this.path = path;
	}

	public SimpleButtonColor(string path) //this is all that is really needed :P
	{
		this.path = path;
	}
}

