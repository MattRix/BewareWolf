using System;
using UnityEngine;

public class MusicPlayer
{
	static public MusicManager manager; //set in core


	static public MusicSong DAYTIME = new MusicSong("TOJam14_daytimeTheme",0.8f,true);
	static public MusicSong NIGHTTIME = new MusicSong("TOJam14_nighttimeTheme",0.8f,true);
	static public MusicSong MENU = new MusicSong("TOJam14_menuTheme",0.8f,false);

	static public MusicSong currentSong = null;

	static public void Update()
	{
		MusicSong songToPlay = currentSong;

		float fadeOutDuration = 0.3f;
		float fadeInDuration = 0.2f;

		if(Core.instance.currentPage is PlayerSelectPage)
		{
			songToPlay = MENU;
		}
		else 
		{
			if(Arena.instance != null)
			{
				if(Arena.instance.dayManager.isDay)
				{
					songToPlay = DAYTIME;
				}
				else 
				{
					songToPlay = NIGHTTIME;
				}
			}

		}

		if(!Config.SHOULD_PLAY_MUSIC)
		{
			songToPlay = null;
		}

		if(songToPlay != currentSong)
		{
			manager.fadeOutDuration = fadeOutDuration;
			manager.fadeInDuration = fadeInDuration;
			manager.PlaySong(songToPlay);
			currentSong = songToPlay;
		}
	}
}

