using UnityEngine;
using System.Collections;
using System.Linq;

namespace UI.MainUI{

public class SingleMeridiansAttributeLabel : MonoBehaviour {

    public UISprite AtbIcon;
    public UILabel StbLabel;
    public UILabel AddStbNumberLabel;

    public string PositionX;
    public string PositionY;

    EffectData myEffectData;

    private int CurrentAddNumber = 0;

    MeridiansPanel myParent;

    public void Init()
    {
        MeridiansEffectPositionData[] meridiansEffectPositionDataBase = myParent.PlayerMeridiansDataManager.PlayerEffectPositionDataBase.MeridiansEffectPositionDataList;
        MeridiansEffectPositionData myEffPoint = meridiansEffectPositionDataBase.First(P=>P.position[0] == PositionX&&P.position[1] == PositionY);
        myEffectData = ItemDataManager.Instance.EffectDatas._effects.First(P=>P.BasePropView == myEffPoint.effectID);
        this.AtbIcon.spriteName = myEffectData.EffectRes;
        StbLabel.SetText(LanguageTextManager.GetString(myEffectData.IDS));
        AddStbNumberLabel.SetText(CurrentAddNumber);
    }

    public void ResetInfo(MeridiansPanel myParent)
    {
        this.myParent = myParent;
        Init();
        CurrentAddNumber = 0;
        this.AtbIcon.spriteName = myEffectData.EffectRes;
        StbLabel.SetText(LanguageTextManager.GetString(myEffectData.IDS));
        AddStbNumberLabel.SetText(CurrentAddNumber);
    }

    public void AddNumber(EffectData effectData, int AddNumber)
    {
        //TraceUtil.Log("显示增加属性："+LanguageTextManager.GetString(effectData.IDS)+",Number:"+AddNumber);
        if (myEffectData.BaseProp != effectData.BaseProp)
            return;
        AddNumber = HeroAttributeScale.GetScaleAttribute(effectData,AddNumber);
        CurrentAddNumber += AddNumber;
        this.AtbIcon.spriteName = effectData.EffectRes;
        string AtbName = LanguageTextManager.GetString(effectData.IDS);
        StbLabel.SetText(AtbName);
        AddStbNumberLabel.SetText(CurrentAddNumber);
    }
}
}