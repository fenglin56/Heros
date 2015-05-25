using UnityEngine;
using System.Collections;

public class LuckDrawItem : MonoBehaviour {

    //闪光外边框
    public GameObject SelectedOutLine;
    public GameObject CostBG;

    public Transform m_itemIconAnchor;
    public UILabel m_itemCount;





    private PlayerLuckDrawData m_luckDrawData;
    public PlayerLuckDrawData Data
    {
        get { return m_luckDrawData; }
    }







    public void Setup(PlayerLuckDrawData data)
    {
        m_luckDrawData = data;
        int playerLevel = PlayerManager.Instance.FindHeroDataModel().GetUnitValue().sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
        GameObject iocnPrefab = data.GetItemIconPrefab(playerLevel);
        GameObject icon = Instantiate(iocnPrefab) as GameObject;
        icon.transform.parent = transform;
        icon.transform.localPosition = Vector3.zero;
        icon.transform.localScale = iocnPrefab.transform.localScale;

        SetSelected(false);


        Init();
    }



    void Init()
    {
        MakeItemCountOrigion();
    }

    public void MakeItemCountMultiple()
    {
        int multiple = CommonDefineManager.Instance.CommonDefineFile._dataTable.LotteryMultipleNum;
        int playerLevel = PlayerManager.Instance.FindHeroDataModel().GetUnitValue().sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
        int count = m_luckDrawData.GetItemCount(playerLevel) * CommonDefineManager.Instance.CommonDefine.LotteryMultipleNum;
        if(count != 0)
        {
            string str = count.ToString();
            m_itemCount.SetText(str);
            CostBG.SetActive(true);
        }
        else
        {
            CostBG.SetActive(false);
            m_itemCount.SetText("");
        }

    }

    public void MakeItemCountOrigion()
    {

        int playerLevel = PlayerManager.Instance.FindHeroDataModel().GetUnitValue().sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
        int count = m_luckDrawData.GetItemCount(playerLevel);
        if(count != 0)
        {
            string str  = count.ToString();
            m_itemCount.SetText(str);
            CostBG.SetActive(true);
        }
        else
        {
            CostBG.SetActive(false);
            m_itemCount.SetText("");

        }
    }



    public void SetSelected(bool selected)
    {
        SelectedOutLine.SetActive(selected);
    }

	// Use this for initialization
	void Start () 
    {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
