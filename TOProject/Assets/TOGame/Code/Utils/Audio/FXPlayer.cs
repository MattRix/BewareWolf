using System;
using System.Collections.Generic;
using UnityEngine;

public class FXPlayer
{
	static public FXManager manager;

	static public void Preload()
	{

	}

	public static void WolfAttack()
	{
		manager.PlaySound(RXRandom.GetRandomString("Arena/bite0","Arena/bite1","Arena/bite2","Arena/bite3"),1.0f);
	}

	public static void VillAttack()
	{
		manager.PlaySound(RXRandom.GetRandomString("Arena/hit01","Arena/hit02","Arena/hit03","Arena/hit04"),1.0f);
	}

	public static void WolfDeath()
	{
		manager.PlaySound(RXRandom.GetRandomString("Arena/growl0","Arena/growl1","Arena/growl2","Arena/growl3"),1.0f);
	}

	public static void VillDeath()
	{
		manager.PlaySound(RXRandom.GetRandomString("Arena/hit05","Arena/hit06","Arena/hit07","Arena/hit08"),0.5f);
	}

	public static void DayStart()
	{
		manager.PlaySound(RXRandom.GetRandomString("Arena/laugh1","Arena/laugh2"),1.0f);
	}

	public static void NightStart()
	{
		manager.PlaySound(RXRandom.GetRandomString("Arena/growl0","Arena/growl1","Arena/growl2","Arena/growl3"),1.0f);
	}

	public static void VillWin()
	{
		manager.PlaySound("Arena/cheer",1.0f);
	}

	public static void WolfWin()
	{
		manager.PlaySound("Arena/growl0",1.0f);
	}

	public static void PopupOpen()
	{
		manager.PlaySound("UI/Popup",0.5f);
	}

	public static void NormalButtonTap()
	{
		manager.PlaySound("UI/ButtonTap",0.5f);
	}

	public static void DisabledButtonTap()
	{
		manager.PlaySound("UI/ButtonDisabled",0.2f);
	}

	public static void MinimapPlotTap()
	{
		manager.PlaySound("UI/ButtonTap",0.5f);
	}

	public static void PanelSlideOpen()
	{
		manager.PlaySound("UI/SlideOpen",0.5f);
	}

	public static void PanelSlideClosed()
	{
		manager.PlaySound("UI/SlideClose");
	}

	public static void ShowToast()
	{
		manager.PlaySound("UI/ShowToast");
	}

	public static void CloseToast()
	{
		manager.PlaySound("UI/SlideClose");
	}

	public static void DoCoinBurst()
	{ 
		manager.PlaySound("Joy/CollectCoinSmall",0.2f);
	}

	public static void DoBuxBurst()
	{
		manager.PlaySound("Joy/CollectBux",0.15f,0,1.3f,0);
	}


}

