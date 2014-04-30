using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;


public class Human : Entity
{
	public WolfActivePlayer player;
	
	public FContainer body;
	public FSprite bodySprite;
	//	public FSprite colorSprite;
	
	public FSprite shadowSprite;
	
	public float offsetY;
	
	public Vector2 inputDirection = Vector2.zero;
	
	public Vector2 speed = Vector2.zero;
	
	public HumanAnimation ani = HumanAnimation.Idle;
	
	public int currentFrame = 0;
	
	public float aniPos = 0;
	
	public bool didAnimationChange = false;
	
	public float attackCooldown = 0;

	public bool isTransformingToWolf = false;
	public bool isTransformingFromWolf = false;

	public float tranPercent = 0.0f;

	public TORect attackableRect;

	public bool isDead = false;

	public Vector2 pushSpeed = new Vector2();

	public HumanHealthBar healthBar;

	public Human(WolfActivePlayer player, EntityArea entityArea, bool shouldStartWithTransform) : base(entityArea)
	{
		this.player = player;

		isTransformingFromWolf = shouldStartWithTransform;
		tranPercent = 0.0f;

		offsetY = 18f;
		
		body = new FContainer();
		bodySprite = new FSprite(HumanAnimation.Idle.frames[0]);
		body.AddChild(bodySprite);
		
		bodySprite.color = player.player.color.color + new Color(0.7f,0.7f,0.7f,0.0f);
		
		bodySprite.y = offsetY;

		body.scaleX = player.isFacingLeft ? 1f : -1f;
		
		//		colorSprite = new FSprite("Arena/Human_Idle1_color");
		//		colorSprite.color = player.player.color.color;
		//		body.AddChild(colorSprite);

		shadowSprite = new FSprite("Arena/VillShadow");
		shadowSprite.alpha = 0.2f;
		shadowSprite.scaleX = 1.9f;
		shadowSprite.scaleY = 1.0f;

		attackableRect = new TORect(0,0,62,29);

		healthBar = new HumanHealthBar(this);
		healthBar.alpha = 0.0f;

		Go.to(healthBar, 1.0f, new TweenConfig().alpha(1.0f));

		float healthPercent = ((float)player.health / (float)Config.HUMAN_MAX_HEALTH);
		healthBar.SetPercent(healthPercent);
	}
	
	override public void HandleAdded()
	{
		base.HandleAdded();
		entityArea.sortYContainer.AddChild(body);
		entityArea.shadowContainer.AddChild(shadowSprite);
		entityArea.uiContainer.AddChild(healthBar);
	}
	
	override public void HandleRemoved()
	{
		base.HandleRemoved();
		body.RemoveFromContainer();
		shadowSprite.RemoveFromContainer();
		healthBar.RemoveFromContainer();
	}

	public void TransformIntoWolf()
	{
		isTransformingToWolf = true;
		tranPercent = 0.0f;
		Go.killAllTweensWithTarget(healthBar);
		Go.to(healthBar, 1.0f, new TweenConfig().alpha(0.0f));
	}

	public void Hit(Vill vill, float hitAngle)
	{
		if(isTransformingToWolf || isTransformingFromWolf || isDead) return;
		
		//		Vector2 offset = new Vector2(wolf.x - x,wolf.y + 10 - y).normalized * -1f;
		
		float rads = -hitAngle * RXMath.DTOR + Mathf.PI;
		Vector2 offset = new Vector2(Mathf.Cos(rads),Mathf.Sin(rads));
		
		pushSpeed += offset * 0.2f;

		player.health--;

		float healthPercent = ((float)player.health / (float)Config.HUMAN_MAX_HEALTH);

		healthBar.SetPercent(healthPercent);

		healthBar.Hit();
		
		if(player.health <= 0)
		{
			Die();
		}
	}
	
	void Die()
	{
		if(isDead) return;
		
		isDead = true;

		FXPlayer.WolfDeath();

		player.OnHumanDeath(this);
	}
	
	override public void Update()
	{
		didAnimationChange = false;

		healthBar.SetPosition(x,y+40);

		if(isTransformingFromWolf)
		{
			tranPercent = Mathf.Clamp01(tranPercent+(Time.deltaTime/HumanConfig.TRANSFORM_TIME));
			
			if(tranPercent >= 1)
			{
				//DESTROY
				isTransformingFromWolf = false;
			}
			else 
			{
				SetAnimationFrameByFloat(HumanAnimation.Tran,(1.0f-tranPercent),1.0f); //reversed
			}
			
			body.SetPosition(x,y);
			shadowSprite.SetPosition(x,y);
		}
		else if(isTransformingToWolf)
		{
			tranPercent = Mathf.Clamp01(tranPercent+(Time.deltaTime/HumanConfig.TRANSFORM_TIME));

			if(tranPercent >= 1)
			{
				//DESTROY
				player.DestroyHumanAndCreateWolf(this);
			}
			else 
			{
				SetAnimationFrameByFloat(HumanAnimation.Tran,tranPercent,1.0f);
			}

			body.SetPosition(x,y);
			shadowSprite.SetPosition(x,y);
		}
		else
		{
			if(attackCooldown > 0)
			{
				attackCooldown -= Time.deltaTime;
			}
			
			speed *= HumanConfig.DRAG;
			
			float fullSpeed = speed.magnitude;
			
			if(fullSpeed < HumanConfig.MAX_SPEED)
			{
				speed += inputDirection * HumanConfig.ACCEL;
			}
			else
			{
				speed *= 0.96f;
			}
			
			if(fullSpeed < 0.05f) //idle
			{
				aniPos += 0.1f;
				SetAnimationFrameByFloat(HumanAnimation.Idle,aniPos,5.0f);
			}
			else 
			{
				aniPos += fullSpeed;
				SetAnimationFrameByFloat(HumanAnimation.Walk,aniPos,32);
			}

			pushSpeed *= 0.9f;
			
			float addX = speed.x + pushSpeed.x;
			float addY = speed.y * Config.ISO_RATIO + pushSpeed.y * Config.ISO_RATIO;
			
			float newX = x+addX;
			float newY = y+addY;
			
			if(!CheckHit(newX,newY))
			{
				x = newX;
				y = newY;
			}
			else if(!CheckHit(x,newY))
			{
				pushSpeed.x *= -0.5f;
				y = newY;
			}
			else if(!CheckHit(newX,y))
			{
				pushSpeed.y *= -0.5f;
				x = newX;
			}
			
			body.SetPosition(x,y);
			shadowSprite.SetPosition(x,y);

			attackableRect.x = x-attackableRect.width/2;
			attackableRect.y = y-10;
			
			if(Mathf.Abs(speed.x) > 0.1f)
			{
				body.scaleX = speed.x < 0 ? 1f : -1f;
				player.isFacingLeft = body.scaleX < 0 ? false : true;
			}
		}
	}
	
	public bool CheckHit(float cx, float cy)
	{
		float sx = 8;
		float sy = 6;
		
		return entityArea.CheckRectHit(new TORect(cx-sx/2,cy-sy/2,sx,sy));
	}
	
	public void SetAnimationFrameByFloat(HumanAnimation ani, float progress, float step)
	{
		float percent = Mathf.Clamp01(RXMath.Mod(progress,step)/step);
		int frame = Mathf.RoundToInt(percent * (float)(ani.numFrames-1));
		
		SetAnimationFrame(ani,frame);
	}
	
	public void SetAnimationFrame(HumanAnimation ani, int frame)
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
		
		attackCooldown = HumanConfig.ATTACK_COOLDOWN;
		
		//DO NOTHING
	}
	
	public class HumanAnimation
	{
		public static HumanAnimation Idle = new HumanAnimation("Idle",4);
		public static HumanAnimation Walk = new HumanAnimation("Walk",6);
		public static HumanAnimation Tran = new HumanAnimation("Transform",13);
		
		public string name;
		public int numFrames;
		public FAtlasElement[] frames;
		
		public HumanAnimation(string name, int numFrames)
		{
			this.name = name;
			this.numFrames = numFrames;
		}
		
		public void Setup()
		{
			this.frames = new FAtlasElement[numFrames];
			for(int f = 0;f<numFrames;f++)
			{
				frames[f] = Futile.atlasManager.GetElementWithName("Arena/Human_"+name+"_"+(f+1).ToString());
			}
		}
		
		public static void SetupAnimations()
		{
			Idle.Setup();
			Walk.Setup();
			Tran.Setup();
		}
	}
}

public static class HumanConfig
{
	public static float MAX_SPEED = 2.0f;
	public static float ACCEL = 0.27f;
	public static float DRAG = 0.79f;
	public static float ATTACK_COOLDOWN = 0.5f;
	public static float TRANSFORM_TIME = 1.0f;
	
	static HumanConfig()
	{
		RXWatcher.Watch(typeof(HumanConfig));
	}
}






