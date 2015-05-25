using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneratorTest : MonoBehaviour {


    public GameObject [] m_ObjPrefabList;
    public GameObject bgPrefab;


    private GameObject testObj;
	// Use this for initialization
	void Start () 
    {
        List<SingleItemConfig> configList = new List<SingleItemConfig>();
        foreach(GameObject obj in m_ObjPrefabList)
        {
            SingleItemConfig config = new SingleItemConfig();
            config.Obj = Instantiate(obj) as GameObject;
            config.LeftSpacing = 20 + Random.Range(1, 5);
            configList.Add(config);
        }

        GameObject bgObj = Instantiate(bgPrefab) as GameObject;

        testObj = InventoryItemInfoGenerator.Generate(configList, bgObj, 5, 2, 100);


	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
