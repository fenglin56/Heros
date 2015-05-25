using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI;
using Chat;

public class ChatRecordManager : ISingletonLifeCycle
{
    private static ChatRecordManager m_instance;
    public static ChatRecordManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new ChatRecordManager();
				//聊天记录不能清理 到了城镇会自动清掉
                //SingletonManager.Instance.Add(m_instance);
            }
            return m_instance;
        }
    }

    private List<SMsgChat_SC> mWorldRecordList = new List<SMsgChat_SC>();
    private Dictionary<int, List<SMsgChat_SC>> mPrivateRecordList = new Dictionary<int, List<SMsgChat_SC>>();

	//new
	private Dictionary<Chat.WindowType,List<SMsgChat_SC>> mPublicRecordDict = new Dictionary<Chat.WindowType, List<SMsgChat_SC>>();

    private int mMyActorID = 0;

    //public bool HasNewPrivateSmg { private set; get; }
    private bool mHasNewPrivateSmg = false;

	public DisplayType CurTownDisplayType //城镇当前公共窗口模式
	{
		get{return m_curTownDisplayType;}
		set{m_curTownDisplayType = value;}
	}
	private DisplayType m_curTownDisplayType;

    public void AddWorldRecord(SMsgChat_SC sChat)
    {
        if (mMyActorID == 0)
        {
            mMyActorID = PlayerManager.Instance.FindHeroDataModel().ActorID;
        }
                
        mWorldRecordList.AddChatRecord(sChat);
    }

	public void AddPublicRecord(Chat.WindowType type, SMsgChat_SC sChat)
	{
		if (mMyActorID == 0)
		{
			mMyActorID = PlayerManager.Instance.FindHeroDataModel().ActorID;
		}
		if(!mPublicRecordDict.ContainsKey(type))
		{
			mPublicRecordDict.Add(type,new List<SMsgChat_SC>());
		}
		mPublicRecordDict[type].AddChatRecord(sChat);
	}

    public void AddPrivateRecord(SMsgChat_SC sChat)
    {
        if(mMyActorID == 0)
        {
            mMyActorID = PlayerManager.Instance.FindHeroDataModel().ActorID;
        }

        if (GameManager.Instance.CurrentState != GameManager.GameState.GAME_STATE_TOWN)
        {
            mHasNewPrivateSmg = true;
        }

        if (sChat.senderActorID == mMyActorID)
        {
            if (mPrivateRecordList.ContainsKey(sChat.accepterActorID))
            {
                mPrivateRecordList[sChat.accepterActorID].AddChatRecord(sChat);
            }
            else
            {
                mPrivateRecordList.Add(sChat.accepterActorID, new List<SMsgChat_SC>());
                mPrivateRecordList[sChat.accepterActorID].AddChatRecord(sChat);
            }
        }
        else
        {
            if (mPrivateRecordList.ContainsKey(sChat.senderActorID))
            {
                mPrivateRecordList[sChat.senderActorID].AddChatRecord(sChat);
            }
            else
            {
                mPrivateRecordList.Add(sChat.senderActorID, new List<SMsgChat_SC>());
                mPrivateRecordList[sChat.senderActorID].AddChatRecord(sChat);
            }
        }
    }

    public bool IsHasNewPrivateSmg()
    {
        if (mHasNewPrivateSmg)
        {
            mHasNewPrivateSmg = false;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 获取所有私聊记录
    /// </summary>
    /// <returns></returns>
    public Dictionary<int, List<SMsgChat_SC>> GetPrivateChatRecordDict()
    {
		if(mMyActorID != PlayerManager.Instance.FindHeroDataModel().ActorID)
		{
			mPrivateRecordList.Clear();
			mMyActorID = PlayerManager.Instance.FindHeroDataModel().ActorID;
		}

        return mPrivateRecordList;
    }
	public List<SMsgChat_SC> GetPrivateChatRecordList(int chaterID)
	{
		if(mMyActorID != PlayerManager.Instance.FindHeroDataModel().ActorID)
		{
			mPrivateRecordList.Clear();
			mMyActorID = PlayerManager.Instance.FindHeroDataModel().ActorID;
		}
		if(!mPrivateRecordList.ContainsKey(chaterID))
		{
			mPrivateRecordList.Add(chaterID,new List<SMsgChat_SC>());
		}
		return mPrivateRecordList[chaterID];
	}
	/// <summary>
	/// 获取公共聊天记录
	/// </summary>
	/// <returns>The world chat record list.</returns>
	public List<SMsgChat_SC> GetPublicChatRecordList(Chat.WindowType type)
	{
		if(mMyActorID != PlayerManager.Instance.FindHeroDataModel().ActorID)
		{
			mPublicRecordDict.Clear();
			mMyActorID = PlayerManager.Instance.FindHeroDataModel().ActorID;
		}
		if(!mPublicRecordDict.ContainsKey(type))
		{
			mPublicRecordDict.Add(type,new List<SMsgChat_SC>());
		}
		return mPublicRecordDict[type];
	}

	/// <summary>
	/// 清除记录
	/// </summary>
	/// <param name="type">Type.</param>
	public void ClearPublicChatRecord(Chat.WindowType type)
	{
		if(mPublicRecordDict.ContainsKey(type))
		{
			mPublicRecordDict[type].Clear();
		}
	}

    public List<SMsgChat_SC> GetWorldChatRecordList()
    {
        return mWorldRecordList;
    }

	public string ParseColorChat(Chat.WindowType type, SMsgChat_SC sChat)
	{
		string str = "";
		switch(type)
		{
		case Chat.WindowType.Team:
		case Chat.WindowType.Town:
		case Chat.WindowType.Private:
		case Chat.WindowType.World:
		{
			if(sChat.senderActorID != mMyActorID)
			{
				str = NGUIColor.SetTxtColor(sChat.SenderName + " : ", TextColor.ChatBlue)
					+ NGUIColor.SetTxtColor(sChat.Chat,  TextColor.white);
			}
			else
			{
				str = NGUIColor.SetTxtColor(sChat.SenderName + " : ", TextColor.ChatGreen)
					+ NGUIColor.SetTxtColor(sChat.Chat,  TextColor.ChatYellow);
			}
		}
		break;
		case Chat.WindowType.System:
			str = NGUIColor.SetTxtColor(":" + sChat.Chat,  TextColor.ChatOrange);
			break;
		}

		return str;
	}

    public void Instantiate()
    {        
    }

    public void LifeOver()
    {
        m_instance = null;
    }
}

