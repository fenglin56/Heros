using UnityEngine;
using System.Collections;
using UI.MainUI;
using System.Text;


public class RoleEquiptPanel_Readonly : MonoBehaviour {
    public UILabel NameLabel;
    public UILabel Levellabel;
    public UILabel ForceLabel;
    
    public GoodsItem[] EquiptSlotList;
    public RoleModelPanel_ranking m_RoleModelPanel;
    public GameObject RoleEffectObj;
    public SingleButtonCallBack DragRoleModelButton;
    public RoleViewPoint RoleViewPoint;
    //public PackInfoPanel MyParent{get;private set;}
    public Transform VipEmblemPoint;
    public Transform TitlePoint;
    void Start()
    {
       
        DragRoleModelButton.SetDragCallback(m_RoleModelPanel.OnDragRoleModel);
        m_RoleModelPanel.transform.ClearChild();
    }

    public void ShowForTime(SMsgInteract_GetPlayerRanking_SC data)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_UIEff_PackageAppear");
        for(int i=0;i<data.dwGoods.Length-1;i++)//第六个是药品，不用
        {
            EquiptSlotList[i].Init(data.dwGoods[i]);
        }
        SetCameraPanelPosition();
        m_RoleModelPanel.ShowHeroModelView();
        m_RoleModelPanel.AttachEffect(RoleEffectObj);
        m_RoleModelPanel.Show(data);
        UpdateHeroAttribute(data);
        ShowVipEmblem(data.byVipLevel);
        UpdateTitleDisplay((int)data.dwTitleID);
    }

    public void SetCameraPanelPosition()
    {
       

        Camera uiCamera = UICamera.mainCamera;
        Vector3 CameraPosition = uiCamera.WorldToScreenPoint(RoleViewPoint.transform.position);
        //          BackGrondPanel.transform.position = BackUICamera.ScreenToWorldPoint(CameraPosition);
        //          roleAttributePanel.SetPanelPosition(CameraPosition);
        Vector3 LPosition = uiCamera.WorldToScreenPoint(RoleViewPoint.LBound.position);
        Vector3 RPosition = uiCamera.WorldToScreenPoint(RoleViewPoint.RBound.position);
        var lRoleRec = uiCamera.ScreenToViewportPoint(LPosition);
        var rRoleRec = uiCamera.ScreenToViewportPoint(RPosition);
        var w = rRoleRec.x - lRoleRec.x;
        StartCoroutine(m_RoleModelPanel.SetCameraPosition(lRoleRec, w));
    }


    public void ShowVipEmblem(int level)
    {
        VipEmblemPoint.ClearChild();
        GameObject go=PlayerDataManager.Instance.GetCurrentVipEmblemPrefab(level);
        if(go!=null)
        {
            UI.CreatObjectToNGUI.InstantiateObj(go,VipEmblemPoint);
        }
        
    }

    //更新显示当前称号
    private void UpdateTitleDisplay(int TitleID)
    {
        TitlePoint.ClearChild();
       // int titleID = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_TITLE;
        var titleData = PlayerDataManager.Instance.GetPlayerTitleConfigData(TitleID);
        if(titleData == null)
            return;
        UI.CreatObjectToNGUI.InstantiateObj( titleData._ModelIdPrefab, TitlePoint);
    }

    void UpdateHeroAttribute(SMsgInteract_GetPlayerRanking_SC data)
    {
        NameLabel.SetText(Encoding.UTF8.GetString(data.szActorName));
        ForceLabel.SetText(data.dwActorFinght.ToString());
        Levellabel.SetText(string.Format("Lv:{0}",data.byActorLevel));
      
    }


}
