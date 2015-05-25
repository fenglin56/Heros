using UnityEngine;
using System.Collections;
namespace UI.Forging
{
public class ForgingGoodsAtt : MonoBehaviour {
        public GameObject go_IconItem;
        public Transform IconPoint;
        public GameObject go_Profession;
        public UILabel IDS_professionLable;
        public UILabel Lable_Profession;
        public UILabel Label_GoodsName;
        public UILabel Lable_Level;
        public UILabel IDS_GoodsDes;
        public SpriteSwith IconBackground;
        public ClickItem clickItem;
        void Awake()
        {
           // Level_des.text=LanguageTextManager.GetString("IDS_I12_10");
            IDS_professionLable.text=LanguageTextManager.GetString("IDS_I12_4");
           // IDS_GoodsDes.SetText("sddsdsd");

        }
	    public void Init(ForgeRecipeData data)
        {  
            clickItem.Init(data.ForgeEquipmentID);
            IconPoint.ClearChild();
            ItemData itemdata=ItemDataManager.Instance.GetItemData(data.ForgeEquipmentID);
            IconBackground.ChangeSprite(itemdata._ColorLevel+1);
            CreatObjectToNGUI.InstantiateObj(ForgingRecipeConfigDataManager.Instance.GetGoodsProfab(data.ForgeEquipmentID),IconPoint);
            Label_GoodsName.text=ForgingRecipeConfigDataManager.Instance.GetGoodsName(data.ForgeEquipmentID);
            IDS_GoodsDes.SetText(LanguageTextManager.GetString(itemdata._szDesc));
            if(itemdata._GoodsClass==1)
            {
                Lable_Level.gameObject.SetActive(true);
                go_Profession.SetActive(true);
                Lable_Profession.SetText(ForgingRecipeConfigDataManager.Instance.GetProfession(data.ForgeEquipmentID));
               // go_IconItem.transform.localPosition=pos_IconItem_equip;
                Lable_Level.SetText(ForgingRecipeConfigDataManager.Instance.GetGoodsLevel(data.ForgeEquipmentID));

            }
            else
            {
                Lable_Level.gameObject.SetActive(false);
                go_Profession.SetActive(false);
               // go_IconItem.transform.localPosition=pos_IconItem_Other;
            }
        }
}
}
