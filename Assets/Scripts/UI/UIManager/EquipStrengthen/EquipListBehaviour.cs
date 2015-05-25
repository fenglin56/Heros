using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// 装备列表控制脚本，挂在Prefab_EquipList预设件上
/// </summary>
using System.Collections.Generic;
using UI.MainUI;

public class EquipListBehaviour : MonoBehaviour {
	public GameObject Prefab_EquipItem;
	public UITable ItemTable;
	public SingleButtonCallBack NormalStren;
	public SingleButtonCallBack StarStren;
	//当前装备详细信息控制脚本
	//引用装备明细，方便处理选择装备后调用。
	[HideInInspector]
	public EquipDetails CurrrEquipDetails;  
	[HideInInspector]
	public PackRightBtnManager PackRightBtnManager; 
	[HideInInspector]
	public bool IsNormalStren=true;

	private List<EquipItem> m_equipItems=new List<EquipItem>();
	private List<ItemFielInfo> m_equipDatainfoList;
	private TweenPosition m_tweenPosComponent;
	private ItemFielInfo m_selectedItemInfo;

	void Awake()
	{
		NormalStren.SetCallBackFuntion((obj)=>
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Switch");
			ChangeStrenType(true);
		});
		StarStren.SetCallBackFuntion((obj)=>
		                               {
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Switch");
			ChangeStrenType(false);
		});
		m_tweenPosComponent=GetComponent<TweenPosition>();

        TaskGuideBtnRegister();
    }
    /// <summary>
    /// 引导按钮注入代码
    /// </summary>
    private void TaskGuideBtnRegister()
    {
//        NormalStren.gameObject.RegisterBtnMappingId(UIType.EquipStrengthen, BtnMapId_Sub.EquipStrengthen_TabStren);
//        StarStren.gameObject.RegisterBtnMappingId(UIType.EquipStrengthen, BtnMapId_Sub.EquipStrengthen_TabStarUpgrade);
    }

    public void Init(ItemFielInfo selectedItem)
	{
        StartCoroutine(RefreshItem(selectedItem));
	}
	/// <summary>
	///  打开界面，飞入动画
	/// </summary>
    /// 
    public void ShowAnim()
    {
        ShowAnim(true);
    }
    public void ShowAnim(bool isStrength)
	{
        ChangeStrenType(isStrength);
		m_tweenPosComponent.Play(true);
		this.CurrrEquipDetails.ShowAnim();
		this.PackRightBtnManager.ShowAnim();
	}
	/// <summary>
	///  打开界面，飞入动画
	/// </summary>
	public void CloseAnim()
	{
		m_tweenPosComponent.Play(false);
		this.CurrrEquipDetails.CloseAnim();
		this.PackRightBtnManager.CloseAnim();
	}
	/// <summary>
	/// 强化或升星成功，刷新界面
	/// </summary>
	public void StrenAndStarUpgradeSuccess()
	{
		StartCoroutine(RefreshItem(m_selectedItemInfo));
	}
	/// <summary>
	/// 读取装备表，重置界面
	/// </summary>
	private IEnumerator RefreshItem(ItemFielInfo selectedItem)
	{
        m_selectedItemInfo = selectedItem;

		ItemTable.transform.ClearChild();
		m_equipItems.Clear();
		//获得装备列表，
        m_equipDatainfoList =new List<ItemFielInfo>();
        foreach(var item in ContainerInfomanager.Instance.itemFielArrayInfo)
        {
            if(item.severItemFielType== SeverItemFielInfoType.Equid && ((EquipmentData)item.LocalItemData)._StrengFlag==1)
            {
                m_equipDatainfoList.Add(item);
            }
        }
     
		//排序(TODO)
		m_equipDatainfoList.Sort(new EquipComparer());
		//实例化，添加到列表中
		//for(int i=m_equipDatainfoList.Count-1;i>=0 ;i--)
        for (int i = 0; i < m_equipDatainfoList.Count; i++)
		{
			var itemGO=NGUITools.AddChild(ItemTable.gameObject,Prefab_EquipItem);
			itemGO.name=Prefab_EquipItem.name+i.ToString();
			var equipItem=itemGO.GetComponent<EquipItem>();
			equipItem.InitItemData(m_equipDatainfoList[i]);
			//监听Item事件
			equipItem.OnItemClick=ItemSelectedEventHandle;
			m_equipItems.Add(equipItem);
            //引导放在EquipItem实现
			//选择默认项
			if(m_selectedItemInfo==null)
			{
				if(i==0)
				{
					equipItem.OnBeSelected();
				}
			}
			else if(m_selectedItemInfo==m_equipDatainfoList[i])
			{
				equipItem.OnBeSelected();
			}
		}
		yield return null;
		ItemTable.Reposition();
	}
	/// <summary>
	/// 改变强化方式（普通或星阶）
	/// </summary>
	public void ChangeStrenType(bool isNormalStren)
	{
		IsNormalStren=isNormalStren;
		//SpriteId 对应： 1  表示未选中，   2   表示选中状态
		NormalStren.spriteSwithList.ApplyAllItem(P=>P.ChangeSprite(isNormalStren?2:1));
		StarStren.spriteSwithList.ApplyAllItem(P=>P.ChangeSprite(isNormalStren?1:2));

		CurrrEquipDetails.IsNormalStren=IsNormalStren;
         if(isNormalStren)
        {
            if(PlayerDataManager.Instance.CanEquipmentQuickStrengthen())
            {
                StartCoroutine( PackRightBtnManager.AddBtn(PackBtnType.Package,PackBtnType.Strength,PackBtnType.QuickStrengthen));
            }
            else
            {
                StartCoroutine( PackRightBtnManager.AddBtn(PackBtnType.Package,PackBtnType.Strength));
            }
        }
        else
        {
            if(PlayerDataManager.Instance.CanEquipmentQuickUpgradeStar())
            {
                StartCoroutine (PackRightBtnManager.AddBtn(PackBtnType.Package,PackBtnType.StarUpgrade,PackBtnType.QuickUpgradeStar));
            }
            else
            {
                StartCoroutine(PackRightBtnManager.AddBtn(PackBtnType.Package,PackBtnType.StarUpgrade));
            }
        }
		//StartCoroutine(PackRightBtnManager.AddBtn(PackBtnType.Package,isNormalStren?PackBtnType.Strength:PackBtnType.StarUpgrade));
	}
	/// <summary>
	/// 装备项选择处理
	/// </summary>
	/// <param name="selectedEquipItem">Selected equip item.</param>
	private void ItemSelectedEventHandle(EquipItem selectedEquipItem)
	{
		//所有项LoseFocus
		m_equipItems.ApplyAllItem(P=>P.OnLoseFocus());
		//当前项GetFocus
		selectedEquipItem.OnGetFocus();
		//调用EquipDetails的方法刷新当前选择装备的详细信息
		CurrrEquipDetails.Init(selectedEquipItem.ItemFielInfo,IsNormalStren);
		m_selectedItemInfo=selectedEquipItem.ItemFielInfo;
	}
	/// <summary>
	/// 当前选择的装备数据实体
	/// </summary>
	/// <value>The curr item fiel info.</value>
	public ItemFielInfo CurrItemFielInfo
	{
		get{
			return  CurrrEquipDetails==null?null:CurrrEquipDetails.CurrItemFielInfo;
		}
	}
}
