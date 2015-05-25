using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BtnSignPanel : MonoBehaviour {

    public UILabel SignText;



    //void Awake()
    //{
    //    foreach(KeyValuePair<int, GameObject> item in m_arrowList)
    //    {
    //        item.Value.SetActive(false);
    //    }
    //}

    public void InitSignPanel(GuideConfigData item)
    {
        SignText.text = LanguageTextManager.GetString(item._BtnSignText);
    }
    public void InitSignPanel(string signIDs)
    {
        SignText.text = LanguageTextManager.GetString(signIDs);
    }

    //public void OnDestroy()
    //{
    //    Destroy(this.gameObject);
    //}
}
