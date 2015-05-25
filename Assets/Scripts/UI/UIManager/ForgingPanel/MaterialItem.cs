using UnityEngine;
using System.Collections;

namespace UI.Forging
{
    public class MaterialItem : MonoBehaviour {
        public GameObject IconPoin;
        public UILabel Lable_Name;
        public UILabel Lable_MaterailCount;
        public SpriteSwith IconBackground;
        public ClickItem clickItem;
//        void Awake()
//        {
//           
//        }
        public void Init(ForgeRecipeData data,int index)
        {
           //   MaterailOwnCountLable_des.SetText(LanguageTextManager.GetString("IDS_I12_6"));
                IconPoin.transform.ClearChild();
                CreatObjectToNGUI.InstantiateObj(ForgingRecipeConfigDataManager.Instance.GetGoodsProfab(data.ForgeCost[index].RecipeID),IconPoin.transform);
                Lable_Name.text=ForgingRecipeConfigDataManager.Instance.GetGoodsName(data.ForgeCost[index].RecipeID);
                TextColor color=ForgingRecipeConfigDataManager.Instance.GetOwnMaterialCount(data.ForgeCost[index].RecipeID)<data.ForgeCost[index].count?TextColor.red:TextColor.green;
                Lable_MaterailCount.SetText(NGUIColor.SetTxtColor( ForgingRecipeConfigDataManager.Instance.GetOwnMaterialCount(data.ForgeCost[index].RecipeID)+"/"+ data.ForgeCost[index].count,color));
//            if(ForgingRecipeConfigDataManager.Instance.GetOwnMaterialCount(data.ForgeCost[index].RecipeID)<data.ForgeCost[index].count)
//            {
//                //.SetText(NGUIColor.SetTxtColor(ForgingRecipeConfigDataManager.Instance.GetOwnMaterialCount(data.ForgeCost[index].RecipeID).ToString(),TextColor.red) );
//            }
//            else
//            {
//               // MaterailOwnCountLable.SetText(ForgingRecipeConfigDataManager.Instance.GetOwnMaterialCount(data.ForgeCost[index].RecipeID));
//            }
                clickItem.Init(data.ForgeCost[index].RecipeID);
                IconBackground.ChangeSprite(ItemDataManager.Instance.GetItemData(data.ForgeCost[index].RecipeID)._ColorLevel+1);
        }
    }
}