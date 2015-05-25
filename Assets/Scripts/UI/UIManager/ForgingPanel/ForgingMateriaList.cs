using UnityEngine;
using System.Collections;
namespace UI.Forging
{
    public class ForgingMateriaList : MonoBehaviour {
    //public UILabel Title_des;
        public MaterialItem[] MaterialItems;

        public void Init(ForgeRecipeData data)
        {
            RestAllItem();
            Vector3 FirstPos=new Vector3(-60*(data.ForgeCost.Length-1),0,-1);
            int j=0;
            for(int i=0;i<data.ForgeCost.Length;i++)
            {


                MaterialItems[i].transform.localPosition=new Vector3(FirstPos.x+120*i,0,-1);
                MaterialItems[i].gameObject.SetActive(true);
                MaterialItems[i].Init(data,i);
            }
        }
        void RestAllItem()
        {
            MaterialItems.ApplyAllItem(c=>c.gameObject.SetActive(false));
        }
    }
}