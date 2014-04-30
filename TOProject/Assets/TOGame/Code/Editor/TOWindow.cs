using System.IO;
using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public class TOWindow : EditorWindow
{
	[MenuItem ("Bewarewolf/Open Window")]
	static void Init () 
	{
		// Get existing open window or if none, make a new one:
		TOWindow window = (TOWindow)EditorWindow.GetWindow (typeof (TOWindow));
		window.position = new Rect(100,100,300,500);
		window.title = "Bewarewolf";
		window.Show(); 
	} 
	
	public void OnGUI()
	{
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("Generate Map"))
		{
			var root = GetMapEditorTransform();

			if(root != null)
			{
				MapEditor.GenerateMap(root,false);
			}
		}
		else if(GUILayout.Button("Clear Map"))
		{
			var root = GetMapEditorTransform();
			
			if(root != null)
			{
				MapEditor.GenerateMap(root,true);
			}
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		Config.SHOULD_DEBUG_BLOCKING_RECTS = GUILayout.Toggle(Config.SHOULD_DEBUG_BLOCKING_RECTS,"Debug Rects?");
		EditorGUILayout.EndHorizontal();
	} 

	public Transform GetMapEditorTransform()
	{
		var trans = UnityEngine.Object.FindObjectsOfType<Transform>();
		foreach(var tran in trans)
		{
			if(tran.name == "MapEditor")
			{
				return tran;
			}
		}
		return null;
	}
} 

