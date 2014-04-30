using UnityEngine;
using System;
using System.Collections.Generic;

public class LevelBar : FContainer
{
	public int numDashes;
	public float dashSpacing;
	public float dashWidth;
	public float dashHeight;
	public float totalWidth;
	public Vector2 dashOffset;
	public Vector2 barOffset;

	public FContainer barContainer;
	public FSprite background;
	public FContainer dashContainer;
	public FSprite borderSprite;

	public List<FSprite>dashes = new List<FSprite>();

	public LevelBar(int numDashes)
	{
		this.numDashes = numDashes;
		string borderElementName = "UI/Stats/LevelBar_"+numDashes;

		if(numDashes == 5)
		{
			dashWidth = 19;
		}
		else if(numDashes == 10)
		{
			dashWidth = 9;
		}
		else if(numDashes == 25)
		{
			dashWidth = 3;
		}

		AddChild(barContainer = new FContainer());
		
		barContainer.AddChild(background = new FSprite("Box")); //black bg
		background.color = Color.black;
		background.SetAnchor(0.0f,0.0f);

		barContainer.AddChild(borderSprite = new FSprite(borderElementName));//white border
		borderSprite.SetAnchor(0.0f,0);
		borderSprite.SetPosition(0,0);

		barContainer.AddChild(dashContainer = new FContainer());

		dashSpacing = 1;
		totalWidth = 100;
		dashHeight = 3;
		barOffset = new Vector2(-51,0);
		dashOffset = new Vector2(1,2);
		ApplyConfig();
	}

	void ApplyConfig()
	{
		barContainer.SetPosition(barOffset);
		background.SetPosition(1,1.5f);
		background.height = dashHeight+1.0f;
		background.width = totalWidth-1;
	}

	public void SetupDashes(List<DashData> dashDatas)
	{
		for(int d = 0; d<dashDatas.Count; d++)
		{
			FSprite dash = new FSprite("Box");
			dash.SetAnchor(0,0);
			dash.width = dashWidth;
			dash.height = dashHeight;
			dash.x = dashOffset.x + d * (dashWidth+dashSpacing);
			dash.y = dashOffset.y;
			dash.color = dashDatas[d].color;
			
			dashes.Add(dash);
			dashContainer.AddChild(dash);
		}
	}

	public Vector2 GetLabelPos(int index)
	{
		return new Vector2(dashOffset.x -0.5f + 20f*index, 13.0f);
	}

	public void PrepForShow()
	{
		dashes.GetLastItem().width = 0; 
	}

	public void ShowLastDash(Action onCompleteHandler)
	{
		FSprite lastDash = dashes.GetLastItem();
		Go.to(lastDash, 0.6f, new TweenConfig().floatProp("width",dashWidth).setEaseType(EaseType.QuadOut).onComplete((t)=>
		{
			if(onCompleteHandler != null) onCompleteHandler();
		}));
	}
}

public class DashData
{
	public Color color;
	public DashData()
	{
	}
}

/*

Need to be able to: 
- set number of dashes
- set color of dashes
- animate last dash in (if needed)
- do something at end of last dash animation
- set labels at specific points 
- set properties of labels at specific points (ex color, font, position, ugh)
- show random labels and stuff above+below bar as needed

*/