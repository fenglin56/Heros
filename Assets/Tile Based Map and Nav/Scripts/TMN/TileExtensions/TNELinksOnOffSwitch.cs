// ====================================================================================================================
//
// Created by Leslie Young
// http://www.plyoung.com/ or http://plyoung.wordpress.com/
// ====================================================================================================================

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This extension allows you to turn on/off the links between the node and its neighbours
/// </summary>
public class TNELinksOnOffSwitch : MonoBehaviour
{
	[System.Serializable]
	public class LinkState
	{
		public TileNode neighbour = null;
		public bool isOn = false;
	}

	public List<LinkState> linkStates = new List<LinkState>();

	/// <summary>returns 1 if on, else 0, and -1 if no link info exist with target node</summary>
	public int LinkIsOn(TileNode node)
	{
		foreach (LinkState l in linkStates)
		{
			if (l.neighbour == node) return (l.isOn?1:0);
		}
		return -1; // did not have link info
	}

	public void SetLinkStateWith(TileNode node, bool on)
	{
		foreach (LinkState l in linkStates)
		{
			if (l.neighbour == node)
			{
				l.isOn = on;
				return;
			}
		}

		// not found? add
		LinkState ls = new LinkState();
		ls.neighbour = node;
		ls.isOn = on;
		linkStates.Add(ls);
	}
}
