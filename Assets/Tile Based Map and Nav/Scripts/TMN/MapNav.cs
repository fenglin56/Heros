// ====================================================================================================================
//
// Created by Leslie Young
// http://www.plyoung.com/ or http://plyoung.wordpress.com/
// ====================================================================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class MapNav : MonoBehaviour 
{
	// ====================================================================================================================
	#region inspector properties

	public bool gizmoDrawNodes = true;
	public bool gizmoDrawLinks = true;
	public bool gizmoColorCoding = true;

	public int tilesLayer = 0;							// on what layer is tiles
	public TilesLayout tilesLayout = TilesLayout.Hex;	// kind of tile layout
	public float tileSpacing = 1f;						// space between tiles
	public float tileSize = 1f;							// how big is a tile
	public bool oneUnitPerTileOnly = false;				// set to true if you don't ever want more than one unit on a tile
	public bool oneUnitExceptionAllowMoveOver = false;	// only used when oneUnitPerTileOnly=true; this will allow units on different layers to move over other uits. for example, air unit can move OVER land unit, but not stop on same tile

	public GameObject[] nodesCache;	// (note) I actually want TileNode, but Unity crash on play/load with huge maps if I cache the TileNode Component rather than the GameObject, here
	public int nodesXCount = 0, nodesYCount = 0;

	#endregion
	// ====================================================================================================================
	#region vars

	public enum TilesLayout : byte { Hex = 0, Square_4 = 1, Square_8 = 2 }

	// get a TileNode at index of nodesCache
	public TileNode this[int index]
	{ 
		get 
		{
			if (nodesCache == null) return null;
			if (index < 0) return null;
			if (index >= nodesCache.Length) return null;
			if (nodesCache[index] == null) return null;
			return nodesCache[index].GetComponent<TileNode>(); 
		} 
	}

	// shortcut to getting the length of nodesCache. Used together with "TileNode this[int index]"
	// above it makes MapNav act like an array of nodes
	public int Length 
	{ 
		get 
		{
			if (nodesCache == null) return 0;
			return nodesCache.Length; 
		} 
	}

	// this is inited in LinkNodes() but only if nodesCache is set
	public TileNode[] nodes { get; set; }

	#endregion
	// ====================================================================================================================
	#region pub

	void Start() 
	{
		LinkNodes();
		ShowAllTileNodes(true);
	}

	/// <summary>
	/// hide and disable or show/enable all TileNodes
	/// </summary>
	public void ShowAllTileNodes(bool show)
	{
		if (nodes == null) return;
		foreach (TileNode n in nodes)
		{
			if (n == null) continue;
			n.Show(show);
		}
	}

	/// <summary>
	/// Only touches the markers which means it does not "enable/disable" the tile by touching the collider too
	/// </summary>
	public void ShowTileNodeMarkers(bool show)
	{
		if (nodesCache == null) return;
		foreach (GameObject go in nodesCache)
		{
			if (go == null) continue;
			go.renderer.enabled = show;
		}
	}

	/// <summary>
	/// Get a path from - to node
	/// </summary>
	public TileNode[] GetPath(TileNode fromNode, TileNode toNode)
	{
		return GetPath(fromNode, toNode, 0);
	}

	/// <summary>
	/// Get a path from - to node, checking that the node mask includes validNodesLayer.
	/// Pass validNodesLayer=0 to ignore the mask check (oneUnitPerTileOnly is only applied if validNodesLayer is also applied)
	/// </summary>
	public TileNode[] GetPath(TileNode fromNode, TileNode toNode, TileNode.TileType validNodesLayer)
	{
		if (fromNode == null || toNode == null) return null;
		
		// reset
		foreach (TileNode n in nodes)
		{
			if (n == null) continue;
			n.SetPathParent(null, 0, 0);
		}

		const int DistanceConst = 10;
		List<TileNode> openList = new List<TileNode>();
		List<TileNode> closeList = new List<TileNode>();
		TileNode tn = fromNode;
		openList.Add(tn);

		while (openList.Count > 0)
		{
			// find one with lowest F score
			tn = null;
			foreach (TileNode ol in openList)
			{
				if (tn == null) tn = ol;
				else if (ol.PathF < tn.PathF) tn = ol;
			}

			// drop it from open list and add to close list
			closeList.Add(tn);
			openList.Remove(tn);

			if (tn == toNode) break; // reached destination, break

			// find neighbours of this point
			foreach (TileNode n in tn.nodeLinks)
			{
				// ignore null,
				if (n == null) continue;

				// in close set
				if (closeList.Contains(n)) continue;

				// check if it is a tile node that can be used
				if (validNodesLayer > 0)
				{
					// allow only one unit per tile?
					if (oneUnitPerTileOnly)
					{
						if (!oneUnitExceptionAllowMoveOver && n.units.Count > 0) continue;
					}

					// now check if another unit is occupying the same layer/level on the node
					if ((n.tileTypeMask & validNodesLayer) == validNodesLayer)
					{
						if (n.GetUnitInLevel(validNodesLayer) != null) continue;
					}

					// else, not a valid node
					else continue;
				}

				// check if there is link switch between the two nodes and if it is on or off
				if (tn.linkOnOffSwitch != null)
				{
					if (tn.linkOnOffSwitch.LinkIsOn(n) == 0) continue;
				}
				if (n.linkOnOffSwitch != null)
				{
					if (n.linkOnOffSwitch.LinkIsOn(tn) == 0) continue;
				}


				// calc G & H
				int G = tn.PathG + DistanceConst;
				int H = DistanceConst * Mathf.Abs((int)Vector3.Distance(n.transform.position, toNode.transform.position));

				// check if there are movement modifiers
				if (n.movesMod != null)
				{
					foreach (TNEMovementModifier.MovementInfo m in n.movesMod.moveInfos)
					{
						if (m.tileType == validNodesLayer) G += DistanceConst * m.movesModifier;
					}
				}

				// apply
				if (openList.Contains(n))
				{	// open list contains the neighbour
					// check if G score to the neighbour will be lower if followed from this point
					if (G < n.PathG) n.SetPathParent(tn, G, H);
				}
				else
				{	// add neighbour to open list
					n.SetPathParent(tn, G, H);
					openList.Add(n);
				}
			}
		}

		// start at dest and build path
		List<TileNode> path = new List<TileNode>();
		tn = toNode;
		while (tn.PathParent != null)
		{
			path.Add(tn);
			tn = tn.PathParent;
		}

		if (path.Count > 0)
		{	// the path is calculated in reverse, swop it around
			// and then return the array of the elements
			path.Reverse();
			return (TileNode[])path.ToArray();
		}

		// else return null
		return null;
	}

	#endregion
	// ====================================================================================================================
	#region create / setup tools

	/// <summary>
	/// Creates a new irid of tile nodes of x by y count
	/// </summary>
	public static void CreateTileNodes(GameObject nodeFab, MapNav map, MapNav.TilesLayout layout, float tileSpacing, float tileSize, TileNode.TileType initialMask, int xCount, int yCount)
	{
		if (xCount <= 0 || yCount <= 0) return;

		map.tilesLayout = layout;
		map.tileSpacing = tileSpacing;
		map.tileSize = tileSize;

		// first delete the old nodes
		List<GameObject> remove = new List<GameObject>();
		foreach (Transform t in map.transform)
		{	// just make sure it is a node in case there are other kinds of objects under MapNav object
			if (t.name.Contains("node")) remove.Add(t.gameObject);
		}
		remove.ForEach(go => DestroyImmediate(go));

		// now create new nodes

		map.nodesXCount = xCount;
		map.nodesYCount = yCount;
		map.nodesCache = new GameObject[map.nodesXCount * map.nodesYCount];

		int count = 0;
		bool atoffs = false;
		float offs = 0f;
		float xOffs = map.tileSpacing;
		float yOffs = map.tileSpacing;

		if (map.tilesLayout == MapNav.TilesLayout.Hex)
		{	// calculate offset to correctly plate hextiles
			xOffs = Mathf.Sqrt(3f) * map.tileSpacing * 0.5f;
			offs = xOffs * 0.5f;
			yOffs = yOffs * 0.75f;
		}

		for (int y = 0; y < yCount; y++)
		{
			for (int x = 0; x < xCount; x++)
			{
				// create the node
				GameObject go = (GameObject)GameObject.Instantiate(nodeFab);
				go.name = "node" + count.ToString();
				go.layer = map.tilesLayer;

				// parent under MapNav and position the node
				go.transform.parent = map.transform;
				go.transform.localPosition = new Vector3(x * xOffs + (atoffs ? offs : 0f), 0f, y * yOffs);
				go.transform.localScale = new Vector3(map.tileSize, map.tileSize, map.tileSize);

				// update TileNode component
				TileNode n = go.GetComponent<TileNode>();
				n.idx = count;
				n.mapnav = map;
				n.tileTypeMask = initialMask;

				// turn off the collider, don't need it now
				go.collider.enabled = false;

				// cache the node
				map.nodesCache[count] = go;
				count++;
			}
			atoffs = !atoffs;
		}
	}

	/// <summary>
	/// Links TileNodes with their neighbouring nodes
	/// </summary>
	public void LinkNodes()
	{
		if (nodesCache == null) return;
		if (nodesCache.Length == 0) return;
		if (nodesCache.Length != nodesXCount * nodesYCount)
		{
			Debug.LogWarning(string.Format("The number of cached nodes {0} != {1} which was expected", nodesCache.Length, (nodesXCount * nodesYCount)));
			return;
		}

		nodes = new TileNode[nodesCache.Length];

		bool atoffs = false;
		int i = 0;

		// === link nodes with their neighbours (hex tile pattern)
		if (tilesLayout == TilesLayout.Hex)
		{
			atoffs = false;
			for (int y = 0; y < nodesYCount; y++)
			{
				for (int x = 0; x < nodesXCount; x++)
				{
					i = y * nodesXCount + x;
					if (nodesCache[i] == null) { nodes[i] = null; continue; }

					TileNode n = nodesCache[i].GetComponent<TileNode>();
					nodes[i] = n; // cache the component
					n.nodeLinks = new TileNode[6] { null, null, null, null, null, null };

					// link with previous node
					if (x - 1 >= 0) n.nodeLinks[0] = nodesCache[i - 1] == null ? null : nodesCache[i - 1].GetComponent<TileNode>();
					// link with next node
					if (x + 1 < nodesXCount) n.nodeLinks[1] = nodesCache[i + 1] == null ? null : nodesCache[i + 1].GetComponent<TileNode>();

					// link with nodes in previous row
					if (y > 0)
					{
						// prev row, same column
						n.nodeLinks[2] = nodesCache[i - nodesXCount] == null ? null : nodesCache[i - nodesXCount].GetComponent<TileNode>(); ;
						if (atoffs)
						{	// prev row, next column
							if (x + 1 < nodesXCount) n.nodeLinks[4] = nodesCache[i - nodesXCount + 1] == null ? null : nodesCache[i - nodesXCount + 1].GetComponent<TileNode>();
						}
						else
						{	// prev row, prev column
							if (x - 1 >= 0) n.nodeLinks[4] = nodesCache[i - nodesXCount - 1] == null ? null : nodesCache[i - nodesXCount - 1].GetComponent<TileNode>();
						}
					}

					// link with nodes in next row
					if (y + 1 < nodesYCount)
					{
						// next row, same column
						n.nodeLinks[3] = nodesCache[i + nodesXCount] == null ? null : nodesCache[i + nodesXCount].GetComponent<TileNode>();
						if (atoffs)
						{	// prev row, next column
							if (x + 1 < nodesXCount) n.nodeLinks[5] = nodesCache[i + nodesXCount + 1] == null ? null : nodesCache[i + nodesXCount + 1].GetComponent<TileNode>();
						}
						else
						{	// prev row, prev column
							if (x - 1 >= 0) n.nodeLinks[5] = nodesCache[i + nodesXCount - 1] == null ? null : nodesCache[i + nodesXCount - 1].GetComponent<TileNode>();
						}
					}
				}
				atoffs = !atoffs;
			}
		}

		// === link nodes with their neighbours (square tile pattern with 4 neighbours)
		if (tilesLayout == TilesLayout.Square_4)
		{
			for (int y = 0; y < nodesYCount; y++)
			{
				for (int x = 0; x < nodesXCount; x++)
				{
					i = y * nodesXCount + x;
					if (nodesCache[i] == null) { nodes[i] = null; continue; }

					TileNode n = nodesCache[i].GetComponent<TileNode>();
					nodes[i] = n; // cache the component
					n.nodeLinks = new TileNode[4] { null, null, null, null };

					// link with previous node
					if (x - 1 >= 0) n.nodeLinks[0] = nodesCache[i - 1] == null ? null : nodesCache[i - 1].GetComponent<TileNode>();
					// link with next node
					if (x + 1 < nodesXCount) n.nodeLinks[1] = nodesCache[i + 1] == null ? null : nodesCache[i + 1].GetComponent<TileNode>();
					// prev row, same column
					if (y > 0) n.nodeLinks[2] = nodesCache[i - nodesXCount] == null ? null : nodesCache[i - nodesXCount].GetComponent<TileNode>();
					// next row, same column
					if (y + 1 < nodesYCount) n.nodeLinks[3] = nodesCache[i + nodesXCount] == null ? null : nodesCache[i + nodesXCount].GetComponent<TileNode>(); ;
				}
			}
		}

		//=== link nodes with their neighbours (square tile pattern with 8 neighbours)
		if (tilesLayout == TilesLayout.Square_8)
		{
			for (int y = 0; y < nodesYCount; y++)
			{
				for (int x = 0; x < nodesXCount; x++)
				{
					i = y * nodesXCount + x;
					if (nodesCache[i] == null) { nodes[i] = null; continue; }

					TileNode n = nodesCache[i].GetComponent<TileNode>();
					nodes[i] = n; // cache the component
					n.nodeLinks = new TileNode[8] { null, null, null, null, null, null, null, null };

					// link with previous node
					if (x - 1 >= 0) n.nodeLinks[0] = nodesCache[i - 1] == null ? null : nodesCache[i - 1].GetComponent<TileNode>();
					// link with next node
					if (x + 1 < nodesXCount) n.nodeLinks[1] = nodesCache[i + 1] == null ? null : nodesCache[i + 1].GetComponent<TileNode>();

					// link with nodes in previous row
					if (y > 0)
					{
						// prev row, same column
						n.nodeLinks[2] = nodesCache[i - nodesXCount] == null ? null : nodesCache[i - nodesXCount].GetComponent<TileNode>();
						// prev row, prev column
						if (x - 1 >= 0) n.nodeLinks[4] = nodesCache[i - nodesXCount - 1] == null ? null : nodesCache[i - nodesXCount - 1].GetComponent<TileNode>();
						// prev row, next column
						if (x + 1 < nodesXCount) n.nodeLinks[6] = nodesCache[i - nodesXCount + 1] == null ? null : nodesCache[i - nodesXCount + 1].GetComponent<TileNode>();
					}

					// link with nodes in next row
					if (y + 1 < nodesYCount)
					{
						// next row, same column
						n.nodeLinks[3] = nodesCache[i + nodesXCount] == null ? null : nodesCache[i + nodesXCount].GetComponent<TileNode>();
						// prev row, prev column
						if (x - 1 >= 0) n.nodeLinks[5] = nodesCache[i + nodesXCount - 1] == null ? null : nodesCache[i + nodesXCount - 1].GetComponent<TileNode>();
						// prev row, next column
						if (x + 1 < nodesXCount) n.nodeLinks[7] = nodesCache[i + nodesXCount + 1] == null ? null : nodesCache[i + nodesXCount + 1].GetComponent<TileNode>();
					}

				}
			}
		}

		// === init forced links
		foreach (TileNode node in nodes)
		{
			if (node == null) continue;
			TNEForcedLink fl = node.gameObject.GetComponent<TNEForcedLink>();
			if (fl != null)
			{
				foreach (TileNode link in fl.links)
				{
					if (link == null) continue;
					
					// check if node not allready linked
					bool found = false;
					foreach (TileNode n in node.nodeLinks)
					{
						if (n == link) { found = true; break; }
					}

					// add if not found
					if (!found)
					{
						// find a null spot to add it in
						int addIdx = -1;
						for (int k=0; k<node.nodeLinks.Length; k++)
						{
							if (node.nodeLinks[k] == null){addIdx = k; break;}
						}

						if (addIdx >= 0)
						{	// found an open slot, add into it
							node.nodeLinks[addIdx] = link;
						}
						else
						{	// no open slot, expand the array
							TileNode[] temp = new TileNode[node.nodeLinks.Length];
							node.nodeLinks.CopyTo(temp, 0);
							node.nodeLinks = new TileNode[node.nodeLinks.Length + 1];
							temp.CopyTo(node.nodeLinks, 0);
							node.nodeLinks[node.nodeLinks.Length - 1] = link;
						}
					}
				}
			}
		}

	}

	/// <summary>
	/// Reset all tilenodes' masks to the given type
	/// </summary>
	public void SetAllNodeMasksTo(TileNode.TileType mask)
	{
		if (nodesCache == null) return;
		foreach (GameObject go in nodesCache)
		{
			if (go == null) continue;
			TileNode n = go.GetComponent<TileNode>();
			if (n != null) n.tileTypeMask = mask;
		}
	}

	public void SetTileNodeMasks(TileNode.TileType mask, Transform parent)
	{
		AddOrSetTileNodeMasks(false, mask, parent, 10f);
	}

	public void SetTileNodeMasks(TileNode.TileType mask, int testAgainstCollidersLayer)
	{
		AddOrSetTileNodeMasks(false, mask, 1<<testAgainstCollidersLayer, 100f);
	}

	public void AddToTileNodeMasks(TileNode.TileType mask, Transform parent)
	{
		AddOrSetTileNodeMasks(true, mask, parent, 10f);
	}

	public void AddToTileNodeMasks(TileNode.TileType mask, int testAgainstCollidersLayer)
	{
		AddOrSetTileNodeMasks(true, mask, 1<<testAgainstCollidersLayer, 100f);
	}

	/// <summary>
	/// Add a mask value to TileNode masks (or reset to given value), but only those under the child transforms 
	/// of the parent transform. Note that this casts a ray from the transform down to the nodes and if it touches
	/// any node's collider, it will make changes to that node. Use offsetY as an offest added to the transform's
	/// y position to cast the ray from. Usefull if you know some of your transforms might be at positions lower
	/// or inside the tilenode colliders.
	/// </summary>
	public void AddOrSetTileNodeMasks(bool isAdd, TileNode.TileType mask, Transform parent, float offsetY)
	{
		if (nodesCache == null || parent == null) return;

		// gonna need the colliders on for this, so turn 'em on
		foreach (GameObject go in nodesCache)
		{
			if (go == null) continue;
			go.collider.enabled = true;
		}

		// run through the child transforms
		foreach (Transform tr in parent.transform)
		{
			LayerMask rayMask = (1 << tilesLayer);
			Vector3 pos = tr.position; pos.y = offsetY;
			RaycastHit hit;
			if (Physics.Raycast(pos, -Vector3.up, out hit, offsetY * 2f, rayMask))
			{
				TileNode node = hit.collider.GetComponent<TileNode>();
				if (node != null)
				{
					if (isAdd) node.tileTypeMask = (node.tileTypeMask | mask);
					else node.tileTypeMask = mask;
				}
			}
		}

		// done, disable the colliders
		foreach (GameObject go in nodesCache)
		{
			if (go == null) continue;
			go.collider.enabled = false;
		}
	}

	public void AddOrSetTileNodeMasks(bool isAdd, TileNode.TileType mask, LayerMask testAgainstCollidersLayerMask, float offsetY)
	{
		if (nodesCache == null) return;

		// run through nodes, cast a ray against provided mask and make changes to node if needed
		foreach (GameObject go in nodesCache)
		{
			if (go == null) continue;

			Vector3 pos = go.transform.position; pos.y = offsetY;
			RaycastHit hit;
			if (Physics.Raycast(pos, -Vector3.up, out hit, offsetY * 2f, testAgainstCollidersLayerMask))
			{
				TileNode node = go.GetComponent<TileNode>();
				if (node != null)
				{
					if (isAdd) node.tileTypeMask = (node.tileTypeMask | mask);
					else node.tileTypeMask = mask;
				}
			}
		}
	}

	/// <summary>
	/// Cast rays down from each node (node height + a little height) and check if hit a collider on the given.
	/// The node's height is adjusted to that of the hit offset, if anything hit in the layer.
	/// If unlinkIsActive is true, then nodes that did not hit any layer will be deleted.
	/// </summary>
	public void SetupNodeHeights(LayerMask checkAgainstLayer, bool unlinkIsActive)
	{
		if (nodesCache == null) return;
		if (nodesCache.Length == 0) return;

		foreach (GameObject go in nodesCache)
		{
			if (go == null) continue;
			Transform tr = go.transform;
			LayerMask rayMask = (1 << checkAgainstLayer);
			Vector3 pos = tr.position; pos.y = 100f;
			RaycastHit hit;
			if (Physics.Raycast(pos, -Vector3.up, out hit, 200f, rayMask))
			{
				pos.y = hit.point.y;
				tr.position = pos;
			}
			else if (unlinkIsActive)
			{
				// node is not over something usefull (like terrain), delete it
				TileNode node = go.GetComponent<TileNode>();
				node.Unlink();
#if UNITY_EDITOR
				DestroyImmediate(go);
#else
				Destroy(go);
#endif
			}
		}
	}

	/// <summary>
	/// This will help setup the node links. It will turn off a link betwene two nodes of the height 
	/// difference betwene them are more than maxHeightDifference. Have a look TNELinksOnOffSwitch.cs
	/// </summary>
	public void SetupNodeLinkSwitches(float maxHeightDifference)
	{
		if (nodesCache == null) return;

		// nodes must be linked for this to work
		if (nodes == null) LinkNodes();
		if (nodes == null) return;

		maxHeightDifference = Mathf.Abs(maxHeightDifference);
		foreach (TileNode node in nodes)
		{
			if (node == null) continue;

			// check this node against its neighbours
			foreach (TileNode n in node.nodeLinks)
			{
				if (n == null) continue;
				float h = Mathf.Abs(node.transform.position.y - n.transform.position.y);
				if (h >= maxHeightDifference)
				{
					// height is greater than allowed, turn off the link
					TNELinksOnOffSwitch ls = node.gameObject.GetComponent<TNELinksOnOffSwitch>();
					if (ls == null) ls = node.gameObject.AddComponent<TNELinksOnOffSwitch>();

					ls.SetLinkStateWith(n, false);

				}
				
			}

		}

	}

	/// <summary>
	/// Remove ALL link switches that turned linking off. Have a look TNELinksOnOffSwitch.cs
	/// </summary>
	public void RemoveNodeLinkSwitches()
	{
		if (nodesCache == null) return;

		foreach (GameObject go in nodesCache)
		{
			if (go == null) continue;
			TNELinksOnOffSwitch[] sw = go.GetComponents<TNELinksOnOffSwitch>();
			if (sw.Length > 0)
			{
				for (int i = 0; i < sw.Length; i++)
				{
#if UNITY_EDITOR
					DestroyImmediate(sw[i]);
#else
					Destroy(sw[i]);
#endif
				}
			}
		}
	}

	/// <summary>
	/// Auto-delete nodes that has no active links.
	/// </summary>
	public void DeleteAllUnlinkedNodes()
	{
		List<TileNode> remove = new List<TileNode>();
		foreach (TileNode n in nodes)
		{
			if (n == null) continue;
			bool isFine = false;
			foreach (TileNode nn in n.nodeLinks)
			{
				if (nn == null) continue;
 				if (n.LinkIsOnWith(nn))
				{
					isFine = true;
					break;
				}
			}
			if (!isFine) remove.Add(n);
		}
		
		foreach (TileNode n in remove)
		{
			n.Unlink();
#if UNITY_EDITOR
			DestroyImmediate(n.gameObject);
#else
			Destroy(n.gameObject);
#endif
		}
	}

	#endregion
	// ====================================================================================================================
}
