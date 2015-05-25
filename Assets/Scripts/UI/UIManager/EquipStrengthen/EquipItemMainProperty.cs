using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// 装备信息，挂在预设件Prefab_EquipMainProperty  (0,112.5,0)
/// </summary>
using UI.MainUI;
using System;


public class EquipItemMainProperty : MonoBehaviour {

	[HideInInspector]
	/// <summary>
	/// 当前装备数据实体
	/// </summary>
	public ItemFielInfo CurrItemFielInfo;
	/// <summary>
	/// 装备预设件
	/// </summary>
	public GameObject EquipItemPrefab;

	public UILabel EquipName;      //装备名称，装备名+强化等级，字体颜色要与装备品质保持一致
	public UILabel EquipAllowLev;   //装备使用等级 玩家等级不足时，等级数字显示为红色
	public SpriteSwith[] Occupations;  //职业限制，多个显示多个。玩家职业不符时，职业文字显示为红色
	public GameObject VactionBg;      //职业不符背景
	public UILabel Part;  //装备类型
	//星阶属性 星阶等级，星星颜色、进度
	//int（星阶等级/10）  星阶名见上文对应表格，分母固定为10，分子为：求余数（星阶等级/10）。进度条进度长度显示星阶(等级/10)百分比长度
	public UILabel StarLev;   //星阶模运算
	public UISlider StarProcess;
	public UILabel StarName;  //    明珠
	public UILabel StarDesc;  //    5/10
	public SpriteSwith Star;  

	private EquipItem m_equipItem;
	private SpriteSwith m_starProcessFore;
	void Awake()
	{
		m_starProcessFore=StarProcess.gameObject.GetComponentInChildren<SpriteSwith>();
		Init(null);
	}

	public void Init(ItemFielInfo itemFielInfo)
	{
		CurrItemFielInfo=itemFielInfo;
		if(CurrItemFielInfo==null)
		{
			EquipName.text=string.Empty;
			EquipAllowLev.text=string.Empty;
			VactionBg.SetActive(false);
			Occupations.ApplyAllItem(P=>P.ChangeSprite(0));

			Part.text=string.Empty;
			StarLev.text=string.Empty;
			StarName.text=string.Empty;

			StarDesc.text=string.Empty;
			StarProcess.sliderValue=0;
			return ;
		}
		//初始化图标
		if(m_equipItem==null)
		{
			Transform attachPoint;
			transform.RecursiveFindObject("ItemAttachPoint",out attachPoint);
			m_equipItem=NGUITools.AddChild(attachPoint.gameObject,EquipItemPrefab).GetComponent<EquipItem>();
		}
		if(CurrItemFielInfo==null)
		{
			return;
		}
		m_equipItem.InitItemData(itemFielInfo);
		//初始化其他属性（名称，其他属性.....）
		EquipName.text=EquipItem.GetItemInfoDetail(itemFielInfo,EquipInfoType.EquipName);
		var playerLev=PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
		var allowLevel=itemFielInfo.LocalItemData._AllowLevel;
		//玩家等级不足时，等级数字显示为红色
		EquipAllowLev.text=allowLevel.ToString().SetColor(allowLevel>playerLev?TextColorType.Red:TextColorType.EquipProperty);
		//玩家职业处理
		string[] ItemVocation = itemFielInfo.LocalItemData._AllowProfession.Split('+');
		VactionBg.SetActive(false);
		Occupations.ApplyAllItem(P=>P.ChangeSprite(0));
		if(ItemVocation.Length>0)
		{
			var vos=ItemVocation.ToList();
			vos.Sort((a,b)=>a.CompareTo(b));
			for(int i=0;i<vos.Count;i++)
			{
				Occupations[i].ChangeSprite(int.Parse(vos[i]));
			}
			if(ItemVocation.Length==1)
			{
				var vo=int.Parse(ItemVocation[0]);
				if(vo!=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION)
				{
					VactionBg.SetActive(true);
				}
			}
		}

		Part.text=((GoodsSubClass)itemFielInfo.LocalItemData._GoodsSubClass).GetGoodsSubClassName();  //扩展方法计算物品类型的IDS
        string s=EquipItem.GetItemInfoDetail(itemFielInfo,EquipInfoType.EquipStarLevel);
        int  starLev=0;
        if(!string.IsNullOrEmpty(s))
        {
            starLev = Convert.ToInt32(s);
        }
	
        StarLev.SetText(starLev);
        int Starclass=(starLev-1)/10;
        int spriteId=starLev==0?1:Starclass+2;

        Star.ChangeSprite(spriteId);

        StarName.text=LanguageTextManager.GetString(string.Format("{0}{1}","IDS_I36_",Starclass+1));//1=明珠 2=夏炉 桐琴 御盾 玉麟 周鼎

		//分母固定为10，分子为：求余数（星阶等级/10）。进度条进度长度显示星阶(等级/10)百分比长度
		//int starLevel=itemFielInfo.equipmentEntity.EQUIP_FIELD_START_LEVEL;
        int currProcess=((starLev-1)%10)+1;
		//currProcess=(currProcess==0&&starLevel!=0)?10:currProcess;
		StarDesc.text=string.Format("{0}/{1}",currProcess,10);

		m_starProcessFore.ChangeSprite(spriteId-1);
		StarProcess.sliderValue=currProcess/10f;
	}

}
