using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PrevillegeItem
{
    public GameObject m_icon;
    public string m_text;
}


[System.Serializable]
public class VipPrevillegeResData
{
    public int m_vipLevel;
   
    public GameObject m_ipLevelIcon;

    public List<PrevillegeItem> m_previllegeList;

}




public class VipPrevillegeResDataBase : ScriptableObject {
    public VipPrevillegeResData[] m_dataTable;
    
}



