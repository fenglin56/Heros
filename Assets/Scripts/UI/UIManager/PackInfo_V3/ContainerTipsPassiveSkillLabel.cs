using UnityEngine;
using System.Collections;

namespace UI.MainUI
{

    public class ContainerTipsPassiveSkillLabel : MonoBehaviour
    {

        public Transform CreatIconTransform;
        public UILabel SkillDesLabel;

        public void ShowEffect(PassiveSkillData skillData)
        {
            CreatIconTransform.ClearChild();
            if (skillData == null)
            {
                SkillDesLabel.SetText("");
            }
            else
            {
                CreatObjectToNGUI.InstantiateObj(skillData.SkillIconPrefab,CreatIconTransform);
                SkillDesLabel.SetText(string.Format("{0}:{1}",LanguageTextManager.GetString(skillData.SkillName),LanguageTextManager.GetString(skillData.SkillDis)));
            }
        }


    }
}