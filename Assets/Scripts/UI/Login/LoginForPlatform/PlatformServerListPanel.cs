using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI.MainUI;

namespace UI.Login
{
    public class PlatformServerListPanel : MonoBehaviour
    {

    //public PlatformSingleServerItem[] singleServerList;

    //private List<Server> m_serverInfo = new List<Server>();

    //private ServerDitionary serverDitionary = new ServerDitionary();

    //private int PageNumber = 1;

    ////public UILabel Title;
    ////public UILabel ServerID;
    ////public UILabel ServerName;
    ////public UILabel ServerIsBusy;
    ////public UILabel ServerRoleNumber;
    //public SingleButtonCallBack JoinGameBtn;
    //public SingleButtonCallBack LastPageBtn;
    //public SingleButtonCallBack NextPageBtn;
    //public SingleCommonUIBottomButton GoBackButton;
    //public SingleButtonCallBack ServerItem;

    //private Server SelectSever;

    //void Awake()
    //{
    //    JoinGameBtn.SetCallBackFuntion(OnJoinGameBtnClick);
    //    LastPageBtn.SetCallBackFuntion(OnLastPageBtnClick);
    //    NextPageBtn.SetCallBackFuntion(OnNextPageBtnClick);

    //    CommonBtnInfo commonBtnInfo = new CommonBtnInfo(OnBackButtonTapped);  //暂时没有返回回调
    //    GoBackButton.InitButton(commonBtnInfo);

    //    //JoinGameBtn.SetButtonText(LanguageTextManager.GetString("IDS_H2_21"));
    //    //LastPageBtn.SetButtonText(LanguageTextManager.GetString("IDS_H1_223"));
    //    //NextPageBtn.SetButtonText(LanguageTextManager.GetString("IDS_H1_224"));
    //    //if(Title!=null)Title.SetText(LanguageTextManager.GetString("IDS_H1_306"));
    //    //ServerID.SetText(LanguageTextManager.GetString("IDS_H1_219"));
    //    //ServerName.SetText(LanguageTextManager.GetString("IDS_H1_220"));
    //    //ServerIsBusy.SetText(LanguageTextManager.GetString("IDS_H1_221"));
    //    //ServerRoleNumber.SetText(LanguageTextManager.GetString("IDS_H1_222"));

    //    AddEventHandler(EventTypeEnum.ReceiveDefaultErrorCode.ToString(), ErrorHandle);
    //}

    //private void ErrorHandle(INotifyArgs e)
    //{
    //    ServerError serverError = (ServerError)e;
    //    switch (serverError.ErrorCode)
    //    {
    //        case SystemErrorCodeDefine.ERROR_CODE_INVALIDUSER://无此用户
    //            UI.MessageBox.Instance.Show(3, "", "无此用户", LanguageTextManager.GetString("IDS_H2_13"), OnLoginFaildMessageBox);
    //            break;
    //        case SystemErrorCodeDefine.ERROR_CODE_PLATFORM_KEYVALUE://平台Key或者SID错误
    //            UI.MessageBox.Instance.Show(3, "", "平台Key或者SID错误", LanguageTextManager.GetString("IDS_H2_13"), OnLoginFaildMessageBox);
    //            break;
    //        case SystemErrorCodeDefine.ERROR_CODE_KEYERROR://密码错误
    //            UI.MessageBox.Instance.Show(3, "", "密码错误", LanguageTextManager.GetString("IDS_H2_13"), OnLoginFaildMessageBox);
    //            break;
    //        case SystemErrorCodeDefine.ERROR_CODE_PADLOCK://帐号被禁
    //            UI.MessageBox.Instance.Show(3, "", "帐号被禁", LanguageTextManager.GetString("IDS_H2_13"), OnLoginFaildMessageBox);
    //            break;
    //        case SystemErrorCodeDefine.ERROR_CODE_USER_REGISTER://注册帐号失败
    //            UI.MessageBox.Instance.Show(3, "", "注册帐号失败", LanguageTextManager.GetString("IDS_H2_13"), OnLoginFaildMessageBox);
    //            break;
    //    }
    //}
    //void OnLoginFaildMessageBox()
    //{
    //    GameManager.Instance.QuitToLogin();
    //    LoginPlatformManager.Instance.ReLoginPlatform();
        
    //    //UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowLodingUI, UI.Login.LoginUIType.Login);
    //}
    //public override void Show()
    //{
    //    InitPanel();
    //    transform.localPosition = Vector3.zero;
    //    if (null != this.SelectSever)
    //    {
    //        OnSelectServer(this.SelectSever);
    //    }
    //}

    //public override void Close()
    //{
    //    transform.localPosition = new Vector3(0, 0, -1000);
    //}

    //public override void DestroyPanel()
    //{
    //    RemoveAllEvent();
    //}
    //void OnDestroy()
    //{
    //    DestroyPanel();
    //}
    //void InitPanel()
    //{
    //    //m_serverInfo.Clear();
    //    //foreach (var child in PlatformLoginBehaviour.Instance.ServerInfo)
    //    //{
    //    //    m_serverInfo.Add(child);
    //    //}
    //    //m_serverInfo.Sort(delegate(Server a, Server b) { return (a.No).CompareTo(b.No); });
    //    //m_serverInfo.Reverse();

    //    //serverDitionary.PageServerList.Clear();
    //    //for (int i = 0; i < m_serverInfo.Count; i++)
    //    //{
    //    //    serverDitionary.AddSeverInfo(m_serverInfo[i]);
    //    //}
    //    //ShowServerList();
    //    //CheckButton();
    //}

    //void ShowServerList()
    //{
    //    foreach (var child in singleServerList)
    //    {
    //        child.gameObject.SetActive(false);
    //    }
    //    List<Server> PageServerInfo = null;
    //    if (this.serverDitionary.PageServerList.TryGetValue(PageNumber, out PageServerInfo))
    //    {
    //        for (int i = 0; i < PageServerInfo.Count; i++)
    //        {
    //            singleServerList[i].Show(PageServerInfo[i], this);
    //        }
    //    }
    //}

    //public void OnSelectServer(Server server)
    //{
    //    this.SelectSever = server;
    //    foreach (var child in singleServerList)
    //    {
    //        child.OnSelectPanel(server);
    //    }
    //    OnJoinGameBtnClick(null);
    //}

    //void OnNextPageBtnClick(object obj)
    //{
    //    if (PageNumber < serverDitionary.PageServerList.Count)
    //    {
    //        SoundManager.Instance.PlaySoundEffect("Sound_Button_Page");
    //        PageNumber++;
    //        ShowServerList();
    //    }
    //    CheckButton();
    //}

    //void OnLastPageBtnClick(object obj)
    //{
    //    if (PageNumber > 1)
    //    {
    //        SoundManager.Instance.PlaySoundEffect("Sound_Button_Page");
    //        PageNumber--;
    //        ShowServerList();
    //    }
    //    CheckButton();
    //}

    //void CheckButton()
    //{
    //    SetButtonActive(LastPageBtn, PageNumber < 2 ? false : true);
    //    SetButtonActive(NextPageBtn, PageNumber >= serverDitionary.PageServerList.Count ? false : true);
    //}

    //void SetButtonActive(SingleButtonCallBack button, bool Flag)
    //{
    //    button.SetImageButtonComponentActive(Flag);
    //    button.SetButtonBackground(Flag ? 1 : 2);
    //    button.collider.enabled = Flag;
    //}

    ////在服务器列表中选择服务器后。点击进入游戏的时候。
    //void OnJoinGameBtnClick(object obj)
    //{
    //    SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
    //    LoginManager.Instance.LoginGameServer(this.SelectSever);
    //}

    //void OnBackButtonTapped(object obj)
    //{
    //    UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowLodingUI, UI.Login.LoginUIType.Login);
    //}

    //public class ServerDitionary
    //{
    //    public Dictionary<int, List<Server>> PageServerList;

    //    public ServerDitionary()
    //    {
    //        PageServerList = new Dictionary<int, List<Server>>();
    //    }

    //    public void AddSeverInfo(Server server)
    //    {
    //        for (int i = 1; i < 100; i++)
    //        {
    //            List<Server> child = null;
    //            if (PageServerList.TryGetValue(i, out child))
    //            {
    //                if (child.Count < 9)
    //                {
    //                    child.Add(server);
    //                    return;
    //                }
    //            }
    //            else
    //            {
    //                child = new List<Server>();
    //                child.Add(server);
    //                PageServerList.Add(i, child);
    //                return;
    //            }
    //        }
    //    }

    //}
}

}
