using UnityEngine;
using System.Collections;

public class VipprevillegeItem : MonoBehaviour {

    public Transform m_iconAnchor;
    public UILabel m_describeText;


	// Use this for initialization
	void Start () {
	



	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void Setup(PrevillegeItem itemData )
    {
        //icon
        GameObject icon = Instantiate(itemData.m_icon) as GameObject;
        Transform iconTrans = icon.transform;
        iconTrans.parent = m_iconAnchor;
        iconTrans.localPosition = Vector3.zero;
        iconTrans.localScale = itemData.m_icon.transform.localScale;

        //text
        m_describeText.SetText(LanguageTextManager.GetString(itemData.m_text));
    }
}
