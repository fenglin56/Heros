// ====================================================================================================================
//
// Created by Leslie Young
// http://www.plyoung.com/ or http://plyoung.wordpress.com/
// ====================================================================================================================

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This extension allows you to create a new link between nodes (even between different NavMaps)
/// </summary>
public class TNEForcedLink : MonoBehaviour
{

	public List<TileNode> links = new List<TileNode>();

	public void LinkWith(TileNode node)
	{
		if (!links.Contains(node))
		{
			links.Add(node);
		}
	}
}
