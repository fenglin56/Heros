// ====================================================================================================================
//
// Created by Leslie Young
// http://www.plyoung.com/ or http://plyoung.wordpress.com/
// ====================================================================================================================

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This extension allows you to add info that tells how movement
/// should be modified when a unit tries to move onto this a tile
/// </summary>
public class TNEMovementModifier : MonoBehaviour 
{
	[System.Serializable]
	public class MovementInfo
	{
		// Each TileNode can be of more than one type, for example air+land, see TileNode.tileTypeMask
		// This extension can only be applied for the given type
		public TileNode.TileType tileType = TileNode.TileType.Normal;

		// how movement points should be influenced, for example '1' would deduct 1 more point from a unit moving
		// onto the tile (that is in aditiona to the normal 1 point that would normally be deducted)
		public int movesModifier = 0;
	}

	public List<MovementInfo> moveInfos = new List<MovementInfo>();
}
