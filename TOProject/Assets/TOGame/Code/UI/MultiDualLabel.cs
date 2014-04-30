using UnityEngine;
using System;
using System.Collections.Generic;

public class MultiDualLabel : FContainer
{
	public float spacing = 0.0f;
	public List<DualLabel> labels = new List<DualLabel>();

	public MultiDualLabel()
	{
		
	}

	public DualLabel AddLabel(DualLabel label)
	{
		labels.Add(label);
		label.SetAnchor(0.0f,0.5f);
		AddChild(label);
		return label;
	}

	public DualLabel RemoveLabel(DualLabel label)
	{
		label.RemoveFromContainer();
		labels.Remove(label);
		return label;
	}

	public float textWidth
	{
		get 
		{
			int count = labels.Count;
			float totalWidth = spacing * (count-1);
			
			for(int c = 0; c<count; c++)
			{
				totalWidth += labels[c].textWidth;
			}
			return totalWidth;
		}
	}

	public void Align(float anchorX)
	{
		int count = labels.Count;
		float totalWidth = spacing * (count-1);

		for(int c = 0; c<count; c++)
		{
			totalWidth += labels[c].textWidth * labels[c].scaleX;
		}

		float leftX = -totalWidth * anchorX;

		for(int c = 0; c<count; c++)
		{
			labels[c].x = leftX;
			leftX += labels[c].textWidth * labels[c].scaleX + spacing;
		}
	}
}