using System;
using UnityEngine;

public class AudioManager
{
	public bool isOtherAudioPlaying = false;

	public GameObject gameObject;

	public FXManager fxManager;
	public MusicManager musicManager;

	public AudioManager()
	{
		gameObject = new GameObject("AudioManager");
		gameObject.AddComponent<AudioListener>(); //we don't need a reference to it, we just need it to exist

		musicManager = new MusicManager(this);
		fxManager = new FXManager(this);
	}

	public void Start()
	{

	}

	public void Update()
	{
		musicManager.Update();
		fxManager.Update();
	}

	public void HandleApplicationPause()
	{
		musicManager.Pause();
	}
	
	public void HandleApplicationResume()
	{
		musicManager.Resume();
	}
}

