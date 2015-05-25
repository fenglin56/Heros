using UnityEngine;
using System.Collections;
using UI;
using UI.Ranking;
using UI.MainUI;

public class GoodsItem : MonoBehaviour {
    public SpriteSwith NormalBg;
    public Transform GoodsIconPoint;
    public SpriteSwith StarIcon;
    public GameObject EqLevel;
    public UILabel EqLevelLable;
    public GameObject JewelLevel;
    public UILabel JewelLable;
    public GameObject Background;
    public enum GoodsType{equip,jewel1,jewel2};
    private SingleButtonCallBack ItemBtn;
    private int currentDataID;


    void OnbtnClick(object obj)
    {
        ItemInfoTipsManager.Instance.Show(currentDataID);
    }
    public void Init(GoodsType type,SEquipRankingData data)
    {
        currentDataID=(int)data.dwGoodsID;
        GoodsIconPoint.ClearChild();
        ItemData itemdata=null;
        switch(type)
        {
            case GoodsType.equip:
                if(JewelLevel)
                {
                JewelLevel.SetActive(false);
                }

                if(data.byStrengLevel>0)
                {
                    EqLevel.SetActive(true);
                    EqLevelLable.SetText(System.Convert.ToString(data.byStrengLevel));
                }
                else
                {
                    EqLevel.SetActive(false);
                }
               
                if(data.byStartLevel>0)
                {
                StarIcon.gameObject.SetActive(true);
                StarIcon.ChangeSprite((data.byStartLevel-1)/10+1);
                }
                else
                {
                    StarIcon.gameObject.SetActive(false);
                }

                itemdata= ItemDataManager.Instance.GetItemData((int)data.dwGoodsID);
               
                break;
            case GoodsType.jewel1:
                JewelLevel.SetActive(true);
                EqLevel.SetActive(false);
                JewelLable.SetText(data.byGemLevel1.ToString());
                StarIcon.gameObject.SetActive(false);
                itemdata= ItemDataManager.Instance.GetItemData((int)data.dwGemID1+PlayerDataManager.JEWEL_BASE_ID);
                break;
            case GoodsType.jewel2:
                JewelLevel.SetActive(true);
                EqLevel.SetActive(false);
                JewelLable.SetText(data.byGemLevel2.ToString());
                StarIcon.gameObject.SetActive(false);
                itemdata= ItemDataManager.Instance.GetItemData((int)data.dwGemID2+PlayerDataManager.JEWEL_BASE_ID);
                break;

        }
        if(itemdata!=null)
        {

            NormalBg.ChangeSprite(itemdata._ColorLevel+1);
            UI.CreatObjectToNGUI.InstantiateObj(itemdata._picPrefab,GoodsIconPoint);
        }
        else
        {
            JewelLevel.SetActive(false);
            EqLevel.SetActive(false);
            StarIcon.gameObject.SetActive(false);
            NormalBg.ChangeSprite(5);
        }


    }

    public void Init(SEquipInfo data)
    {
        if(ItemBtn==null)
        {
            ItemBtn=gameObject.AddComponent<SingleButtonCallBack>();
            if(GetComponent<BoxCollider>()==null)
            {
                BoxCollider colldider=gameObject.AddComponent<BoxCollider>();
                colldider.size=new Vector3(100,100,0);
            }
            
        }
        ItemBtn.SetCallBackFuntion(OnbtnClick);
        currentDataID=(int)data.dwGoodsID;
                GoodsIconPoint.ClearChild();
            if (data.byStrengLevel > 0)
        {
            EqLevel.SetActive(true);
            EqLevelLable.SetText(data.byStrengLevel);
        } else
        {
            EqLevel.SetActive(false);
        }
            if (data.byStartLevel > 0)
        {
            StarIcon.gameObject.SetActive(true);
            StarIcon.ChangeSprite((data.byStartLevel-1) / 10+1);
        } else
        {
            StarIcon.gameObject.SetActive(false);
        }
                ItemData itemdata=null;
                itemdata= ItemDataManager.Instance.GetItemData((int)data.dwGoodsID);
                
  
        if(itemdata!=null)
        {
            NormalBg.ChangeSprite(itemdata._ColorLevel+1);
           GameObject obj= CreatObjectToNGUI.InstantiateObj(itemdata._picPrefab,GoodsIconPoint);
            if(Background)
            {
                Background.SetActive(false);
            }
           // obj.name=data.dwGoodsID+"w";
        }
        else
        {
           if(Background)
            {
                Background.SetActive(true);
            }
            EqLevel.SetActive(false);
            StarIcon.gameObject.SetActive(false);
            NormalBg.ChangeSprite(5);
        }
        
        
    }
}
