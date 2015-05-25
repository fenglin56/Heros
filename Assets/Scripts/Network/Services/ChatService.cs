using UnityEngine;
using System.Collections;
using System.Linq;

public class ChatService : Service
{
    public override void SaveResponseHandleToInvoke(byte[] dataBuffer, int socketId, byte cmdtype)
    {
        MasterMsgType masterMsgType = (MasterMsgType)cmdtype;
        Package package = PackageHelper.ParseReceiveData(dataBuffer);
        //TraceUtil.Log("收到聊天主消息" + masterMsgType + "收到子消息" + package.GetSubMsgType());
        switch (masterMsgType)
        {
            case MasterMsgType.NET_ROOT_CHAT:
                switch (package.GetSubMsgType())
                {
					case ChatDefineManager.MSG_CHAT_CURRENT:
						this.AddToInvoker(ReceiveTownMsgHandle, package.Data, socketId);
						break;
					case ChatDefineManager.MSG_CHAT_SYSTEM:
						this.AddToInvoker(ReceiveSystemMsgHandle, package.Data, socketId);
						break;
                    case ChatDefineManager.MSG_CHAT_WORLD:
                        this.AddToInvoker(ReceiveWorldMsgHandle, package.Data, socketId);
                        break;
					case ChatDefineManager.MSG_CHAT_TEAM:
						this.AddToInvoker(ReceiveTeamMsgHandle, package.Data, socketId);
						break;
                    case ChatDefineManager.MSG_CHAT_PRIVATE:
                        this.AddToInvoker(ReceivePrivateMsgHandle, package.Data, socketId);
                        break;
                    case ChatDefineManager.MSG_CHAT_SPACE:
                        this.AddToInvoker(ReceiveSpaceMsgHandle, package.Data, socketId);
                        break;
					case ChatDefineManager.MSG_CHAT_GETITEMTIP:
						this.AddToInvoker(ReceiveGetItemtipHandle, package.Data, socketId);
						break;
                }
                break;            
            default:
                //TraceUtil.Log("不能识别的主消息:" + package.GetMasterMsgType());
                break;
        }
    }
	//城镇频道
	CommandCallbackType ReceiveTownMsgHandle(byte[] dataBuffer, int socketID)
	{
		SMsgChat_SC sMsgChat_SC = SMsgChat_SC.ParsePackage(dataBuffer);
		sMsgChat_SC.L_LabelChat = ChatRecordManager.Instance.ParseColorChat(Chat.WindowType.Town,sMsgChat_SC);
		sMsgChat_SC.L_Channel = (int)Chat.WindowType.Town;
		ChatRecordManager.Instance.AddPublicRecord( Chat.WindowType.Town,sMsgChat_SC);
		UIEventManager.Instance.TriggerUIEvent(UIEventType.WorldChatMsg, sMsgChat_SC);
		return CommandCallbackType.Continue;
	}
	//世界频道
    CommandCallbackType ReceiveWorldMsgHandle(byte[] dataBuffer, int socketID)
    {
        SMsgChat_SC sMsgChat_SC = SMsgChat_SC.ParsePackage(dataBuffer);
		sMsgChat_SC.L_LabelChat = ChatRecordManager.Instance.ParseColorChat(Chat.WindowType.World,sMsgChat_SC);
		sMsgChat_SC.L_Channel = (int)Chat.WindowType.World;
		if(GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_TOWN)
		{
			UI.GoodsMessageManager.Instance.AddNoticeMessage(sMsgChat_SC);
		}
		ChatRecordManager.Instance.AddPublicRecord( Chat.WindowType.World, sMsgChat_SC);
        UIEventManager.Instance.TriggerUIEvent(UIEventType.WorldChatMsg, sMsgChat_SC);
        return CommandCallbackType.Continue;
    }

	//队伍频道
	CommandCallbackType ReceiveTeamMsgHandle(byte[] dataBuffer, int socketID)
	{
		SMsgChat_SC sMsgChat_SC = SMsgChat_SC.ParsePackage(dataBuffer);
		sMsgChat_SC.L_LabelChat = ChatRecordManager.Instance.ParseColorChat(Chat.WindowType.Team,sMsgChat_SC);
		sMsgChat_SC.L_Channel = (int)Chat.WindowType.Team;
		ChatRecordManager.Instance.AddPublicRecord( Chat.WindowType.Team,sMsgChat_SC);
		UIEventManager.Instance.TriggerUIEvent(UIEventType.WorldChatMsg, sMsgChat_SC);
		return CommandCallbackType.Continue;
	}

	//私人频道
    CommandCallbackType ReceivePrivateMsgHandle(byte[] dataBuffer, int socketID)
    {
        SMsgChat_SC sMsgChat_SC = SMsgChat_SC.ParsePackage(dataBuffer);
		sMsgChat_SC.L_LabelChat = ChatRecordManager.Instance.ParseColorChat(Chat.WindowType.Private,sMsgChat_SC);
		sMsgChat_SC.L_Channel = (int)Chat.WindowType.Private;
		sMsgChat_SC.L_ChaterID = PlayerManager.Instance.FindHeroDataModel().ActorID == sMsgChat_SC.senderActorID ? sMsgChat_SC.accepterActorID : sMsgChat_SC.senderActorID ;
        ChatRecordManager.Instance.AddPrivateRecord(sMsgChat_SC);
		UIEventManager.Instance.TriggerUIEvent(UIEventType.PrivateChatMsg, sMsgChat_SC);
        return CommandCallbackType.Continue;
    }

	//系统频道
	CommandCallbackType ReceiveSystemMsgHandle(byte[] dataBuffer, int socketID)
	{
		switch((Chat.CHAT_MSG_SYSTYPE)dataBuffer[0])
		{
		case Chat.CHAT_MSG_SYSTYPE.NONE_SYS_CHAT_TYPE:	
		case Chat.CHAT_MSG_SYSTYPE.WARING_SYS_CHAT_TYPE:
			SMsgSysChat_SC sMsgSysChat_SC = SMsgSysChat_SC.ParsePackage(dataBuffer);
			SMsgChat_SC sMsgChat_SC = new SMsgChat_SC();
			sMsgChat_SC.Chat = sMsgSysChat_SC.Chat;
			sMsgChat_SC.L_LabelChat = ChatRecordManager.Instance.ParseColorChat(Chat.WindowType.System,sMsgChat_SC);
			sMsgChat_SC.L_Channel = (int)Chat.WindowType.System;
			if(GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_TOWN)
			{
				SMsgChat_SC bChat = sMsgChat_SC;
				bChat.L_LabelChat = bChat.L_LabelChat.Remove(8,1);
				UI.GoodsMessageManager.Instance.AddNoticeMessage(bChat);
			}
			ChatRecordManager.Instance.AddPublicRecord( Chat.WindowType.System, sMsgChat_SC);
			UIEventManager.Instance.TriggerUIEvent(UIEventType.WorldChatMsg, sMsgChat_SC);	
			break;		
		case Chat.CHAT_MSG_SYSTYPE.BROADCAST_SYS_CHAT_TYPE:
			SMsgBroadCastSysTips_SC sMsgBroadCastSysTips_SC = SMsgBroadCastSysTips_SC.ParsePackage(dataBuffer);
			SMsgChat_SC broadcastChat = new SMsgChat_SC();
			if(TownRobotManager.Instance == null)
			{
				return CommandCallbackType.Continue;
			}
			BroadcastConfigData configData = TownRobotManager.Instance.GetBroadcastConfig(sMsgBroadCastSysTips_SC.dwSysTipsID);
			string parmStr = "";
			switch(configData.BroadcastType)
			{
			case 1:
				var sirenData = SirenDataManager.Instance.GetPlayerSirenList().SingleOrDefault(p=>p._sirenID == configData.BroadcastConditions);
				parmStr = LanguageTextManager.GetString(sirenData._name);
				break;
			case 2:
				var itemData = ItemDataManager.Instance.GetItemData(configData.BroadcastConditions);
				parmStr = UI.NGUIColor.SetTxtColor(LanguageTextManager.GetString(itemData._szGoodsName),(UI.TextColor)itemData._ColorLevel);
				break;
			case 3:
				var ectypeData = EctypeConfigManager.Instance.EctypeContainerConfigList[configData.BroadcastConditions];
				parmStr = LanguageTextManager.GetString(ectypeData.lEctypeName);
				break;
			case 4:
				var titleData = PlayerDataManager.Instance.GetPlayerTitleConfigData(configData.BroadcastConditions);
				//parmStr = LanguageTextManager.GetString(titleData._TitleName);
				break;
			case 5:
				var taskData = GuideConfigManager.Instance.TaskNewConfigFile.Datas.SingleOrDefault(p=>p.TaskID == configData.BroadcastConditions);
				parmStr = LanguageTextManager.GetString(taskData.TaskTitle);
				break;			
			}
			broadcastChat.Chat = string.Format(LanguageTextManager.GetString(configData.BroadcastContent),sMsgBroadCastSysTips_SC.SZActorName,parmStr);
			broadcastChat.L_LabelChat = ChatRecordManager.Instance.ParseColorChat(Chat.WindowType.System,broadcastChat);
			broadcastChat.L_Channel = (int)Chat.WindowType.System;
			if(GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_TOWN)
			{
				SMsgChat_SC bChat = broadcastChat;
				bChat.L_LabelChat = bChat.L_LabelChat.Remove (8,1);
				UI.GoodsMessageManager.Instance.AddNoticeMessage(bChat);
			}
			ChatRecordManager.Instance.AddPublicRecord( Chat.WindowType.System, broadcastChat);
			UIEventManager.Instance.TriggerUIEvent(UIEventType.WorldChatMsg, broadcastChat);
			break;
		}

		return CommandCallbackType.Continue;
	}

    CommandCallbackType ReceiveSpaceMsgHandle(byte[] dataBuffer, int socketID)
    {
        SMsgChat_SC sMsgChat_SC = SMsgChat_SC.ParsePackage(dataBuffer);
		sMsgChat_SC.L_LabelChat = ChatRecordManager.Instance.ParseColorChat(Chat.WindowType.Town,sMsgChat_SC);
		sMsgChat_SC.L_Channel = (int)Chat.WindowType.Town;
		if(sMsgChat_SC.senderActorID == PlayerManager.Instance.FindHeroDataModel().ActorID && sMsgChat_SC.bChatType == 1)
		{
			return CommandCallbackType.Continue;
		}
		ChatRecordManager.Instance.AddPublicRecord( Chat.WindowType.Town,sMsgChat_SC);
        UIEventManager.Instance.TriggerUIEvent(UIEventType.WorldChatMsg, sMsgChat_SC);
        return CommandCallbackType.Continue;
    }

	CommandCallbackType ReceiveGetItemtipHandle(byte[] dataBuffer,int socketID)
	{
		SMsgChat_SC sMsgChat_SC = SMsgChat_SC.ParsePackage(dataBuffer);
		SMsgTipChat_SC sMsgTipChat_SC = SMsgTipChat_SC.ParsePackage(sMsgChat_SC.tipsMessage);

		if(sMsgTipChat_SC.byTipLevel == (byte)SMsgTipChat_SC.CHAT_TIPS_LEVEL.CHAT_TIPS_LEVEL3)
		{
			if(UI.GoodsMessageManager.Instance != null)
			{
				for(int i=0;i<sMsgTipChat_SC.byNumber;i++)
				{
					UI.GoodsMessageManager.Instance.Show(sMsgTipChat_SC.ShowGoodsInfos[i].dwGoodsID,
					                                     sMsgTipChat_SC.ShowGoodsInfos[i].dwGoodsNum);
				}

			}
		}

		return CommandCallbackType.Continue;
	}


    #region 上发聊天信息
	/// <summary>
	/// 公共信息
	/// </summary>
	/// <param name="actorID">Actor I.</param>
	/// <param name="accepterActorID">Accepter actor I.</param>
	/// <param name="chat">Chat.</param>
	/// <param name="chatType">Chat type.</param>
	/// <param name="define">Define.</param>
    public void SendChat(uint actorID, uint accepterActorID, string chat, byte chatType, Chat.ChatDefine define)
    {
		//统一此处过滤敏感字符
		chat = CommonDefineManager.Instance.IllegalCharacterConfig.ReplaceCharacter(chat);
        Package package = SMsgChat_CS.GeneratePackage(actorID, accepterActorID, "", chat, chatType, define);
        this.Request(package);
    }

	/// <summary>
	/// 私聊信息
	/// </summary>
	/// <param name="actorID">Actor I.</param>
	/// <param name="accepterActorID">Accepter actor I.</param>
	/// <param name="accepterName">Accepter name.</param>
	/// <param name="chat">Chat.</param>
	/// <param name="chatType">Chat type.</param>
	/// <param name="define">Define.</param>
	public void SendChat(uint actorID, uint accepterActorID,string accepterName, string chat, byte chatType, Chat.ChatDefine define)
	{
		//统一此处过滤敏感字符
		chat = CommonDefineManager.Instance.IllegalCharacterConfig.ReplaceCharacter(chat);
		Package package = SMsgChat_CS.GeneratePackage(actorID, accepterActorID,accepterName, chat, chatType, define);
		this.Request(package);
	}

	public IEnumerator RequestPropagandaList(ButtonCallBack callBack)
	{
		WWW www = new WWW("http://jh.fanhougame.net/gm/?from=front&service=GameManagerService&action=getAutoTalkInfo");
		yield return www;		
		if (www.isDone && string.IsNullOrEmpty(www.error))
		{
			PhpAutoPropaganda phpPropaganda = JsonConvertor<PhpAutoPropaganda>.Json2Object(www.text);
			callBack(phpPropaganda);
		}
	}


    #endregion


}
