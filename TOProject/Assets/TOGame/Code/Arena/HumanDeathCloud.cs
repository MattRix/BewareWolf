using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;


public class HumanDeathCloud : Entity
{
	public Human human;
	
	public FContainer cloudHolder;
	public FSprite cloudSprite;
	
	public FAtlasElement[] frames;
	
	public int frame = 0;
	
	public int totalFrame = 0;
	
	public FContainer graveHolder;
	public FSprite graveSprite;
	public bool isCloudDone = false;
	
	public HumanDeathCloud(Human human) : base(human.entityArea)
	{
		this.human = human;
		
		this.x = human.x;
		this.y = human.y+4;
		
		frames = new FAtlasElement[]
		{
			Futile.atlasManager.GetElementWithName("Arena/Vill_Death1"),
			Futile.atlasManager.GetElementWithName("Arena/Vill_Death2"),
			Futile.atlasManager.GetElementWithName("Arena/Vill_Death3"),
			Futile.atlasManager.GetElementWithName("Arena/Vill_Death4")
		};
		
		cloudHolder = new FContainer();
		
		cloudSprite = new FSprite(frames[0]);
		cloudSprite.scaleX = RXRandom.Bool() ? -1f : 1f;
		cloudHolder.AddChild(cloudSprite);
		cloudSprite.shader = FShader.Additive;
		cloudSprite.alpha = RXRandom.Range(0.8f,0.9f);
		cloudSprite.color = human.player.player.color.color + new Color(0.1f,0.1f,0.1f,0.0f);
		cloudSprite.scale = 2.0f;
		
		graveHolder = new FContainer();
		graveSprite = new FSprite("Arena/Human_Grave_1");
		graveHolder.AddChild(graveSprite);
		graveSprite.color = human.player.player.color.color + new Color(0.5f,0.5f,0.5f);
		
		graveSprite.y =  16;
		
		Update();
	}
	
	override public void HandleAdded()
	{
		base.HandleAdded();
		entityArea.sortYContainer.AddChild(graveHolder);
		entityArea.frontContainer.AddChild(cloudHolder);
	}
	
	override public void HandleRemoved()
	{
		base.HandleRemoved();
		graveHolder.RemoveFromContainer();
		cloudHolder.RemoveFromContainer();
	}
	
	override public void Update()
	{	
		graveHolder.SetPosition(x,y);
		
		if(!isCloudDone)
		{
			cloudHolder.SetPosition(x,y);
			
			if(totalFrame % 10 == 0)
			{
				frame ++;
				if(frame < frames.Length)
				{
					cloudSprite.element = frames[frame];
				}
				else 
				{
					cloudHolder.RemoveFromContainer();
					isCloudDone = true;
				}
			}
			
			cloudSprite.y += 0.5f;
			
			totalFrame++;
		}
		
	}
	
	//	void OnTweenComplete(AbstractTween obj)
	//	{
	//		RemoveFromArea();
	//	}
}






