using System;
using System.Collections.Generic;
using UnityEngine;

public class FXManager
{
	static public string prefix = "Audio/";


	public GameObject gameObject;
	public AudioSource basicAudioSource;

	public List<FXSource>allSources;
	public List<AudioSource>extraAudioSources;
	
	static public Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

	bool _isMuted = false;
	
	public FXManager (AudioManager audioManager)
	{
		gameObject = new GameObject("FXManager");
		gameObject.transform.parent = audioManager.gameObject.transform;
		basicAudioSource = gameObject.AddComponent<AudioSource>();
		allSources = new List<FXSource>();
		extraAudioSources = new List<AudioSource>();
	}

	public void AddExtraAudioSource(AudioSource customAudioSource)
	{
		customAudioSource.mute = _isMuted;
		extraAudioSources.Add(customAudioSource);
	}

	public void RemoveExtraAudioSource(AudioSource customAudioSource)
	{
		extraAudioSources.Remove(customAudioSource);
	}

	FXSource GetNextSource()
	{
		int sourceCount = allSources.Count;

		for(int s = 0;s<sourceCount;s++)
		{
			FXSource source = allSources[s];

			if(!source.audioSource.isPlaying)
			{
				return source;
			}
		}

		FXSource newSource = new FXSource();
		allSources.Add(newSource);
		newSource.audioSource = gameObject.AddComponent<AudioSource>();
		return newSource;
	}

	void CleanUpSources()
	{

	}

	public void PlaySound(String resourceName)
	{
		PlaySound(resourceName,1.0f);
	}
	
	public void PlaySound(String resourcePath, float volume) //it is not necessary to preload sounds in order to play them
	{
		PlaySound(resourcePath,volume,0,1.0f,0.0f);
	}

	public void PlaySoundPitched(String resourcePath, float volume, float pitchRange)
	{
		PlaySound(resourcePath,volume,0,1.0f + RXRandom.Range(-pitchRange*0.5f,pitchRange*0.5f),0);
	}

	public AudioSource PlaySound(String resourcePath, float volume, float pan, float pitch, float delay) //it is not necessary to preload sounds in order to play them
	{
		FXSource newSource = null;

		Action playSound = ()=> {

			AudioClip audioClip = LoadAudioClip(resourcePath);

			if(audioClip != null)
			{
				if(pan != 0.0f || pitch != 1.0f)
				{
					newSource = GetNextSource();
					newSource.resourcePath = resourcePath;
					newSource.audioSource.clip = audioClip;
					newSource.audioSource.pitch = pitch;
					newSource.audioSource.pan = pan;
					newSource.audioSource.volume = volume;
					newSource.audioSource.mute = _isMuted;
					newSource.audioSource.Play();
				}
				else 
				{
					basicAudioSource.PlayOneShot(audioClip, volume);
				}
			}
		};

		if(delay <= 0)
		{
			playSound();
		}
		else 
		{
			Futile.instance.StartDelayedCallback(playSound,delay);
		}

		if(newSource == null)
		{
			return null;
		}
		else 
		{
			return newSource.audioSource; //only return if we created a new audio source for this purpose
		}
	}

	public AudioClip LoadAudioClip(string resourcePath)
	{
		resourcePath = prefix + resourcePath;

		AudioClip audioClip;
		
		if(audioClips.ContainsKey(resourcePath))
		{
			audioClip = audioClips[resourcePath];
		}
		else
		{
			audioClip = Resources.Load(resourcePath) as AudioClip;	
			
			if(audioClip == null)
			{
				TODebug.Log("FXManager couldn't find sound at: " + resourcePath);
				return null; //can't play the sound because we can't find it!
			}
			else
			{
				audioClips[resourcePath] = audioClip;
			}
		}
		
		return audioClip;
	}
	
	public void UnloadAudioClip(String resourcePath)
	{
		resourcePath = prefix + resourcePath;

		if(audioClips.ContainsKey(resourcePath)) //check if we have it
		{
			AudioClip clip = audioClips[resourcePath];
			Resources.UnloadAsset(clip);
			audioClips.Remove(resourcePath);
		}
	}
	
	public void UnloadAllAudioClips()
	{
		foreach(AudioClip audioClip in audioClips.Values)
		{
			Resources.UnloadAsset(audioClip);
		}
		
		audioClips.Clear();
	}

	public void Update()
	{
		#if UNITY_EDITOR
		gameObject.name = "FXManager ("+allSources.Count+" sources)";
		#endif
	}

	void UpdateMuted()
	{
		for(int s = 0;s<allSources.Count;s++)
		{
			FXSource source = allSources[s];
			source.audioSource.mute = _isMuted;
		}

		for(int e = 0;e<extraAudioSources.Count; e++)
		{
			extraAudioSources[e].mute = _isMuted;
		}

		basicAudioSource.mute = _isMuted;
	}
	
	public bool isMuted
	{
		get {return _isMuted;}
		set {if(_isMuted != value) {_isMuted = value; UpdateMuted();}}
	}

	public class FXSource
	{
		public AudioSource audioSource;
		public string resourcePath;
	}
}

