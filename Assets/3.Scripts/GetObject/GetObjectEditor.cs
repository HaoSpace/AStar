using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(GetObject))]
public class GetObjectEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		GetObject myScript = (GetObject)target;
		
		if(GUILayout.Button("Instatiate Data"))
		{
			myScript.InstatiateData();
		}

		if(GUILayout.Button("Read CSV"))
		{
			myScript.Read();
		}
	}
}