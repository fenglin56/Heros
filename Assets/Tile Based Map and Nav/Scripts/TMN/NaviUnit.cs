// ====================================================================================================================
//
// Created by Leslie Young
// http://www.plyoung.com/ or http://plyoung.wordpress.com/
// ====================================================================================================================

using UnityEngine;
using System.Collections;

public class NaviUnit : MonoBehaviour 
{
	// ====================================================================================================================
	#region inspector properties

	// the level/layer this unit can occupy on a tile, see TileNode.units for more info
	public TileNode.TileType tileLevel = TileNode.TileType.Normal;

	// set to true for units that "jump" from node to node when moving
	public bool jumpMovement = false;

	// how fast a unit will be move from tile to tile
	public float moveSpeed = 2f;

	// Multiplayer used with moveSpeed. Setting this will make a unit move faster
	// from node to node when it has to move further, so as not to bore players ;)
	public float moveSpeedMulti = 0.1f;

	// --------------------------------------------------
	// the following are not used when jumpMovement is on

	// should unit tilt (look up/down) when moving from node-to-node that are not on equel level?
	public bool tiltToPath = false;

	// these are used to have the unit adjust as needed. Usefull on uneven terrain where the unit might go through
	// terrain if only following nodes blindly or if you would like it to rotate according to the terrain normals
	public bool adjustToCollider = false;
	public bool adjustNormals = false; // if this is true then adjustToCollider will be performed too
	public LayerMask adjustmentLayerMask = 0;

	// set this to false if your units will not be moving at the same, if only one unit at a time will move and 
	// complete its movement before any other unit will move. This will allow for a little smoother movement.
	public bool usingSameTimeMovement = true;

	// --------------------------------------------------

	#endregion
	// ====================================================================================================================
	#region vars

	// the MapNav this unit is on
	public MapNav mapnav { get; set; }

	// node that this unit is currently standing on
	public TileNode node { get; set; }

	// callbacks for when unit completed a task
	// eventCode 1 = movement to node completed
	public delegate void UnitEventDelegate(NaviUnit unit, int eventCode);
	protected UnitEventDelegate onUnitEvent = null;

	protected bool isMoving = false;					// unit is moving
	protected bool isDelayed = false;					// will be true if unit is waiting for another to move outa the way while isMoving is true
	protected Vector3 endPointToReach = new Vector3();	// where unit is trying to get to

	protected Transform _tr;				// cached transform

	protected TileNode[] followPath = null;	// path being followed
	protected int followMaxMoves = 0;		// how far may be moved on the followPath
	protected int followIdx = 0;			// next node to move to
	protected Hashtable iTweenOpt = null;	// cached movement options

	private float delayTimer = 0f;			// used when unit isDelayed

	#endregion
	// ====================================================================================================================
	#region pub

	/// <summary>Instantiates a new unit on target node</summary>
	public static NaviUnit SpawnUnit(GameObject unitFab, MapNav mapnav, TileNode node)
	{
		GameObject go = (GameObject)GameObject.Instantiate(unitFab);
		go.transform.position = node.transform.position;
		NaviUnit unit = go.GetComponent<NaviUnit>();
		unit.mapnav = mapnav;
		unit._tr = go.transform;
		unit.LinkWith(node);
		return unit;
	}

	/// <summary>Init a unit. You normally want to do this right after it was spawned</summary>
	/// <param name="callback">Optional callback to call after tasks, like moving to a node, are completed</param>
	public virtual void Init(UnitEventDelegate callback)
	{
		this.onUnitEvent = callback;
	}

	// ====================================================================================================================

	/// <summary>
	/// Link with target node and unlink with any currently linked node. aka, set NaviUnit.node variable.
	/// Note that this does not check if targetNode is a valid node that this Unit could stand on.
	/// Use CanStandOn to check for that.
	/// </summary>
	public void LinkWith(TileNode targetNode)
	{
		// first unlink with previous node if standing on any
		if (this.node != null)
		{
			this.node.units.Remove(this);
		}

		// link with new node
		this.node = targetNode;
		if (this.node != null)
		{
			this.node.units.Add(this);
		}
	}

	/// <summary>
	/// checks if unit can stand on the target node
	/// </summary>
	/// <param name="andLevelIsOpen">also check if there is a unit in the way?</param>
	public bool CanStandOn(TileNode targetNode, bool andLevelIsOpen)
	{
		if (targetNode == null) return false;

		// can this unit move onto a lvel on this tile?
		if ((targetNode.tileTypeMask & this.tileLevel) == this.tileLevel)
		{
			// check if there might be unit in the way on target tile?
			if (andLevelIsOpen)
			{
				// success if there is not a unit standing in same level on target tilenode
				if (targetNode.GetUnitInLevel(this.tileLevel) == null) return true;
			}
			else
			{
				return true;
			}
		}

		// fail
		return false;
	}

	// ====================================================================================================================

	/// <summary>called when movement stopped for whatever reason. override if you want to change playing animation for example</summary>
	protected virtual void OnMovementStopped() { }

	/// <summary>called when movement was delayed because of another unit being in the way. Something that can hapen when units all move at same time</summary>
	protected virtual void OnMovementDelayed() { }

	/// <summary>called when movement continue afte being delayed (see OnMovementDelayed)</summary>
	protected virtual void OnMovementContinue() { }

	/// <summary>return a list if nodes that can be used to reach the target node.</summary>
	public TileNode[] GetPathTo(TileNode targetNode)
	{
		return mapnav.GetPath(this.node, targetNode, this.tileLevel);
	}

	/// <summary>
	/// Move the unit to the specified target node. Will make a callback with 
	/// eventCode=1 when done moving if a callback was set in Init()
	/// iTween (free on Unity Asset Store) is used to move the Unit
	/// </summary>
	/// <param name="map">NavMap to use</param>
	/// <param name="targetNode">Node to reach</param>
	/// <param name="moves">
	/// Number of moves that can be use to reach target. Pass (-1) to ignore move limits. 
	/// The unit will move as close as possible if not enough moves given.
	/// ref moves = (moves - actual_moves_use) on return (thus moves left); or if moves = -1 passed, it will be set to number of moves that will be taken.
	/// </param>
	/// <returns>True if the unit started moving towards the target</returns>
	public bool MoveTo(TileNode targetNode, ref int moves)
	{
		if (isMoving) StopMoving();		// first stop current movement
		if (moves == 0) return false;	// could not move

		followPath = mapnav.GetPath(this.node, targetNode, this.tileLevel);
		if (followPath == null) { if (moves == -1)moves = 0; return false; }
		if (followPath.Length == 0) { if (moves == -1)moves = 0; return false; }

		// check if enough moves to reach destination
		followMaxMoves = followPath.Length;
		if (moves == -1)
		{
			moves = followMaxMoves;
		}
		else
		{
			if (moves >= 0)
			{
				if (moves <= followMaxMoves) { followMaxMoves = moves; moves = 0; }
				else moves -= followMaxMoves;
			}
			else moves = followPath.Length;
		}

		_StartMoving();

		return true;
	}

	public bool MoveTo(TileNode targetNode)
	{
		if (isMoving) StopMoving();		// first stop current movement

		followPath = mapnav.GetPath(this.node, targetNode, this.tileLevel);
		if (followPath == null) return false;
		if (followPath.Length == 0) return false;

		followMaxMoves = followPath.Length;
		isMoving = true;
		_StartMoving();
		return true;
	}

	public void StopMoving()
	{
		isMoving = false;
		isDelayed = false;
		OnMovementStopped();
	}

	#endregion
	// ====================================================================================================================
	#region internal

	public virtual void Start()
	{
		this._tr = gameObject.transform;
	}

	public virtual void Update()
	{
		if (isDelayed)
		{
			delayTimer -= Time.deltaTime;
			if (delayTimer <= 0f)
			{
				isDelayed = false;
				OnMovementContinue();
				_GotoNextNode();
			}
		}
	}

	private void _StartMoving()
	{
		// If using same time moving, then a unit will have to mvoe from tile to tile and
		// 1st check ifallowed to mvoe further before going to its next tile since 
		// another unit might have entered the target tile

		if (usingSameTimeMovement)
		{
			// setup iTween options to be used with this move
			if (jumpMovement)
			{
				iTweenOpt = iTween.Hash(
							"path", null,
							"speed", moveSpeed,
							"orienttopath", true,
							"easetype", "easeInOutExpo",
							"axis", "y",
							"oncomplete", "_OnCompleteMoveTo",
							"oncompletetarget", gameObject);

			}
			else
			{
				endPointToReach = followPath[followMaxMoves - 1].transform.position;
				iTweenOpt = iTween.Hash(
							"position", Vector3.zero,
							"speed", ((moveSpeedMulti * followMaxMoves) + 1f) * moveSpeed,
							"orienttopath", true,
							"easetype", "linear",
							"oncomplete", "_OnCompleteMoveTo",
							"oncompletetarget", gameObject);

				if (!tiltToPath || adjustNormals) iTweenOpt.Add("axis", "y");

			}

			if ((adjustNormals || adjustToCollider) && adjustmentLayerMask != 0)
			{
				iTweenOpt.Add("onupdate", "_OnMoveToUpdate");
				iTweenOpt.Add("onupdatetarget", gameObject);
			}

			// start moving
			followIdx = 0;
			_GotoNextNode();
		}

		// *** not usingSameTimeMovement
		else
		{
			// setup iTween options to be used with this move
			if (jumpMovement)
			{
				iTweenOpt = iTween.Hash(
							"path", null,
							"speed", moveSpeed,
							"orienttopath", true,
							"easetype", "easeInOutExpo",
							"axis", "y",
							"oncomplete", "_OnCompleteMoveTo",
							"oncompletetarget", gameObject);


				if ((adjustNormals || adjustToCollider) && adjustmentLayerMask != 0)
				{
					iTweenOpt.Add("onupdate", "_OnMoveToUpdate");
					iTweenOpt.Add("onupdatetarget", gameObject);
				}

				// start moving
				followIdx = 0;
				_GotoNextNode();
			}
			else
			{

				iTweenOpt = iTween.Hash(
									"speed", ((moveSpeedMulti * followMaxMoves) + 1f) * moveSpeed,
									"orienttopath", true,
									"easetype", "linear",
									"oncomplete", "_OnCompleteMoveTo",
									"oncompletetarget", gameObject);

				if (!tiltToPath || adjustNormals) iTweenOpt.Add("axis", "y");

				if ((adjustNormals || adjustToCollider) && adjustmentLayerMask != 0)
				{
					iTweenOpt.Add("onupdate", "_OnMoveToUpdate");
					iTweenOpt.Add("onupdatetarget", gameObject);
				}

				if (followMaxMoves > 1)
				{
					Vector3[] points = new Vector3[followMaxMoves];
					for (int i = 0; i < followMaxMoves; i++) points[i] = followPath[i].transform.position;
					endPointToReach = points[points.Length - 1];
					iTweenOpt.Add("path", points);
				}
				else
				{
					endPointToReach = followPath[0].transform.position;
					iTweenOpt.Add("position", endPointToReach);
				}

				// unlink with current node and link with destination node
				node.units.Remove(this);
				node = followPath[followMaxMoves-1];
				node.units.Add(this);

				// start movement
				iTween.MoveTo(this.gameObject, iTweenOpt);
			}
		}

	}

	private void _GotoNextNode()
	{
		if (followIdx < followMaxMoves)
		{
			// first check if nothing in way of moving to the next node
			bool isFine = true;
			if (followPath[followIdx].units.Count > 0)
			{
				// allow only one unit per tile?
				if (mapnav.oneUnitPerTileOnly)
				{
					if (!mapnav.oneUnitExceptionAllowMoveOver) isFine = false;
				}

				if (isFine)
				{
					// now check if another unit is occupying the same layer/level on the node
					if (followPath[followIdx].GetUnitInLevel(tileLevel) != null) isFine = false;
				}
			}

			if (!isFine)
			{
				// will wait a bit and then try to move again
				isDelayed = true;
				delayTimer = 0.1f;
				OnMovementDelayed();
				return;
			}

			// unlink with current node and link with destination node
			node.units.Remove(this);
			node = followPath[followIdx];
			node.units.Add(this);

			this.mapnav = node.mapnav; // make sure this unit uses same mapnav as new node, just incase it moved onto new map

			// move unit with iTween
			if (jumpMovement)
			{
				endPointToReach = followPath[followIdx].transform.position;
				float dist = Vector3.Distance(_tr.position, endPointToReach);
				Vector3[] points = new Vector3[3];
				points[0] = _tr.position;
				points[1] = Vector3.MoveTowards(points[0], endPointToReach, dist / 2f);
				float h = Mathf.Abs(points[0].y - endPointToReach.y) * 2f;
				if (h <= 0.01f) h = dist / 2f;
				points[1].y += h;
				points[2] = endPointToReach;

				iTweenOpt["path"] = points;
				iTween.MoveTo(this.gameObject, iTweenOpt);
			}
			else
			{
				iTweenOpt["position"] = followPath[followIdx].transform.position;
				iTween.MoveTo(this.gameObject, iTweenOpt);
			}

			// set next node that will be moved to
			followIdx++;
		}
		else
		{	// done
			StopMoving();
			if (onUnitEvent != null) onUnitEvent(this, 1);
		}
	}

	private void _OnMoveToUpdate()
	{
		_UpdateUnitNormal();
	}
	
	private void _OnCompleteMoveTo()
	{
	    _UpdateUnitNormal();
		if (usingSameTimeMovement)
		{
			_GotoNextNode();
		}
		else
		{
			StopMoving();
			if (onUnitEvent != null) onUnitEvent(this, 1);
		}
	}

	private Vector3 _smoothedNormal = Vector3.up;
	protected void _UpdateUnitNormal()
	{
		if ( (!adjustNormals && !adjustToCollider) || adjustmentLayerMask==0 ) return;

		Vector3 pos = _tr.position; pos.y += 10f;
		RaycastHit hit;
		if (Physics.Raycast(pos, -Vector3.up, out hit, 50f, adjustmentLayerMask))
		{
			pos.y -= hit.distance;
			_tr.position = pos;

			if (adjustNormals)
			{
				_smoothedNormal = Vector3.Lerp(_smoothedNormal, hit.normal, 10f * Time.deltaTime);
				Quaternion tilt = Quaternion.FromToRotation(Vector3.up, _smoothedNormal);
				transform.rotation = tilt * Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
			}
		}
	}

	#endregion
	// ====================================================================================================================
}
