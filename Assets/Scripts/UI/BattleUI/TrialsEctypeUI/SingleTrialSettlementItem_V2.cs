using UnityEngine;
using System.Collections;

namespace UI.Battle
{

    public class SingleTrialSettlementItem_V2 : MonoBehaviour
    {

        public UIPanel TweenShowPanel;

        public UILabel ProgressLabel;
        public UILabel TypeLabel;
        public UILabel ItemNameLabel;

        public SpriteSwith[] IsLastSprite;

        public TrialsSettlementpanel_V2 MyParent { get; private set; }

        void Awake()
        {
            TweenShowPanel.alpha = 0;
            IsLastSprite.ApplyAllItem(P=>P.ChangeSprite(1));
        }

        public void Show(int progress, int itemID, int itemNumber, bool isLast,TrialsSettlementpanel_V2 myParent)
        {
            this.MyParent = myParent;
            TraceUtil.Log("获得物品ID:"+itemID);
            ItemData itemData = ItemDataManager.Instance.GetItemData(itemID);
            string typeStr = string.Empty;
            string itemNameStr = string.Empty;
            switch (itemID)
            {
                case 3050001://铜币
                    typeStr = LanguageTextManager.GetString(itemData._szGoodsName);
                    itemNameStr = string.Format("+{0}", itemNumber);
                    break;
                case 3050002://元宝
                    typeStr = LanguageTextManager.GetString(itemData._szGoodsName);
                    itemNameStr = string.Format("+{0}", itemNumber);
                    break;
                case 3050003://活力
                    typeStr = LanguageTextManager.GetString(itemData._szGoodsName);
                    itemNameStr = string.Format("+{0}", itemNumber);
                    break;
                case 3050004://经验
                    typeStr = LanguageTextManager.GetString(itemData._szGoodsName);
                    itemNameStr = string.Format("+{0}", itemNumber);
                    break;
                case 3050005://妖气值
                    typeStr = LanguageTextManager.GetString(itemData._szGoodsName);
                    itemNameStr = string.Format("+{0}", itemNumber);
                    break;
                case 3050006://修为
                    typeStr = LanguageTextManager.GetString(itemData._szGoodsName);
                    itemNameStr = string.Format("+{0}", itemNumber);
                    break;
                default:
                    typeStr = LanguageTextManager.GetString("IDS_H1_558");//道具
                    itemNameStr = string.Format("{0}X{1}",NGUIColor.SetTxtColor(LanguageTextManager.GetString(itemData._szGoodsName),GetItemColor(itemData)), itemNumber);
                    break;
            }
            IsLastSprite.ApplyAllItem(P=>P.ChangeSprite(isLast?2:1));
            TextColor colortype = isLast ? TextColor.purple : TextColor.white;
            ProgressLabel.SetText(LanguageTextManager.GetString(string.Format("IDS_H1_{0}", 534 + progress)));
            TypeLabel.SetText(NGUIColor.SetTxtColor(typeStr,colortype));
            ItemNameLabel.SetText(itemNameStr);
            TweenFloat.Begin(0.3f,0,1,SetPanelAlpha);
        }

        void SetPanelAlpha(float value)
        {
            TweenShowPanel.alpha = value;
        }

        TextColor GetItemColor(ItemData itemData)
        {
            TextColor NameTextColor = TextColor.white;
            switch (itemData._ColorLevel)//物品品质颜色
            {
                case 0:
                    NameTextColor = TextColor.EquipmentGreen;
                    break;
                case 1:
                    NameTextColor = TextColor.EquipmentBlue;
                    break;
                case 2:
                    NameTextColor = TextColor.EquipmentMagenta;
                    break;
                case 3:
                    NameTextColor = TextColor.EquipmentYellow;
                    break;
                default:
                    break;
            }
            return NameTextColor;
        }
    }
}