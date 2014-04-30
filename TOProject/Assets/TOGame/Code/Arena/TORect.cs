using UnityEngine;
using System;
using System.Collections.Generic;


public class TORect : RXRect
{
	public bool shouldBlockVills = true;

	public TORect(float x, float y, float width, float height) : base (x,y,width,height)
	{

	}
	
	public TORect()
	{
		
	}
}
