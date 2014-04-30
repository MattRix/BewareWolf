using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;


public class House : Entity
{
	public FContainer main;
	public FSprite mainSprite;
	public TORect blockingRect;
	public float offsetY = -20;
	
	public House(float x, float y, EntityArea entityArea) : base(entityArea)
	{
		this.x = x;
		this.y = y;

		main = new FContainer();
		mainSprite = new FSprite("Arena/House_1");
		mainSprite.y = -offsetY;
		main.AddChild(mainSprite);

		blockingRect = new TORect(x-40,y-24,80,42);
	}
	
	override public void HandleAdded()
	{
		base.HandleAdded();
		entityArea.sortYContainer.AddChild(main);
		entityArea.blockingRects.Add(blockingRect);
	}
	
	override public void HandleRemoved()
	{
		base.HandleRemoved();
		main.RemoveFromContainer();
		entityArea.blockingRects.Remove(blockingRect);
	}
	
	override public void Update()
	{
		main.SetPosition(x,y+offsetY);
	}
}
