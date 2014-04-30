#if UNITY_EDITOR
#define DEBUG_LOG_ON
#endif

using System;
using UnityEngine;

public class TODebug
{
	public static void Log(string message)
	{
		#if DEBUG_LOG_ON
		Debug.Log(message);
		#endif
	}

	public static void Log(string message, UnityEngine.Object context)
	{
		#if DEBUG_LOG_ON
		Debug.Log(message, context);
		#endif
	}

	public static void LogWarning(string message)
	{
		#if DEBUG_LOG_ON
		Debug.LogWarning(message);
		#endif
	}
	
	public static void LogWarning(string message, UnityEngine.Object context)
	{
		#if DEBUG_LOG_ON
		Debug.LogWarning(message, context);
		#endif
	}

	public static void LogError(string message)
	{
		#if DEBUG_LOG_ON
		Debug.LogError(message);
		#endif
	}
	
	public static void LogError(string message, UnityEngine.Object context)
	{
		#if DEBUG_LOG_ON
		Debug.LogError(message, context);
		#endif
	}
}

