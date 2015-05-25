using UnityEngine;
using System.Collections;

public class GuangHuanBehaviour : MonoBehaviour 
{
    private float HIGHT = 1f;
    private Transform thisTransform;

	void Start () 
    {
        thisTransform = this.transform;
	}
	
	void Update () 
    {
        Vector3 pos = thisTransform.position;
        pos.y = HIGHT;
        thisTransform.position = pos;
	}
}
