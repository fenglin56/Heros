using UnityEngine;
using System.Collections;
using System;

public class VipLevelItem : MonoBehaviour {

    public Transform m_iconAnchor;
    public GameObject m_selectedOutLine;
    
    private VipPrevillegeResData m_data;
    public VipPrevillegeResData GetData
    {
        get { return m_data; }
    }

    private event Action<VipLevelItem> onItemSelected;

    void Awake()
    {
        SetSelected(false);
    }

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Setup(VipPrevillegeResData data, Action<VipLevelItem> onItemClick)
    {
        m_data = data;

        //icon
        GameObject icon = Instantiate(m_data.m_ipLevelIcon) as GameObject;
        Transform iconTrans = icon.transform;
        iconTrans.parent = m_iconAnchor;
        iconTrans.localPosition = Vector3.zero;
        iconTrans.localScale = m_data.m_ipLevelIcon.transform.localScale;

        //action
        onItemSelected = onItemClick;
    }

    public void SetSelected(bool selected)
    {
        m_selectedOutLine.SetActive(selected);
    }

    public void OnClick()
    {
        onItemSelected(this);
        SoundManager.Instance.PlaySoundEffect("Sound_Button_VIPPrivilegeChoice");
    }


}
