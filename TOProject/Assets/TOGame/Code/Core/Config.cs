using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Config
{
	public static bool SHOULD_SKIP_PLAYER_SELECT = false;
	public static bool SHOULD_AUTO_SELECT_TEAMS = false;
	public static bool SHOULD_DEBUG_BLOCKING_RECTS = false;
	public static bool SHOULD_ADD_KEYBOARD_PLAYER = true;
	public static bool SHOULD_PLAY_MUSIC = true;

	public static int MAX_PLAYERS = 7;
	public static int MIN_PLAYERS_PER_TEAM = 1;
	public static float WIDTH;
	public static float HEIGHT;
	public static float ISO_RATIO = 0.75f; //perspective ratio

	public static int HUMAN_MAX_HEALTH = 60;
	public static int VILLAGERS_PER_WOLF = 25;
	public static int VILLAGER_MAX_HEALTH = 1;

	public static float DAY_SECONDS = 9.0f;
	public static float DAY_TRANSITION_DURATION = 1.0f;

	public static float VILLAGER_ATTACK_COOLDOWN = 0.3f;

	public static void Setup()
	{
		WIDTH = 640;
		HEIGHT = 360;
		//WIDTH = Futile.screen.width;
		//HEIGHT = Futile.screen.height;
	}

}


