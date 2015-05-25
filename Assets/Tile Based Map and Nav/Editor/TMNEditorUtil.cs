// ====================================================================================================================
//
// Created by Leslie Young
// http://www.plyoung.com/ or http://plyoung.wordpress.com/
// ====================================================================================================================

using UnityEditor;
using UnityEngine;


public class TMNEditorUtil 
{

	private static Texture2D _dummyTexture = null;
	public static Texture2D DummyTexture { get{if (_dummyTexture == null) _dummyTexture = CreateTexture("dummyTexture", 1, 1, Color.white); return _dummyTexture; } }

	public static Texture2D CreateTexture(string name, int w, int h, Color col)
	{
		Texture2D t = new Texture2D(w, h);
		t.hideFlags = HideFlags.DontSave;
		t.filterMode = FilterMode.Point;
		t.name = name;
		for (int i=0; i<w; i++)
		{
			for (int j=0; j<h; j++) t.SetPixel(i,j, col);
		}
		t.Apply();
		return t;
	}

	static public bool BeginToggleGroup(string label, bool fold)
	{
		GUILayout.Space(15f);
		return EditorGUILayout.BeginToggleGroup(label, fold);
	}

	static public void DrawSpacer()
	{
		EditorGUILayout.Space();
		if (Event.current.type == EventType.Repaint)
		{
			Texture2D tex = DummyTexture;
			Rect rect = GUILayoutUtility.GetLastRect();
			GUI.color = new Color(1f, 1f, 1f, 0.1f);
			GUI.DrawTexture(new Rect(0f, rect.yMax-2f, Screen.width, 1f), tex);
			GUI.color = Color.white;
		}
	}
}
