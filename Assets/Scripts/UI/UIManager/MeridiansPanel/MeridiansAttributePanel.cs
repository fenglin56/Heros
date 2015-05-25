using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.MainUI
{

    public class MeridiansAttributePanel : MonoBehaviour
    {

        public SingleMeridiansAttributeLabel[] MeridiansAtbList;

        public void Show(MeridiansPanel myParent)
        {
            int currentMeridiansLv = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_MERIDIANS_LEVEL;
            var currentData = myParent.PlayerMeridiansDataManager.playerMeridiansDataBase.PlayermeridiansDataList.Where(P => P.MeridiansLevel <= currentMeridiansLv);
            MeridiansAtbList.ApplyAllItem(P=>P.ResetInfo(myParent));
            foreach (var child in currentData)
            {
                if (child.MeridiansLevel == 0)
                    continue;
                EffectData addEffect = ItemDataManager.Instance.EffectDatas._effects.First(P=>P.m_SzName == child.EffectAdd.Split('+')[0]);
                MeridiansAtbList.ApplyAllItem(P => P.AddNumber(addEffect, int.Parse(child.EffectAdd.Split('+')[1])));
            }
        }

    }
}