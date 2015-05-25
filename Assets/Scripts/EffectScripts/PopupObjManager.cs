using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// BattleUI/TownUI Scene BattleManager 
/// </summary>
public class PopupObjManager : MonoBehaviour
{
    public Camera UICamera;
    public Transform UIPanelForPopupParent;
    public GameObject PopupObj;
    public GameObject NormalPopupObj;
    public GameObject AddHPPopupObj;
    public GameObject AddMPPopupObj;
    public GameObject AddMoneyPopupObj;
    //public GameObject BattleResult;
    public GameObject PopUpHpObj;
	public GameObject PopUpCritObj;
    public GameObject PopUpIconObj;
	//vip升级时弹出界面
	public GameObject vipUpgradeEffPanel;
	//快速购买弹出界面
	public GameObject quickBuy;
    //private PopupBaseObj m_baseObj;

    public GameObject BuyVigourMessagePrefab;

    public GameObject FoceAddedPrefab;
    private GameObject FoceAdded_go;
    private static PopupObjManager _instance = null;

    private Transform Point;
    public static PopupObjManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType(typeof(PopupObjManager)) as PopupObjManager;

            return _instance;
        }
    }

    void OnDestroy()
    {
        _instance = null;
    }
    void Awake()
    {
        _instance = this;
    }
    // Use this for initialization
    void Start()
    {
        Point=UICamera.transform.Find("Anchor");
		//InvokeRepeating ("OpenQuickBuyPanel",30,50);
        // m_baseObj = new PopupBaseObj();
    }
	private List<GameObject> popVipPanelList = new List<GameObject> ();
	public void RemovePopVip()
	{
		popVipPanelList.RemoveAt (0);
	}
	//显示pop界面
	public void OpenVipUpgradePanel(ERewardPopType popType,params object[] value)
	{
		if (vipUpgradeEffPanel == null)
			return;
        GameObject effPanel = UI.CreatObjectToNGUI.InstantiateObj (vipUpgradeEffPanel,Point);
		effPanel.transform.localPosition = new Vector3 (0,0,popVipPanelList.Count*10-120);
		popVipPanelList.Add (effPanel);
		VipUpgradeEffPanel vipEff = effPanel.GetComponent<VipUpgradeEffPanel>();
		vipEff.Show (popType,value);
	}
    public void ForceAddEffect(int force)
    {
        Destroy(FoceAdded_go);
            if (FoceAddedPrefab)
            {
            FoceAdded_go = UI.CreatObjectToNGUI.InstantiateObj (FoceAddedPrefab,Point);

            }
            else
            {
                return;
            }
        FoceAdded_go.transform.localPosition = new Vector3 (0,0,-500);
        ForceAddScript sc_forceAdd=FoceAdded_go.GetComponent<ForceAddScript>();
        sc_forceAdd.ShowEffect(force);

    }
	//铜币不足弹出快速购买界面
	public void NotEnoughMoneyPanel()
	{
		OpenQuickBuyPanel (CommonDefineManager.Instance.CommonDefine.QuickBuyCopperCoin);
	}
	//弹出快速购买界面\
	private GameObject buyPanel; 
	public void OpenQuickBuyPanel(int shopConfigID)
	{
		if (quickBuy == null)
			return;
		if (buyPanel != null)
			DestroyImmediate (buyPanel);
		buyPanel = UI.CreatObjectToNGUI.InstantiateObj (quickBuy,UICamera.transform.Find("Anchor"));
		buyPanel.transform.localPosition = new Vector3 (0,0,-100);
		QuickBuy buy = buyPanel.GetComponent<QuickBuy>();
		buy.Show (shopConfigID);

	}

    private UI.VigourMessagePanel BuyVigourMessagePanel;
    public void ShowAddVigour()
    {
        if(null == BuyVigourMessagePrefab)
        {
            return;
        }

        if (BuyVigourMessagePanel == null) { BuyVigourMessagePanel = UI.CreatObjectToNGUI.InstantiateObj(BuyVigourMessagePrefab, UICamera.transform.Find("Anchor")).GetComponent<UI.VigourMessagePanel>(); }
        string ShowStr = string.Format(LanguageTextManager.GetString("IDS_H1_158"), CommonDefineManager.Instance.CommonDefine.EnergyAdd);
        BuyVigourMessagePanel.Show(string.Format("{0}\n{1}", LanguageTextManager.GetString("IDS_H1_166"), ShowStr));
    }

    //public void ShowBattleResult(bool isSuccess)
    //{
    //    StartCoroutine(ShowSettleUI(isSuccess));
    //}
    //IEnumerator ShowSettleUI(bool isSuccess)
    //{
    //    yield return new WaitForSeconds(1.5f);
    //    string resultMsg = "";
    //    if (isSuccess)
    //    {
    //        resultMsg = "Success!";
    //    }
    //    else
    //    {
    //        resultMsg = "Failed!";
    //    }
    //    var m_popupWidget = Instantiate(this.BattleResult, Vector3.zero, Quaternion.identity) as GameObject;
    //    var scale = m_popupWidget.transform.localScale;
    //    var recTaskUI = m_popupWidget.GetComponent<RecTaskUI>();
    //    if (recTaskUI != null)
    //    {
    //        recTaskUI.BattleResult.text = resultMsg;
    //    }
    //    m_popupWidget.transform.parent = PopupObjManager.Instance.UICamera.transform;

    //    m_popupWidget.transform.localScale = scale;
    //}
}
