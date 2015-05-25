// ====================================================================================================================
//
// Created by Leslie Young
// http://www.plyoung.com/ or http://plyoung.wordpress.com/
// ====================================================================================================================

using UnityEngine;
using System.Collections;

public class GameControllerSample6 : MonoBehaviour 
{
	// This is a simple sample of how to spawn a unit on a tile that was clicked

	public CameraMove camMover;	// used to move camera around (like make it follow a transform)
	public Camera rayCam;
	public GameObject unitFab;	// unit prefab
	public MapNav map;			// the mapnav
	public LayerMask tilesLayer;// layer the tiles are on

	IEnumerator Start()
	{
		// wait for a frame for everything else to start and then enable the colliders for the TielNodes
		yield return null;

		// now enable the colliders of the TileNodes.
		// they are disabled by default, but for this sample to work I need the player to be able to click on any tile.
		// for your game you will have to decide when the best time would be to this or even which tiles would be
		// best to enable. For example, you might only want to spawn new units around some building, so only
		// enable the the tiles around the building so that the player cannot click on other tiles and disable 
		// the tiles whne yo uare done with them

		foreach (TileNode n in map.nodes)
		{
			n.collider.enabled = true;
		}

	}

	void Update()
	{
		// don;t do anything else if there was not a jouse click
		if (!Input.GetMouseButtonUp(0)) return;

		// cast a ray to check what the player "clicked" on. Only want to know 
		// about TILE clicks, so pass mask to check against layer for tiles only
		Ray ray = rayCam.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 500f, tilesLayer))
		{
			// a tile gameobject was clicked on
			
			// get the TileNode
			TileNode node = hit.collider.GetComponent<TileNode>();
			if (node == null) return; // sanity check
			
			// dont spawn here if there is alrelady a unit on this tile
			if (node.units.Count > 0) return;

			// finally, spawn a unit on the tile
			Unit.SpawnUnit(unitFab, map, node);

		}

	}

	// ====================================================================================================================
}
