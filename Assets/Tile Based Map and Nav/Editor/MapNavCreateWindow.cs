// ====================================================================================================================
//
// Created by Leslie Young
// http://www.plyoung.com/ or http://plyoung.wordpress.com/
// ====================================================================================================================

using UnityEngine;
using UnityEditor;

public class MapNavCreateWindow : EditorWindow
{
	private GameObject tileNodeFab = null;
	private int tileLayer = 0;
	private int tilesLayout = 0;
	private float tileSpacing = 1f;
	private float tileSize = 1f;
	private bool oneUnitPerTileOnly = false;
	private bool oneUnitExceptionAllowMoveOver = false;
	private int[] edNewNodesXY = { 0, 0 };
	private TileNode.TileType initialTileTypeMask = (TileNode.TileType.Normal | TileNode.TileType.Air);

	[MenuItem("Window/Map and Nav/New MapNav")]
	static void Init()
	{
		MapNavCreateWindow window = EditorWindow.GetWindow<MapNavCreateWindow>();
		window.title = "New MapNav";
	}

	void OnGUI()
	{
		GUILayout.Label("Layers", EditorStyles.boldLabel);
		tileLayer = EditorGUILayout.LayerField("Tiles Layer", tileLayer);
		oneUnitPerTileOnly = EditorGUILayout.Toggle("One Unit Per Tile Only", oneUnitPerTileOnly);
		oneUnitExceptionAllowMoveOver = EditorGUILayout.Toggle("One Unit Exception", oneUnitExceptionAllowMoveOver);

		EditorGUILayout.Space();
		tilesLayout = EditorGUILayout.Popup("Tiles Layout", tilesLayout, MapNavEditor.TilesLayoutStrings);

		EditorGUILayout.Space();
		tileSpacing = EditorGUILayout.FloatField("Tile Spacing", tileSpacing);
		tileSize = EditorGUILayout.FloatField("Tile Size", tileSize);

		EditorGUILayout.Space();
		GUILayout.Label("Create Tile Nodes", EditorStyles.boldLabel);
		GUILayout.Label("Ignore the following if you don't want to create tiles now", EditorStyles.label);
		tileNodeFab = (GameObject)EditorGUILayout.ObjectField("TileNode Prefab", tileNodeFab, typeof(GameObject), false);
		initialTileTypeMask = (TileNode.TileType)EditorGUILayout.EnumMaskField("Initial Tile Mask", initialTileTypeMask);
		EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Width x Length: ", EditorStyles.label);
			edNewNodesXY[0] = EditorGUILayout.IntField(edNewNodesXY[0]);
			edNewNodesXY[1] = EditorGUILayout.IntField(edNewNodesXY[1]);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space();
		if (GUILayout.Button("Create MapNav"))
		{
			CreateMapNav();
			Close();
		}
	}

	private void CreateMapNav()
	{
		GameObject go = new GameObject();
		go.name = "MapNav";
		MapNav mn = go.AddComponent<MapNav>();
		mn.tilesLayer = tileLayer;
		mn.tilesLayout = (MapNav.TilesLayout)tilesLayout;
		mn.tileSpacing = tileSpacing;
		mn.tileSize = tileSize;
		mn.oneUnitPerTileOnly = oneUnitPerTileOnly;
		mn.oneUnitExceptionAllowMoveOver = oneUnitExceptionAllowMoveOver;

		if (edNewNodesXY[0] > 0 && edNewNodesXY[1] > 0 && tileNodeFab != null)
		{
			MapNav.CreateTileNodes(tileNodeFab, mn, (MapNav.TilesLayout)tilesLayout, tileSpacing, tileSize, initialTileTypeMask, edNewNodesXY[0], edNewNodesXY[1]);
			mn.LinkNodes();
		}
	}

	// ====================================================================================================================
}
