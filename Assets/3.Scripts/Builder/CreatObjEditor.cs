using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CreatObj))]
public class CreatObjEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		CreatObj myScript = (CreatObj)target;

		if(GUILayout.Button("Build Object"))
		{
			myScript.CreateBlock();
		}

		if(GUILayout.Button("Clear All"))
		{
			myScript.ClearAll();
		}
	}
}