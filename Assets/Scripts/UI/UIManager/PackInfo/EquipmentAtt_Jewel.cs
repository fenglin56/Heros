using UnityEngine;
using System.Collections;

namespace UI.MainUI
{
    public class EquipmentAtt_Jewel : MonoBehaviour
    {
        public UILabel Title;
        public UILabel Content;
        public Transform iconPoint;

        public void init(Jewel jewel, PassiveSkillData skill, bool IsSuit, int type)
        {
            if (IsSuit)
            {
                Title.SetText(LanguageTextManager.GetString("IDS_I9_6"));
            } else
            {
                Title.SetText(LanguageTextManager.GetString("IDS_I9_5"));
            }
            CreatObjectToNGUI.InstantiateObj(skill.SkillIconPrefab, iconPoint);
            Content.SetText(NGUIColor.SetTxtColor(LanguageTextManager.GetString(skill.SkillName) + ":", (TextColor)type) + NGUIColor.SetTxtColor("lv" + skill.SkillLevel, TextColor.ChatYellow)+" "+ LanguageTextManager.GetString(skill.SkillDis));

        }

        TextColor  GetNameColor(int type)
        {
            if (type == 0)
            {
                                
                return TextColor.red;
                        
            } else if (type == 1)
            {
                                
                return TextColor.yellow;
                        
            } else
            {
                return TextColor.blue;          
            }
        }
    }
}
