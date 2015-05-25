// ====================================================================================================================
//
// Created by Leslie Young
// http://www.plyoung.com/ or http://plyoung.wordpress.com/
// ====================================================================================================================

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(RadiusMarker))]
public class RadiusMarkerEditor : Editor
{
	private MapNav.TilesLayout markerLayout = MapNav.TilesLayout.Hex;	// kind of tile layout
	private float markerSize = 1f;		// the size of one marker node (normally same size as the tiles)
	private float markerSpacing = 1f;	// spacing between amrker nodes (normally same as tile)
	private int markerRadius = 1;		// furthest tile this marker could indicate
	private int tilesMask = 0;
	private bool adaptToTileHeight = false;
	public GameObject markerFab;

	void OnEnable()
	{
		RadiusMarker marker = (target as RadiusMarker);
		markerLayout = marker.markerLayout;
		markerSpacing = marker.markerSpacing;
		markerSize = marker.markerSize;
		markerRadius = marker.markerRadius;
		markerFab = marker.prefab;
		adaptToTileHeight = marker.adaptToTileHeight;
		tilesMask = marker.tilesMask;
	}

	public override void OnInspectorGUI()
	{
		EditorGUILayout.Space();
		markerFab = (GameObject)EditorGUILayout.ObjectField("Marker Node Prefab", markerFab, typeof(GameObject), false);
		markerLayout = (MapNav.TilesLayout)EditorGUILayout.EnumPopup("Marker Layout", markerLayout);
		markerSpacing = EditorGUILayout.FloatField("Marker Node Spacing", markerSpacing);
		markerSize = EditorGUILayout.FloatField("Marker Node Size", markerSize);
		adaptToTileHeight = EditorGUILayout.Toggle("Adapt to Tile Height", adaptToTileHeight);
		GUI.enabled = adaptToTileHeight;
		tilesMask = EditorGUILayout.LayerField("Tiles Layer", tilesMask);
		GUI.enabled = true;
		EditorGUILayout.BeginHorizontal();
			markerRadius = EditorGUILayout.IntField("Marker Radius", markerRadius);
			if (GUILayout.Button("-")) { markerRadius--; RadiusMarker.UpdateMarker(markerFab, (RadiusMarker)target, markerLayout, markerSpacing, markerSize, markerRadius, adaptToTileHeight, tilesMask); }
			if (GUILayout.Button("+")) { markerRadius++; RadiusMarker.UpdateMarker(markerFab, (RadiusMarker)target, markerLayout, markerSpacing, markerSize, markerRadius, adaptToTileHeight, tilesMask); }
		EditorGUILayout.EndHorizontal();

		//  update the marker with new values
		EditorGUILayout.Space();
		if (GUILayout.Button("Update"))
		{
			RadiusMarker.UpdateMarker(markerFab, (RadiusMarker)target, markerLayout, markerSpacing, markerSize, markerRadius, adaptToTileHeight, tilesMask);
		}
	}

	// ====================================================================================================================
}
