using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoundedRect : FSliceSprite 
{

	public RoundedRect(float width, float height, bool hasBorder) : base(hasBorder ? "UI/Rounded2_BorderFilled" : "UI/Rounded2",width,height,4,4,4,4)
	{

	}
}
