using UnityEngine;
using System;
using System.Collections.Generic;

public class SparkleSprite : FSprite
{
	public Rect rect;
	public float padding = 3;
	public float rotationSpeed = 0;
	public bool isReadyToSpark;

	public SparkleSprite(Rect rect) : base("Extra/Sparkle_Big")
	{
		this.rect = rect;
		this.scale = 0.0f;
		isReadyToSpark = false;

		Futile.instance.StartDelayedCallback(DoSpark,RXRandom.Float()); //first start will be delayed

		ListenForUpdate(HandleUpdate);
	}

	void HandleUpdate()
	{
		this.rotation += rotationSpeed;

		if(isReadyToSpark)
		{
			DoSpark();
		}
	}

	void DoSpark()
	{
		isReadyToSpark = false;
		rotationSpeed = RXRandom.Range(-2.0f,2.0f);

		x = RXRandom.Range(rect.x+padding,rect.x + rect.width - padding*2);
		y = RXRandom.Range(rect.y+padding,rect.y + rect.height - padding*2);
		scale = 0;

		Go.to(this, 0.5f, new TweenConfig().scaleXY(1.0f).setDelay(0.2f).backOut().onComplete((t)=>{
			Go.to(this, 0.2f, new TweenConfig().scaleXY(0,0f).onComplete((tw)=>{
				isReadyToSpark = true;
			}));
		}));
	}
}
