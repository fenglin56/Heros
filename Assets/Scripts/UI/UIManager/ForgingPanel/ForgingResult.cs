using UnityEngine;
using System.Collections;
namespace UI.Forging
{
public class ForgingResult : MonoBehaviour {
    public ForgingGoodsAtt sc_ForgingGoodsAtt;
    public ForgingMateriaList sc_ForgingMateriaList;
   // public UILabel TipsLabe;
        private ForgeRecipeData CurrentData;

    public void InitForgResult(ForgeRecipeData data)
    {
            CurrentData=data;
            if(sc_ForgingGoodsAtt!=null)
            {
                sc_ForgingGoodsAtt.Init(data);
            }
            if(sc_ForgingMateriaList!=null)
            {
                sc_ForgingMateriaList.Init(data);
            }

    }
        public void RefreshCurrentResult()
        {
            if(sc_ForgingGoodsAtt!=null)
            {
                sc_ForgingGoodsAtt.Init(CurrentData);
            }
            if(sc_ForgingMateriaList!=null)
            {
                sc_ForgingMateriaList.Init(CurrentData);
            }
        }

}
}
