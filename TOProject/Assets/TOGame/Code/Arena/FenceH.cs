using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;


public class FenceH : Entity
{
	public FContainer main;
	public FSprite mainSprite;
	public TORect blockingRect;
	public float offsetY = -3;
	
	public FenceH(float x, float y, EntityArea entityArea) : base(entityArea)
	{
		this.x = x;
		this.y = y;

		main = new FContainer();
		mainSprite = new FSprite("Arena/fenceH_1");
		mainSprite.y = -offsetY;
		main.AddChild(mainSprite);

		blockingRect = new TORect(x-32,y-4,64,6);
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
