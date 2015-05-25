using UnityEngine;
using System.Collections;
using System.Linq;

namespace UI.MainUI
{
    public class DailyTaskRewardItem : MonoBehaviour
    {
        public int BoxSequence { get; set; }
        public UISprite Sprite_Item_chest;
        public UILabel Label_active;//需要活跃值
        public SpriteSwith[] Switch_Chest = new SpriteSwith[2];
        //public UILabel[] Label_Chest_Txt = new UILabel[2];
        public UILabel[] Label_Chest_Value = new UILabel[2];
        public UILabel[] Label_Prop = new UILabel[2];

        public void Init(DailyTaskRewardConfigData configData)
        {
            var playerData = PlayerManager.Instance.FindHeroDataModel();
            int vocation = playerData.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;

            Sprite_Item_chest.spriteName = configData._boxDisplayId;
            Label_active.text = configData._requirementActiveValue.ToString();

            ShowRewardInfo(configData._awardType[0], vocation, configData,
                           Switch_Chest[0], Label_Chest_Value[0], Label_Prop[0]);

            if (configData._awardType.Length < 2)
            {
                Switch_Chest[1].gameObject.SetActive(false);
                //Label_Chest_Txt[1].gameObject.SetActive(false);
                Label_Chest_Value[1].gameObject.SetActive(false);
                Label_Prop[1].gameObject.SetActive(false);
            }
            else
            {
                Switch_Chest[1].gameObject.SetActive(true);
                //Label_Chest_Txt[1].gameObject.SetActive(true);
                Label_Chest_Value[1].gameObject.SetActive(true);
                Label_Prop[1].gameObject.SetActive(true);
                ShowRewardInfo(configData._awardType[1], vocation, configData,
                    Switch_Chest[1],  Label_Chest_Value[1], Label_Prop[1]);
            }           
        }
        private void ShowRewardInfo(int type, int vocation, DailyTaskRewardConfigData data, SpriteSwith ss,  UILabel label_value, UILabel label_prop)
        {
            ss.gameObject.SetActive(true);
            ss.ChangeSprite(type);
            switch (type)
            {
                case 1:
                    var awardItem = data._awardItem.SingleOrDefault(p => p.Profession == vocation);
                    if (awardItem != null)
                    {                        
                        var itemConfig = ItemDataManager.Instance.GetItemData(awardItem.PropID);
                        //label_txt.text = LanguageTextManager.GetString(itemConfig._szGoodsName);
                        label_prop.text = LanguageTextManager.GetString(itemConfig._szGoodsName);
                        label_value.text = "";
                    }
                    break;
                case 2:
                    //label_txt.text = LanguageTextManager.GetString("IDS_D2_17");
                    label_prop.text = "";
                    label_value.text = "+" + data._awardMoney;
                    break;
                case 3:
                    //label_txt.text = LanguageTextManager.GetString("IDS_A1_5017");
                    label_prop.text = "";
                    label_value.text = "+" + data._awardExp;
                    break;
                case 4:
                    //label_txt.text = LanguageTextManager.GetString("IDS_H1_120");
                    label_prop.text = "";
                    label_value.text = "+" + data._awardActive;
                    break;
                case 5:
                    //label_txt.text = LanguageTextManager.GetString("IDS_A1_5019");
                    label_prop.text = "";
                    label_value.text = "+" + data._awardXiuwei;
                    break;
                case 6:
                    //label_txt.text = LanguageTextManager.GetString("IDS_D2_18");
                    label_prop.text = "";
                    label_value.text = "+" + data._awardIngot;
                    break;
            }
            
        }
    }


}