using UnityEngine;
using System.Collections;

public class PopupIconObj : MonoBehaviour {


    public Transform CreatIconPoint;
    public UILabel GetNumberLabel;

    public void Show(FightEffectType fightEffectType,string addNumber)
    {
        if (CreatIconPoint.childCount > 0)
        {
            CreatIconPoint.ClearChild();
        }
        GetNumberLabel.SetText(addNumber);
        switch (fightEffectType)
        {
            case FightEffectType.BATTLE_ADDMONEY:
                UI.CreatObjectToNGUI.InstantiateObj(IconPrefabManager.Instance.getIcon("BattleGet_GameMoney"),CreatIconPoint);
                break;
            case FightEffectType.BATTLE_EFFECT_SHILIAN_EXPSHOW:
                UI.CreatObjectToNGUI.InstantiateObj(IconPrefabManager.Instance.getIcon("BattleGet_Exp"), CreatIconPoint);
                break;
            case FightEffectType.BATTLE_EFFECT_SHILIAN_XIUWEI:
                UI.CreatObjectToNGUI.InstantiateObj(IconPrefabManager.Instance.getIcon("BattleGet_Practice"), CreatIconPoint);
                break;
            case FightEffectType.BATTLE_EFFECT_EXPSHOW:
                UI.CreatObjectToNGUI.InstantiateObj(IconPrefabManager.Instance.getIcon("BattleGet_Exp"), CreatIconPoint);
                break;
            case FightEffectType.TOWN_EFFECT_ZHANLI:
                UI.CreatObjectToNGUI.InstantiateObj(IconPrefabManager.Instance.getIcon("TownGet_ZhanLi"), CreatIconPoint);
                break;
            default:
                break;
        }
    }


}
