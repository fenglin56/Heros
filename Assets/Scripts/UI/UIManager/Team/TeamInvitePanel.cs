using UnityEngine;
using System.Collections;
using UI.Friend;
using System.Linq;
using System.Collections.Generic;

public class TeamInvitePanel : MonoBehaviour {

    //public UIGrid UIGridTrans;
    public ItemPagerManager ItemPagerManager_Friend;
    public LocalButtonCallBack ClosePanelButton;
    public TeamFriendInfoItem TeamFriendInfoItemPrefab;

    private Transform m_thisTransform;

    private List<TeamFriendInfoItem> m_friendItemList = new List<TeamFriendInfoItem>();

    void Start()
    {
        ClosePanelButton.SetCallBackFuntion(OnClosePanelButtonClick, null);
        m_thisTransform = this.transform;

        m_thisTransform.gameObject.SetActive(false);
        ItemPagerManager_Friend.OnPageChanged += ItemPageChangedHandle;
    }
    
    public void ShowFriendList()
    {
        //避免下发消息先到问题
        if (m_thisTransform == null)
            return;
       
        if (!m_thisTransform.gameObject.activeInHierarchy)
        {
            m_thisTransform.gameObject.SetActive(true);
        }

        var teamMemberData = TeamManager.Instance.MyTeamProp.TeamMemberNum_SC.SMsgTeamPropMembers.ToList();
        List<PanelElementDataModel> friendList =  new List<PanelElementDataModel>();
        
        List<PanelElementDataModel> onlineFreindList = new List<PanelElementDataModel>();
        var friendQueue = FriendDataManager.Instance.GetFriendListData.Where(p => ((PanelElementDataModel)p).sMsgRecvAnswerFriends_SC.bOnLine == 1).ToList();
        for (int i = 0; i < friendQueue.Count; i++)
        {
            onlineFreindList.Add((PanelElementDataModel)friendQueue[i]);
        }
        onlineFreindList.AddRange(FriendDataManager.Instance.GetOnlineFriendList);        
        onlineFreindList.ApplyAllItem(p =>
            {
                if (!teamMemberData.Exists(k => k.TeamMemberContext.dwActorID == p.sMsgRecvAnswerFriends_SC.dwFriendID))
                {
                    friendList.Add(p);
                }
            });

        m_friendItemList.ApplyAllItem(p =>
            {                
                Destroy(p.gameObject);
            });
        m_friendItemList.Clear();

        for (int i = 0; i < friendList.Count; i++)
        {
            AddTeamFriendItem();
            m_friendItemList[i].UpdateInfo(friendList[i]);
        }
        
        /*
        if (friendList.Count > m_friendItemList.Count)
        {
            int addNum = friendList.Count - m_friendItemList.Count;
            for (int i = 0; i < addNum; i++)
            {
                AddTeamFriendItem();
            }
            int num = 0;
            m_friendItemList.ApplyAllItem(p =>
                {
                    p.UpdateInfo(friendList[num]);
                    num++;
                });
        }
        else
        {
            int num = 0;
            int friendDataLength = friendList.Count;
            m_friendItemList.ApplyAllItem(p =>
                {
                    if (friendDataLength > 0)
                    {
                        p.UpdateInfo(friendList[num]);
                    }
                    else
                    {
                        p.Close();
                    }
                    friendDataLength--;
                    num++;
                });
        }
        */
        if (m_friendItemList.Count == 0)
        {
            ItemPagerManager_Friend.InitPager(1, 1, 0);
        }
        else
        {
            ItemPagerManager_Friend.InitPager(m_friendItemList.Count, 1, 0);
        }
        

        //UIGridTrans.repositionNow = true;   //排列
    }

    private void AddTeamFriendItem()
    {
        TeamFriendInfoItem item = ((GameObject)Instantiate(TeamFriendInfoItemPrefab.gameObject)).GetComponent<TeamFriendInfoItem>();
        item.transform.parent = ItemPagerManager_Friend.transform;
        item.transform.localScale = Vector3.one;
        item.transform.localPosition -= Vector3.forward*12; //\

        m_friendItemList.Add(item);
    }

    void OnClosePanelButtonClick(object obj)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
        m_thisTransform.gameObject.SetActive(false);
    }

    void ItemPageChangedHandle(PageChangedEventArg pageSmg)
    {
        m_friendItemList.ApplyAllItem(p =>
            {
                p.gameObject.SetActive(false);
            });
        int size = ItemPagerManager_Friend.PagerSize;
        var showFriendArray = m_friendItemList.Skip((pageSmg.StartPage - 1) * size).Take(size).ToArray();
        showFriendArray.ApplyAllItem(p =>
            {
                p.gameObject.SetActive(true);
            });
        ItemPagerManager_Friend.UpdateItems(showFriendArray, "friendList");
    }
	
}
