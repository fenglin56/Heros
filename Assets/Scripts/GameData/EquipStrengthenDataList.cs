using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class EquipStrengthenData
{
    public int StartLv;
    public int EndLv;
    public string Prefix;
    public string Postfix;
}
public class EquipStrengthenDataList : ScriptableObject
{
    public EquipStrengthenData[] _EquipStrengthenDatas;

    public string GetBeforeEquipLevel(int currentLevel)
    {
        string result = string.Empty;
        if (currentLevel == 0)
        {
            result = LanguageTextManager.GetString("IDS_H1_186");//"未强化";
        }
        else
        {
            int ilevel = currentLevel;
            string prefix = string.Empty, postfix = string.Empty;
            EquipStrengthenData equipStrengthenData = _EquipStrengthenDatas.SingleOrDefault(P => currentLevel >= P.StartLv && currentLevel <= P.EndLv);

            ilevel = ilevel % 10;
            ilevel = ilevel == 0 ? 10 : ilevel;
            prefix = LanguageTextManager.GetString(equipStrengthenData.Prefix);  //TODO:IDS  红玉，暗金 什么的...
            postfix = LanguageTextManager.GetString(equipStrengthenData.Postfix); // 级

            result = string.Format("{0}{1}{2}", prefix, ilevel, postfix);
        }
        
        return result;
    }
    public string GetAfterEquipLevel(int targetLevel)
    {
        string result = string.Empty;
        if (targetLevel == 0)
        {
            result = LanguageTextManager.GetString("IDS_H1_186");//"未强化";
        }
        else
        {
            int ilevel = targetLevel;
            string prefix = string.Empty, postfix = string.Empty;
            EquipStrengthenData equipStrengthenData = _EquipStrengthenDatas.SingleOrDefault(P => targetLevel >= P.StartLv && targetLevel <= P.EndLv);
            if (equipStrengthenData != null)
            {
                ilevel = ilevel % 10;
                ilevel = ilevel == 0 ? 10 : ilevel;
                prefix = LanguageTextManager.GetString(equipStrengthenData.Prefix);  //TODO:IDS  红玉，暗金 什么的...
                postfix = LanguageTextManager.GetString(equipStrengthenData.Postfix); // 级
            }

            result = string.Format("{0}{1}{2}", prefix, ilevel, postfix);
        }

        return result;
    }
}