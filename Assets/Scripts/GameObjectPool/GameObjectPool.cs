using UnityEngine;
using System.Collections.Generic;
using System;
using System.Reflection;

[AddComponentMenu("Utils/Game Object Pool")]
public class GameObjectPool : MonoBehaviour
{
	private static GameObjectPool _instance = null;

	public static GameObjectPool Instance
	{
		get
		{
			if (_instance == null)
				_instance = FindObjectOfType(typeof(GameObjectPool)) as GameObjectPool;

			return _instance;
		}
	}
	
	void OnDestroy()
	{
		ClearCache();
		_instance = null;
	}
	
	
	public static  bool _disable = false;
	
	private class Pool
	{
		public GameObject Prefab;
		public Stack<GameObject> FreeStack = new Stack<GameObject>(50);
	}
	
	private Dictionary<GameObject, Pool> _pools = new Dictionary<GameObject, Pool>(20); // Prefabs to pools
	private Dictionary<GameObject, Pool> _objects = new Dictionary<GameObject, Pool>(100); // Objects to pools
	

	public void Awake()
	{
		_instance = this;
	}

	
	
	public GameObject AcquireLocal(GameObject prefab ,Vector3 position , Quaternion rotation)
	{
		return AcquireLocal(prefab,position,rotation,true);
	}
	
	public GameObject AcquireLocal(GameObject prefab ,Vector3 position , Quaternion rotation, bool usePool)
	{		
		Pool pool = null;
		if (usePool && !_pools.TryGetValue(prefab, out pool)) {
			pool = new Pool();
			pool.Prefab = prefab;
			_pools.Add(prefab, pool);
		}
		
		GameObject go = null;
		if ( (pool != null && pool.FreeStack.Count == 0) || !usePool) {
			go = GameObject.Instantiate(prefab, position, rotation) as GameObject;
		}else{
			go = pool.FreeStack.Pop();
			go.transform.parent = null;
			go.transform.localPosition = prefab.transform.localPosition;
			go.transform.localRotation = prefab.transform.localRotation;
			go.transform.localScale = prefab.transform.localScale;
			
			go.transform.position = position;
			go.transform.rotation = rotation;
			
			go.SetActive(true);
		}
		
		_objects.Add(go, pool);

		return go;
	}
	
	
	public void Release(GameObject go)
	{
		Release(go, true);
	}

	
	public void Release(GameObject go, bool usePool)
	{
		if(_disable || !usePool)
		{
			Destroy(go);
			return;
		}
		
		Pool pool;
		if (!_objects.TryGetValue(go, out pool))
		{
			return;
		}
		_objects.Remove(go);
		
		pool.FreeStack.Push(go);
		go.transform.parent = transform;
		go.SetActive(false);
	
	
	}
	
	public void ClearCache()
	{

		List<GameObject> clearedPrefabs = new List<GameObject>();
		
		foreach (KeyValuePair<GameObject, Pool> pair in _pools) {
			foreach (GameObject go in pair.Value.FreeStack) {
				Destroy(go);
				
			}
			
			pair.Value.FreeStack.Clear();
			clearedPrefabs.Add(pair.Key);
		}
		
		
		foreach (GameObject prefab in clearedPrefabs) {
			Pool p;
			if(_pools.TryGetValue(prefab, out p))
			{
				if(!_objects.ContainsValue(p)) 
				{
					_pools.Remove(prefab);
				}
			}
		}
		
		_pools.Clear();
		Resources.UnloadUnusedAssets();

	}
}

