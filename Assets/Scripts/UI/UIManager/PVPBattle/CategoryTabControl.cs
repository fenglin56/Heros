using UnityEngine;
using System.Collections;
using System.Linq;


[ExecuteInEditMode]
public class CategoryTabControl : MonoBehaviour 
{
    public LocalButtonCallBack Button_Bronze;
    public LocalButtonCallBack Button_Silver;
    public LocalButtonCallBack Button_Gold;
    public LocalButtonCallBack Button_Diamond;

    public UISprite InitialStateSprite;
    public UISprite BeSelectedSprite;
    public string InitialStateString;
    public string BeSelectedString;


    public enum GroupType
    {
        bronze = 1,
        silver = 2,
        gold = 3,
        diamond = 4,
    }

    private int[] m_guideBtnID;

    void Awake () 
    {
        m_guideBtnID = new int[4];
        Button_Bronze.SetCallBackFuntion(OnButtonBronzeClick, null);
        Button_Silver.SetCallBackFuntion(OnButtonSilverClick, null);
        Button_Gold.SetCallBackFuntion(OnButtonGoldClick, null);
        Button_Diamond.SetCallBackFuntion(OnButtonDiamondClick, null);

        //TODO GuideBtnManager.Instance.RegGuideButton(Button_Bronze.gameObject, UI.MainUI.UIType.PVPBattle, SubType.PVPCategoryRank, out m_guideBtnID[0]);
        //TODO GuideBtnManager.Instance.RegGuideButton(Button_Silver.gameObject, UI.MainUI.UIType.PVPBattle, SubType.PVPCategoryRank, out m_guideBtnID[1]);
        //TODO GuideBtnManager.Instance.RegGuideButton(Button_Gold.gameObject, UI.MainUI.UIType.PVPBattle, SubType.PVPCategoryRank, out m_guideBtnID[2]);
        //TODO GuideBtnManager.Instance.RegGuideButton(Button_Diamond.gameObject, UI.MainUI.UIType.PVPBattle, SubType.PVPCategoryRank, out m_guideBtnID[3]);
	}

    void OnDestroy()
    {
        for (int i = 0; i < m_guideBtnID.Length; i++ )
        {
            //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID[i]);
        }
    }

    public void ShowDefaultTop()
    {
        //默认排行数据
        var playerData = PlayerManager.Instance.FindHeroDataModel();
        var playerPrestigeList = PlayerDataManager.Instance.GetPlayerPrestigeList();
        var pvpData = playerPrestigeList.SingleOrDefault(p => p._pvpLevel == playerData.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_PRESTIGE_LEVEL);
        if (pvpData != null)
        {
            switch (pvpData._pvpGroup)
            {
                case 1:
                    this.OnButtonBronzeClick(null);
                    break;
                case 2:
                    this.OnButtonSilverClick(null);
                    break;
                case 3:
                    this.OnButtonGoldClick(null);
                    break;
                case 4:
                    this.OnButtonDiamondClick(null);
                    break;
                default:
                    TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"playerPrestige group is wrong");
                    break;
            }
        }
    }

    void OnButtonBronzeClick(object obj)
    {
        SetAllInitialState(GroupType.bronze);
        Button_Bronze.SetButtonSprite(BeSelectedString);        
    }

    void OnButtonSilverClick(object obj)
    {
        SetAllInitialState(GroupType.silver);
        Button_Silver.SetButtonSprite(BeSelectedString);        
    }

    void OnButtonGoldClick(object obj)
    {
        SetAllInitialState(GroupType.gold);
        Button_Gold.SetButtonSprite(BeSelectedString);        
    }

    void OnButtonDiamondClick(object obj)
    {
        SetAllInitialState(GroupType.diamond);
        Button_Diamond.SetButtonSprite(BeSelectedString);        
    }

    private void SetAllInitialState(GroupType groupType)
    {
        Button_Bronze.SetButtonSprite(InitialStateString);
        Button_Silver.SetButtonSprite(InitialStateString);
        Button_Gold.SetButtonSprite(InitialStateString);
        Button_Diamond.SetButtonSprite(InitialStateString);
        //
        SMSGPVPGetRankingListGroup groupData = new SMSGPVPGetRankingListGroup()
        {
            groupID = (int)groupType
        };
        EntityController.Instance.RaiseEvent(EventTypeEnum.ClearRankingList.ToString(), groupData);


    }

}
