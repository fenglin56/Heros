using UnityEngine;
using System.Collections;

public class PVPRankInfoControl : MonoBehaviour, IPagerItem
{
    public UILabel Label_RankingIndex;
    public UILabel Label_Level;
    public UILabel Label_Name;
    public UILabel Label_Prestige;
    public UILabel Label_FightingNum;
    public UISlicedSprite Sprite_Bg;

    private Color mColor = new Color(0, 1f, 1f);

//    public void Set(RankingData_SC rankingData)
//    {
//        Label_RankingIndex.text = rankingData.byRankingIndex.ToString();
//        Label_Level.text = rankingData.byLevel.ToString();
//        Label_Name.text = rankingData.szName;
//        Label_Prestige.text = rankingData.dwPrestige.ToString();
//        Label_FightingNum.text = rankingData.dwFightNum.ToString();
//
//        //Label_RankingIndex.text = "1";
//        //Label_Level.text = "1";
//        //Label_Name.text = "无名";
//        //Label_Prestige.text = "1";
//        //Label_FightingNum.text = "1";
//    }

    public void SetMyInfoColor()
    {
        Label_RankingIndex.color = mColor;
        Label_Level.color = mColor;
        Label_Name.color = mColor;
        Label_Prestige.color = mColor;
        Label_FightingNum.color = mColor;
        Sprite_Bg.color = Color.red;
    }

    //public void SetMyData()
    //{
    //    var playerData = PlayerManager.Instance.FindHeroDataModel();
    //    //Label_RankingIndex.text = playerData;
    //    Label_Level.text = playerData.UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL.ToString();
    //    Label_Name.text = playerData.Name;  //名字
    //    Label_Prestige.text = playerData.PlayerValues.PLAYER_FIELD_PRESTIGE.ToString();
    //    Label_FightingNum.text = playerData.UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_FIGHTING.ToString();       
    //}

    public void SetRankNo(ushort rankingNo)
    {
        var playerData = PlayerManager.Instance.FindHeroDataModel();
        if (rankingNo >= 10000)
        {
            Label_RankingIndex.text = LanguageTextManager.GetString("IDS_H1_391");
        }
        else
        {
            Label_RankingIndex.text = rankingNo.ToString();
        }
        
        Label_Level.text = playerData.UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL.ToString();
        Label_Name.text = playerData.Name;  //名字
        Label_Prestige.text = playerData.PlayerValues.PLAYER_FIELD_PRESTIGE.ToString();
        Label_FightingNum.text = playerData.UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_FIGHTING.ToString();               
    }

    public void OnGetFocus()
    {
        
    }

    public void OnLoseFocus()
    {
        
    }

    public void OnBeSelected()
    {
       
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
