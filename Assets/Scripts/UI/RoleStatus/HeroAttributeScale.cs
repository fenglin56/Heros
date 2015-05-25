using UnityEngine;
using System.Collections;
using System.Linq;


public class HeroAttributeScale : MonoBehaviour {


    public static int GetScaleAttribute(int basePropView, int ResAtb)
    {
        EffectData effectData = ItemDataManager.Instance.EffectDatas._effects.First(P => P.BasePropView == basePropView);
        return GetScaleAttribute(effectData, ResAtb);
    }

    public static int GetScaleAttribute(string EffectName, int ResAtb)
    {
        EffectData effectData = ItemDataManager.Instance.EffectDatas._effects.First(P => LanguageTextManager.GetString(P.IDS) == EffectName);
        return GetScaleAttribute(effectData,ResAtb);
    }

    public static int GetScaleAttribute(HeroAttributeScaleType heroAttributeScaleType, int ResAtb)
    {
        EffectData effectData = ItemDataManager.Instance.EffectDatas._effects.First(P => P.BasePropView == (int)heroAttributeScaleType);
        return GetScaleAttribute(effectData, ResAtb);
    }

    public static int GetScaleAttribute(EffectData effectData, int ResAtb)
    {
        float newAtb = 0;
        switch (effectData.BasePropView)
        {
            case 10:
                newAtb = (float)ResAtb * CommonDefineManager.Instance.CommonDefineFile._dataTable.Display_MaxHp;
                break;
            case 20:
                newAtb = (float)ResAtb * CommonDefineManager.Instance.CommonDefineFile._dataTable.Display_MaxMP;
                break;
            case 30:
                newAtb = (float)ResAtb * CommonDefineManager.Instance.CommonDefineFile._dataTable.Display_HIT;
                break;
            case 40:
                newAtb = (float)ResAtb * CommonDefineManager.Instance.CommonDefineFile._dataTable.Display_ATK;
                break;
            case 50:
                newAtb = (float)ResAtb * CommonDefineManager.Instance.CommonDefineFile._dataTable.Display_DEF;
                break;
            case 60:
                newAtb = (float)ResAtb * CommonDefineManager.Instance.CommonDefineFile._dataTable.Display_EVA;
                break;
            case 70:
                newAtb = (float)ResAtb * 1;
                break;
            case 80:
                newAtb = (float)ResAtb * CommonDefineManager.Instance.CommonDefineFile._dataTable.Display_CurHp;
                break;
            case 90:
                newAtb = (float)ResAtb * CommonDefineManager.Instance.CommonDefineFile._dataTable.Display_CurMp;
                break;
            case 100:
                newAtb = (float)ResAtb * CommonDefineManager.Instance.CommonDefineFile._dataTable.Display_Crit;
                break;
            case 110:
                newAtb = (float)ResAtb * CommonDefineManager.Instance.CommonDefineFile._dataTable.Display_ResCrit;
                break;
            case 120:
                newAtb = (float)ResAtb * 1;
                break;
            case 130:
                newAtb = (float)ResAtb * CommonDefineManager.Instance.CommonDefineFile._dataTable.Display_Combat;
                break;
        }
        return (int)newAtb;
    }

}
public enum HeroAttributeScaleType
{ 
    Display_MaxHp = 10,                 //界面显示参数_生命值上限
    Display_CurHp = 80,                  //界面显示参数_当前生命值
    Display_MaxMP = 20,                      //界面显示参数_真气值上限
    Display_CurMp = 90,                          //界面显示参数_当前真气值
    Display_HIT = 30,                            //界面显示参数_命中
    Display_ATK = 40,                    //界面显示参数_攻击
    Display_DEF = 50,                        //界面显示参数_防御
    Display_EVA = 60,                                //界面显示参数_躲闪
    Display_Crit = 100,                   //界面显示参数_暴击值
    Display_ResCrit = 110,                        //界面显示参数_抗暴击
    Display_Combat = 130,                     //界面显示参数_战力
}
