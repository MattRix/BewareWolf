using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;

public class Core : FContainer
{
	public static Core instance;

	public static PlayerManager playerManager;

	public static AudioManager audioManager;

	public static EffectManager topEffectManager;

	public FContainer pageContainer;

	public Page currentPage;

	public Core ()
	{
		instance = this;

		AddChild(pageContainer = new FContainer());
		AddChild(topEffectManager = new EffectManager(true));

		audioManager = new AudioManager();
		FXPlayer.manager = audioManager.fxManager;
		MusicPlayer.manager = audioManager.musicManager;
		FXPlayer.Preload();

		playerManager = new PlayerManager();

		playerManager.Setup();

		ShowPage(new PlayerSelectPage());

		ListenForUpdate(Update);
	}

	public void StartGame()
	{
		ShowPage(new Arena());
	}

	public void ShowPage(Page page)
	{
		if(currentPage != null)
		{
			currentPage.Destroy();
			currentPage.RemoveFromContainer();
			currentPage = null;
		}

		currentPage = page;
		AddChild(currentPage);
		currentPage.Start();
	}

	void Update()
	{
		MusicPlayer.Update();
		audioManager.Update();
		playerManager.Update();

		
		if(Input.GetKeyDown(KeyCode.R))
		{
			Restart(); 
		}
	}

	public void Restart()
	{
		ShowPage(new PlayerSelectPage());
	}

	public void HandleApplicationPause()
	{
		audioManager.HandleApplicationPause();
	}
	
	public void HandleApplicationResume()
	{
		audioManager.HandleApplicationResume();
	}
}





















