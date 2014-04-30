using UnityEngine;
using System;
using System.Collections.Generic;

public class DualLabel : FContainer
{
	public FLabel mainLabel;
	public FLabel shadowLabel;

	public Color normalColor;
	public Color blinkColor;

	public DualLabel(string fontName, string text) : this(fontName, text, Color.white, new Color(0,0,0,0.3f))
	{

	}

	public DualLabel(string fontName, string text, Color mainColor, Color shadowColor)
	{
		AddChild(shadowLabel = new FLabel(fontName, text));
		AddChild(mainLabel = new FLabel(fontName, text));

		mainLabel.color = mainColor;
		shadowLabel.color = shadowColor;

		shadowLabel.y = -1; //move the shadow down 1
	}

	public void Blink(Color blinkColor)
	{
		this.normalColor = mainLabel.color;
		this.blinkColor = blinkColor;
		ListenForUpdate(HandleUpdate);
	}

	void HandleUpdate()
	{
		if(Time.time % 0.3f < 0.15f)
		{
			mainLabel.color = normalColor;
		}
		else 
		{
			mainLabel.color = blinkColor;
		}
	}

	public string text
	{
		set {shadowLabel.text = value; mainLabel.text = value;}
		get {return mainLabel.text;}
	}

	public float textWidth
	{
		get {return mainLabel.textRect.width;}
	}

	public float textHeight
	{
		get {return mainLabel.textRect.height;}
	}

	public void SetAnchor(float anchorX, float anchorY)
	{
		mainLabel.SetAnchor(anchorX,anchorY);
		shadowLabel.SetAnchor(anchorX,anchorY);
	}

}
