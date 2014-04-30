using System;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager
{
	static public string prefix = "Audio/Music/";
	public static int NUM_CLIPS_TO_CACHE = 3;

	bool _isPaused = true;
	bool _isMuted = false;

	public GameObject gameObject;

	public List<AudioClipInfo> audioClipInfos;
	public AudioSource audioSource;

	public MusicSong currentSong = null;
	public MusicSong nextSong = null;

	public float fadeOutDuration = 0.3f; //these are overriden in musicplayer
	public float fadeInDuration = 0.2f; //these are overriden in musicplayer

	public bool isFadingOut = false;

	public float songPauseTime = 0.0f;

	public MusicManager(AudioManager audioManager)
	{
		gameObject = new GameObject("MusicManager");
		gameObject.transform.parent = audioManager.gameObject.transform;
		audioSource = gameObject.AddComponent<AudioSource>();

		audioClipInfos = new List<AudioClipInfo>();

		Resume();
	}

	public void PlaySong(MusicSong song)
	{
		nextSong = song;

		FadeOutCurrentSong();
	}

	void FadeOutCurrentSong()
	{
		if(isFadingOut) return; //we're already fading out!
		isFadingOut = true;

		if(currentSong == null)
		{
			OnCurrentSongFadeOutComplete();
		}
		else 
		{
			if(currentSong.shouldResume)
			{
				currentSong.playTime = audioSource.time;
			}

			Go.killAllTweensWithTarget(audioSource);
			Go.to(audioSource,fadeOutDuration,new TweenConfig().floatProp("volume",0).onComplete(OnCurrentSongFadeOutComplete));
		}
	}

	void OnCurrentSongFadeOutComplete()
	{
		isFadingOut = false;
		PlayNextSong();
	}

	public AudioClip LoadAudioClip(string resourcePath)
	{
		resourcePath = prefix + resourcePath;

		for(int a = 0; a<audioClipInfos.Count; a++)
		{
			AudioClipInfo clipInfo = audioClipInfos[a];

			if(clipInfo.resourcePath == resourcePath)
			{
				//make sure this clip is at the top of the stack now
				audioClipInfos.RemoveAt(a);
				audioClipInfos.Add(clipInfo);
				return clipInfo.clip;
			}
		}

		AudioClip audioClip = Resources.Load(resourcePath) as AudioClip;	
		
		if(audioClip == null)
		{
			TODebug.Log("MusicManager couldn't find music at: " + resourcePath);
			return null; //can't play the sound because we can't find it!
		}

		AudioClipInfo newClipInfo = new AudioClipInfo();
		newClipInfo.clip = audioClip;
		newClipInfo.resourcePath = resourcePath;

		audioClipInfos.Add(newClipInfo);

		while(audioClipInfos.Count > NUM_CLIPS_TO_CACHE)
		{
			AudioClipInfo clipInfoToRemove = audioClipInfos[0];
			Resources.UnloadAsset(clipInfoToRemove.clip);
			audioClipInfos.RemoveAt(0);
		}
		
		return newClipInfo.clip;
	}
	
	void PlayNextSong()
	{
		currentSong = nextSong;
		nextSong = null;

		AudioClip clip = LoadAudioClip(currentSong.resourcePath);

		if(clip != null)
		{
			audioSource.clip = clip;
			audioSource.volume = 0;
			Go.killAllTweensWithTarget(audioSource);
			Go.to(audioSource,fadeInDuration,new TweenConfig().floatProp("volume",currentSong.volume));
			audioSource.loop = true;

			if(_isPaused)
			{
				songPauseTime = audioSource.time = currentSong.playTime;
				audioSource.Stop();
			}
			else
			{
				songPauseTime = audioSource.time = currentSong.playTime;
				audioSource.Play();
			}
		}
		else 
		{
			audioSource.clip = null;//don't play anything
			audioSource.Stop();
			currentSong = null;
		}
	}

	public void Pause()
	{
		if(_isPaused) return;
		_isPaused = true;

		if(currentSong != null)
		{
			songPauseTime = audioSource.time;
			audioSource.Stop();
		}
	}

	public void Resume()
	{
		//if (iOSAudioSession.OtherAudioIsPlaying()) return;

		if(!_isPaused) return;
		_isPaused = false;

		if(currentSong != null)
		{
			audioSource.time = songPauseTime; //resume where we left off
			audioSource.Play();

			//fade in
			audioSource.volume = 0;
			Go.killAllTweensWithTarget(audioSource);
			Go.to(audioSource,fadeInDuration,new TweenConfig().floatProp("volume",currentSong.volume));
		}
	}

	public void Update()
	{
	}

	void UpdateMuted()
	{
		audioSource.mute = _isMuted;
	}

	public bool isMuted
	{
		get {return _isMuted;}
		set {if(_isMuted != value) {_isMuted = value; UpdateMuted();}}
	}

	public class AudioClipInfo
	{
		public AudioClip clip;
		public string resourcePath;
	}
}

public class MusicSong
{
	public string resourcePath;
	public float volume;
	public float playTime;
	public bool shouldResume;
	
	public MusicSong(string resourcePath, float volume, bool shouldResume)
	{
		this.resourcePath = resourcePath;
		this.volume = volume;
		this.playTime = 0;
		this.shouldResume = shouldResume;
	}
}

