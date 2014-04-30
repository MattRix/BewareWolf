using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;


public class Entity
{
	public EntityArea entityArea;
	public float x;
	public float y;
	public bool isAdded = false;

	public Entity(EntityArea entityArea)
	{
		this.entityArea = entityArea;
	}

	public void AddToArea()
	{
		HandleAdded();
	}

	public void RemoveFromArea()
	{
		HandleRemoved();
	}

	virtual public void HandleAdded()
	{
		entityArea.entities.Add(this);
		isAdded = true;
	}

	virtual public void HandleRemoved()
	{
		entityArea.entities.Remove(this);
		isAdded = false;
	}

	virtual public void Update()
	{

	}

	public void SetPosition(float x, float y)
	{
		this.x = x;
		this.y = y;
	}
	
	public void SetPosition(Vector2 pos)
	{
		this.x = pos.x;
		this.y = pos.y;
	}
	
	public Vector2 GetPosition()
	{
		return new Vector2(x,y);	
	}
}
