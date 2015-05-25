using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestPool : MonoBehaviour {
	
	
	public GameObject _player;
	
	// Use this for initialization
	void Start () {
		
		List<GameObject> objects = new List<GameObject>();
		for(int i = 0; i < 10; i++)
		{	
			GameObject go = GameObjectPool.Instance.AcquireLocal(_player, Vector3.zero, Quaternion.identity);
			objects.Add(go);
			
		}
		foreach(GameObject go in objects)
		{
			GameObjectPool.Instance.Release(go);	
		}
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
