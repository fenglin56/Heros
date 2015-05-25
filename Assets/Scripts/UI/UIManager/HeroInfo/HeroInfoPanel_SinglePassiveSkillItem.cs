using UnityEngine;
using System.Collections;

namespace UI.MainUI
{

    public class HeroInfoPanel_SinglePassiveSkillItem : MonoBehaviour
    {

        public UILabel DesLabel;
        public Transform CreatItemIconTransform;

        public void Show(PassiveSkillData passiveSkillData)
        {
            ResetWidgetInfo();
            DesLabel.SetText(string.Format("{0} : {1}", LanguageTextManager.GetString(passiveSkillData.SkillName), LanguageTextManager.GetString(passiveSkillData.SkillDis)));
            CreatObjectToNGUI.InstantiateObj(passiveSkillData.SkillIconPrefab, CreatItemIconTransform);
        }

        public void ResetWidgetInfo()
        {
            DesLabel.SetText(string.Empty);
            CreatItemIconTransform.ClearChild();
        }

    }
}