using UnityEngine;
using System.Collections;
using System;
using System.Linq;

/// <summary>
/// 技能列表，挂在Prefab_SkillList预设件上
/// </summary>
using System.Collections.Generic;
using UI.MainUI;


public class SkillListBehaviour : MonoBehaviour {

	public GameObject Prefab_SkillDragUnit;
	public GameObject Prefab_SkillItem;
	public UIGrid Layout;
	public SkillsItem CurrentSelectedItem;
	public Action<SkillsItem> OnItemClick;//点击处理委托
	private TweenPosition m_tweenPosComponent;
	private SingleSkillInfoList m_singleSkillInfoList;
    private bool m_shouldMove;
    //引导列表滚动到指定项
    private float m_noticeToDragAmount;
    private UIDraggablePanel m_dragPanelComp;
	//初始化
	//出现动画
	//消失动画
	//选中默认项
	private List<SkillsItem> m_skillsItems=new List<SkillsItem>();

	void Awake()
	{
		m_tweenPosComponent=GetComponent<TweenPosition>();
        m_dragPanelComp = transform.GetComponentInChildren<UIDraggablePanel>();
	}
	public void Init(SingleSkillInfoList singleSkillInfoList)
	{
		StartCoroutine(RefreshItem(singleSkillInfoList));
	}
    public void UpgradeSkillItem(SingleSkillInfo upgradedItem)
    {
        //StartCoroutine(RefreshItem(singleSkillInfoList, upgradedItem));
        var targetSkillItem=m_skillsItems.SingleOrDefault(P => P.ItemFielInfo.localSkillData.m_skillId == upgradedItem.localSkillData.m_skillId);

        if (targetSkillItem != null)
        {
            targetSkillItem.InitItemData(upgradedItem);
            targetSkillItem.SkillUpgradeSuccess(true);
        }
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
	/// 
	/// </summary>
	/// <param name="selectedSkill">Selected skill.</param>
	public void FocusSkillItem(SingleSkillInfo selectedSkill)
	{
		//所有项LoseFocus
		m_skillsItems.ApplyAllItem(P=>{P.OnLoseFocus();P.EquipChange();});
		var selectItem=m_skillsItems.SingleOrDefault(P=>P.ItemFielInfo==selectedSkill);
		if(selectItem!=null)
		{
			//当前项GetFocus
			selectItem.OnGetFocus();
			StartCoroutine(DragAmountSlerp(selectItem.DragAmount));
		}
	}
    /// <summary>
    /// 记下要自动滚动到的位置
    /// </summary>
    /// <param name="targetAmount"></param>
    private void NoticeToDragSlerp(float targetAmount)
    {
        m_noticeToDragAmount = targetAmount;
    }
	private IEnumerator DragAmountSlerp(float targeAmount)
	{
        yield return null;
       
        if (m_shouldMove)
        {
            float smoothTime = 0.3f, currentSmoothTime = 0; ;
            float currentAmount = 0;
            while (true)
            {
                currentSmoothTime += Time.deltaTime;
                currentAmount = Mathf.Lerp(currentAmount, targeAmount, Time.deltaTime * 20);
                m_dragPanelComp.SetDragAmount(0, currentAmount, false);
                yield return null;
                if ((targeAmount - currentAmount) <= float.Epsilon || currentSmoothTime >= smoothTime)
                {
                    if (HasGuideArrow)
                    {
                        m_dragPanelComp.LockDraggable = true;
                    }
                    else
                    {
                        m_dragPanelComp.LockDraggable = false;
                    }
                    break;
                }
            }
        }
	}
    void Update()
    {
        if (m_dragPanelComp != null && m_dragPanelComp.LockDraggable)
        {
            m_dragPanelComp.LockDraggable = HasGuideArrow;
        }
    }
    /// <summary>
    /// 是否有引导箭头
    /// </summary>
    /// <returns></returns>
    private bool HasGuideArrow
    {
        get
        {
            foreach(var item in m_skillsItems)
            {
                var btnGuideBehaviour = item.GetComponent<GuideBtnBehaviour>();
                if (btnGuideBehaviour != null)
                {
                    if (btnGuideBehaviour.BtnFrame != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
    private IEnumerator RefreshItem(SingleSkillInfoList singleSkillInfoList)
	{
		m_singleSkillInfoList=singleSkillInfoList;
		Layout.transform.ClearChild();
		var playerSkillList = m_singleSkillInfoList.singleSkillInfoList ;// .CanUpdateSkillInfos;
		List<UIGrid> dragUnitGrid=new List<UIGrid>();
		GameObject dragUnit=null;
		GameObject skillItem;
		UIGrid dragUnitLayout=null;
		m_skillsItems.Clear();
		int columnAmount=Mathf.CeilToInt(playerSkillList.Count/3.0f);
        m_shouldMove = playerSkillList.Count > 9;
		int j=0;
        for (int i = 0; i < playerSkillList.Count; i++)
        {
            if (i % 3 == 0)  //3项一排
            {
                dragUnit = NGUITools.AddChild(Layout.gameObject, Prefab_SkillDragUnit);
                dragUnitLayout = dragUnit.GetComponentInChildren<UIGrid>();
                dragUnitGrid.Add(dragUnitLayout);
                j++;
            }
            skillItem = NGUITools.AddChild(dragUnitLayout.gameObject, Prefab_SkillItem);
            var item = skillItem.GetComponent<SkillsItem>();
            item.DragAmount = j / columnAmount;
            //监听Item事件
            item.OnItemClick = ItemSelectedEventHandle;
            item.InitItemData(playerSkillList[i]);

            m_skillsItems.Add(item);
            //默认项选择
            if (CurrentSelectedItem == null)
            {
                if (i == 0)
                {
                    item.OnBeSelected();
                }
            }
            else if (CurrentSelectedItem.ItemFielInfo == playerSkillList[i])
            {
                item.OnBeSelected();
            }
        }
		yield return null;
		//UIGRID里面的Item重排
		dragUnitGrid.ApplyAllItem(P=>P.Reposition());
		Layout.Reposition();        
        #region`    引导注入代码
        m_skillsItems.ApplyAllItem(P =>
            {
                if (P.ItemFielInfo.localSkillData.m_skillId == 3)
                {
                    TraceUtil.Log(SystemModel.Rocky, "开始实例化技能3");
                }
                //P.gameObject.RegisterBtnMappingId(P.ItemFielInfo.localSkillData.m_skillId, UIType.Skill, BtnMapId_Sub.Skill_ListItem
                //    , NoticeToDragSlerp, P.DragAmount);
            });
        #endregion

        yield return null;

        if (m_noticeToDragAmount != 0)
        {
            StartCoroutine(DragAmountSlerp(m_noticeToDragAmount));
            m_noticeToDragAmount = 0;
        }
        else
        {
            if (HasGuideArrow)
            {
                m_dragPanelComp.LockDraggable = true;
            }
            else
            {
                m_dragPanelComp.LockDraggable = false;
            }
        }
	}
	private void ItemSelectedEventHandle(SkillsItem selectedEquipItem)
	{
		if(selectedEquipItem.ItemFielInfo!=null&&selectedEquipItem.ItemFielInfo.IsUnlock())
		{
			SelectItem(selectedEquipItem);
		}
	}
	public void SelectItem(SkillsItem selectedEquipItem)
	{
		CurrentSelectedItem=selectedEquipItem;
		//所有项LoseFocus
		m_skillsItems.ApplyAllItem(P=>P.OnLoseFocus());
		if(selectedEquipItem!=null)
		{
			//当前项GetFocus
			selectedEquipItem.OnGetFocus();
			//把事件冒泡出去
			if(OnItemClick!=null)
			{
				OnItemClick(selectedEquipItem);
			}
		}
	}
}
