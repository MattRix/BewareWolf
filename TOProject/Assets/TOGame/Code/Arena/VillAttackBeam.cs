using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;


public class VillAttackBeam : Entity
{
	public Vill vill;

	public float angle;
	
	public FContainer main;
	public FSprite sprite;

	public bool hasDoneAttack = false;
	
	public VillAttackBeam(Vill vill, float angle) : base(vill.entityArea)
	{
		this.vill = vill;
		this.angle = angle;
		
		main = new FContainer();
		main.rotation = angle;

		sprite = new FSprite("Arena/Wolf_AttackBeam");
		sprite.scale = 0.5f;
		sprite.x = -2f;
		main.AddChild(sprite);
		sprite.shader = FShader.Additive;
		sprite.color = vill.player.player.color.color;
	}
	
	override public void HandleAdded()
	{
		base.HandleAdded();

		entityArea.frontContainer.AddChild(main);
	}
	
	override public void HandleRemoved()
	{
		base.HandleRemoved();
		main.RemoveFromContainer();
	}
	
	override public void Update()
	{	
		main.SetPosition(x,y);
		if(!hasDoneAttack)
		{
			hasDoneAttack = true;
			sprite.scale = 0.4f;
			Go.to(sprite,0.2f, new TweenConfig().x(-7.0f).scaleXY(0.5f).alpha(0.0f).onComplete(OnTweenComplete));
			float size = 24;
			Arena.instance.HitHumansInRect(this.vill, new TORect(x-size/2,y-size/2,size,size), angle);
		}
	}

	void OnTweenComplete(AbstractTween obj)
	{
		RemoveFromArea();
	}
}






