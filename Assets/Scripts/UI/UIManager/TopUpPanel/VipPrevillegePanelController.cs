using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VipPrevillegePanelController : MonoBehaviour {

    //dta
    public VipPrevillegeResDataBase m_vipVillegeResDataBase;

    //level item panel
    public Transform m_vipItemGrid;
    public  List<GameObject> m_levelItemList = new List<GameObject>();
    private VipLevelItem m_selectedLevelItem;

    public GameObject m_vipLevelItemPrefab;

    public UIDraggablePanel m_vipItemDragPanel;

    public UIDraggablePanel m_vipPrevillegePanel;

    //privilege item panel
    public Transform m_vipVillegeGrid;

    public GameObject m_vipPrevillegeItemPrefab;

    public List<GameObject> m_vipPrevillegeItemList = new List<GameObject>();

    private TweenPosition m_tweenPosComponent;

	private bool m_inited = false;

	// Use this for initialization
    void Awake()
    {
        m_tweenPosComponent = this.GetComponent<TweenPosition>();

    }

	public void Init()
	{
		if (!m_inited) 
		{
			SetupVipLevelItems();
			SelectedDefaultVipLevelItem();
			m_inited = true;
		}
	}

    IEnumerator LateSelectVipLevelItem()
    {
        yield return null;
        yield return null;
        SelectedDefaultVipLevelItem();
    }


	void Start () {
        
        //m_tweenPosComponent.Play(true);

	}
	
	// Update is called once per frame
	void Update () {
	
	}


    public void SetupVipLevelItems()
    {
        foreach(VipPrevillegeResData data in m_vipVillegeResDataBase.m_dataTable)
        {
            if(data.m_vipLevel != 0)
            {
                GameObject obj  = Instantiate(m_vipLevelItemPrefab) as GameObject;
                VipLevelItem objItem = obj.GetComponent<VipLevelItem>();
                objItem.Setup(data, OnLevelItemSelected);
                Transform objTrans = obj.transform;
                objTrans.parent = m_vipItemGrid;
                objTrans.localScale = Vector3.one;
                objTrans.localPosition = Vector3.zero;
                m_levelItemList.Add(obj);


            }
            m_vipItemGrid.GetComponent<UIGrid>().Reposition();
        }
    }

    public void OnLevelItemSelected(VipLevelItem item)
    {
        if(item == null)
        {
            return;
        }

        if( m_selectedLevelItem == item)
        {
            return;
        }
        else
        {
            if(m_selectedLevelItem != null )
            {
                m_selectedLevelItem.SetSelected(false);
            }

            m_selectedLevelItem = item;
            m_selectedLevelItem.SetSelected(true);
            RefreshVipPrevillegeItems();

        }

    }

    public void SelectedDefaultVipLevelItem()
    {
        int selectedVipLevel = 0;
        int vipLevel = PlayerDataManager.Instance.GetPlayerVIPLevel();
        if(vipLevel == 0)
        {
            selectedVipLevel = 1;
        }
        else
        {
            selectedVipLevel = vipLevel;
        }

        m_vipItemDragPanel.SetDragAmount(0, (float)(selectedVipLevel -1)/((float)m_levelItemList.Count * 0.85f), false);
        VipLevelItem item = m_levelItemList[selectedVipLevel-1].GetComponent<VipLevelItem>();
        OnLevelItemSelected(item);

    }

    void ClearVipPrevillegeItems()
    {
        if(m_vipPrevillegeItemList != null)
        {
            foreach(GameObject obj in m_vipPrevillegeItemList)
            {
                DestroyImmediate(obj);
            }
        }
        m_vipPrevillegeItemList.Clear();
        m_vipPrevillegePanel.ResetPosition();

    }


    void RefreshVipPrevillegeItems()
    {
        if(null != m_selectedLevelItem)
        {
            ClearVipPrevillegeItems();
            foreach(PrevillegeItem item in m_selectedLevelItem.GetData.m_previllegeList)
            {

                GameObject obj  = Instantiate(m_vipPrevillegeItemPrefab) as GameObject;
                VipprevillegeItem objItem = obj.GetComponent<VipprevillegeItem>();
                objItem.Setup(item);
                Transform objTrans = obj.transform;
                objTrans.parent = m_vipVillegeGrid;
                objTrans.localPosition = Vector3.zero;
                objTrans.localScale = Vector3.one;


                m_vipPrevillegeItemList.Add(obj);
            }
            m_vipVillegeGrid.GetComponent<UIGrid>().repositionNow = true;
            m_vipPrevillegePanel.ResetPosition();

        }

    }
    [ContextMenu("Play")]
    public void PlayTween()
    {
        if(m_tweenPosComponent == null)
        {
            m_tweenPosComponent = this.GetComponent<TweenPosition>();

        }
        m_tweenPosComponent.Reset();
        m_tweenPosComponent.Play(true);
    }

    public void OnShow()
    {
		Init ();
        //SelectedDefaultVipLevelItem();
        StartCoroutine(LateSelectVipLevelItem());
        PlayTween();
    }

}
