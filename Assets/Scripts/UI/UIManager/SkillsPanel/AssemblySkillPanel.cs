using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class AssemblySkillPanel : MonoBehaviour {

	public GameObject Prefab_SkillItem;
    public UIDraggablePanel DragPanel;
	public UIGrid Layout;
	[HideInInspector]
	public SkillsItem[] AssemblySkillItems;
	public GameObject SelectedEffPrefab;  //技能栏待装备循环特效
	public Action<SkillsItem,int> EquipItemClickAct;  //点击处理，int为装备位置
	private SingleSkillInfo[] m_equipSkillList;
	private TweenPosition m_tweenPosComponent;
	//初始化
	//待装备
	//装备完成
	//选择Item事件

	void Awake()
	{
        DragPanel.LockDraggable = true;
		AssemblySkillItems=new SkillsItem[4];
		m_tweenPosComponent=GetComponent<TweenPosition>();
		StartCoroutine(InitSkillItem());
	}
	/// <summary>
	/// 启动初始化装备栏Prefab
	/// </summary>
	/// <returns>The skill item.</returns>
	IEnumerator InitSkillItem()
	{
		for (int i = 0; i < 4; i++ )
		{
			var SkillsItem=NGUITools.AddChild(Layout.gameObject,Prefab_SkillItem).GetComponent<SkillsItem>();
			SkillsItem.SelectedEff=NGUITools.AddChild(SkillsItem.gameObject,SelectedEffPrefab);
            SkillsItem.SelectedEff.transform.localPosition = new Vector3(0, 7, -1);
			AssemblySkillItems[i]=SkillsItem;

            #region`    引导注入代码
            //SkillsItem.gameObject.RegisterBtnMappingId(UI.MainUI.UIType.Skill, BtnMapId_Sub.Skill_AssemblyItem);
            #endregion

			SkillsItem.Empty.GetComponent<SpriteSwith>().ChangeSprite(i+1);
			SkillsItem.OnItemClick=(P)=>
			{ 
				for(int j=0;j<4;j++)
				{
					if(AssemblySkillItems[j]==P)
					{
						EquipItemClickAct(P,j);
						break;
					}
				}
			};
		}	
		yield return null;
		Layout.Reposition();
	}
	public void Init(SingleSkillInfo[] singleSkillInfo)
	{
		m_equipSkillList = singleSkillInfo;
		AssemblySkillItems.ApplyAllItem(P=>P.InitItemData(null));
		for (int i = 0; i < m_equipSkillList.Length; i++ )
        {
			var item=m_equipSkillList[i];
			if (item != null)
            {
				AssemblySkillItems[item.BattleIconPosition-1].InitItemData(item);
            }
		}
	}
    public void UpgradeSkillItem(SingleSkillInfo upgradeSkillInfo)
    {
        foreach (var item in AssemblySkillItems)
        {
            if (item != null && item.ItemFielInfo!=null)
            {
                if (item.ItemFielInfo.localSkillData.m_skillId == upgradeSkillInfo.localSkillData.m_skillId)
                {
                    item.InitItemData(upgradeSkillInfo);
                    break;
                }
            }
        }
    }
	public void SkillEquipSuucess(SingleSkillInfo[] singleSkillInfo,SkillsItem currentSelectedItem)
	{
		Init(singleSkillInfo);
		SkillInListBeSelected(currentSelectedItem);
	}
	/// <summary>
	///  打开界面，飞入动画
	/// </summary>
	public void ShowAnim()
	{
		m_tweenPosComponent.Play(true);
	}
	/// <summary>
	///  打开界面，飞入动画
	/// </summary>
	public void CloseAnim()
	{
		m_tweenPosComponent.Play(false);
	}
	/// <summary>
	/// 当技能列表中技能被先中时，装备栏的处理。
	/// 如该技能已装备，该栏闪烁
	/// 如未装备，四个装备栏同时闪烁  JH_Eff_UI_Skill_Select
	/// </summary>
	/// <param name="skillsItem">Skills item.</param>
	public void SkillInListBeSelected(SkillsItem skillsItem)
	{
		var selectedItem=AssemblySkillItems.SingleOrDefault(P=>P.ItemFielInfo==skillsItem.ItemFielInfo);

		if(selectedItem==null)
		{
			AssemblySkillItems.ApplyAllItem(P=>{P.OnLoseFocus();P.WaitToAssembly(true);});
		}
		else
		{
			AssemblySkillItems.ApplyAllItem(P=>{P.OnLoseFocus();P.WaitToAssembly(false);});
			selectedItem.OnGetFocus();
		}
	}
}
