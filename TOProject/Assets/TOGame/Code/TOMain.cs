using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TOMain : MonoBehaviour
{	
	public static TOMain instance;

	public Core core;

	private void Start()
	{
		instance = this; 
		
		Go.defaultEaseType = EaseType.Linear;
		Go.duplicatePropertyRule = DuplicatePropertyRuleType.RemoveRunningProperty;
		
		//Time.timeScale = 0.1f;
		
		bool isIPad = SystemInfo.deviceModel.Contains("iPad");
		
		bool shouldSupportPortraitUpsideDown = isIPad; //only support portrait upside-down on iPad
		
		FutileParams fparams = new FutileParams(true,true,true,shouldSupportPortraitUpsideDown);

		fparams.backgroundColor = RXUtils.GetColorFromHex(0x212121);
		fparams.shouldLerpToNearestResolutionLevel = true;
		fparams.resolutionLevelPickMode = FResolutionLevelPickMode.Downwards;

		fparams.AddResolutionLevel(640.0f,	1.0f,	1.0f,	"");
		fparams.AddResolutionLevel(1280.0f,	2.0f,	1.0f,	"");
		fparams.AddResolutionLevel(1920.0f,	3.0f,	1.0f,	"");
		fparams.AddResolutionLevel(2560.0f,	4.0f,	1.0f,	"");
		
		fparams.origin = new Vector2(0.5f,0.5f);
		
		Futile.instance.Init (fparams);

		FFacetType.Quad.maxEmptyAmount = 100;
		FFacetType.Quad.expansionAmount = 100;
		FFacetType.Quad.initialAmount = 100;
		
		FAtlas mainAtlas = Futile.atlasManager.LoadAtlas("Atlases/MainAtlas");
		mainAtlas.texture.filterMode = FilterMode.Point;

		TOFonts.Load();
		
		Config.Setup();

		Wolf.WolfAnimation.SetupAnimations();
		Human.HumanAnimation.SetupAnimations();

		core = new Core();
		Futile.stage.AddChild(core);
		
		//FSoundManager.PlayMusic ("NormalMusic",0.5f);

		Futile.screen.SignalResize += HandleSignalResize;
		HandleSignalResize(false);
	}

	void HandleSignalResize (bool wasResizedDueToOrientationChange)
	{
		Futile.stage.scale = Futile.screen.width/Config.WIDTH;
	}

	public void Update()
	{
		if(Input.GetKeyDown(KeyCode.F))
		{
			Screen.fullScreen = !Screen.fullScreen;
		}
	}

	void OnApplicationQuit()
	{
	
	}
	
	void OnApplicationPause(bool isPaused)
	{
		if(isPaused && Core.instance != null)
		{
			Core.instance.HandleApplicationPause();
		}
		else if (!isPaused && Core.instance != null)
		{
			Core.instance.HandleApplicationResume();
		}
	}
	
}









