using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;


public class FenceV : Entity
{
	public FContainer main;
	public FSprite mainSprite;
	public TORect blockingRect;
	public float offsetY = 28;
	
	public FenceV(float x, float y, EntityArea entityArea) : base(entityArea)
	{
		this.x = x;
		this.y = y;

		main = new FContainer();
		mainSprite = new FSprite("Arena/fenceV_1");
		mainSprite.y = -offsetY;
		main.AddChild(mainSprite);

		blockingRect = new TORect(x-4,y-30,8,60);
		blockingRect.shouldBlockVills = false;
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
