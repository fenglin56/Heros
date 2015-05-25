using UnityEngine;
using System.Collections;

public class RankingUnit : MonoBehaviour {
    public SpriteSwith Topthree_swith;
    public UILabel RankingLable;
    public GameObject RankingBg;
    public void InitData(int Level)
    {
       
        if(Level<=3&&Level>0)
        {
            Topthree_swith.gameObject.SetActive(true);
            RankingLable.gameObject.SetActive(false);
            RankingBg.SetActive(false);
            Topthree_swith.ChangeSprite(Level);
        }
        else
        {
            RankingLable.SetText(Level);
            RankingLable.gameObject.SetActive(true);
            Topthree_swith.gameObject.SetActive(false);
            RankingBg.SetActive(true);
        }
    }

}
