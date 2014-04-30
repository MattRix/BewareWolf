using UnityEngine;
using System;
using System.Collections.Generic;

public class CircularParticleSystem : FParticleSystem
{
	public FAtlasElement element;
	public float timePerParticle;
	public float innerRadius;
	public float timeUntilNextParticle;

	public float lifetime;
	public float speed;

	public FParticleDefinition particleDef;

	public bool shouldRotate = false;

	public CircularParticleSystem(FAtlasElement element, float innerRadius, float lifetime, float speed, float particlesPerSecond, int maxParticleCount) : base(maxParticleCount)
	{
		this.element = element;
		this.innerRadius = innerRadius;
		this.lifetime = lifetime;
		this.speed = speed;
		this.timePerParticle = 1.0f/particlesPerSecond;

		timeUntilNextParticle = timePerParticle;

		particleDef = new FParticleDefinition(element);
		particleDef.endColor = Color.white.CloneWithNewAlpha(0.0f);

		ListenForAfterUpdate(HandleUpdate);
	}

	void HandleUpdate()
	{
		timeUntilNextParticle -= Time.deltaTime;

		while(timeUntilNextParticle <= 0)
		{
			timeUntilNextParticle += timePerParticle;

			float angle = RXRandom.Range(0.0f,RXMath.DOUBLE_PI);
			float radius = innerRadius * RXRandom.Range(0.9f,1.0f);
			float useSpeed = speed * RXRandom.Range(0.7f,1.0f);
			particleDef.lifetime = lifetime * RXRandom.Range(0.7f,1.0f);

			particleDef.x = Mathf.Cos(angle) * radius;
			particleDef.y = Mathf.Sin(angle) * radius;
			particleDef.speedX = Mathf.Cos(angle) * useSpeed;
			particleDef.speedY = Mathf.Sin(angle) * useSpeed;

			if(shouldRotate)
			{
				particleDef.startRotation = RXRandom.Range(-180.0f,180.0f);
				particleDef.endRotation = RXRandom.Range(-180.0f,180.0f);
			}

			AddParticle(particleDef);
		}
	}
}









