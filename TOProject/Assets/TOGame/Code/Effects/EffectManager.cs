using UnityEngine;
using System;
using System.Collections.Generic;

public class EffectManager : FContainer
{
	public bool hasParticles;

	FParticleSystem particleSystem;

	public FContainer flyCoinContainer;

	public EffectManager(bool hasParticles)
	{
		this.hasParticles = hasParticles;

		AddChild(flyCoinContainer = new FContainer());

		if(hasParticles)
		{
			AddChild(particleSystem = new FParticleSystem(60));
			particleSystem.shader = FShader.Additive;
		}
	}

	public void ShowParticleBurst(float startX, float startY, int count, string elementName, params Color[] colors)
	{
		ShowParticleBurst(startX,startY,count,elementName,0.6f,0.2f,colors);
	}

	public void ShowParticleBurst(float startX, float startY, int count, string elementName, float startScale, float endScale, params Color[] colors)
	{
		FParticleDefinition pd = new FParticleDefinition(elementName);
		pd.x = startX;
		pd.y = startY;
		pd.startScale = startScale;
		pd.endScale = endScale;

		//somewhat evenly spread the particles in a circle

		float radiansPerParticle = RXMath.DOUBLE_PI / (float) count;
		float nextAngle = 0.0f;

		for(int c = 0; c<count; c++)
		{
			pd.lifetime = RXRandom.Range(0.3f,0.5f);

			float speed = RXRandom.Range(30.0f,80.0f);
			float useAngle = nextAngle + RXRandom.Range(-0.01f,0.01f);

			pd.speedX = Mathf.Cos(useAngle) * speed;
			pd.speedY = Mathf.Sin(useAngle) * speed;

			nextAngle += radiansPerParticle;

			pd.startColor = RXRandom.GetRandomItem(colors);
			pd.endColor = RXRandom.GetRandomItem(colors).CloneWithNewAlpha(0.0f);

			particleSystem.AddParticle(pd);
		}
	}

	public void ShowButtonHighlight(SimpleButton button)
	{
		ShowButtonHighlight(button,Vector2.zero,button.width,button.height,Color.white);
	}

	public FNode ShowButtonHighlight(FNode node, Vector2 offset, float width, float height, Color color)
	{
		return ShowButtonHighlight(node,offset,width,height,color,true);
	}

	public FNode ShowButtonHighlight(FNode node, Vector2 offset, float width, float height, Color color, bool shouldPlaySound)
	{
		if(shouldPlaySound)
		{
			FXPlayer.NormalButtonTap();
		}
		
		FSliceSprite highlight = new FSliceSprite("UI/ButtonHighlight",width,height,8,8,8,8);
		AddChild(highlight);

		highlight.SetPosition(this.OtherToLocal(node,offset));

		highlight.shader = FShader.Additive;

		highlight.scale = 1.0f;
		highlight.alpha = 0.35f;
		highlight.color = color + new Color(0.5f,0.5f,0.5f);//brighten the color
		
		//uniform scaling
		float growSize = 8.0f;
		float growScaleX = (width+growSize) / width;
		float growScaleY = (height+growSize) / height;
		
		Go.to(highlight, 0.15f, new TweenConfig().floatProp("scaleX",growScaleX).floatProp("scaleY",growScaleY).floatProp("alpha",0.0f).setEaseType(EaseType.Linear).removeWhenComplete());

		return highlight;
	}

	public void ShowParticleBox(FNode baseNode, Rect rect)
	{
		Vector2 offset = this.OtherToLocal(baseNode,Vector2.zero);
		float cx = offset.x + rect.x + rect.width/2;
		float cy = offset.y + rect.y + rect.height/2;;

//		FSprite sprite = new FSprite("Box");
//		sprite.SetPosition(cx,cy);
//		sprite.width = rect.width;
//		sprite.height = rect.height;
//
//		AddChild(sprite);

		int count = 50;

		float halfWidth = rect.width/2;
		float halfHeight = rect.height/2;

		FParticleDefinition pd = new FParticleDefinition("Extra/Particle_Noise");
		pd.startScale = 0.3f;
		pd.endScale = 0.7f;

		float speed = 30.0f;

		for(int c = 0; c<count; c++)
		{
			pd.lifetime = RXRandom.Range(0.9f,1.3f);
			//pd.delay = RXRandom.Range(0.0f,0.1f);

			pd.x = cx+RXRandom.Range(-halfWidth,halfWidth);
			pd.y = cy+RXRandom.Range(-halfHeight,halfHeight);
			
			pd.speedX = RXRandom.Range(-speed,speed);
			pd.speedY = RXRandom.Range(-speed,speed);
			
			pd.startColor = new Color(1,1,0.8f,0.7f);
			pd.endColor = new Color(1,1,0.8f,0.0f);

			pd.startRotation = RXRandom.Range(0,360.0f);
			pd.endRotation = pd.startRotation + RXRandom.Range(-180.0f,180.0f);
			
			particleSystem.AddParticle(pd);
		}
	}


}
