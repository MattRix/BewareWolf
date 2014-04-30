using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Hopper
{
	public Entity entity;
	public HopperConfig config;

	public float jumpTime; //value that is jump time elapsed
	public float jumpPercent; //value that is jump completion percentage (0 at start, 1 at end)
	public float jumpY; //1 is top of jump, 0 is bottom

	public Vector2 inputDirection = new Vector2(0,0);

	public Vector2 startPos = new Vector2(0,0);
	public Vector2 endPos = new Vector2(0,0);

	public float speedX = 0;
	public float speedY = 0;

	public float delayUntilFirstJump = 0.0f;

	public float curJumpDuration = 1.0f;

	public Hopper(Entity entity)
	{
		this.entity = entity;
		this.config = new HopperConfig();
		jumpTime = 0;
		jumpPercent = 0;
		jumpY = 0;

		delayUntilFirstJump = RXRandom.Range(0,0.4f);
	}

	public void Update()
	{
		float x = entity.x;
		float y = entity.y;

		float deltaTime = Time.deltaTime;

		if(delayUntilFirstJump > 0)
		{
			delayUntilFirstJump -= deltaTime;
			return;
		}

		if(jumpTime > 0)
		{
			jumpTime += deltaTime;
		}

		if(jumpTime >= curJumpDuration)
		{
			jumpTime = 0;
			speedX = 0;
			speedY = 0;
		}

		if(jumpTime <= 0)
		{
			if(inputDirection.sqrMagnitude > 1)
			{
				inputDirection.Normalize();
			}

			bool shouldMove = false;

			Vector2 moveVector = Vector2.zero;

			if(inputDirection.sqrMagnitude > 0.1f)
			{
				moveVector = inputDirection;
				shouldMove = true;
			}
			else if(RXRandom.Float() < 0.01f)
			{
				moveVector = RXRandom.Vector2Normalized() * 0.1f;
				shouldMove = true;
			}

			if(shouldMove)
			{
				Vector2 farOffset = moveVector * config.jumpDist;
				Vector2 closeOffset = moveVector * config.jumpDist * 0.5f;

				bool canJumpClose = !entity.entityArea.CheckVillPointHit(x+closeOffset.x,y+closeOffset.y);
				bool canJumpFar = !entity.entityArea.CheckVillPointHit(x+farOffset.x,y+farOffset.y);

				Vector2 winningOffset = closeOffset;

				if(canJumpClose)
				{
					winningOffset = closeOffset;

					if(canJumpFar)
					{
						winningOffset = farOffset;
					}
				}
				else 
				{
					canJumpClose = !entity.entityArea.CheckVillPointHit(x,y+closeOffset.y);
					canJumpFar = !entity.entityArea.CheckVillPointHit(x,y+farOffset.y);

					if(canJumpClose)
					{
						winningOffset = new Vector2(0,closeOffset.y);
						
						if(canJumpFar)
						{
							winningOffset = new Vector2(0,farOffset.y);
						}
					}
					else 
					{
						canJumpClose = !entity.entityArea.CheckVillPointHit(x+closeOffset.x,y);
						canJumpFar = !entity.entityArea.CheckVillPointHit(x+farOffset.x,y);
						
						if(canJumpClose)
						{
							winningOffset = new Vector2(closeOffset.x,0);
							
							if(canJumpFar)
							{
								winningOffset = new Vector2(farOffset.x,0);
							}
						}
						else 
						{
							shouldMove = false;
						}
					}

				}

				if(shouldMove)
				{
					//do the jump!
					curJumpDuration = RXRandom.Range(0.9f,1.1f) * config.jumpDuration;
					jumpTime = 0.0001f; 
					speedX = winningOffset.x/curJumpDuration;
					speedY = winningOffset.y/curJumpDuration;
				}
			}
		}

		if(jumpTime > 0)
		{
			x += speedX * deltaTime;
			y += speedY * deltaTime * Config.ISO_RATIO;
		}

		jumpPercent = Mathf.Clamp01(jumpTime/curJumpDuration);
		jumpY = -4.0f * jumpPercent * (-1.0f+jumpPercent);

		entity.x = x;
		entity.y = y;
	}
}

public class HopperConfig
{
	public float jumpDist;
	public float jumpDuration;
	public float jumpHeight;

	public HopperConfig()
	{

	}
}


