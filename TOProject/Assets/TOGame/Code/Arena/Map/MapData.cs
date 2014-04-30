using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;


public class MapData
{
	public List<MapItem> items = new List<MapItem>();
	public List<StartPosMI> startPoses = new List<StartPosMI>();

	public MapData()
	{

	}

	public void Add(MapItem item)
	{
		items.Add(item);

		if(item is StartPosMI)
		{
			startPoses.Add(item as StartPosMI);
		}
	}

}

public class MapItem
{
	public float x;
	public float y;

	public MapItem(float x, float y)
	{
		this.x = x;
		this.y = y;
	}
}

public class StartPosMI : MapItem
{
	public StartPosMI(float x, float y) : base (x,y)
	{

	}
}

public class House1MI : MapItem
{
	public House1MI(float x, float y) : base (x,y)
	{
		
	}
}

public class FenceHMI : MapItem
{
	public FenceHMI(float x, float y) : base (x,y)
	{
		
	}
}

public class FenceVMI : MapItem
{
	public FenceVMI(float x, float y) : base (x,y)
	{
		
	}
}



