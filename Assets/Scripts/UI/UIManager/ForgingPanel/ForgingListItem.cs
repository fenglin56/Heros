using UnityEngine;
using System.Collections;
using System;

namespace UI.Forging
{
[RequireComponent(typeof(BoxCollider),typeof(UIEventListener))]
public class ForgingListItem : MonoBehaviour {
       // public GameObject IsOwnIcon;
        public UILabel Lable_Name;
        public UILabel Lable_Level;
        public UILabel Lable_Isown;
        public GameObject SelectedStyle;
        public Transform IconPoint;
        public ForgeRecipeData ForgeRecipeItem;
        public SpriteSwith IconBackground;
        public Action<ForgingListItem> OnClickCallBack;
      //  public ClickItem clickItem;
        void Awake()
        {
           // Material_des.text=LanguageTextManager.GetString("IDS_I12_3");
            GetComponent<UIEventListener>().onClick=OnItemClick;
        }
        void OnItemClick(GameObject obj)
        {
            if(OnClickCallBack!=null)
            {
                SoundManager.Instance.PlaySoundEffect("Sound_Button_Forge_Click");
                ForgingPanelManager.GetInstance().CloseSelectPanel();
                OnClickCallBack(this);
            }
        }
    
        public  void BeSelected()
        {
            OnClickCallBack(this);
        }
        public  void OnGetFocus() 
        {
            SelectedStyle.SetActive(true);
        }
    
        public  void OnLoseFocus() 
        {
            SelectedStyle.SetActive(false);   
        }

        public void InitItemData(ForgeRecipeData RecipeItem)
        {
                if(RecipeItem!=null)
                {
               // clickItem.Init(RecipeItem.ForgeEquipmentID);
                   // IsOwnIcon.SetActive(ForgingRecipeConfigDataManager.Instance.IsOwn(RecipeItem));
                    ForgeRecipeItem=RecipeItem;
                    Lable_Name.text= ForgingRecipeConfigDataManager.Instance.GetGoodsName(RecipeItem.ForgeEquipmentID);
                      Lable_Level.text= ForgingRecipeConfigDataManager.Instance.GetGoodsLevel(RecipeItem.ForgeEquipmentID);
                     IconPoint.ClearChild();
                     Lable_Isown.gameObject.SetActive(ForgingRecipeConfigDataManager.Instance.IsCanForging(RecipeItem));
                    UI.CreatObjectToNGUI.InstantiateObj(ForgingRecipeConfigDataManager.Instance.GetGoodsProfab(RecipeItem.ForgeEquipmentID),IconPoint);
                    IconBackground.ChangeSprite(ItemDataManager.Instance.GetItemData(RecipeItem.ForgeEquipmentID)._ColorLevel+1);
                }
                else
                {
                    IconBackground.ChangeSprite(5);
                }
             //RefreshItem();
        }

        string GetMarialText(ForgeRecipeData RecipeItem)
            {
                int TotalNeed=0;
                int OwnCount=0;
                string str;
                foreach(var item in RecipeItem.ForgeCost )
                 {
                   TotalNeed+=item.count;
                   Debug.Log( ForgingRecipeConfigDataManager.Instance.GetOwnMaterialCount(item.RecipeID)+"/"+item.count+"/"+item.RecipeID);
                   OwnCount+= Math.Min( ForgingRecipeConfigDataManager.Instance.GetOwnMaterialCount(item.RecipeID),item.count);
                 }
                if(OwnCount<TotalNeed)
                {
                     str=NGUIColor.SetTxtColor(OwnCount+"/"+TotalNeed,TextColor.red);
                }
                else
                 {
                        str=OwnCount+"/"+TotalNeed;
                 }
                return str;
        
        }
}
}
