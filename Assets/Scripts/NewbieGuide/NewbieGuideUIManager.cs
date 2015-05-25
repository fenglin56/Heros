using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Guide
{
    //public class NewbieGuideUIManager : View
    //{
    //    public GameObject GuideDialogPanel;
    //    public GameObject BtnSignPanel;

    //    private GameObject m_dialogPanel;
    //    private GameObject m_btnSignPanel;
    //    private GameObject m_uiHighlight;

    //    private int m_curOrder = 0;
    //    private static uint m_curBtnId;
    //    private uint m_GoToBattleID = 151003;

    //    void Awake()
    //    {
    //        this.RegisterEventHandler();
    //    }

    //    void Update()
    //    {
    //        if (!GameManager.Instance.IsNewbieGuide)
    //            return;

    //        if (NewbieGuideManager.Instance.GetGuideState && NewbieGuideManager.Instance.IsCompleteEctype)
    //        {
    //            if (NewbieGuideManager.Instance.GetCurLevelCount() > 0)
    //            {
    //                if (NewbieGuideManager.Instance.GetCurLevelList.Count >0)
    //                {
    //                    NewbieGuideManager.Instance.SetGuideState(false);
    //                    NewbieGuideManager.Instance.IsGuideFinish = false;
    //                    ShowGuideDialog(NewbieGuideManager.Instance.GetCurLevelList[m_curOrder]);
    //                }
    //                else
    //                {
    //                    NewbieGuideManager.Instance.NextGuideLevel();
    //                }
    //            }
    //            else
    //            {
    //                NewbieGuideManager.Instance.SetGuideState( false);
    //                NewbieGuideManager.Instance.IsCompleteEctype = false;
    //                GuideBtnManager.Instance.ResetButtonList();
    //                NewbieGuideManager.Instance.IsGuideFinish = true;
    //            }
    //        }
    //    }

    //    protected override void RegisterEventHandler()
    //    {
    //        this.AddEventHandler(EventTypeEnum.GuideDialogEnd.ToString(), DialogEndHandle);
    //        this.AddEventHandler(EventTypeEnum.ClickTheGuideBtn.ToString(), ClickedBtnHandle);
    //    }

    //    /// <summary>
    //    /// 设置对话面板
    //    /// </summary>
    //    /// <param name="item"></param>
    //    public void ShowGuideDialog(GuideConfigData item)
    //    {
    //        //TraceUtil.Log("@@@@@@@@@@@@@@@@@@@@@ShowGuideDialog" + item._GuideBtnID);

    //        m_curBtnId = (uint)item._GuideBtnID;
    //        Invoke("SetActiveButton", 0.6f);

    //        if (item._PreDialogList[0] == "0" || item._PreDialogList[0].Length <= 1)
    //        {
    //            DialogEndHandle(null);
    //            return;
    //        }

    //        if (m_dialogPanel == null)
    //        {
    //            m_dialogPanel = (GameObject)GameObject.Instantiate(GuideDialogPanel, this.transform.position, Quaternion.identity);
    //            m_dialogPanel.transform.parent = this.transform;
    //            m_dialogPanel.transform.localScale = Vector3.one;
    //        }
    //        m_dialogPanel.GetComponent<NewbieDialogPanel>().InitDialogPanel(item);
    //    }
        
    //    /// <summary>
    //    /// 设置Button的剪头指示
    //    /// </summary>
    //    /// <param name="item"></param>
    //    public void ShowBtnSign(GuideConfigData item, Transform btn)
    //    {
    //        Vector3 pos = btn.position;
    //        pos.z = -1.8f;
    //        m_uiHighlight.transform.position = pos;

    //        if (m_btnSignPanel == null)
    //        {
    //            m_btnSignPanel = (GameObject)GameObject.Instantiate(item._BtnSignPrefab, Vector3.zero, Quaternion.identity);
    //            m_btnSignPanel.transform.parent = btn;
    //            m_btnSignPanel.transform.localScale = Vector3.one;
    //        }

    //        m_btnSignPanel.transform.position = new Vector3(pos.x + item._BtnSignOffsetX, pos.y + item._BtnSignOffsetY, pos.z); //+ item._BtnSignOffsetX + item._BtnSignOffsetY
    //        m_btnSignPanel.GetComponent<BtnSignPanel>().InitSignPanel(item);
    //    }

    //    /// <summary>
    //    /// 设置按钮高光亮圈
    //    /// </summary>
    //    void SetHighlight(Transform btn)
    //    {
    //        //NewbieGuideManager.Instance.IsDisableClick = true;
    //        if(m_uiHighlight == null)
    //        {
    //            m_uiHighlight = (GameObject)GameObject.Instantiate(NewbieGuideManager.Instance.GetCurLevelList[m_curOrder]._HighlightRes
    //            , Vector3.zero, Quaternion.identity);
    //            m_uiHighlight.transform.parent = btn;
    //            m_uiHighlight.transform.localScale = Vector3.one;
    //        }
    //        //TraceUtil.Log("##################################" + m_curBtnId);

    //        ShowBtnSign(NewbieGuideManager.Instance.GetCurLevelList[m_curOrder], btn);
            
    //    }
        
    //    /// <summary>
    //    /// 按钮跳过规则
    //    /// </summary>
    //    void SkipRole()
    //    {
    //        ///如果在当前按钮管理列表中无当前引导按钮ID，则判断当前按钮是否可以跳过，否则就会报错
    //        if(!GuideBtnManager.Instance.GetButtonList.ContainsKey(m_curBtnId))
    //        {
    //            foreach(var item in NewbieGuideManager.Instance.GetCurLevelList)
    //            {
    //                if(item._GuideBtnID == m_curBtnId)// && item._SkipRole)
    //                {
    //                        ResetNewbieGuide();
    //                        NewbieGuideManager.Instance.SetGuideState(true);
    //                        return;
    //                }
    //            }
    //        }

    //        ///根据ID获取当前引导按钮的类型UIType和SubUIType
    //        GuideBtnParam param = GuideBtnManager.Instance.GetButtonParamList[m_curBtnId];
            
			
    //        ///如果当前按钮主类型为背包，子类型为背包列表
    //        if (param.UIType == UI.MainUI.UIType.PackInfo && param.SubUIType == SubUIType.PackBoxList)
    //        {
				
    //            ///判断当前背包格是否有物品，如果没有则跳过，进行下一步引导
    //            int order = (int)m_curBtnId % 100;
    //            bool isHave = UI.MainUI.ContainerInfomanager.Instance.ContainerGoodsIsFull(order);
    //            //bool isHave = GuideBtnManager.Instance.GetButtonList[m_curBtnId].GetComponent<UI.MainUI.LocalPackBoxInfo>().IsFull();
    //            if (!isHave)
    //            {
    //                ResetNewbieGuide();
    //                NewbieGuideManager.Instance.SetGuideState(true);
    //                return;
    //            }
    //        }
    //        ///如果当前引导按钮主类型为强化，子类型为强化子项	
    //        if (param.UIType == UI.MainUI.UIType.EquipStrengthen && param.SubUIType == SubUIType.EquipItemBtn)
    //        {
    //            ///判断是否有足够金币给予强化，如果金币不够则跳过
    //            ///这里强化新手引导需要重做。
    //            //bool isEnough = GuideBtnManager.Instance.GetButtonList[m_curBtnId].transform.parent.GetComponent<EquipStrengthenInfoPanel>().IsEnough;
    //            //if (!isEnough)
    //            //{
    //            //    ResetNewbieGuide();
    //            //    NewbieGuideManager.Instance.SetGuideState(true);
    //            //    return;
    //            //}
    //        }

    //        ///如果无须跳过则屏蔽其它按钮，显示当前按钮，并显示亮圈
    //        SetHighlight(GuideBtnManager.Instance.GetButtonList[m_curBtnId].transform);
            
    //    }
			

    //    private void DialogEndHandle(INotifyArgs notifyArgs)
    //    {
    //        SkipRole();
    //    }

    //    /// <summary>
    //    /// 单击引导按钮后的回调函数
    //    /// </summary>
    //    /// <param name="notifyArgs"></param>
    //    private void ClickedBtnHandle(INotifyArgs notifyArgs)
    //    {
    //        //Invoke("DelSign", 0.25f);

    //        DelSign();
    //        //NewbieGuideManager.Instance.IsDisableClick = false;
    //        ResetNewbieGuide();

    //        if (m_curBtnId == m_GoToBattleID)
    //        {
    //            DelSign();
    //            NewbieGuideManager.Instance.IsCompleteEctype = false;
    //            RaiseEvent(EventTypeEnum.GuideGoToBattle.ToString(), null);
    //        }
    //        else
    //            NewbieGuideManager.Instance.SetGuideState(true);
    //            //Invoke("GuideEnable", 0.6f);
                
    //    }

    //    void DelSign()
    //    {
    //        DestroyImmediate(m_uiHighlight);
    //        DestroyImmediate(m_btnSignPanel);
    //    }

    //    void SetActiveButton()
    //    {
    //        GuideBtnManager.Instance.SetActiveButton(m_curBtnId);
    //    }
    //    /// <summary>
    //    /// 重置新手引导
    //    /// </summary>
    //    private void ResetNewbieGuide()
    //    {
    //        if (m_curOrder < NewbieGuideManager.Instance.GetCurLevelList.Count - 1)
    //        {
    //            m_curOrder += 1;
    //        }
    //        else
    //        {
    //            m_curOrder = 0;
    //            if (m_curBtnId != m_GoToBattleID)
    //                NewbieGuideManager.Instance.DegressionLevelCount();                  

    //            RaiseEvent(EventTypeEnum.CloseSystemMainBtn.ToString(), null);
    //        }
    //    }

    //}
}
