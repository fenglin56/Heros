using UnityEngine;
using System.Collections;

namespace UI.Battle
{

    public class BattleSkillButtons : View
    {

        public BattleButton[] BattleButtons;
        private SkillConfigData[] ButtonInfo;

        protected override void RegisterEventHandler()
        {
            throw new System.NotImplementedException();
        }

        public void ShowButtons()
        {
            ButtonInfo = new SkillConfigData[4];
            for (int i = 0; i < ButtonInfo.Length; i++)
            {
                ButtonInfo[i] = SkillDataManager.Instance.GetSkillConfigData(PlayerManager.Instance.HeroSMsgSkillInit_SC.wSkillEquipList[i]);
            }
        }

    }
}