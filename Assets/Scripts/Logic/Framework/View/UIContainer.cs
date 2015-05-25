using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 场景管理类
/// </summary>
public abstract class UIContainer : ViewNotifier
{
    public int UILayer = 8;
    public GameObject /*BackgroundObj,*/panelObj;
    public Dictionary<string,GameObject> ObjMap = new Dictionary<string,GameObject>();
    public GameObject m_dialogObj;

    /// <summary>
    ///  UIContainer父类管理对象的初始化
    /// </summary>
    public void Init()
    {
        panelObj = GameObject.Find("Panel");
        transform.parent = panelObj.transform;

        //规范View对象的相对位置和大小
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localScale = Vector3.one;
        gameObject.layer = UILayer;     

    }

    //创建UI对象
    protected abstract void GenerateUIObject();

    protected Color GetColorByRGB(float red, float green, float blue)
    {
        return new Color(red / 256, green / 256, blue / 256);
    }

    #region Edit by Rocky
    //销毁指定Map对象
    protected void DestrocyMayObj(string key)
    {
        if (ObjMap.ContainsKey(key))
        {
            Destroy(ObjMap[key]);
            ObjMap.Remove(key);
        }
    }
    #endregion   
}
