using UnityEngine;
using System.Collections;
using UI.MainUI;
using System;


public class AttPanel : MonoBehaviour {
    public UISprite SP_EffIcon1;
    public UILabel Lable_EffName1;
    public UILabel Lable_CurrentValue1;
    public UILabel Lable_nxtValue1;
    public UISprite SP_EffIcon2;
    public UILabel Lable_EffName2;
    public UILabel Lable_CurrentValue2;
    public UILabel Lable_nxtValue2;
	public void Init(ItemFielInfo itemfileInfo,UpgradeType type)
    {
        SetBaseValue(itemfileInfo);

        switch(type)
        {
            case UpgradeType.Strength:
                SetStrengthAddValue(itemfileInfo);
                break;
            case UpgradeType.StarUp:
                SetStarUpAddValue(itemfileInfo);

                break;
            case UpgradeType.Upgrade:
                SetUpgradeAddValue(itemfileInfo);
                break;
               
        }
    }

    public void SetBaseValue(ItemFielInfo itemfileInfo)
    {
        SP_EffIcon1.spriteName=EquipItem.GetItemInfoDetail(itemfileInfo,EquipInfoType.Prop1IconName);
        Lable_EffName1.SetText(EquipItem.GetItemInfoDetail(itemfileInfo,EquipInfoType.Prop1Name));
        Lable_CurrentValue1.SetText(EquipItem.GetItemInfoDetail(itemfileInfo,EquipInfoType.Prop1Value));
        SP_EffIcon2.spriteName=EquipItem.GetItemInfoDetail(itemfileInfo,EquipInfoType.Prop2IconName);
        Lable_EffName2.SetText(EquipItem.GetItemInfoDetail(itemfileInfo,EquipInfoType.Prop2Name));
        Lable_CurrentValue2.SetText(EquipItem.GetItemInfoDetail(itemfileInfo,EquipInfoType.Prop2Value));
    }

	void SetAddValue (ItemFielInfo itemfileInfo,bool isNormal)
	{
		string value1 = EquipItem.GetItemInfoDetail (itemfileInfo, EquipInfoType.Prop1Add, isNormal);
		if (value1 == "+0") {
			Lable_nxtValue1.gameObject.SetActive (false);
		}
		else {
			Lable_nxtValue1.gameObject.SetActive (true);
			Lable_nxtValue1.SetText (value1);
		}
		string value2 = EquipItem.GetItemInfoDetail (itemfileInfo, EquipInfoType.Prop2Add, isNormal);
		if (value2 == "+0") {
			Lable_nxtValue2.gameObject.SetActive (false);
		}
		else {
			Lable_nxtValue2.gameObject.SetActive (true);
			Lable_nxtValue2.SetText (value2);
		}
	}

    public void SetStrengthAddValue(ItemFielInfo itemfileInfo)
    {
		if(PlayerDataManager.Instance.GetEquipmentStrengthLevel((EquiptSlotType)itemfileInfo.sSyncContainerGoods_SC.nPlace)<CommonDefineManager.Instance.CommonDefine.StrengthLimit)
		{
			SetAddValue (itemfileInfo,true);
		}
		else
		{
			Lable_nxtValue1.gameObject.SetActive (false);
			Lable_nxtValue2.gameObject.SetActive (false);
		}
    }


    public void SetStarUpAddValue(ItemFielInfo itemfileInfo)
    {
		if(PlayerDataManager.Instance.GetEquipmentStarLevel((EquiptSlotType)itemfileInfo.sSyncContainerGoods_SC.nPlace)<CommonDefineManager.Instance.CommonDefine.StartStrengthLimit)
		{
		  SetAddValue (itemfileInfo,false);
		}
		else
		{
			Lable_nxtValue1.gameObject.SetActive (false);
			Lable_nxtValue2.gameObject.SetActive (false);
		}
    }

    public void SetUpgradeAddValue(ItemFielInfo itemfileInfo)
    {
        ItemFielInfo NextLevelItem=new ItemFielInfo((itemfileInfo.LocalItemData as EquipmentData).UpgradeID);
		if(NextLevelItem!=null)
		{
	        int nextValue1=Convert.ToInt32( EquipItem.GetItemInfoDetail(NextLevelItem,EquipInfoType.Prop1Value));
	        int CurrentValue1=Convert.ToInt32( EquipItem.GetItemInfoDetail(itemfileInfo,EquipInfoType.Prop1Value));
			if(nextValue1>CurrentValue1)
			{
				Lable_nxtValue1.gameObject.SetActive (true);
				Lable_nxtValue1.SetText("+"+(nextValue1-CurrentValue1));
			}
			else
			{
				Lable_nxtValue1.gameObject.SetActive (false);
			}

	        int nextValue2=Convert.ToInt32( EquipItem.GetItemInfoDetail(NextLevelItem,EquipInfoType.Prop2Value));
	        int CurrentValue2=Convert.ToInt32( EquipItem.GetItemInfoDetail(itemfileInfo,EquipInfoType.Prop2Value));
			if(nextValue2>CurrentValue2)
			{
				Lable_nxtValue2.gameObject.SetActive (true);
				Lable_nxtValue2.SetText("+"+(nextValue2-CurrentValue2));
			}
			else
			{
				Lable_nxtValue2.gameObject.SetActive (false);
			}
		}
		else
		{
			Lable_nxtValue2.gameObject.SetActive (false);
		}
    }
}
