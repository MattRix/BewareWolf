using UnityEngine;
using System;
using System.Collections.Generic;

public class Blocker : FButton
{
	bool _isShown = false;
	FContainer _containerToUse;

	public Action SignalShowComplete;
	public Action SignalHideComplete;

	public float hideDuration = 0.2f;

	public Blocker(FContainer containerToUse, float blockAlpha) : base("Box")
	{
		_containerToUse = containerToUse;

		sprite.width = Config.WIDTH;
		sprite.height = Config.HEIGHT;
		sprite.color = new Color(0.05f,0.05f,0.1f); //very dark blue
		sprite.alpha = blockAlpha;
		this.alpha = 0.0f;
	}

	public void Show()
	{
		Show(0.2f);
	}

	public void Show(float duration)
	{
		if(_isShown) return;
		
		_isShown = true;
		
		_containerToUse.AddChildAtBack(this);
		Go.killAllTweensWithTarget(this);
		Go.to(this, duration, new TweenConfig().floatProp("alpha",1.0f).onComplete(HandleShowComplete));
	}

	public void ShowInstantly()
	{
		if(_isShown) return;
		
		_isShown = true;

		_containerToUse.AddChildAtBack(this);
		Go.killAllTweensWithTarget(this);
		this.alpha = 1.0f;
		HandleShowComplete(null);
	}

	void HandleShowComplete(AbstractTween obj)
	{
		if(SignalShowComplete != null) SignalShowComplete();	
	}
	
	public void Hide()
	{
		if(!_isShown) return;
		
		_isShown = false;
		
		Go.killAllTweensWithTarget(this);
		Go.to(this, hideDuration, new TweenConfig().floatProp("alpha",0.0f).onComplete(HandleHideComplete));
	}

	void HandleHideComplete(AbstractTween obj)
	{
		this.RemoveFromContainer();
		if(SignalHideComplete != null) SignalHideComplete();
	}

	public bool isShown
	{
		get{return _isShown;}
	}
}


