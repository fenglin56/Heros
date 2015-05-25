using UnityEngine;
using System.Collections;

namespace UI.Login
{
    public class PlatformSingleServerItem : MonoBehaviour
    {

        //// Use this for initialization
        ////public UILabel ServerID;
        //public UILabel ServerName;
        ////public UILabel ServerIsbusy;
        //public SpriteSwith IsbusyIcon;
        ////public UILabel RoleNumber;

        //public UISprite SelectBackground;
        //public UISprite NormalBackground;

        //Server server;
        //PlatformServerListPanel myParent;

        //void Awake()
        //{
        //    SelectBackground.enabled = false;
        //    //SetSelectedStatus(false);
        //}

        //public void Show(Server server, PlatformServerListPanel myParent)
        //{
        //    this.gameObject.SetActive(true);
        //    this.myParent = myParent;
        //    this.server = server;
        //    //this.ServerID.SetText(server.No);
        //    this.ServerName.SetText(server.Name);
        //    //this.RoleNumber.SetText(server.ActorNumber);
        //    SetServerStatus();
        //}

        //public void Clear()
        //{
        //    this.server = null;
        //    //this.ServerID.SetText("");
        //    this.ServerName.SetText("");
        //    //this.ServerIsbusy.SetText("");
        //    //this.RoleNumber.SetText("");
        //    this.IsbusyIcon.ChangeSprite(0);
        //    SelectBackground.enabled = false;
        //}
        //private void SetSelectedStatus(bool isSelected)
        //{
        //    if (this.SelectBackground != null)
        //    {
        //        this.SelectBackground.enabled = isSelected;
        //    }
        //    if (this.NormalBackground != null)
        //    {
        //        this.NormalBackground.enabled = !isSelected;
        //    }
        //}
        //void SetServerStatus()
        //{
        //    string StatusStr = "";
        //    switch (server.Status)
        //    {
        //        case 0:
        //            StatusStr = LanguageTextManager.GetString("IDS_H1_228");
        //            break;
        //        case 1:
        //            StatusStr = LanguageTextManager.GetString("IDS_H1_227");
        //            break;
        //        case 2:
        //            StatusStr = LanguageTextManager.GetString("IDS_H1_226");
        //            break;
        //        case 3:
        //            StatusStr = LanguageTextManager.GetString("IDS_H1_225");
        //            break;
        //        default:
        //            break;
        //    }
        //    //this.ServerIsbusy.SetText(StatusStr);
        //    this.IsbusyIcon.ChangeSprite(server.Status + 1);
        //}

        //void OnClick()
        //{
        //    //SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
        //    PlatformLoginBehaviour.message+=" onClick server is null:" + (this.server == null);
        //    if (this.server != null)
        //    {
        //        myParent.OnSelectServer(this.server);
        //    }
        //}

        //public void OnSelectPanel(Server server)
        //{
        //    //TraceUtil.Log(this.name);
        //    //SetSelectedStatus(this.server == server);
        //    if (this.server == server)
        //    {
        //        SelectBackground.enabled = true;
        //    }
        //    else
        //    {
        //        SelectBackground.enabled = false;
        //    }
        //}

        //protected override void RegisterEventHandler()
        //{
        //}
    }
}
