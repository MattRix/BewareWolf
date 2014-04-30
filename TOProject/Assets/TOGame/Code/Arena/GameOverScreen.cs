using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;

public class GameOverScreen : FContainer
{
	public FSprite bg;

	public DualLabel label;

	public GameOverScreen(bool didWolfWin)
	{
		bg = new FSprite("WhiteBox");
		bg.color = Color.black.CloneWithNewAlpha(0.7f);
		AddChild(bg);
		bg.width = Config.WIDTH;
		bg.height = Config.HEIGHT;

		string message = didWolfWin ? "WEREWOLVES WIN!" : "VILLAGERS WIN!";

		label = new DualLabel(TOFonts.MEDIUM_BOLD,message);

		label.scale = 4.0f;
		label.mainLabel.y += 0.25f;
		AddChild(label);
		label.y += 7;

		DualLabel sub = new DualLabel(TOFonts.MEDIUM_BOLD,"PRESS [START] or [R] TO RESTART");
		sub.scale = 2.0f;
		sub.y -= 22.0f;
		AddChild(sub);	
	}
}