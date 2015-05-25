using UnityEngine;
using System.Collections;
using UI;
using System.Text;
using System.Linq;
public class PlayerHeadUnit : MonoBehaviour {
    public GameObject HeadIcon_prefab;
    public Transform HeadIconPoint;
    public SpriteSwith profession_swith;
    public UILabel NameLable;
    public UILabel Level_des;
    public UILabel Level;
    public Transform VipPoint;
    void Awake()
    {
        //Level_des.text=LanguageTextManager.GetString("");
    }
    public void InitData(int professionIndex,string name,int level,int vipLevel,uint fashID)
    {
        var HeroIcon_Ranking= CommonDefineManager.Instance.CommonDefine.HeroIcon_Ranking.SingleOrDefault(c=>c.FashionID==fashID&&c.VocationID==professionIndex);
        if(HeroIcon_Ranking!=null)
        {
            HeadIconPoint.GetComponent<UISprite>().spriteName=HeroIcon_Ranking.ResName;
        }
        profession_swith.ChangeSprite(professionIndex);
        NameLable.SetText(name);
        Level.SetText(level.ToString());
        VipPoint.ClearChild();
        CreatObjectToNGUI.InstantiateObj(PlayerDataManager.Instance.GetCurrentVipEmblemPrefab(vipLevel),VipPoint);

    }
}
