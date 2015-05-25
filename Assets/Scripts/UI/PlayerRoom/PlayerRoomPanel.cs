using UnityEngine;
using System.Collections;
using UI.PlayerRoom;
using NetworkCommon;
using System;
using System.Linq;
using UI;
public class PlayerRoomPanel : View 
{
    public enum XiuLianType
    {
        NONE_XIULIAN_TYPE = 0,
        OFFLINE_XIULIAN_TYPE,		//线下修为累积领取
        ONLINE_XIULIAN_TYPE,		//线上修为累积领取
        ROOMDES_XIULIAN_TYPE,		//武馆销毁时领取修为
        LEAVE_XIULIAN_TYPE,			//离开房间领取修为
        BREAK_XIULIAN_TYPE,			//突破修为累积领取
    };

    public FollowCamera FollowCamera;

    public UILabel Label_Homeowners;
    public UILabel Label_RoomID;
    public UILabel Label_PlayerNum;

    public SingleButtonCallBack Button_Detail;
    public SingleButtonCallBack Button_Get;
    public UISlider Slider_Practice;
    public UILabel Label_Practice;

    public GameObject Menu_Left;
    public SingleButtonCallBack Button_ReturnTown;
    public SingleButtonCallBack Button_ShowChat;
    public SingleButtonCallBack Button_ShowFriend;
    public SingleButtonCallBack Button_ShowSirenl;
    public GameObject Image_ShowSiren;

    public SingleButtonCallBack Button_Break;
    public SingleButtonCallBack Button_Exit;

    public PlayerRoomDetailsPracticePanel Panel_DetailsPractice;
    public PlayerRoomSirenControlPanel Panel_SirenControl;
    public PlayerRoomBreakPanel Panel_Break;
    public PlayerRoomPracticeOutcomesPanel Panel_PracticeOutcomes;
    public PlayerRoomChatPanel Panel_Chat;
    public PlayerRoomFriendPanel Panel_Friend;
    public PlayerRoomBoxControlPanel Panel_Box;

    public CollectPracticeAnimation CollectPracticeAnimation;
    public PlayerRoomAccoutConfigDataBase PlayerRoomAccoutConfigDataBase;

    public Transform HomerPoint;//房主位置

    private const int ColdingGetButtonSeconds = 5;
    private int m_unfreezeGetButtonSeconds;

    private float mPracticeTime = 0;//修炼时长
    private PlayerRoomAccoutConfigData m_HomerAccountConfigData;//当前房间模版

    private int m_RoomMemberNum = 0;//房间人数

    private bool m_IsHomer = false;//是否房主

    private int[] m_guideBtnID = new int[8];

    void Awake()
    {
        Button_Detail.SetCallBackFuntion(ShowDetailPanelHandle, null);
        Button_Get.SetCallBackFuntion(GetPracticeOutcomesHandle, null);

        Button_ReturnTown.SetCallBackFuntion(ExitRoomHandle, null);
        Button_ShowChat.SetCallBackFuntion(ShowChatPanelHandle, null);
        Button_ShowFriend.SetCallBackFuntion(ShowFriendPanelHandle, null);
        Button_ShowSirenl.SetCallBackFuntion(ShowSirenControlPanelHanle, null);
        

        Button_Break.SetCallBackFuntion(ShowBreakPanelHandle, null);
        Button_Exit.SetCallBackFuntion(ExitLoginHandle, null);//改为退出游戏

        Panel_Box.SetActionCallBack(GetAndReturnTown);

        RegisterEventHandler();

    }
    void Start()
    {
        ResponseHandleInvoker.Instance.IsPaused = false;
        ReceiveUpdateRoomSeatInfoHandle(null);//重连情况下重取房间信息

        CoolingGetButtonHandle(null);//冷却消息        

        ResetPracticeTime();//重置时间

        Panel_PracticeOutcomes.SetAction(PlayCollectParcticeAnimation);//关联回调        
                
        //妖女展现
        var yaonvUpdateInfo = PlayerRoomManager.Instance.GetYaoNvUpdateInfo();

        Panel_SirenControl.CreateSirens();
        if (yaonvUpdateInfo.dwYaoNvList != null)
        {
            Panel_SirenControl.UpdateSirenModel(yaonvUpdateInfo.dwYaoNvList);
        }

		//离线修为
		if(PlayerRoomManager.Instance.GetXiuLianInfo().XiuLianNum>0)
		{
			ReceiveXiuLianAccountHandle(PlayerRoomManager.Instance.GetXiuLianInfo());
		}

        //TODO GuideBtnManager.Instance.RegGuideButton(Button_ReturnTown.gameObject, UI.MainUI.UIType.MartialArtsRoom, SubType.MartialPlayerRoom, out m_guideBtnID[0]);
        //TODO GuideBtnManager.Instance.RegGuideButton(Button_ShowChat.gameObject, UI.MainUI.UIType.MartialArtsRoom, SubType.MartialPlayerRoom, out m_guideBtnID[1]);
        //TODO GuideBtnManager.Instance.RegGuideButton(Button_ShowFriend.gameObject, UI.MainUI.UIType.MartialArtsRoom, SubType.MartialPlayerRoom, out m_guideBtnID[2]);
        //TODO GuideBtnManager.Instance.RegGuideButton(Button_ShowSirenl.gameObject, UI.MainUI.UIType.MartialArtsRoom, SubType.MartialPlayerRoom, out m_guideBtnID[3]);
        //TODO GuideBtnManager.Instance.RegGuideButton(Button_Detail.gameObject, UI.MainUI.UIType.MartialArtsRoom, SubType.MartialPlayerRoom, out m_guideBtnID[4]);
        //TODO GuideBtnManager.Instance.RegGuideButton(Button_Get.gameObject, UI.MainUI.UIType.MartialArtsRoom, SubType.MartialPlayerRoom, out m_guideBtnID[5]);
        //TODO GuideBtnManager.Instance.RegGuideButton(Button_Break.gameObject, UI.MainUI.UIType.MartialArtsRoom, SubType.MartialPlayerRoom, out m_guideBtnID[6]);
        //TODO GuideBtnManager.Instance.RegGuideButton(Button_Exit.gameObject, UI.MainUI.UIType.MartialArtsRoom, SubType.MartialPlayerRoom, out m_guideBtnID[7]);        
    }


    void FixedUpdate()
    {
        if (Time.fixedTime * 25 % (25 * 1)== 0)
        {
            //TraceUtil.Log("[CountPracticeResult]");
            if (m_HomerAccountConfigData != null)
            {
                UpdatePracticeInfo();
            }
        }
    }

    private void UpdatePracticeInfo()
    {
        int result = CountPracticeResult();
        int maxValue = m_HomerAccountConfigData._upperLimit;
        Slider_Practice.sliderValue = result / maxValue;
        Label_Practice.text = result.ToString() + "/" + maxValue.ToString();
    }

    void GetPracticeOutcomesHandle(object obj)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
        NetServiceManager.Instance.EntityService.SendAccountXiuLian();
        Button_Get.SetButtonColliderActive(false);
    }
    void ShowChatPanelHandle(object obj)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
        Panel_Chat.ShowPanel();
    }
    void ShowFriendPanelHandle(object obj)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
        Panel_Friend.ShowPanel();
        Menu_Left.SetActive(false);
    }
    void CloseFriendPanelHandle()
    {
        Menu_Left.SetActive(true);
    }
    void ShowDetailPanelHandle(object obj)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
        if (m_HomerAccountConfigData != null)
        {
            Panel_DetailsPractice.Show(m_HomerAccountConfigData, CountSirenAddition(), m_RoomMemberNum, CountPracticeResult(60), CountPracticeResult(), m_IsHomer);
        }
    }
    void ShowBreakPanelHandle(object obj)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
        Panel_Break.Show(CountSirenAddition(), CountBreakResult(4 * 60));
    }
    void ExitLoginHandle(object obj)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
        GameManager.Instance.QuitToLogin();
    }
    void ShowSirenControlPanelHanle(object obj)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
        Panel_SirenControl.ShowPanel(m_IsHomer);
    }
    void ExitRoomHandle(object obj)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");

        //打坐未超过一分钟
        float time = Time.time - mPracticeTime;
        if (time < 60)
        {
            //TraceUtil.Log("[SendEctypeRequestReturnCity]");
            //请求返回城镇
            long UIDEntity = PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity;
            NetServiceManager.Instance.EctypeService.SendEctypeRequestReturnCity(UIDEntity);
            return;
        }

        if (m_IsHomer)//如果是房主
        {
            Panel_Box.ShowPanel(PlayerRoomBoxControlPanel.BoxType.HomeOwner, CountPracticeResult());
        }
        else
        {
            Panel_Box.ShowPanel(PlayerRoomBoxControlPanel.BoxType.Roomer_Get, CountPracticeResult());
            //Panel_Box.ShowPanel(PlayerRoomBoxControlPanel.BoxType.Roomer_GetAndLeave, CountPracticeResult());
        }
    }
    void GetAndReturnTown()
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_RoomAward");
        PlayCollectParcticeAnimation(CountPracticeResult());
        Button_Get.SetButtonColliderActive(false);
    }

    //冷却领取修为按钮
    private void CoolingGetButton(int seconds)
    {
        CancelInvoke("UnfreezeGetButton");
        Button_Get.SetButtonColliderActive(false);
        Button_Get.textLabel.gameObject.SetActive(true);
        Button_Get.SetImageAlpha(0.5f);
        Button_Get.SetButtonText("");
        m_unfreezeGetButtonSeconds = seconds + 3;        
        InvokeRepeating("UnfreezeGetButton", 0, 1f);
    }
    void UnfreezeGetButton()
    {
        m_unfreezeGetButtonSeconds--;
        if (m_unfreezeGetButtonSeconds > 0)
        {
            int min = m_unfreezeGetButtonSeconds / 60;
            int sec = m_unfreezeGetButtonSeconds % 60;
            Button_Get.SetButtonText(ParseClock(min) + ":" + ParseClock(sec));
        }
        else
        {
            Button_Get.SetButtonColliderActive(true);
            Button_Get.textLabel.gameObject.SetActive(false);
            Button_Get.SetImageAlpha(1);            
            CancelInvoke("UnfreezeGetButton");
        }
    }
    private string ParseClock(int time)
    {
        if (time < 10)
        {
            return "0" + time.ToString();
        }
        return time.ToString();
    }
    //计算当前可得到的修为值
    private int CountPracticeResult()
    {
        if (m_HomerAccountConfigData == null)
            return 0;
        float time = Time.time - mPracticeTime;
        int minute = (int)(time / 60);
        int result = Mathf.Clamp(CountPracticeResult(minute), 0, m_HomerAccountConfigData._upperLimit);
        return result;
    }
    /// <summary>
    /// 计算修为
    /// </summary>
    /// <param name="time">时间(分)</param>
    /// <returns></returns>
    private int CountPracticeResult(float time)
    {
        if (m_HomerAccountConfigData == null)
            return 1;
        int sirenAddition = CountSirenAddition();
        int ownerAddition = m_IsHomer == true ? m_HomerAccountConfigData._ownerAddition : 0;
        int result = (int)(time * m_HomerAccountConfigData._basicsParam * (1 + ownerAddition / 100f + sirenAddition / 100f
            + m_HomerAccountConfigData._guestAddition / 100f * (m_RoomMemberNum - 1)) + 0.001f);
        //当前获得修为=向下取整(修炼时间×房间修炼基础修为×（1+房主修炼加成+妖女修炼加成+单个房客修为增加值×(当前房间人数-1)）
        return result;
    }

    /// <summary>
    /// 计算突破修为
    /// </summary>
    /// <param name="time">时间(分)</param>
    /// <returns></returns>
    private int CountBreakResult(float time)
    {
        if (m_HomerAccountConfigData == null)
            return 1;
        int sirenAddition = CountSirenAddition();
        TraceUtil.Log("[result]" + time * m_HomerAccountConfigData._basicsParam * (1 + sirenAddition / 100f));
        int result = (int)(time * m_HomerAccountConfigData._basicsParam * (1 + sirenAddition / 100f));
        //当前获得修为=向下取整(修炼时间×房间修炼基础修为×（1+妖女修炼加成））
        //TraceUtil.Log("[CountBreakResult]basicsParam=" + m_HomerAccountConfigData._basicsParam + " , sirenAddition=" + sirenAddition + " ,time" + time);
        return result;
    }

    //计算自身妖女加成
    private int CountSirenAddition()
    {
        var sirenConfigList = PlayerRoomDataManager.Instance.GetPlayerSirenList();
        int sirenAddition = 0;
        SirenManager.Instance.GetYaoNvList().ApplyAllItem(p =>
        {
            var sirenConfig = sirenConfigList.SingleOrDefault(k => k._sirenID == p.byYaoNvID);
            if (sirenConfig != null)
            {
                var sirenLevelConfig = sirenConfig._sirenConfigDataList.SingleOrDefault(m => m._growthLevels == p.byLevel);
                if (sirenLevelConfig != null)
                {
                    sirenAddition += sirenLevelConfig._sitEffect;
                }
            }
        });
        return sirenAddition;
    }

    //重置修炼时间(得到修为值后)
    private void ResetPracticeTime()
    {
        mPracticeTime = Time.time;
    }

    //设置摄像头位置
    private void SetCamera()
    {
        //var hero = PlayerManager.Instance.FindPlayer(homerUID);
        //if (hero != null)
        //{
        //    FollowCamera.SetInitDistanceFromPlayer(hero.transform, m_HomerAccountConfigData._camera, true);
        //}
        //摄像机
        HomerPoint.position = m_HomerAccountConfigData.PlayerPosList.First();
        FollowCamera.SetInitDistanceFromPlayer(HomerPoint, m_HomerAccountConfigData._camera, true);
    }

    void ReceiveXiuLianAccountHandle(INotifyArgs arg)
    {
        SMsgActionXiuLianInfo_SC sMsgActionXiuLianInfo_SC = (SMsgActionXiuLianInfo_SC)arg;
        TraceUtil.Log("[XiuLianInfo]" + sMsgActionXiuLianInfo_SC.byXiuLianType + " , " + sMsgActionXiuLianInfo_SC.XiuLianTime + " , " + sMsgActionXiuLianInfo_SC.XiuLianNum);
        switch ((XiuLianType)sMsgActionXiuLianInfo_SC.byXiuLianType)
        {
            case  XiuLianType.OFFLINE_XIULIAN_TYPE:                
                //离线修为下发
                Panel_PracticeOutcomes.ShowPanel(PlayerRoomPracticeOutcomesPanel.LineType.Offline, sMsgActionXiuLianInfo_SC);
                ResetPracticeTime();
                break;
            case XiuLianType.ONLINE_XIULIAN_TYPE:                
                //在线修为下发
                Panel_PracticeOutcomes.ShowPanel(PlayerRoomPracticeOutcomesPanel.LineType.Online, sMsgActionXiuLianInfo_SC);
                ResetPracticeTime();
                break;
            case XiuLianType.ROOMDES_XIULIAN_TYPE:
                
                break;
            case XiuLianType.BREAK_XIULIAN_TYPE:
                //突破修为下发
                Panel_PracticeOutcomes.ShowPanel(PlayerRoomPracticeOutcomesPanel.LineType.Online, sMsgActionXiuLianInfo_SC);
                break;
            default:
                break;
        }                
    }

    void PlayCollectParcticeAnimation(int value)
    {
        //获取修为动画
        int roomAwardDisplay = CommonDefineManager.Instance.CommonDefine.RoomAwardDisplay;
        int num = Mathf.CeilToInt((value * 1f / roomAwardDisplay));
        int lastCollectValue = value % roomAwardDisplay;
        TraceUtil.Log("[lastCollectValue]" + lastCollectValue);
        for (int i = 0; i < num; i++)
        {
            if (i == num - 1)
            {
                roomAwardDisplay = lastCollectValue;
            }
            CollectPracticeAnimation collectPracticeAnimation = CreatObjectToNGUI.InstantiateObj(CollectPracticeAnimation.gameObject, transform).GetComponent<CollectPracticeAnimation>();
            collectPracticeAnimation.Show(Vector3.zero, roomAwardDisplay, Button_Get.transform.localPosition);
        }
    }

    void ReceiveUpdateRoomSeatInfoHandle(INotifyArgs arg)
    {
        var roomSeatInfo = PlayerRoomManager.Instance.GetRoomSeatInfo();
        if (roomSeatInfo.dwPlayerNum == 0)
        {
            return;
        }
        m_RoomMemberNum = (int)roomSeatInfo.dwPlayerNum;
        var playerData = PlayerManager.Instance.FindHeroDataModel();        
        if (playerData.ActorID == 0 || !GameManager.Instance.CreateEntityIM)
        {
            StartCoroutine(LateUpdateRoomSeatInfo());
            return;
        }
        m_IsHomer = playerData.ActorID == roomSeatInfo.dwHomerActorID;

        Label_RoomID.text = "(" + roomSeatInfo.dwRoomID.ToString() + ")";
        Label_PlayerNum.text = m_RoomMemberNum.ToString() + "/6";
        Label_Homeowners.text = string.Format(LanguageTextManager.GetString("IDS_H1_474"), roomSeatInfo.HomerName);
        if (m_HomerAccountConfigData == null)
        {
            //TraceUtil.Log("[RoomType]" + roomSeatInfo.byRoomType);
            m_HomerAccountConfigData = PlayerRoomAccoutConfigDataBase._dataTable.SingleOrDefault(p => p._roomTypeID == roomSeatInfo.byRoomType);
            UpdatePracticeInfo();
            
        }
        //妖女展示按钮
        Button_ShowSirenl.gameObject.SetActive(m_IsHomer);
        Image_ShowSiren.SetActive(m_IsHomer);
        //设置玩家站位和角度
        SetPlayersPosAndAngle();
        //设置摄像头
        SetCamera();
        //突破信息
        Button_Break.SetButtonText(CountBreakResult(4 * 60).ToString());
        //离开信息
        Button_Exit.SetButtonText(CountPracticeResult(12 * 60).ToString());
    }
    IEnumerator LateUpdateRoomSeatInfo()
    {
        yield return new WaitForEndOfFrame();
        ReceiveUpdateRoomSeatInfoHandle(null);
    }

    private void SetPlayersPosAndAngle()
    {
//		Int64 roomerUID = PlayerRoomManager.Instance.GetRoomSeatInfo().dwHomerUID;
//		GameObject roomerOBj = PlayerManager.Instance.FindPlayer(roomerUID);
//		if(roomerOBj!=null)
//		{
//			roomerOBj.transform.position = m_HomerAccountConfigData.PlayerPosList[0];
//		}

        var playerList = PlayerManager.Instance.PlayerList;        
        for (int i = 0; i < playerList.Count; i++)
        {
			var playerModel = playerList.FirstOrDefault(p=>			                          
				Mathf.Approximately(p.GO.transform.position.x,m_HomerAccountConfigData.PlayerPosList[i].x)
                  && Mathf.Approximately(p.GO.transform.position.z,m_HomerAccountConfigData.PlayerPosList[i].z));
			if(playerModel!=null)
			{
				Vector3 heroPos = playerModel.GO.transform.position ;
				heroPos.y = m_HomerAccountConfigData.PlayerPosList[i].y;
				playerModel.GO.transform.position = heroPos;
				playerModel.GO.transform.eulerAngles = m_HomerAccountConfigData.PlayerAngleList[i];
			}           
        }
    }

    void ReceiveUpdateRoomYaoNvHandle(INotifyArgs arg)
    {        
        SMsgEctypePracice_YaoNvUpdate_CSC yaoNvUpdate = (SMsgEctypePracice_YaoNvUpdate_CSC)arg;
        Panel_SirenControl.UpdateSirenModel(yaoNvUpdate.dwYaoNvList.ToArray());
    }

    void CoolingGetButtonHandle(object obj)
    {
        Int64 uid = PlayerManager.Instance.FindHeroDataModel().UID;
        ColdWorkInfo coldInfo = ColdWorkManager.Instance.GetColdWorkInfoClone(uid, ColdWorkClass.ECold_ClassID_MODEL, (uint)ColdWorkType.GetXiuLianColdWork);
        if (coldInfo != null)
        {
            CoolingGetButton((int)coldInfo.ColdTime / 1000);
        }
    }

    void ReceiveRoomChatMsgHandle(object obj)
    {
        SMsgChat_SC chat = (SMsgChat_SC)obj;
        Panel_Chat.CreateChat(chat);
    }

    void ReceiveCloseMainUIHandle(object arg)
    {
        CloseFriendPanelHandle();
    }

    void OnDestroy()
    {
        UIEventManager.Instance.RemoveUIEventHandel(UIEventType.NewColdWorkFromSever, CoolingGetButtonHandle);
        UIEventManager.Instance.RemoveUIEventHandel(UIEventType.RoomChatMsg, ReceiveRoomChatMsgHandle);
        UIEventManager.Instance.RemoveUIEventHandel(UIEventType.CloseMainUI, ReceiveCloseMainUIHandle);
        RemoveEventHandler(EventTypeEnum.XiuLianAccount.ToString(), ReceiveXiuLianAccountHandle);
        RemoveEventHandler(EventTypeEnum.UpdateRoomSeatInfo.ToString(), ReceiveUpdateRoomSeatInfoHandle);
        RemoveEventHandler(EventTypeEnum.UpdateRoomYaoNv.ToString(), ReceiveUpdateRoomYaoNvHandle);
        

        for (int i = 0; i < m_guideBtnID.Length; i++)
        {
            //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID[i]);
        }
    }
    protected override void RegisterEventHandler()
    {
        AddEventHandler(EventTypeEnum.XiuLianAccount.ToString(), ReceiveXiuLianAccountHandle);
        AddEventHandler(EventTypeEnum.UpdateRoomSeatInfo.ToString(), ReceiveUpdateRoomSeatInfoHandle);
        AddEventHandler(EventTypeEnum.UpdateRoomYaoNv.ToString(), ReceiveUpdateRoomYaoNvHandle);
        UIEventManager.Instance.RegisterUIEvent(UIEventType.NewColdWorkFromSever, CoolingGetButtonHandle);
        UIEventManager.Instance.RegisterUIEvent(UIEventType.RoomChatMsg, ReceiveRoomChatMsgHandle);
        UIEventManager.Instance.RegisterUIEvent(UIEventType.CloseMainUI, ReceiveCloseMainUIHandle);
    }
}
