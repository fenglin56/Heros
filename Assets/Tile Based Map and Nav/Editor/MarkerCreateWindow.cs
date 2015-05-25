// ====================================================================================================================
//
// Created by Leslie Young
// http://www.plyoung.com/ or http://plyoung.wordpress.com/
// ====================================================================================================================

using UnityEngine;
using UnityEditor;

public class MarkerCreateWindow : EditorWindow
{
	private int markerLayout = 0;
	private float markerSize = 1f;
	private float markerSpacing = 1f;
	private int markerRadius = 1;
	private int tilesMask = 0;
	private bool adaptToTileHeight = false;	
	private GameObject markerFab;

	[MenuItem("Window/Map and Nav/New Marker")]
	static void Init()
	{
		MarkerCreateWindow window = EditorWindow.GetWindow<MarkerCreateWindow>();
		window.title = "New Marker";
	}

	void OnGUI()
	{
		EditorGUILayout.Space();
		GUILayout.Label("Choosing values similar to your MapNav\ntile values will work best here", EditorStyles.label);
		EditorGUILayout.Space();
		markerFab = (GameObject)EditorGUILayout.ObjectField("Marker Node Prefab", markerFab, typeof(GameObject), false);
		markerLayout = EditorGUILayout.Popup("Marker Layout", markerLayout, MapNavEditor.TilesLayoutStrings);
		markerSpacing = EditorGUILayout.FloatField("Marker Node Spacing", markerSpacing);
		markerSize = EditorGUILayout.FloatField("Marker Node Size", markerSize);
		markerRadius = EditorGUILayout.IntField("Marker Radius", markerRadius);
		adaptToTileHeight = EditorGUILayout.Toggle("Adapt to Tile Height", adaptToTileHeight);
		GUI.enabled = adaptToTileHeight;
		tilesMask = EditorGUILayout.LayerField("Tiles Layer", tilesMask);
		GUI.enabled = true;

		EditorGUILayout.Space();
		if (GUILayout.Button("Create Marker"))
		{
			RadiusMarker.CreateMarker(markerFab, (MapNav.TilesLayout)markerLayout, markerSpacing, markerSize, markerRadius, adaptToTileHeight, tilesMask);
			Close();
		}
	}

	// ====================================================================================================================
}
