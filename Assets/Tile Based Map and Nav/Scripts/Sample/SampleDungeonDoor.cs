// ====================================================================================================================
//
// Created by Leslie Young
// http://www.plyoung.com/ or http://plyoung.wordpress.com/
// ====================================================================================================================

using UnityEngine;
using System.Collections;

public class SampleDungeonDoor : MonoBehaviour 
{
	public GameObject doorPart;	// the part that will be shown/hidden
	public TileNode tileNode;	// the tile this door is occupying
	public bool isOpen = false;	// state of the door

	void Update()
	{
		// check if player clicked on door and open/close it
		if (Input.GetMouseButtonUp(0) && tileNode != null && doorPart != null)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 500f))
			{
				if (hit.transform.parent == this.transform)
				{
					// door was clicked on

					if (isOpen)
					{
						// close the door, bit not if a unit is standing in the door
						if (tileNode.units.Count == 0)
						{
							OpenDoor(false);
						}
					}

					else
					{
						// open the door
						OpenDoor(true);
					}

				}
			}
		}

	}

	private void OpenDoor(bool open)
	{
		isOpen = open;
		doorPart.collider.enabled = !open;
		doorPart.renderer.enabled = !open;

		// update the tilenode links
		tileNode.SetLinkStateWithAll(open);

	}
}
