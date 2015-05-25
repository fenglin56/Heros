using UnityEngine;
using System.Collections;
using System.Linq;
using UI.MainUI;
using System.Collections.Generic;
using UI;

public class DailyTaskPanel : BaseUIPanel
{
    public DailyTaskConfigDataBase DailyTaskConfig;    
    public DailyTaskRewardPanel RewardPanel;

    public GameObject Eff_UI_Settlement_Box_Prefab;
	public GameObject GetDailyTaskRewardAnimationPrefab;

    public UILabel Label_Title_TaskNum;
    public LocalButtonCallBack Button_Exit;        

    //rewardInfo
    public LocalButtonCallBack Button_Help;    
    
    public UILabel Label_active;
    public UISlider Slider_active;

    public GameObject RewardInfo;
    public LocalButtonCallBack Button_ReceiveReward;

    public SpriteSwith Switch_ChestType_1;
    public SpriteSwith Switch_ChestType_2;
    public UILabel Label_ChestType_Value1;
    public UILabel Label_ChestType_Value2;
    public UILabel Label_ChestType_Txt1;
    public UILabel Label_ChestType_Txt2;

    //private int m_rewardProcess = 0;

    //tasklist
    public UIDraggablePanel DraggablePanel;
    public UIGrid Grid;

    private Dictionary<int, DailyTaskItem> m_taskItemDict = new Dictionary<int, DailyTaskItem>();

    public GameObject DailyTaskItemPrefab;

    void Awake()
    {
        Button_Exit.SetCallBackFuntion(OnExitHandle, null);
        Button_Help.SetCallBackFuntion(OnHelpHandle, null);
        Button_ReceiveReward.SetCallBackFuntion(OnReceiveRewardHandle, null);

        RegisterEventHandler();        
    }

	void OnDestroy()
	{
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OpenActiveChestUI, OpenUIHandle);
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OpenActiveChest, OpenActiveChestHandle);
	}

    public override void Show(params object[] value)
    {
        NetServiceManager.Instance.EquipStrengthenService.SendRequestActiveChestProgressCommand();//获取奖励进度
        UpdateTaskItemInfos();//更新任务
        UpdateRewardsInfo();//更新活跃值信息
		ShowWillReceiveRewardInfo();//显示最接近领取的宝箱
        base.Show(value);
    }

    public override void Close()
    {
        if (!IsShow)
            return;
        base.Close();
    }

    void OnExitHandle(object obj)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
        Close();
        CleanUpUIStatus();
    }
    void OnHelpHandle(object obj)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");        
        RewardPanel.SetActive(true);
    }
    void OnReceiveRewardHandle(object obj)
    {
		int nextProcess = DailyTaskManager.Instance.RewardProcess + 1;
        NetServiceManager.Instance.EquipStrengthenService.SendOpenChestValueCommand(nextProcess);
        Button_ReceiveReward.SetBoxCollider(false);
        Button_ReceiveReward.ButtonBackground.alpha = 0;
    }
    void OpenUIHandle(object eventArg)
    {
        //SMsgOpenActiveChestUISC sMsgOpenActiveChestUISC = (SMsgOpenActiveChestUISC)eventArg;   
        //this.m_rewardProcess = sMsgOpenActiveChestUISC.dwProgress;       
		ShowWillReceiveRewardInfo();
        JudgeOpenNewChest();
    }
    void OpenActiveChestHandle(object eventArg)
    {
        //CloseReceiveRewardInfo();
        SMsgOpenActiveChestSC sMsgOpenActiveChestSC = (SMsgOpenActiveChestSC)eventArg;
        //动画特效
        GameObject eff = (GameObject)Instantiate(Eff_UI_Settlement_Box_Prefab);
        eff.transform.parent = RewardInfo.transform.parent;
        eff.transform.localPosition = Button_ReceiveReward.transform.localPosition + Vector3.forward * -10f;
        eff.transform.localScale = Vector3.one * 0.62f; //需求指明缩小到62%
        eff.AddComponent<DestroySelf>();

		//奖励动画
		GameObject empty = new GameObject();
		empty.transform.position = Button_ReceiveReward.transform.position;
		empty.transform.parent = Button_Exit.transform.parent;
		Vector3 startPos =  Button_Exit.transform.parent.localPosition+ empty.transform.localPosition+ Vector3.forward*-20;
		Vector3 endPos =  Button_Exit.transform.parent.localPosition+ Button_Exit.transform.localPosition+ Vector3.forward*-20;
		Destroy(empty);
		if(Label_ChestType_Txt1.gameObject.activeInHierarchy)
		{
			GameObject rewardGo = CreatObjectToNGUI.InstantiateObj(GetDailyTaskRewardAnimationPrefab, transform);
			GetDailyTaskRewardAnimation rewardAnimation = rewardGo.GetComponent<GetDailyTaskRewardAnimation>();
			rewardAnimation.Show(Switch_ChestType_1.GetComponent<UISprite>().spriteName,startPos,
			                     Label_ChestType_Value1.text,endPos);
		}
		if(Label_ChestType_Txt2.gameObject.activeInHierarchy)
		{
			GameObject rewardGo = CreatObjectToNGUI.InstantiateObj(GetDailyTaskRewardAnimationPrefab, transform);
			GetDailyTaskRewardAnimation rewardAnimation = rewardGo.GetComponent<GetDailyTaskRewardAnimation>();
			rewardAnimation.Show(Switch_ChestType_2.GetComponent<UISprite>().spriteName,startPos,
			                     Label_ChestType_Value2.text,endPos);
		}

        StartCoroutine("DestroyChest");
    }

    IEnumerator DestroyChest()
    {
        yield return new WaitForSeconds(2f);
		//ShowWillReceiveRewardInfo();
        yield return new WaitForSeconds(2f);
        //继续请求进度
        NetServiceManager.Instance.EquipStrengthenService.SendRequestActiveChestProgressCommand();//获取奖励进度
    }

    private void InitTasks()
    {
        //var TaskConfigData = NewbieGuideManager_V2.Instance.ExecuteTask;
        //DailyTaskConfig._dataTable.ApplyAllItem(p =>
        //    {
        //        //if (p._triggerCondition <= TaskConfigData._TaskID)
        //        //{
        //        //    GameObject taskItem = (GameObject)Instantiate(DailyTaskItemPrefab);
        //        //    taskItem.transform.parent = Grid.transform;
        //        //    taskItem.transform.localPosition = Vector3.zero;
        //        //    taskItem.transform.localScale = Vector3.one;
        //        //    DailyTaskItem itemBehaviour = taskItem.GetComponent<DailyTaskItem>();
        //        //    itemBehaviour.Init(p, new STaskLogUpdate() { nTaskID = 0, nStatus = 0, nTaskType = 1 });
        //        //}
        //        GameObject taskItem = (GameObject)Instantiate(DailyTaskItemPrefab);
        //        taskItem.transform.parent = Grid.transform;
        //        taskItem.transform.localPosition = Vector3.zero;
        //        taskItem.transform.localScale = Vector3.one;
        //        DailyTaskItem itemBehaviour = taskItem.GetComponent<DailyTaskItem>();
        //        itemBehaviour.Init(p, new STaskLogUpdate() { nTaskID = 0, nStatus = 0, nTaskType = 1 });
        //    });

       
    }

    private void UpdateTaskItemInfos()
    {        
        DailyTaskManager.Instance.GetTaskLogList().ApplyAllItem(p =>
        {
            if (!m_taskItemDict.Any(item => item.Key == p.LogUpdate.nTaskID))
            {
                var config = DailyTaskConfig._dataTable.SingleOrDefault(data => data._taskID == p.LogUpdate.nTaskID);
                if (config == null)
                {
                    TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"[this DailyTaskConfig is null]");
                    return;
                }
                GameObject taskItem = (GameObject)Instantiate(DailyTaskItemPrefab);
                taskItem.transform.parent = Grid.transform;
                taskItem.transform.localPosition = Vector3.zero;
                taskItem.transform.localScale = Vector3.one;
                DailyTaskItem itemBehaviour = taskItem.GetComponent<DailyTaskItem>();
                itemBehaviour.Init(config, p.LogUpdate, p.LogContext);

                m_taskItemDict.Add(p.LogUpdate.nTaskID, itemBehaviour);
            }
            else
            {
                m_taskItemDict[p.LogUpdate.nTaskID].UpdateView(p.LogUpdate, p.LogContext);
            }
        });
        DraggablePanel.ResetPosition();
        Grid.Reposition();
        //更新任务数量
        var completeList = m_taskItemDict.Values.Where(p => p.Status == 2).ToArray();
        Label_Title_TaskNum.text = completeList.Length.ToString() + "/" + m_taskItemDict.Count.ToString();
    }

    void Start()
    {
        DraggablePanel.ResetPosition();
        Grid.Reposition();
    }

    //更新活跃值信息栏
    private void UpdateRewardsInfo()
    {
        var playerData = PlayerManager.Instance.FindHeroDataModel();
        //更新活跃值信息
		int allActive = 0;
		var taskItemArray = m_taskItemDict.Values.ToArray();
		for(int i=0; i< taskItemArray.Length ;i++)
		{
			allActive+=taskItemArray[i].CanGetActiveValue;
		}
		Label_active.text = playerData.PlayerValues.PLAYER_FIELD_ACTIVE_VALUE + "/" + allActive;
		Slider_active.sliderValue = playerData.PlayerValues.PLAYER_FIELD_ACTIVE_VALUE * 1f / allActive;        
    }
    //判断是否开启新宝箱
    private void JudgeOpenNewChest()
    {
        var playerData = PlayerManager.Instance.FindHeroDataModel();
		var rewardConfigDataTable = DailyTaskDataManager.Instance.GetDailyTaskRewardConfigArray();
		var curRewardLevel = rewardConfigDataTable.SingleOrDefault(p=>p._boxSequence == DailyTaskManager.Instance.RewardProcess+1);
		if(curRewardLevel !=null)
		{
			if(playerData.PlayerValues.PLAYER_FIELD_ACTIVE_VALUE >= curRewardLevel._requirementActiveValue)
			{
				//开启
				RewardInfo.SetActive(true);
				Button_ReceiveReward.gameObject.SetActive(true);
				Button_ReceiveReward.SetBoxCollider(true);
				Button_ReceiveReward.ButtonBackground.alpha = 1f;			
				
				ShowRewardInfo(curRewardLevel._awardType[0], playerData.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION, curRewardLevel,
				               Switch_ChestType_1, Label_ChestType_Txt1, Label_ChestType_Value1);
				
				if (curRewardLevel._awardType.Length < 2)
				{
					Label_ChestType_Txt2.gameObject.SetActive(false);
					Label_ChestType_Value2.gameObject.SetActive(false);
					Switch_ChestType_2.gameObject.SetActive(false);
				}
				else
				{
					Label_ChestType_Txt2.gameObject.SetActive(true);
					Label_ChestType_Value2.gameObject.SetActive(true);
					Switch_ChestType_2.gameObject.SetActive(true);
					
					ShowRewardInfo(curRewardLevel._awardType[1], playerData.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION, curRewardLevel,
					               Switch_ChestType_2, Label_ChestType_Txt2, Label_ChestType_Value2);
				}    

				return;
			}
		}
        for (int i = 0; i < rewardConfigDataTable.Length; i++)
        {
            if (playerData.PlayerValues.PLAYER_FIELD_ACTIVE_VALUE < rewardConfigDataTable[i]._requirementActiveValue)
            {
                TraceUtil.Log("[JudgeOpenNewChest is right]");
                if (i > 0)
                {
					//TraceUtil.Log("[rewardProcess]" + DailyTaskManager.Instance.RewardProcess + " ? " + rewardConfigDataTable[i - 1]._boxSequence);
					if (DailyTaskManager.Instance.RewardProcess < rewardConfigDataTable[i - 1]._boxSequence)
                    {
                        //开启
                        RewardInfo.SetActive(true);
                        Button_ReceiveReward.gameObject.SetActive(true);
                        Button_ReceiveReward.SetBoxCollider(true);
                        Button_ReceiveReward.ButtonBackground.alpha = 1f;
                        

                        //显示宝箱信息 
                        var configData = rewardConfigDataTable[i - 1];

                        ShowRewardInfo(configData._awardType[0], playerData.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION, configData,
                           Switch_ChestType_1, Label_ChestType_Txt1, Label_ChestType_Value1);

                        if (configData._awardType.Length < 2)
                        {
                            Label_ChestType_Txt2.gameObject.SetActive(false);
                            Label_ChestType_Value2.gameObject.SetActive(false);
                            Switch_ChestType_2.gameObject.SetActive(false);
                        }
                        else
                        {
                            Label_ChestType_Txt2.gameObject.SetActive(true);
                            Label_ChestType_Value2.gameObject.SetActive(true);
                            Switch_ChestType_2.gameObject.SetActive(true);

                            ShowRewardInfo(configData._awardType[1], playerData.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION, configData,
                                Switch_ChestType_2, Label_ChestType_Txt2, Label_ChestType_Value2);
                        }    
						break;
                    }
                }                
                break;
            }
        }
    }

    private void ShowRewardInfo(int type, int vocation, DailyTaskRewardConfigData data, SpriteSwith ss, UILabel  label_txt, UILabel label_value)
    {
        switch (type)
        {
            case 1:
                var awardItem = data._awardItem.SingleOrDefault(p => p.Profession == vocation);
                if (awardItem != null)
                {
                    var itemConfig = ItemDataManager.Instance.GetItemData(awardItem.PropID);
                    label_txt.text = LanguageTextManager.GetString(itemConfig._szGoodsName);
                    label_value.text = "";                   
                }
                break;
            case 2:
                label_txt.text = LanguageTextManager.GetString("IDS_D2_17");
                label_value.text = "+" + data._awardMoney;
                break;
            case 3:
                label_txt.text = LanguageTextManager.GetString("IDS_A1_5017");
                label_value.text = "+" + data._awardExp;
                break;
            case 4:
                label_txt.text = LanguageTextManager.GetString("IDS_H1_120");
                label_value.text = "+" + data._awardActive;
                break;
            case 5:
                label_txt.text = LanguageTextManager.GetString("IDS_A1_5019");
                label_value.text = "+" + data._awardXiuwei;
                break;
            case 6:
                label_txt.text = LanguageTextManager.GetString("IDS_D2_18");
                label_value.text = "+" + data._awardIngot;
                break;
        }
        ss.ChangeSprite(type);
    }

    private void ShowWillReceiveRewardInfo()
    {
        //关闭宝箱信息
        //RewardInfo.SetActive(false);
        //Button_ReceiveReward.gameObject.SetActive(false);

		//读取新的宝箱信息
		var rewardConfigDataTable = DailyTaskDataManager.Instance.GetDailyTaskRewardConfigArray();
		var rewardConfig = rewardConfigDataTable.SingleOrDefault(p=>p._boxSequence == DailyTaskManager.Instance.RewardProcess+1);
		if(rewardConfig != null)
		{
			var playerData = PlayerManager.Instance.FindHeroDataModel();

			RewardInfo.SetActive(true);
			Button_ReceiveReward.gameObject.SetActive(true);
			Button_ReceiveReward.SetBoxCollider(false);
			Button_ReceiveReward.ButtonBackground.alpha = 0;

			//显示宝箱信息 
			//var configData = rewardConfig;
			
			ShowRewardInfo(rewardConfig._awardType[0], playerData.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION, rewardConfig,
			               Switch_ChestType_1, Label_ChestType_Txt1, Label_ChestType_Value1);
			
			if (rewardConfig._awardType.Length < 2)
			{
				Label_ChestType_Txt2.gameObject.SetActive(false);
				Label_ChestType_Value2.gameObject.SetActive(false);
				Switch_ChestType_2.gameObject.SetActive(false);
			}
			else
			{
				Label_ChestType_Txt2.gameObject.SetActive(true);
				Label_ChestType_Value2.gameObject.SetActive(true);
				Switch_ChestType_2.gameObject.SetActive(true);
				
				ShowRewardInfo(rewardConfig._awardType[1], playerData.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION, rewardConfig,
				               Switch_ChestType_2, Label_ChestType_Txt2, Label_ChestType_Value2);
			}                    
		}
    }


    protected override void RegisterEventHandler()
    {
        //UIEventManager.Instance.RegisterUIEvent(UIEventType.ShowCanReceiveActiveChest, ShowCanReceiveActiveChestHandle);
        UIEventManager.Instance.RegisterUIEvent(UIEventType.OpenActiveChestUI, OpenUIHandle);
        UIEventManager.Instance.RegisterUIEvent(UIEventType.OpenActiveChest, OpenActiveChestHandle);
    }
}
