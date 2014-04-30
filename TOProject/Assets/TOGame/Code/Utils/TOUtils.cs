using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

public static class TOExtensions
{
	public static TweenConfig destroyWhenComplete(this TweenConfig config)
	{
		config.onComplete((tween) => {((tween as Tween).target as TODestroyable).Destroy();});	
		return config;
	}
}

public interface TODestroyable
{
	void Destroy();
}

public static class TOUtils
{

	public static string Commafy(string input)
	{
		string result = "";
		while (input.Length > 3)
		{
			result = ","+input.Substring(input.Length-3,3) + result;
			input = input.Substring(0,input.Length-3);
		}
		return input+result;
	}

	public static string FormatTime(int seconds, bool shouldIncludeSubTime)
	{
		seconds = Math.Max(0,seconds);//no negative numbers

		int mainTime = seconds;
		string mainTimeSuffix = "s";
		
		int subTime = -1;
		string subTimeSuffix = "";

		//108000 = 3600*30
		//86400 = 3600*24 = 24 hours 
		if(mainTime > 3600*30) //greater than 30 hours: 1d 4h
		{
			subTime = (mainTime % 86400) / 3600;
			subTimeSuffix = "h";
			mainTime = mainTime/86400;
			mainTimeSuffix = "d";
		}
		if(mainTime > 3600) //greater than an hour: 43h 10m
		{
			subTime = (mainTime % 3600) / 60;
			subTimeSuffix = "m";
			mainTime = mainTime/3600;
			mainTimeSuffix = "h";
		}
		else if(mainTime > 60) //greater than a minute: 22m 14s
		{
			subTime = mainTime % 60;
			subTimeSuffix = "s";
			mainTime = mainTime/60;
			mainTimeSuffix = "m";
		}
		
		if(subTime != -1 && shouldIncludeSubTime)
		{
			return mainTime + mainTimeSuffix + " " + subTime + subTimeSuffix;
		}
		else 
		{
			return mainTime + mainTimeSuffix;
		}
	}

	public static string GetTicksAsDateString(long ticks)
	{
		ticks -= DateTime.UtcNow.Ticks - DateTime.Now.Ticks; //convert from UTC to local time
		return new DateTime(ticks).ToString("MMM d, h:mmtt");
	}

	public static string GetNameForInt(int theInt)
	{
		if(theInt == 0) return "zero";
		if(theInt == 1) return "one";
		if(theInt == 2) return "two";
		if(theInt == 3) return "three";
		if(theInt == 4) return "four";
		if(theInt == 5) return "five";
		if(theInt == 6) return "six";
		if(theInt == 7) return "seven";
		if(theInt == 8) return "eight";
		if(theInt == 9) return "nine";
		if(theInt == 10) return "ten";
		if(theInt == 11) return "eleven";
		if(theInt == 12) return "twelve";
		if(theInt == 13) return "thirteen";
		if(theInt == 14) return "fourteen";
		if(theInt == 15) return "fiften";
		if(theInt == 16) return "sixteen";
		if(theInt == 17) return "seventeen";
		if(theInt == 18) return "eighteen";
		if(theInt == 19) return "nineteen";
		if(theInt == 20) return "twenty";
		return "unknown";
	}

	public static void GoToColor(FMeshNode meshNode, float duration, Color goalColor)
	{
		Go.killAllTweensWithTarget(meshNode);
		if(duration == 0)
		{
			meshNode.color = goalColor;
		}
		else 
		{
			Go.to(meshNode,duration,new TweenConfig().colorProp("color",goalColor));
		}
	}

	public static void GoToColor(FSprite sprite, float duration, Color goalColor)
	{
		Go.killAllTweensWithTarget(sprite);
		if(duration == 0)
		{
			sprite.color = goalColor;
		}
		else 
		{
			Go.to(sprite,duration,new TweenConfig().colorProp("color",goalColor));
		}
	}

	public static Dictionary<string,int> DictifyString(string input)
	{
		Dictionary<string,int> dict = new Dictionary<string, int>();

		if(input == "")
		{
			return dict;
		}

		string[] entries = input.Split(',');

		int entryCount = entries.Length;

		for(int e = 0; e<entryCount; e++)
		{
			string[] parts = entries[e].Split(':');
			dict.Add(parts[0],int.Parse(parts[1]));
		}

		return dict;
	}

	public static string StringifyDict(Dictionary<string,int> dict)
	{
		StringBuilder sb = new StringBuilder();

		int i = 0;
		foreach(var kv in dict)
		{
			if(i != 0) sb.Append(',');
			sb.Append(kv.Key);
			sb.Append(':');
			sb.Append(kv.Value);
			i++;
		}
		
		return sb.ToString();
	}

	public static string StringifyStringDict(Dictionary<string,string> dict)
	{
		StringBuilder sb = new StringBuilder();
		
		int i = 0;
		foreach(var kv in dict)
		{
			if(i != 0) sb.Append(',');
			sb.Append(kv.Key);
			sb.Append(':');
			sb.Append(kv.Value);
			i++;
		}
		
		return sb.ToString();
	}

	public static Dictionary<string,string> DictifyStringDictString(string input)
	{
		Dictionary<string,string> dict = new Dictionary<string, string>();
		
		if(input == "")
		{
			return dict;
		}
		
		string[] entries = input.Split(',');
		
		int entryCount = entries.Length;
		
		for(int e = 0; e<entryCount; e++)
		{
			string[] parts = entries[e].Split(':');
			dict.Add(parts[0],parts[1]);
		}
		
		return dict;
	}
	

	public static string WriteTempFile(string fileName, string text)
	{
		#if UNITY_EDITOR && !UNITY_WEBPLAYER
		string folderPath = 
			System.IO.Directory.GetCurrentDirectory()+
			System.IO.Path.DirectorySeparatorChar+
			"Temp"+
			System.IO.Path.DirectorySeparatorChar;
		
		if(!System.IO.Directory.Exists(folderPath))
		{
			System.IO.Directory.CreateDirectory(folderPath);
		}
		
		string fullPath = folderPath+fileName;
		
		System.IO.File.WriteAllText(fullPath, text);

		//var info:FileInfo = new FileInfo("/Applications/TextEdit.app/Contents/MacOS/TextEdit");
		//System.Diagnostics.Process.Start(info.FullName);

		return fullPath;
		#else 
		return "COULD NOT WRITE"+fileName;
		#endif
	}


}







