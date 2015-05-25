using UnityEngine;
using System.Collections;
using System.Linq;

public class PVPInterfaceInfo : MonoBehaviour
{
    public UILabel Label_Category;
    //public UILabel Label_Name;
    public UILabel Label_Rank;
    public UILabel Label_Renown;
    public UILabel Label_Fighting;
    public UILabel Label_ConsecutiveWin;
    public UILabel Label_ChallengingTimes;
    public UISlider Slider_ChanllengingTimes;

    public void ReadPlayerInfo()
    {
        //this.Init();

        var playerData = PlayerManager.Instance.FindHeroDataModel();
        //Label_Name.text = playerData.Name;  //名字
        //Label_Rank.text = m_strRank + "1";  //排名

        int prestigeLevel = playerData.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_PRESTIGE_LEVEL;
        var prestigeData = PlayerDataManager.Instance.GetPlayerPrestigeList().SingleOrDefault(p => p._pvpLevel == prestigeLevel);
        if (prestigeData != null)
        {
            Label_Category.text = LanguageTextManager.GetString(prestigeData._groupName);
        }
        Label_Renown.text = playerData.PlayerValues.PLAYER_FIELD_PRESTIGE.ToString();   //威望
        Label_Fighting.text = playerData.UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_FIGHTING.ToString();    //战力
        Label_ConsecutiveWin.text = playerData.PlayerValues.PLAYER_FIELD_WINNINGSTREAK_TIMES.ToString();    //连胜
        //Sprite_HeadIcon.spriteName = GetSpriteName(playerData.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION);
        Label_ChallengingTimes.text = playerData.PlayerValues.PLAYER_FIELD_PVP_TIMES + "/10";
        
        Slider_ChanllengingTimes.sliderValue = playerData.PlayerValues.PLAYER_FIELD_PVP_TIMES / 10f;
    }

    public void SetRankLabel(int no)
    {
        if (no < 10000)
        {
            Label_Rank.text = no.ToString();
        }
        else
        {
            Label_Rank.text = LanguageTextManager.GetString("IDS_H1_391");
        }
    }


    private string GetSpriteName(int kind)
    {
        switch(kind)
        {
            case 1:
                return "JH_UI_HeadIcon_001_01";
            case 4:
                return "JH_UI_HeadIcon_002_01";
            default:
                return "JH_UI_HeadIcon_001_01";
        }        
    }
}
