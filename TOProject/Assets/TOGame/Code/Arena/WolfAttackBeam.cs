using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;


public class WolfAttackBeam : Entity
{
	public Wolf wolf;

	public float angle;
	
	public FContainer main;
	public FSprite sprite;

	public bool hasDoneAttack = false;
	
	public WolfAttackBeam(Wolf wolf, float angle) : base(wolf.entityArea)
	{
		this.wolf = wolf;
		this.angle = angle;
		
		main = new FContainer();
		main.rotation = angle;

		sprite = new FSprite("Arena/Wolf_AttackBeam");
		sprite.x = -4f;
		main.AddChild(sprite);
		sprite.shader = FShader.Additive;
		sprite.color = wolf.player.player.color.color;
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
			sprite.scale = 0.9f;
			Go.to(sprite,0.2f, new TweenConfig().x(-10.0f).scaleXY(1.0f).alpha(0.0f).onComplete(OnTweenComplete));
			float size = 48;
			Arena.instance.HitVillagersInRect(this.wolf, new TORect(x-size/2,y-size/2,size,size), angle);
		}
	}

	void OnTweenComplete(AbstractTween obj)
	{
		RemoveFromArea();
	}
}






