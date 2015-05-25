using UnityEngine;
using System.Collections;
using System.Linq;

namespace UI.MainUI{

	public class SingleItemTipsEffect : MonoBehaviour {
		public enum EffectType{MainAttribute,MainAttributeCompair,MainProAdd,MainProAddForStar}

		public UILabel TitleLabel;
		public UISprite Effect1Icon;
		public UISprite Effect2Icon;
		public UILabel Effect1NameLabel;
		public UILabel Effect1NumLabel;
		public UILabel Effect2NameLabel;
		public UILabel Effect2NumLabel;

		#region 装备对比图标及数字，仅Effecttype为MainAttributeCompair时使用
		public GameObject CompairIconPrefab_UP;
		public GameObject CompairIconPrefab_Down;
		public Transform Effet1CompairIconPos;
		public Transform Effet2CompairIconPos;
		public UILabel Effect1CompairNumLabel;
		public UILabel Effect2CompairNumLabel;
		#endregion

		/// <summary>
		/// 普通装备属性显示
		/// </summary>
		/// <param name="itemFielInfo">Item fiel info.</param>
		/// <param name="effectType">Effect type.</param>
		public void Init(ItemFielInfo itemFielInfo, EffectType effectType)
		{
			Effect1Icon.spriteName = EquipItem.GetItemInfoDetail(itemFielInfo,EquipInfoType.Prop1IconName);
			Effect2Icon.spriteName = EquipItem.GetItemInfoDetail(itemFielInfo,EquipInfoType.Prop2IconName);
			string effect1Name = EquipItem.GetItemInfoDetail(itemFielInfo,EquipInfoType.Prop1Name);
			string effect2Name =  EquipItem.GetItemInfoDetail(itemFielInfo,EquipInfoType.Prop2Name);
			string effect1AddNum = string.Empty;
			string effect2AddNum = string.Empty;
			switch (effectType)
			{
			case EffectType.MainAttribute:
                {
				TitleLabel.SetText(LanguageTextManager.GetString("IDS_I3_25"));
				Effect1NameLabel.SetText(effect1Name);
				Effect2NameLabel.SetText(effect2Name);
                    string TatalValue=EquipItem.GetItemInfoDetail(itemFielInfo,EquipInfoType.Prop1Value);
                    if(string.IsNullOrEmpty(TatalValue))
                    {
                        TatalValue="0";
                    }
                    string AddValue=(int.Parse(TatalValue)-itemFielInfo.equipmentEntity.EQUIP_FIELD_EFFECTBASE0_VALUE).ToString();
                    if(AddValue=="0")
                    {
                        AddValue="";
                    }
                    else
                    {
                        AddValue="(+"+AddValue+")";
                    }
                    effect1AddNum = string.Format("{0} {1}",TatalValue,GetEffectYellowText(AddValue));
                    
                    string TatalValue1=EquipItem.GetItemInfoDetail(itemFielInfo,EquipInfoType.Prop2Value);
                    if(string.IsNullOrEmpty(TatalValue1))
                    {
                        TatalValue1="0";
                    }
                    string AddValue1=(int.Parse(TatalValue1)-itemFielInfo.equipmentEntity.EQUIP_FIELD_EFFECTBASE1_VALUE).ToString();
                    if(AddValue1=="0")
                    {
                        AddValue1="";
                    }
                    else
                    {
                        AddValue1="(+"+AddValue1+")";
                    }
                    effect2AddNum =string.Format("{0} {1}",TatalValue1,GetEffectYellowText(AddValue1));
                }
				break;
			case EffectType.MainAttributeCompair:
                {
				TitleLabel.SetText(LanguageTextManager.GetString("IDS_I3_25"));
				Effect1NameLabel.SetText(effect1Name);
				Effect2NameLabel.SetText(effect2Name);

                    string TatalValue=EquipItem.GetItemInfoDetail(itemFielInfo,EquipInfoType.Prop1Value);
                    if(string.IsNullOrEmpty(TatalValue))
                    {
                        TatalValue="0";
                    }
                    string AddValue=(int.Parse(TatalValue)-itemFielInfo.equipmentEntity.EQUIP_FIELD_EFFECTBASE0_VALUE).ToString();
                    if(AddValue=="0")
                    {
                        AddValue="";
                    }
                    else
                    {
                        AddValue="(+"+AddValue+")";
                    }
                    effect1AddNum = string.Format("{0} {1}",TatalValue,GetEffectYellowText(AddValue));

                    string TatalValue1=EquipItem.GetItemInfoDetail(itemFielInfo,EquipInfoType.Prop2Value);
                    if(string.IsNullOrEmpty(TatalValue1))
                    {
                        TatalValue1="0";
                    }
                    string AddValue1=(int.Parse(TatalValue1)-itemFielInfo.equipmentEntity.EQUIP_FIELD_EFFECTBASE1_VALUE).ToString();
                    if(AddValue1=="0")
                    {
                        AddValue1="";
                    }
                    else
                    {
                        AddValue1="(+"+AddValue1+")";
                    }
                    effect2AddNum =string.Format("{0} {1}",TatalValue1,GetEffectYellowText(AddValue1));

				int effect1CompairNum = GetCompairNum(itemFielInfo,EquipInfoType.Prop1Value);
				int effect2CompairNum = GetCompairNum(itemFielInfo,EquipInfoType.Prop2Value);
				Effect1CompairNumLabel.SetText(effect1CompairNum>0?GetEffectYellowText(effect1CompairNum.ToString()):GetEffectRedText(effect1CompairNum.ToString()));
				Effect2CompairNumLabel.SetText(effect2CompairNum>0?GetEffectYellowText(effect2CompairNum.ToString()):GetEffectRedText(effect2CompairNum.ToString()));
                    if(effect1CompairNum==0)
                    {
                        Effect1CompairNumLabel.gameObject.SetActive(false);
                    }
                    else
                    {
                        Effect1CompairNumLabel.gameObject.SetActive(true);
                    }
                    if(effect2CompairNum==0)
                    {
                        Effect2CompairNumLabel.gameObject.SetActive(false);
                    }
                    else
                    {
                        Effect2CompairNumLabel.gameObject.SetActive(true);
                    }
				GenerateCompairIcon(effect1CompairNum,Effet1CompairIconPos);
				GenerateCompairIcon(effect2CompairNum,Effet2CompairIconPos);
                }
				break;
			case EffectType.MainProAdd:
				TitleLabel.SetText(LanguageTextManager.GetString("IDS_I3_59"));
				Effect1NameLabel.SetText(GetEffectBlueText(string.Format("{0} {1}",LanguageTextManager.GetString("IDS_I3_27"),effect1Name)));
				Effect2NameLabel.SetText(GetEffectBlueText(string.Format("{0} {1}",LanguageTextManager.GetString("IDS_I3_27"),effect2Name)));
				effect1AddNum = GetEffectYellowText(EquipItem.GetItemInfoDetail(itemFielInfo,EquipInfoType.Prop1MainAdd));
				effect2AddNum = GetEffectYellowText(EquipItem.GetItemInfoDetail(itemFielInfo,EquipInfoType.Prop2MainAdd));
				break;
			case EffectType.MainProAddForStar:
				TitleLabel.SetText(LanguageTextManager.GetString("IDS_I3_60"));
				Effect1NameLabel.SetText(GetEffectGreenText(string.Format("{0} {1}",LanguageTextManager.GetString("IDS_I3_28"),effect1Name)));
				Effect2NameLabel.SetText(GetEffectGreenText(string.Format("{0} {1}",LanguageTextManager.GetString("IDS_I3_28"),effect2Name)));
				effect1AddNum = GetEffectYellowText(EquipItem.GetItemInfoDetail(itemFielInfo,EquipInfoType.Prop1StarAdd));
				effect2AddNum = GetEffectYellowText(EquipItem.GetItemInfoDetail(itemFielInfo,EquipInfoType.Prop2StarAdd));
				break;
			}
			Effect1NumLabel.SetText(effect1AddNum);
			Effect2NumLabel.SetText(effect2AddNum);	
		}
		
		/// <summary>
		/// 获取与装备上的装备对比属性值
		/// </summary>
		/// <param name="itemFielInfo">Item fiel info.</param>
		/// <param name="effectType">Effect type.</param>
		int GetCompairNum(ItemFielInfo itemfileInfo,EquipInfoType equipInfoType)
		{
            string numStr=EquipItem.GetItemInfoDetail(itemfileInfo,equipInfoType).Replace("+","");
            if(string.IsNullOrEmpty(numStr))
            {
                numStr="0";
            }
			int getNum = int.Parse(numStr);
			var equipItemList = ContainerInfomanager.Instance.GetEquiptItemList();
			ItemFielInfo equipItem = equipItemList.FirstOrDefault(P=>(P.LocalItemData as EquipmentData)._vectEquipLoc == (itemfileInfo.LocalItemData as EquipmentData)._vectEquipLoc);
			if(equipItem!=null)
			{
                string numStr1=EquipItem.GetItemInfoDetail(equipItem,equipInfoType).Replace("+","");
                if(string.IsNullOrEmpty(numStr1))
                {
                    numStr1="0";
                }
				getNum = getNum - int.Parse(numStr1);
			}
			return getNum;
		}
		
		void GenerateCompairIcon(int compairNum,Transform iconParent)
		{
			iconParent.ClearChild();
			if(compairNum>0)
			{
				CreatObjectToNGUI.InstantiateObj(CompairIconPrefab_UP,iconParent);
			}else if(compairNum<0)
			{
				CreatObjectToNGUI.InstantiateObj(CompairIconPrefab_Down,iconParent);
			}
		}
		/// <summary>
		/// 字体转为淡黄色
		/// </summary>
		string GetEffectYellowText(string text)
		{
			return string.Format("[ece09e]{0}[-]",text);
		}
		/// <summary>
		/// 字体转为淡红色
		/// </summary>
		string GetEffectRedText(string text)
		{
			return string.Format("[fe768b]{0}[-]",text);
		}
		
		/// <summary>
		/// 字体转为淡绿色
		/// </summary>
		string GetEffectGreenText(string text)
		{
			return string.Format("[9bfd98]{0}[-]",text);
		}
		/// <summary>
		/// 字体转为淡蓝色
		/// </summary>
		string GetEffectBlueText(string text)
		{
			return string.Format("[bedbff]{0}[-]",text);
		}

	}
}