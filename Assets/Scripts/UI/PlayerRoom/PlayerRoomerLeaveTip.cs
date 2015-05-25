using UnityEngine;
using System.Collections;
namespace UI.MainUI
{
    public class PlayerRoomerLeaveTip : MonoBehaviour
    {        
        void Start()
        {
            if (GameManager.Instance.IsPlayerRoomerLeave)
            {
                //MessageBox.Instance.Show(4, "", LanguageTextManager.GetString("IDS_H1_498"), LanguageTextManager.GetString("IDS_H2_55"), null);
				var info = PlayerRoomManager.Instance.GetXiuLianInfo();
				string smg = LanguageTextManager.GetString("IDS_H1_498")+string.Format(LanguageTextManager.GetString("IDS_H1_499"),info.XiuLianNum.ToString());
				MessageBox.Instance.Show(4,"",smg,LanguageTextManager.GetString("IDS_H2_55"), null);
				GameManager.Instance.IsPlayerRoomerLeave = false;
            }
        }

    }
}