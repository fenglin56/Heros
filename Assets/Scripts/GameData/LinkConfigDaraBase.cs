using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class LinkConfigItemData
{
    public string LinkID;
    public LinkType LinkType;
    public string LinkPara;
	public int LinkChildren;
    public string LinkName;
    public string Des;
    public List<GameObject> LinkIcon;
}
public class LinkConfigDaraBase : ScriptableObject {
    public LinkConfigItemData[] LinkConfigItemList;
}
