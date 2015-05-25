// ====================================================================================================================
// 
// Created by Leslie Young
// http://www.plyoung.com/ or http://plyoung.wordpress.com/
// ====================================================================================================================

using UnityEngine;

public abstract class TMNController : MonoBehaviour 
{
	// ====================================================================================================================
	#region inspector properties

	public Camera rayCam;	// the main game camera should be linked here
	public MapNav map;		// the MapNav used with this controller
	public int unitsLayer=21;	// on what layer is units

	#endregion
	// ====================================================================================================================
	#region vars

	private GameObject _selectedUnitGo = null;	// the currently selected unit
	private GameObject _hoverNodeGo = null;		// node that mouse is hovering over
	private LayerMask _rayMask = 0;				// used to determine what can be clicked on (Tiles and Units) Inited in Start()

	#endregion
	// ====================================================================================================================
	#region start/init

	public virtual void Start()
	{
		if (map == null)
		{
			Debug.LogWarning("The 'map' property was not set, attempting to find a MapNav in the scene.");
			Object obj = GameObject.FindObjectOfType(typeof(MapNav));
			if (obj != null) map = obj as MapNav;

			// I'm not gonan do extra if() tests in the HandleInput.. tell coder now there is problem he should be sorting out asap
			if (map == null) Debug.LogError("Could not find a MapNav in the scene. You gonna get NullRef errors soon!");
		}

		_rayMask = (1<<map.tilesLayer | 1<<this.unitsLayer);
	}

	#endregion
	// ====================================================================================================================
	#region update/input

	/// <summary>Call this every frame to handle input (detect clicks on units and tiles)</summary>
	protected void HandleInput()
	{
		// only continue if left-mouse-click deltected or iof a unit is currently selected
		if (!Input.GetMouseButtonUp(0) && _selectedUnitGo == null) return;

		bool unselect = (Input.GetMouseButtonUp(0) ? true : false);

		Ray ray = rayCam.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 500f, _rayMask))
		{
			// *** Ray hit a Tile
			if (hit.collider.gameObject.layer == map.tilesLayer)
			{
				if (Input.GetMouseButtonUp(0))
				{	// mouse-click/touch detected
					unselect = false;
					OnTileNodeClick(hit.collider.gameObject);
				}
				else
				{	// else, mouse hovering over tile
					OnTileNodeHover(hit.collider.gameObject);
				}
			}
			else if (_hoverNodeGo != null)
			{
				OnTileNodeHover(null);
			}

			// *** Raycast hit a Unit
			if (hit.collider.gameObject.layer == this.unitsLayer)
			{
				if (Input.GetMouseButtonUp(0))
				{	// mouse-click/touch on the unit
					unselect = false;

					// first clear any previous selection
					if (_selectedUnitGo != null)
					{
						OnTileNodeHover(null);
						OnClearNaviUnitSelection(hit.collider.gameObject);
					}

					// select clicked unit
					OnNaviUnitClick(hit.collider.gameObject);
				}
			}
		}
		else if (_hoverNodeGo != null)
		{
			OnTileNodeHover(null);
		}		

		if (unselect)
		{
			OnTileNodeHover(null);
			OnClearNaviUnitSelection(null);
		}
	}

	// ====================================================================================================================

	/// <summary>Handles tile clicks</summary>
	protected virtual void OnTileNodeClick(GameObject nodeGo)
	{
	}

	/// <summary>Handles mouse cursor hover over tile</summary>
	protected virtual void OnTileNodeHover(GameObject nodeGo)
	{
		_hoverNodeGo = nodeGo;
	}

	/// <summary>Handles unit clicks</summary>
	protected virtual void OnNaviUnitClick(GameObject unitGo)
	{
		_selectedUnitGo = unitGo;
	}

	/// <summary>Handles unit unselect</summary>
	protected virtual void OnClearNaviUnitSelection(GameObject clickedAnotherUnit)
	{
		_selectedUnitGo = null;
	}

	#endregion
	// ====================================================================================================================
}
