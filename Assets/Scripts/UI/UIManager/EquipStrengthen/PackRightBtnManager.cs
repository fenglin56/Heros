using UnityEngine;
using System.Collections;
using System;
/// <summary>
/// 背包及强化右边控制管理器，挂在预设件 Prefab_PackRightBtnList上
/// </summary>
using System.Collections.Generic;
using UI.MainUI;


public class PackRightBtnManager : MonoBehaviour {

	public GameObject RightBtnPrefab;
	public JHGridExt GridLayout;	
	public Action<PackBtnType> PackBtnOnClick;

	private Dictionary<PackBtnType,RightBtnBehaviour> m_BtnCaches=new Dictionary<PackBtnType, RightBtnBehaviour>();

	private List<PackBtnType> m_packBtnTypes=new List<PackBtnType>();
	private TweenPosition m_tweenPosComponent;
	void Awake()
	{
		m_tweenPosComponent=GetComponent<TweenPosition>();
	}
	/// <summary>
	/// 添加按钮，可以添加多个
	/// </summary>
	/// <param name="packBtn">Pack button.</param>
	public IEnumerator AddBtn(params PackBtnType[] packBtns)
	{
		GridLayout.transform.ClearChild();
		m_BtnCaches.Clear();
		m_packBtnTypes.Clear();
		var grid=GridLayout.gameObject;
		if(packBtns!=null&&packBtns.Length>0)
		{
			m_packBtnTypes.AddRange(packBtns);
			packBtns.ApplyAllItem(item=>
			{
				var btn=NGUITools.AddChild(grid,RightBtnPrefab);
				var btnBehaviour=btn.GetComponent<RightBtnBehaviour>();
                
				//初始化按钮，并监听回调
				btnBehaviour.Init(item,PackBtnOnClick);
				m_BtnCaches.Add(item,btnBehaviour);               
			});
		}

		yield return null;
		GridLayout.Reposition();
	}
    /// <summary>
    /// 给背包右边的按钮加引导
    /// </summary>
    /// <param name="packBtnType"></param>
    /// <param name="uiType"></param>
    /// <param name="btnMapId_Sub"></param>
    public void RegisterGuideBtn(PackBtnType packBtnType, UIType uiType, BtnMapId_Sub btnMapId_Sub)
    {
        #region`    引导注入代码
        var btnBehaviour = GetBtn(packBtnType);
        if (btnBehaviour != null)
        {
            btnBehaviour.gameObject.RegisterBtnMappingId(uiType, btnMapId_Sub);
        }
        #endregion
    }
	/// <summary>
	///  打开界面，飞入动画
	/// </summary>
	public void ShowAnim()
	{
		m_tweenPosComponent.Play(true);
	}
	public void CloseAnim()
	{
		m_tweenPosComponent.Play(false);
	}
	/// <summary>
	/// 移除按钮
	/// </summary>
	/// <param name="packBtn">Pack button.</param>
	public void RemoveBtn(params PackBtnType[] packBtns)
	{
		if(packBtns!=null&&packBtns.Length>0)
		{
			packBtns.ApplyAllItem(item=>
			 {
				m_packBtnTypes.Remove(item);
			});
		}
		StartCoroutine(AddBtn(m_packBtnTypes.ToArray()));
	}
	/// <summary>
	/// 获得相关类型的按钮组件，可能为空
	/// </summary>
	/// <returns>The button.</returns>
	/// <param name="type">Type.</param>
	public RightBtnBehaviour GetBtn(PackBtnType type)
	{
		RightBtnBehaviour result;
		m_BtnCaches.TryGetValue(type,out result);
		return result;
	}
}
