using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class IconPrefabManager : MonoBehaviour {

    Dictionary<string, GameObject> IconList= new Dictionary<string, GameObject>();

    public CommonIconDataList commonIconDataList;

    private static IconPrefabManager m_Instance;

    void Awake()
    {
        foreach (CommonIconData child in commonIconDataList._CommonIcons)
        {
            IconList.Add(child._IconName,child._IconPrefab);
        }
        m_Instance = this;
    }

    void OnDestroy()
    {
        m_Instance = null;
    }

    public static IconPrefabManager Instance
    {
        get
        {
            return m_Instance;
        }
    }

    public GameObject getIcon(string IconName)
    {
        GameObject getObj = null;
        IconList.TryGetValue(IconName,out getObj);
        return getObj;
    }

}
