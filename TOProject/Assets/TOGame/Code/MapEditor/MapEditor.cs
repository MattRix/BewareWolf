using UnityEngine;
using System.Collections;

public class MapEditor : MonoBehaviour 
{

	public static string result = "";

	// Use this for initialization
	void Start () 
	{

	}

	[ContextMenu ("ClearMap")]
	public void ClearMap() 
	{
		GenerateMap(transform, true);
	}
	
	[ContextMenu ("ExportMap")]
	public void ExportMap()
	{
		GenerateMap(transform, false);
	}

	public static void GenerateMap(Transform transform, bool shouldClear) 
	{
		result = "";
		result += "using System;\n";
		result += "public class MapGenerator {\n\n";
		
		result += "public static MapData Generate(){\n\n";
		result += "MapData md = new MapData();\n\n";
		
		if(!shouldClear)
		{
			for(int c = 0; c<transform.childCount; c++)
			{
				var child = transform.GetChild(c);
				
				if(child.name == "StartPos")
				{
					AddObject("StartPos",child);
				}
				else if(child.name == "House_1")
				{
					AddObject("House1",child);
				}
				else if(child.name == "FenceH")
				{
					AddObject("FenceH",child);
				}
				else if(child.name == "FenceV")
				{
					AddObject("FenceV",child);
				}
			}
		}
		
		result += "\nreturn md;}}";
		
		WriteFile(result);

		if(shouldClear)
		{
			Debug.Log("Map cleared!");
		}
		else 
		{
			Debug.Log("Map written!");
		}
		
	}
	
	static void AddObject(string name, Transform child)
	{
		int pixelX = Mathf.RoundToInt(child.transform.position.x * 100f);
		int pixelY = Mathf.RoundToInt(child.transform.position.y * 100f);
		result += "md.Add(new "+name+"MI("+pixelX+","+pixelY+"));\n";
	}
	
	public static string WriteFile(string text)
	{
		#if UNITY_EDITOR && !UNITY_WEBPLAYER
		string fullPath = 
			System.IO.Directory.GetCurrentDirectory()+
				System.IO.Path.DirectorySeparatorChar+
				"Assets"+
				System.IO.Path.DirectorySeparatorChar+
				"TOGame"+
				System.IO.Path.DirectorySeparatorChar+
				"Code"+
				System.IO.Path.DirectorySeparatorChar+
				"MapEditor"+
				System.IO.Path.DirectorySeparatorChar+
				"GeneratedMap.cs";
		
		
		System.IO.File.WriteAllText(fullPath, text);
		
		//var info:FileInfo = new FileInfo("/Applications/TextEdit.app/Contents/MacOS/TextEdit");
		//System.Diagnostics.Process.Start(info.FullName);
		
		return fullPath;
		#else 
		return "COULD NOT WRITE";
		#endif
	}
}
