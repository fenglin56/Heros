using UnityEngine;
using System.Collections;

public class GetAllScriptableObj : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var childScriptableObjs = transform.GetComponentsInChildren<MonoBehaviour>();
        childScriptableObjs.ApplyAllItem(P => TraceUtil.Log(P.name));
	}
	
	// Update is called once per frame
	void Update () {
	    
	}
}
