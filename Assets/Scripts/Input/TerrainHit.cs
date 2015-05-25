using UnityEngine;
using System.Collections;

public class TerrainHit : MonoBehaviour
{
    void OnTouchDown()
    {
        TargetSelected targetSelected = new TargetSelected();
        targetSelected.Type = ResourceType.Terrain;
        //targetSelected.SelectedPoint=mou
    }
}
