using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;


public class EntityArea : FContainer
{
	public FContainer shadowContainer;
	public FContainer backContainer;
	public FSortYContainer sortYContainer;
	public FContainer frontContainer;
	public FContainer overlayContainer;
	public FContainer uiContainer;
	
	public List<Entity> entities = new List<Entity>();
	public List<TORect> blockingRects = new List<TORect>();
	public List<FSprite> debugSprites = new List<FSprite>();
	
	public TORect bounds = new TORect(-100000,-100000,200000,200000);
	
	public EntityArea()
	{
		AddChild(shadowContainer = new FContainer());
		AddChild(backContainer = new FContainer());
		AddChild(sortYContainer = new FSortYContainer());
		AddChild(overlayContainer = new FContainer());
		AddChild(frontContainer = new FContainer());
		AddChild(uiContainer = new FContainer());
		
		ListenForUpdate(HandleUpdate);
	}
	
	void HandleUpdate()
	{
		int entityCount = entities.Count;
		
		for(int e = entityCount-1; e>=0; e--)//reverse order so removals ain't no thang
		{
			entities[e].Update();
		}
		
		
		if(Time.frameCount % 10 == 0)//don't bother updating them every single frame
		{
			if(debugSprites.Count > 0)
			{
				for(int d = 0; d<debugSprites.Count; d++)
				{
					debugSprites[d].RemoveFromContainer();
				}
				debugSprites.Clear();
			}
			
			if(Config.SHOULD_DEBUG_BLOCKING_RECTS)
			{
				for(int r = 0; r<blockingRects.Count; r++)
				{
					debugSprites.Add(CreateDebugSprite(blockingRects[r],Color.red));
				}
			}
		}
	}

	public void AddBlockingRect(TORect rect)
	{
		blockingRects.Add(rect);
	}
	
	public FSprite CreateDebugSprite(TORect rect, Color rectColor)
	{
		FSliceSprite debugSprite = new FSliceSprite("Debug/Square",rect.width,rect.height,4,4,4,4);
		debugSprite.color = rectColor;
		debugSprite.alpha = 0.7f;
		debugSprite.SetAnchor(0,0);
		debugSprite.SetPosition(rect.x,rect.y);
		frontContainer.AddChild(debugSprite);
		return debugSprite;
	}
	
	public void RemoveBlockingRect(TORect rect)
	{
		blockingRects.Remove(rect);
	}
	
	public bool CheckPointHit(float checkX, float checkY)
	{
		if(checkX < bounds.x) return true;
		if(checkX > bounds.x+bounds.width) return true;
		if(checkY < bounds.y) return true;
		if(checkY > bounds.y+bounds.height) return true;
		
		int rectCount = blockingRects.Count;
		
		for(int r = 0; r<rectCount; r++)
		{
			if(blockingRects[r].Contains(checkX,checkY))
			{
				return true;
			}
		}
		return false;
	}

	public bool CheckVillPointHit(float checkX, float checkY)
	{
		if(checkX < bounds.x) return true;
		if(checkX > bounds.x+bounds.width) return true;
		if(checkY < bounds.y) return true;
		if(checkY > bounds.y+bounds.height) return true;
		
		int rectCount = blockingRects.Count;
		
		for(int r = 0; r<rectCount; r++)
		{
			if(blockingRects[r].shouldBlockVills)
			{
				if(blockingRects[r].Contains(checkX,checkY))
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool CheckRectHit(TORect rect)
	{
		int rectCount = blockingRects.Count;

		if(rect.x < bounds.x) return true;
		if(rect.x+rect.width > bounds.x+bounds.width) return true;
		if(rect.y < bounds.y) return true;
		if(rect.y+rect.height > bounds.y+bounds.height) return true;
		
		for(int r = 0; r<rectCount; r++)
		{
			if(blockingRects[r].CheckIntersect(rect))
			{
				return true;
			}
		}
		return false;
	}
	
	virtual public void Destroy()
	{
		for(int e = entities.Count-1; e >= 0; e--)
		{
			entities[e].HandleRemoved();
		}
	}
}

