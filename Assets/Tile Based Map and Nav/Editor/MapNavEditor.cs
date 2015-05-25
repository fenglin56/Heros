// ====================================================================================================================
//
// Created by Leslie Young
// http://www.plyoung.com/ or http://plyoung.wordpress.com/
// ====================================================================================================================

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(MapNav))]
public class MapNavEditor : Editor
{
	public static string[] TilesLayoutStrings = { "Hex (6 neighbours)", "Square (4 neighbours)", "Square (8 neighbours)" };

	private static int[] edNewNodesXY = { 0, 0 };

	private SerializedProperty gizmoDrawNodes;
	private SerializedProperty gizmoDrawLinks;
	private SerializedProperty gizmoColorCoding;
	private SerializedProperty tilesLayer;
	private SerializedProperty tilesLayout;
	private SerializedProperty tileSpacing;
	private SerializedProperty tileSize;
	private SerializedProperty oneUnitPerTileOnly;
	private SerializedProperty oneUnitExceptionAllowMoveOver;
	private SerializedProperty nodesCache;

	private static GameObject new_TileNodeFab = null;
	private static int new_tilesLayout = -2;
	private static float new_tileSpacing = 0.0f;
	private static float new_tileSize = 0.0f;
	private static TileNode.TileType initialTileTypeMask = (TileNode.TileType.Normal | TileNode.TileType.Air);

	private static Transform maskAutomation_ParentObject = null;
	private static LayerMask maskAutomation_ColliderMask = 0;
	private static TileNode.TileType maskAutomation_ResetMask = 0;
	private static TileNode.TileType maskAutomation_AddMask = 0;

	private static LayerMask heightAutomation_Mask = 0;
	private static bool heightAutomation_DelNodes = false;

	private static float maxHeightBeforeLinkBreak = 1f;

	private static bool[] folds = { false, false, false, false, true, true, false, false };

	void OnEnable()
	{
		gizmoDrawNodes = serializedObject.FindProperty("gizmoDrawNodes");
		gizmoDrawLinks = serializedObject.FindProperty("gizmoDrawLinks");
		gizmoColorCoding = serializedObject.FindProperty("gizmoColorCoding");
		tilesLayer = serializedObject.FindProperty("tilesLayer");
		tilesLayout = serializedObject.FindProperty("tilesLayout");
		tileSpacing = serializedObject.FindProperty("tileSpacing");
		tileSize = serializedObject.FindProperty("tileSize");
		oneUnitPerTileOnly = serializedObject.FindProperty("oneUnitPerTileOnly");
		oneUnitExceptionAllowMoveOver = serializedObject.FindProperty("oneUnitExceptionAllowMoveOver");
		nodesCache = serializedObject.FindProperty("nodesCache");

		if (new_tilesLayout == -2) new_tilesLayout = tilesLayout.intValue;
		if (new_tileSpacing == 0.0f) new_tileSpacing = tileSpacing.floatValue;
		if (new_tileSize == 0.0f) new_tileSize = tileSize.floatValue;

		if (((MapNav)target).nodesXCount > 0 && edNewNodesXY[0] == 0) edNewNodesXY[0] = ((MapNav)target).nodesXCount;
		if (((MapNav)target).nodesYCount > 0 && edNewNodesXY[1] == 0) edNewNodesXY[1] = ((MapNav)target).nodesYCount;
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		// gizmo related and info fields
		TMNEditorUtil.DrawSpacer();
		folds[4] = EditorGUILayout.Foldout(folds[4], folds[4] ? "" : "Gizmos");
		if (folds[4])
		{
			GUILayout.Label("Gizmos", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(gizmoDrawNodes);
			EditorGUILayout.PropertyField(gizmoDrawLinks);
			EditorGUILayout.PropertyField(gizmoColorCoding);
		}

		// info
		TMNEditorUtil.DrawSpacer();
		folds[5] = EditorGUILayout.Foldout(folds[5], folds[5] ? "" : "Basic Settings & Info" );
		if (folds[5])
		{
			GUILayout.Label("Info", EditorStyles.boldLabel);
			EditorGUILayout.LabelField("MapNav Size: ", nodesCache.arraySize.ToString() + " (" + ((MapNav)target).nodesXCount + "x" + ((MapNav)target).nodesYCount + ")");
			EditorGUILayout.LabelField("Tiles Layout: ", ((MapNav.TilesLayout)tilesLayout.enumValueIndex).ToString());
			EditorGUILayout.LabelField("Tile Spacing: ", tileSpacing.floatValue.ToString());
			EditorGUILayout.LabelField("Tile Size: ", tileSize.floatValue.ToString());

			// basic fields
			EditorGUILayout.Space();
			GUILayout.Label("Basic Settings", EditorStyles.boldLabel);
			tilesLayer.intValue = EditorGUILayout.LayerField("Tiles Layer", tilesLayer.intValue);
			EditorGUILayout.PropertyField(oneUnitPerTileOnly);
			oneUnitExceptionAllowMoveOver.boolValue = EditorGUILayout.Toggle("One Unit Exception", oneUnitExceptionAllowMoveOver.boolValue);
		}

		// ...
		serializedObject.ApplyModifiedProperties();

		TMNEditorUtil.DrawSpacer();

		// fields to create new tile grid
		folds[0] = TMNEditorUtil.BeginToggleGroup("TileNodes Create Tool", folds[0]);
		if (folds[0])
		{
			GUILayout.Label("Note that this will delete current nodes", EditorStyles.label);
			EditorGUILayout.Space();
			new_TileNodeFab = (GameObject)EditorGUILayout.ObjectField("TileNode Prefab", new_TileNodeFab, typeof(GameObject), false);
			new_tilesLayout = EditorGUILayout.Popup("Tiles Layout", new_tilesLayout, MapNavEditor.TilesLayoutStrings);
			new_tileSpacing = EditorGUILayout.FloatField("Tile Spacing", new_tileSpacing);
			new_tileSize = EditorGUILayout.FloatField("Tile Size", new_tileSize);
			initialTileTypeMask = (TileNode.TileType)EditorGUILayout.EnumMaskField("Initial Tile Mask", initialTileTypeMask);

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Width x Length: ", EditorStyles.label);
			edNewNodesXY[0] = EditorGUILayout.IntField(edNewNodesXY[0]);
			edNewNodesXY[1] = EditorGUILayout.IntField(edNewNodesXY[1]);
			EditorGUILayout.EndHorizontal();
			if (GUILayout.Button("Create"))
			{
				tilesLayout.intValue = new_tilesLayout;
				tileSpacing.floatValue = new_tileSpacing;
				tileSize.floatValue = new_tileSize;

				if (edNewNodesXY[0] <= 0) edNewNodesXY[0] = 1;
				if (edNewNodesXY[1] <= 0) edNewNodesXY[1] = 1;

				//GameObject nodeFab = (GameObject)AssetDatabase.LoadAssetAtPath(MapNavEditor.PREFABS_PATH + "tile_nodes/" + ((MapNav.TilesLayout)new_tilesLayout == MapNav.TilesLayout.Hex ? "TIleNode_Hex.prefab" : "TIleNode_Square.prefab"), typeof(GameObject));
				//MapNav.CreateTileNodes(nodeFab, ((MapNav)target), (MapNav.TilesLayout)new_tilesLayout, new_tileSpacing, new_tileSize, initialTileTypeMask, edNewNodesXY[0], edNewNodesXY[1]);
				if (new_TileNodeFab != null)
				{
					MapNav.CreateTileNodes(new_TileNodeFab, ((MapNav)target), (MapNav.TilesLayout)new_tilesLayout, new_tileSpacing, new_tileSize, initialTileTypeMask, edNewNodesXY[0], edNewNodesXY[1]);
					((MapNav)target).LinkNodes();
				}
			}
		}
		EditorGUILayout.EndToggleGroup();		

		// various tools
		if (((MapNav)target).nodesCache != null)
		{
			// hide show node markers
			folds[1] = TMNEditorUtil.BeginToggleGroup("TileNodes Marker Visibility", folds[1]);
			if (folds[1])
			{
				GUILayout.Space(10f);
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("Show"))
				{
					((MapNav)target).ShowTileNodeMarkers(true);
					EditorUtility.SetDirty(target);
				}
				if (GUILayout.Button("Hide"))
				{
					((MapNav)target).ShowTileNodeMarkers(false);
					EditorUtility.SetDirty(target);
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndToggleGroup();

			// automated setup of nodes' masks
			folds[2] = TMNEditorUtil.BeginToggleGroup("TileNodes Auto Mask-Setup Tool", folds[2]);
			if (folds[2])
			{
				GUILayout.Space(10f);
				maskAutomation_ParentObject = (Transform)EditorGUILayout.ObjectField("Use Parent Object", maskAutomation_ParentObject, typeof(Transform), true);
				if (maskAutomation_ParentObject != null) maskAutomation_ColliderMask = 0;
				maskAutomation_ColliderMask = EditorGUILayout.LayerField("or use Colliders Layer", maskAutomation_ColliderMask);
				if (maskAutomation_ColliderMask != 0) maskAutomation_ParentObject = null;
				GUILayout.Space(10f);
				maskAutomation_AddMask = (TileNode.TileType)EditorGUILayout.EnumMaskField("New TileNode Mask", maskAutomation_AddMask);
				GUILayout.Space(10f);
				EditorGUILayout.BeginHorizontal();
					if (GUILayout.Button("Set"))
					{
						if (maskAutomation_ParentObject != null) ((MapNav)target).SetTileNodeMasks(maskAutomation_AddMask, maskAutomation_ParentObject);
						else ((MapNav)target).SetTileNodeMasks(maskAutomation_AddMask, maskAutomation_ColliderMask);
						EditorUtility.SetDirty(target);
					}
					if (GUILayout.Button("Add"))
					{
						if (maskAutomation_ParentObject != null) ((MapNav)target).AddToTileNodeMasks(maskAutomation_AddMask, maskAutomation_ParentObject);
						else ((MapNav)target).AddToTileNodeMasks(maskAutomation_AddMask, maskAutomation_ColliderMask);
						EditorUtility.SetDirty(target);
					}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.HelpBox("This tool helps automate the TileNode Mask setup process.\n\nExample, if you wanted to set nodes under tree meshes to be of Wall type, then you would set 'Parent Object' to the container of the trees and select from 'TileNode Mask' the Wall type (if you defined such in TileNode.TileType) and hit Add/Set.\n\nThe tool will go through all child transforms of the Parent Object and check which nodes are under them and update their tile-type masks.\n\nNote that 'Add' will add the mask to a node's current mask while 'Set' will reset the node mask to the given mask.\n\nYou also have the option to rather check against colliders of objects in your scene. This is useful where an object like a wall might span more than one tile and you want those tiles all to be set to tile type, wall. In this case you want to set 'Colliders Layer' to the layer of the objects to test against and make sure they have colliders since this will use ray casting. Make sure 'Parent Object' is not set in this case else this option will be ignored and Parent Object will be used.", MessageType.None);
			}
			EditorGUILayout.EndToggleGroup();

			// reseting nodes' masks
			folds[3] = TMNEditorUtil.BeginToggleGroup("TileNodes Mask Reset Tool", folds[3]);
			if (folds[3])
			{
				GUILayout.Space(10f);
				maskAutomation_ResetMask = (TileNode.TileType)EditorGUILayout.EnumMaskField("Set all to", maskAutomation_ResetMask);
				if (GUILayout.Button("Reset"))
				{
					((MapNav)target).SetAllNodeMasksTo(maskAutomation_ResetMask);
					EditorUtility.SetDirty(target);
				}
			}
			EditorGUILayout.EndToggleGroup();

			// calc node heights
			folds[6] = TMNEditorUtil.BeginToggleGroup("TileNodes Auto Height-Setup Tool ", folds[6]);
			if (folds[6])
			{
				GUILayout.Space(10f);
				heightAutomation_Mask = EditorGUILayout.LayerField("Colliders Layer", heightAutomation_Mask);
				heightAutomation_DelNodes = EditorGUILayout.Toggle("Node Deletion Active", heightAutomation_DelNodes);
				if (GUILayout.Button("Run Setup"))
				{
					((MapNav)target).SetupNodeHeights(heightAutomation_Mask, heightAutomation_DelNodes);
					EditorUtility.SetDirty(target);
				}
				EditorGUILayout.HelpBox("This tool helps automate the TileNodes height setup. Set the 'Layer' field to the layer used by objects that will determine what height the nodes must be placed at. It is important that the object (for example a variable height terrain) use the same layer and has a collider since this tool will use raycasts the determine at what height to place nodes.\n\nIf Node Deletion is Active, then nodes that did not find anything to adjust their heights against will be unlinked and deleted. This is usefull if you have a non-square terrain area with some overlapping nodes that won't be used and must be deleted.", MessageType.None);
			}
			EditorGUILayout.EndToggleGroup();

			// auto link setup
			folds[7] = TMNEditorUtil.BeginToggleGroup("TileNodes Auto Link-Setup Tool ", folds[7]);
			if (folds[7])
			{
				GUILayout.Space(10f);
				maxHeightBeforeLinkBreak = EditorGUILayout.FloatField("Max Height", maxHeightBeforeLinkBreak);
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("Run Setup"))
				{
					((MapNav)target).SetupNodeLinkSwitches(maxHeightBeforeLinkBreak);
					EditorUtility.SetDirty(target);
				}
				if (GUILayout.Button("Reset ALL"))
				{
					((MapNav)target).RemoveNodeLinkSwitches();
					EditorUtility.SetDirty(target);
				}
				if (GUILayout.Button("Delete Unlinked"))
				{
					((MapNav)target).DeleteAllUnlinkedNodes();
					EditorUtility.SetDirty(target);
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.HelpBox("This tool helps automate setting up the links betwene nodes, depending on the height difference betwene the nodes. Enter a max height and links between nodes will be turned off if the height difference between them are higher.\n\nThe 'Delete' button allows for the auto-deletion of all nodes that have no active links.", MessageType.None);

			}
			EditorGUILayout.EndToggleGroup();
		}

	}

	// ====================================================================================================================
}
