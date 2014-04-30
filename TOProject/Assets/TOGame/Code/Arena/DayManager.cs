using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;


public class DayManager
{
	public bool isDay = true;
	public float timeUntilSwitch = 0;
	public Arena arena;

	public DayManager()
	{
		arena = Arena.instance;
		GoDay();
	}

	public void Update()
	{
		if(arena.isGameOver) return; //no more day change if game over


		timeUntilSwitch += Time.deltaTime;

		if(timeUntilSwitch > Config.DAY_SECONDS)
		{
			timeUntilSwitch = 0.0f;

			if(isDay)
			{
				GoNight();
			}
			else 
			{
				GoDay();
			}
		}
	}

	void GoDay()
	{
		isDay = true;

		ShowMessage("DAY IS HERE!","CHASE THE WEREWOLF!",new Color(1f,1f,0.9f));

		Go.killAllTweensWithTarget(arena.colorOverlay);
		Go.to(arena.colorOverlay, Config.DAY_TRANSITION_DURATION, new TweenConfig().colorProp("color",new Color(0,0,0,0.0f)));

		for(int w = arena.wolves.Count-1; w>=0; w--) //reversed for removals
		{
			var wolf = arena.wolves[w];
			wolf.TransformIntoHuman();
		}

		FXPlayer.DayStart();
	}

	void GoNight()
	{
		isDay = false;

		ShowMessage("NIGHT HAS COME!","CHASE THE VILLAGERS!",new Color(0.9f,0.95f,1f));

		Go.killAllTweensWithTarget(arena.colorOverlay);
		Go.to(arena.colorOverlay, Config.DAY_TRANSITION_DURATION, new TweenConfig().colorProp("color",new Color(0,0.03f,0.18f,0.5f)));

		foreach(var human in arena.humans)
		{
			human.TransformIntoWolf();
		}

		FXPlayer.NightStart();
	}

	void ShowMessage(string title, string message, Color color)
	{
		FContainer callout = new FContainer();
		callout.scale = 0.5f;

		DualLabel titleLabel = new DualLabel(TOFonts.MEDIUM_BOLD,title);
		callout.AddChild(titleLabel);
		titleLabel.mainLabel.color = color;
		titleLabel.scale = 4.0f;
		titleLabel.y = 20.0f;
		titleLabel.shadowLabel.y += 0.5f;

		DualLabel messageLabel = new DualLabel(TOFonts.MEDIUM_BOLD,message);
		callout.AddChild(messageLabel);
		messageLabel.mainLabel.color = color;
		messageLabel.scale = 2.0f;
		messageLabel.y = -16.0f;

		Arena.instance.frontContainer.AddChild(callout);

		titleLabel.alpha = 0;
		messageLabel.alpha = 0;

		Go.to(titleLabel, 0.5f, new TweenConfig().alpha(1.0f));
		Go.to(messageLabel, 0.5f, new TweenConfig().alpha(1.0f));
		
		Go.to(callout, 3.0f, new TweenConfig().y(10.0f).removeWhenComplete());
		Go.to(callout,0.8f,new TweenConfig().setDelay(2.2f).alpha(0.0f));
	}
}














