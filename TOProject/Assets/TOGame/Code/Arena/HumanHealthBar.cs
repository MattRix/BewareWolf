using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;


public class HumanHealthBar : FContainer
{
	public Human human;

	public FSprite bg;
	public FSprite border;
	public FWipeSprite inner;

	public float redness = 0.0f;

	public HumanHealthBar(Human human)
	{
		this.human = human;
		AddChild(bg = new FSprite("Arena/HumanHealthBar_BG"));
		AddChild(border = new FSprite("Arena/HumanHealthBar_Border"));
		AddChild(inner = new FWipeSprite("Arena/HumanHealthBar_Inner"));

		bg.color = human.player.player.color.color * new Color(0.2f,0.2f,0.2f,1f);
		inner.color = human.player.player.color.color;

		ListenForUpdate(Update);
		SetPercent(1.0f);
	}

	public void Hit()
	{
		redness = 1.0f;
	}

	void Update()
	{
		if(redness > 0)
		{
			redness -= 0.1f;
		}

		redness = Mathf.Clamp01(redness);

		border.color = new Color(1f,1f-redness,1f-redness,1f);
	}

	public void SetPercent(float percent)
	{
		inner.wipeLeftAmount = 0.02f+(percent*0.96f);
	}
}


