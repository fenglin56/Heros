using UnityEngine;
using System.Collections;
using UI.MainUI;
using System.Collections.Generic;
using UI;


public class StarUppanel : MonoBehaviour {

    public EquipIconItem IconItem;
    public UILabel Lable_position;
    public UILabel Lable_Currentforce;
    public UILabel Lable_addForce;
    public UILabel Lable_name;
    public Transform Eff_point;
    public GameObject Eff_Prefab;
    private GameObject Eff_go; 
    public List<SpriteSwith> Sps_stars;
    private Color32[] Colors=new Color32[]
    {
        new Color32{r=78,g=78,b=78,a=128},//银
        new Color32{r=41,g=255,b=41,a=128},//绿
        new Color32{r=41,g=41,b=255,a=128},//蓝
        new Color32{r=255,g=45,b=241,a=128},//紫
        new Color32{r=174,g=60,b=77,a=128},//粉
        new Color32{r=255,g=241,b=45,a=128},//金
        new Color{r=255,g=41,b=41,a=128},//红

    };

    void Awake()
    {
        Eff_go=UI.CreatObjectToNGUI.InstantiateObj(Eff_Prefab,Eff_point);
        Debug.Log("");
		TaskGuideBtnRegister ();
    }
	private void TaskGuideBtnRegister()
	{
		IconItem.gameObject.RegisterBtnMappingId(UIType.EquipmentUpgrade, BtnMapId_Sub.EquipmentUpgrade_Star_RightIconBtn);
	}
    public void Init(ItemFielInfo itemfileInfo)
    {
        IconItem.Init(itemfileInfo);
        Lable_name.SetText(NGUIColor.SetTxtColor(LanguageTextManager.GetString( itemfileInfo.LocalItemData._szGoodsName),(TextColor)itemfileInfo.LocalItemData._ColorLevel));
        Lable_position.SetText(EquipmentUpgradeDataManger.Instance.PositionDic[itemfileInfo.LocalItemData._GoodsSubClass]);
        Lable_Currentforce.SetText((int)EquipItem.GetEquipForce(itemfileInfo));
		int addforce=((int)EquipItem. GetNextLevelEquipForce(itemfileInfo,UpgradeType.StarUp)-(int)EquipItem.GetEquipForce(itemfileInfo));
		if(addforce>0)
		{
			Lable_addForce.gameObject.SetActive(true);
			Lable_addForce.SetText("+"+addforce);
		}
		else
		{
			Lable_addForce.gameObject.SetActive(false);
		}
        //Lable_strengthLevle.SetText(EquipmentUpgradeDataManger.Instance.GetStrengthLevel(itemfileInfo));
		int starLevel=PlayerDataManager.Instance.GetEquipmentStarLevel((EquiptSlotType)itemfileInfo.sSyncContainerGoods_SC.nPlace);
		if(starLevel<CommonDefineManager.Instance.CommonDefine.StartStrengthLimit)
		{
			Eff_go.SetActive(true);
		
		}
		else
		{
			Eff_go.SetActive(false);
		}
		SetStarColor(starLevel);
    }
    public void SetStarColor(int level)
    {
        int temp=level-1;
        int par=temp/10;
        int star=(temp%10)+1;
        if(level>0)
        {
            Sps_stars.ApplyAllItem(c=>c.ChangeSprite(par+1));

            for(int i=0;i<star;i++)
            {
                Sps_stars[i].ChangeSprite(par+2);
            }
            if(star<10)
            {
                Eff_point.transform.localPosition= Sps_stars[star].transform.localPosition;
                Eff_go.transform.GetChild(0).renderer.material.SetColor("_TintColor",Colors[par]);

            }
             else
            {
                Eff_point.transform.localPosition= Sps_stars[0].transform.localPosition;
                Eff_go.transform.GetChild(0).renderer.material.SetColor("_TintColor",Colors[par+1]);
            }
          
        }
        else
        {
            Eff_point.transform.localPosition= Sps_stars[0].transform.localPosition;
            Eff_go.transform.GetChild(0).renderer.material.SetColor("_TintColor",Colors[0]);
            Sps_stars.ApplyAllItem(c=>c.ChangeSprite(1));
        }
    }
}
