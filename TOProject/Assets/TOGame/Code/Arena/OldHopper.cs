using UnityEngine;
using System;
using System.Collections.Generic;

public class OldHopper
{
	public Entity entity;
	
	public OldHopperConfig config;
	
	
	public float speedX;
	public float speedY;
	public float timeToWait = 0;
	public float jumpY = 0;
	public bool isMoving = false;
	public int numJumpsToDo = 0;
	public float timePerJump = 0;
	public float tripDistPerJump = 0;
	public float jumpingTimeToUse = 0;
	
	public float jumpHeight;
	public float jumpDistance;
	public float baseSpeed;
	
	public bool shouldGoToTarget = false;
	public float targetX = 0;
	public float targetY = 0;
	
	bool _isAwake = true;
	
	public OldHopper(Entity entity, OldHopperConfig config)
	{
		this.entity = entity;
		this.config = config;
		jumpHeight = config.jumpHeight;
		jumpDistance = config.jumpDistance;
		baseSpeed = config.baseSpeed;
		
		timeToWait = 0.001f + RXRandom.Range(config.waitMinTime,config.waitMaxTime)*0.5f; //wait less the first time
	}
	
	public void Update()
	{
		float x = entity.x;
		float y = entity.y;
		
		float deltaTime = Time.deltaTime;
		
		//less waiting if you have a target!
		if(_isAwake && shouldGoToTarget)
		{
			timeToWait *= 0.005f;
		}
		
		if(timeToWait > 0)
		{
			isMoving = false;
			
			if(_isAwake)
			{
				timeToWait -= deltaTime;
			}
			
			if(timeToWait <= 0)
			{
				float speedModifier = RXRandom.Range(0.75f,1.5f);
				float speed = baseSpeed * speedModifier;
				
				float angle;
				float dist;
				
				if(shouldGoToTarget)
				{
					float randomRange = 20.0f;
					
					float dx = (targetX + RXRandom.Range(-randomRange,randomRange)) - x;
					float dy = (targetY + RXRandom.Range(-randomRange,randomRange)) - y;
					angle = Mathf.Atan2(dy,dx);
					dist = Mathf.Sqrt(dx*dx + dy*dy);
				}
				else 
				{
					angle = RXRandom.Range(0,RXMath.DOUBLE_PI);
					dist = RXRandom.Range(10.0f,30.0f);
				}
				
				tripDistPerJump = jumpDistance*speedModifier;
				numJumpsToDo = Mathf.CeilToInt(dist/tripDistPerJump);
				
				speedX = Mathf.Cos(angle) * speed;
				speedY = Mathf.Sin(angle) * speed;
				
				timePerJump = (tripDistPerJump/speed);
				jumpingTimeToUse = 0;
				
				if(shouldGoToTarget)
				{
					numJumpsToDo = 1; //only do one jump towards the target at a time
				}
				
				if(CheckIfNextJumpWillHitSomething(x,y))
				{
					numJumpsToDo = 0;
				}
				
				if(numJumpsToDo == 0)
				{
					timeToWait = 0.001f + RXRandom.Range(config.waitMinTime,config.waitMaxTime);
				}
			}
		}
		
		if(numJumpsToDo > 0)
		{
			isMoving = true;
			
			jumpingTimeToUse += Time.deltaTime;
			
			while(jumpingTimeToUse > timePerJump)
			{
				jumpingTimeToUse -= timePerJump;
				
				if(numJumpsToDo > 0)
				{
					if(shouldGoToTarget) 
					{
						numJumpsToDo = 0; 
					}
					else 
					{
						if(CheckIfNextJumpWillHitSomething(x,y))
						{
							numJumpsToDo = 0;
						}
						else 
						{
							numJumpsToDo--;
						}
					}
				}
			}
			
			if(numJumpsToDo == 0)
			{
				//STOP
				jumpY = 0;
				x += speedX * deltaTime;
				y += speedY * deltaTime * Config.ISO_RATIO;
				
				timeToWait = 0.001f + RXRandom.Range(config.waitMinTime,config.waitMaxTime);
				isMoving = false;
			}
			else 
			{
				float jumpAmount = jumpingTimeToUse/timePerJump;
				jumpY = jumpHeight * -4.0f * jumpAmount * (-1.0f+jumpAmount);
				x += speedX * deltaTime;
				y += speedY * deltaTime * Config.ISO_RATIO;
			}
		}
		
		entity.x = x;
		entity.y = y;
	}
	
	bool CheckIfNextJumpWillHitSomething(float baseX, float baseY)
	{
		Vector2 moveVector = new Vector2(speedX,speedY).normalized * tripDistPerJump;
		
		if(entity.entityArea.CheckPointHit(baseX+moveVector.x,baseY+moveVector.y))
		{
			return true;
		}
		
		//		int numSubChecks = 2; //ex. numSubChecks 2 will check 50% and 100% of the way
		//		float subCheckPercent = 1.0f / (float)numSubChecks;
		//
		//		for(int i = 1; i<numSubChecks+1; i++)
		//		{
		//			float checkDist = tripDistPerJump * ((float)i)*subCheckPercent;
		//			if(entity.entityArea.CheckPointHit(baseX+moveVector.x*checkDist,baseY+moveVector.y*checkDist))
		//			{
		//				return true;
		//			}
		//		}
		
		return false;
	}
	
	void UpdateAwake()
	{
		if(_isAwake)
		{
			if(numJumpsToDo == 0)
			{
				timeToWait = 0.01f;//start jumping right away when they wake up!
			}
		}
		else 
		{
			if(numJumpsToDo > 1) 
			{
				numJumpsToDo = 1;
			}
		}
	}
	
	public bool isAwake
	{
		get {return _isAwake;}
		set {if(_isAwake != value) {_isAwake = value; UpdateAwake();}}
	}
}


public class OldHopperConfig
{
	public float baseSpeed = 15.0f; //speed in pixels/second
	public float jumpHeight = 2.0f; //how high a single jump goes
	public float jumpDistance = 3.0f; //how far a single jump travels
	public float waitMinTime = 0.1f; //minimum time to stop before moving
	public float waitMaxTime = 9.0f; //maximum time to stop before moving
	public float distMin = 10.0f;
	public float distMax = 30.0f;
	
	public OldHopperConfig()
	{
		
	}
	
	public OldHopperConfig SetAll(float speed, float jumpHeight, float jumpDistance, float waitMinTime, float waitMaxTime, float distMin, float distMax)
	{
		this.baseSpeed = speed;
		this.jumpHeight = jumpHeight;
		this.jumpDistance = jumpDistance;
		this.waitMinTime = waitMinTime;
		this.waitMaxTime = waitMaxTime;
		this.distMin = distMin;
		this.distMax = distMax;
		
		return this;
	}
	
	public OldHopperConfig SetSpeed(float speed) {this.baseSpeed = speed; return this;}
	public OldHopperConfig SetJumpHeight(float jumpHeight) {this.jumpHeight = jumpHeight; return this;}
	public OldHopperConfig SetJumpDistance(float jumpDistance) {this.jumpDistance = jumpDistance; return this;}
	public OldHopperConfig SetWaitMinTime(float waitMinTime) {this.waitMinTime = waitMinTime; return this;}
	public OldHopperConfig SetWaitMaxTime(float waitMaxTime) {this.waitMaxTime = waitMaxTime; return this;}
	public OldHopperConfig SetDistMin(float distMin) {this.distMin = distMin; return this;}
	public OldHopperConfig SetDistMax(float distMax) {this.distMax = distMax; return this;}
}

public interface IHoppingEntity
{
	OldHopper GetHopper();
}




