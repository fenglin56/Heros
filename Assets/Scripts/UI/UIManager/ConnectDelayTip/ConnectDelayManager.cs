using UnityEngine;
using System.Collections;

public class ConnectDelayManager : MonoBehaviour {
	public GameObject delayTipPrefab;
	void Start()
	{
		NGUITools.AddChild (gameObject,delayTipPrefab);
	}
}
