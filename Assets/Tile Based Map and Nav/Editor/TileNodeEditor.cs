// ====================================================================================================================
//
// Created by Leslie Young
// http://www.plyoung.com/ or http://plyoung.wordpress.com/
// ====================================================================================================================

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CanEditMultipleObjects]
[CustomEditor(typeof(TileNode))]
public class TileNodeEditor : Editor
{
	private SerializedProperty idx;
	private SerializedProperty tileTypeMask;
	private int v_tileTypeMask = 0;

	void OnEnable()
	{
		idx = serializedObject.FindProperty("idx");
		tileTypeMask = serializedObject.FindProperty("tileTypeMask");
		v_tileTypeMask = tileTypeMask.intValue;
	}

	public override void OnInspectorGUI()
	{
		EditorGUILayout.Space();
		serializedObject.Update();

		// show node id only when one node selected
		if (!idx.hasMultipleDifferentValues)
		{
			EditorGUILayout.PropertyField(idx, new GUIContent("Unique ID"));
		}

		// can edit multiple nodes' masks at same time
		if (idx.hasMultipleDifferentValues)
		{
			int new_value = (int)((TileNode.TileType)EditorGUILayout.EnumMaskField("Tile Type Mask", (TileNode.TileType)v_tileTypeMask));
			if (new_value != v_tileTypeMask)
			{	// only update masks on ndoes if value actually changed, else this will mess up the values if user just selected a few nodes without doing anything else
				v_tileTypeMask = new_value;
				tileTypeMask.intValue = new_value;
				serializedObject.ApplyModifiedProperties();
			}

			// show the Link OnOff Switch button
			GUILayout.Label("Link between selected: ", EditorStyles.boldLabel);
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Create Link")) CreateLinkBetweenSelected();
			EditorGUILayout.EndHorizontal();

			// show the Link OnOff Switch button
			GUILayout.Label("Set Link between selected: ", EditorStyles.boldLabel);
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Off")) SetLinkBetweenSelected(false);
			if (GUILayout.Button("On")) SetLinkBetweenSelected(true);
			EditorGUILayout.EndHorizontal();
		}
		else
		{
			tileTypeMask.intValue = (int)((TileNode.TileType)EditorGUILayout.EnumMaskField("Tile Type Mask", (TileNode.TileType)tileTypeMask.intValue));
			serializedObject.ApplyModifiedProperties();
		}

		EditorGUILayout.Space();
	}

	private void CreateLinkBetweenSelected()
	{
		// get list of selected nodes
		List<TileNode> nodes = new List<TileNode>();
		foreach (GameObject go in Selection.gameObjects)
		{
			TileNode n = go.GetComponent<TileNode>();
			if (n != null) nodes.Add(n);
		}

		// now update links between these nodes
		foreach (TileNode n in nodes)
		{
			TNEForcedLink ls = n.gameObject.GetComponent<TNEForcedLink>();
			if (ls == null) ls = n.gameObject.AddComponent<TNEForcedLink>();

			// set link state with all other selected nodes
			foreach (TileNode n2 in nodes)
			{
				if (n2 != n)
				{
					ls.LinkWith(n2);
				}
			}
		}
	}

	private void SetLinkBetweenSelected(bool on)
	{
		// get list of selected nodes
		List<TileNode> nodes = new List<TileNode>();
		foreach (GameObject go in Selection.gameObjects)
		{
			TileNode n = go.GetComponent<TileNode>();
			if (n != null) nodes.Add(n);
		}

		// now update links between these nodes
		foreach (TileNode n in nodes)
		{
			TNELinksOnOffSwitch ls = n.gameObject.GetComponent<TNELinksOnOffSwitch>();
			if (ls == null) ls = n.gameObject.AddComponent<TNELinksOnOffSwitch>();

			// set link state with all other selected nodes
			foreach (TileNode n2 in nodes)
			{
				if (n2 != n)
				{
					// first do a neighbours check if possible
					if (n.nodeLinks != null)
					{
						if (!n.IsDirectNeighbour(n2)) continue;
					}

					ls.SetLinkStateWith(n2, on);
				}
			}
		}

	}

	// ====================================================================================================================
}
