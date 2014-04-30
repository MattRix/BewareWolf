using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;


public class Wolf : Entity
{
	public WolfActivePlayer player;
	
	public FContainer body;
	public FSprite bodySprite;
//	public FSprite colorSprite;

	public FSprite shadowSprite;
	
	public float offsetY;

	public Vector2 inputDirection = Vector2.zero;

	public Vector2 speed = Vector2.zero;

	public WolfAnimation ani = WolfAnimation.Idle;

	public int currentFrame = 0;

	public float aniPos = 0;

	public bool didAnimationChange = false;

	public float attackCooldown = 0;
	
	public bool isAttacking = false;
	public bool hasDoneAttackBeam = false;
	public float attackPercent = 0;
	public float attackBeamAngle = 0;

		
	public Wolf(WolfActivePlayer player, EntityArea entityArea) : base(entityArea)
	{
		this.player = player;
		
		offsetY = 12f;

		body = new FContainer();
		bodySprite = new FSprite(WolfAnimation.Idle.frames[0]);
		body.AddChild(bodySprite);

		bodySprite.color = player.player.color.color + new Color(0.5f,0.5f,0.5f,0.0f);

		bodySprite.y = offsetY;
		
		body.scaleX = player.isFacingLeft ? 1f : -1f;
//		colorSprite = new FSprite("Arena/Wolf_Idle1_color");
//		colorSprite.color = player.player.color.color;
//		body.AddChild(colorSprite);
//		
		shadowSprite = new FSprite("Arena/VillShadow");
		shadowSprite.alpha = 0.2f;
		shadowSprite.scaleX = 1.9f;
		shadowSprite.scaleY = 1.0f;

	}
	
	override public void HandleAdded()
	{
		base.HandleAdded();
		entityArea.sortYContainer.AddChild(body);
		entityArea.shadowContainer.AddChild(shadowSprite);
	}
	
	override public void HandleRemoved()
	{
		base.HandleRemoved();
		body.RemoveFromContainer();
		shadowSprite.RemoveFromContainer();
	}

	public void TransformIntoHuman()
	{
		player.DestroyWolfAndCreateHuman(this);
	}
	
	override public void Update()
	{
		didAnimationChange = false;

		if(attackCooldown > 0)
		{
			attackCooldown -= Time.deltaTime;
		}

		speed *= WolfConfig.DRAG;

		float fullSpeed = speed.magnitude;

		if(fullSpeed < WolfConfig.MAX_SPEED)
		{
			speed += inputDirection * WolfConfig.ACCEL;
		}
		else
		{
			speed *= 0.96f;
		}

		if(isAttacking)
		{
			attackPercent += Time.deltaTime / WolfConfig.ATTACK_TIME;

			SetAnimationFrameByFloat(WolfAnimation.Attack,Mathf.Clamp01(attackPercent),1.0f);

			if(!hasDoneAttackBeam && attackPercent >= 0.5f)
			{
				hasDoneAttackBeam = true;
				CreateAttackBeam();
			}

			if(hasDoneAttackBeam)
			{
				speed *= 0.1f;
			}

			if(attackPercent >= 1)
			{
				attackPercent = 0;
				isAttacking = false;
			}
		}
		else if(fullSpeed < 0.05f) //idle
		{
			aniPos += 0.15f;
			SetAnimationFrameByFloat(WolfAnimation.Idle,aniPos,5.0f);
		}
		else 
		{
			aniPos += fullSpeed;
			SetAnimationFrameByFloat(WolfAnimation.Walk,aniPos,32);
		}

		float addX = speed.x;
		float addY = speed.y * Config.ISO_RATIO;
		 
		float newX = x+addX;
		float newY = y+addY;

		if(!CheckHit(newX,newY))
		{
			x = newX;
			y = newY;
		}
		else if(!CheckHit(x,newY))
		{
			y = newY;
		}
		else if(!CheckHit(newX,y))
		{
			x = newX;
		}
	
		body.SetPosition(x,y);
		shadowSprite.SetPosition(x,y);
		

		if(Mathf.Abs(speed.x) > 0.1f)
		{
			body.scaleX = speed.x < 0 ? 1f : -1f;
			player.isFacingLeft = body.scaleX < 0 ? false : true;
		}
	}

	void CreateAttackBeam()
	{
		var attackBeam = new WolfAttackBeam(this,attackBeamAngle);

		float beamDist = 10;
		float rads = -attackBeamAngle * RXMath.DTOR + Mathf.PI;
		Vector2 offset = new Vector2(Mathf.Cos(rads),Mathf.Sin(rads))*beamDist;
		attackBeam.x = x+offset.x;
		attackBeam.y = y+15+offset.y;

		attackBeam.AddToArea();
		attackBeam.Update();
	}

	public bool CheckHit(float cx, float cy)
	{
		float sx = 8;
		float sy = 6;

		return entityArea.CheckRectHit(new TORect(cx-sx/2,cy-sy/2,sx,sy));
	}

	public void SetAnimationFrameByFloat(WolfAnimation ani, float progress, float step)
	{
		float percent = RXMath.Mod(progress,step)/step;
		int frame = Mathf.RoundToInt(percent * (float)(ani.numFrames-1));
		
		SetAnimationFrame(ani,frame);
	}

	public void SetAnimationFrame(WolfAnimation ani, int frame)
	{
		this.ani = ani;
		this.currentFrame = frame % ani.numFrames; //wrap it 

		FAtlasElement newElement = ani.frames[currentFrame];

		if(bodySprite.element != newElement)
		{
			bodySprite.element = newElement;
			didAnimationChange = true;
		}
	}

	public void DoAttack()
	{
		if(attackCooldown > 0) return;

		attackCooldown = WolfConfig.ATTACK_COOLDOWN;

		if(inputDirection.magnitude > 0.1f)
		{
			attackBeamAngle = Mathf.Atan2(-inputDirection.y,inputDirection.x) * RXMath.RTOD;
		}
		else 
		{
			attackBeamAngle = player.isFacingLeft ? 180 : 0;
		}

		FXPlayer.WolfAttack();

		attackBeamAngle += 180;

		float rads = -attackBeamAngle * RXMath.DTOR + Mathf.PI;
		Vector2 attackVector = new Vector2(Mathf.Cos(rads),Mathf.Sin(rads));

		speed += attackVector * 7.0f; //launch doggy

		isAttacking = true;
		hasDoneAttackBeam = false;
		attackPercent = 0;
	}

	public class WolfAnimation
	{
		public static WolfAnimation Idle = new WolfAnimation("Idle",4);
		public static WolfAnimation Walk = new WolfAnimation("walk",8);
		public static WolfAnimation Attack = new WolfAnimation("Attack",8);

		public string name;
		public int numFrames;
		public FAtlasElement[] frames;

		public WolfAnimation(string name, int numFrames)
		{
			this.name = name;
			this.numFrames = numFrames;
		}

		public void Setup()
		{
			this.frames = new FAtlasElement[numFrames];
			for(int f = 0;f<numFrames;f++)
			{
				frames[f] = Futile.atlasManager.GetElementWithName("Arena/Wolf_"+name+(f+1).ToString());
			}
		}

		public static void SetupAnimations()
		{
			Idle.Setup();
			Walk.Setup();
			Attack.Setup();
		}
	}
}

public static class WolfConfig
{
	public static float MAX_SPEED = 3.8f;
	public static float ACCEL = 0.37f;
	public static float DRAG = 0.8f;
	public static float ATTACK_COOLDOWN = 0.5f;
	public static float ATTACK_TIME = 0.3f;

	static WolfConfig()
	{
		RXWatcher.Watch(typeof(WolfConfig));
	}
}






