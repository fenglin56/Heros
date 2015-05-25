using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace UI.MainUI
{
    [RequireComponent(typeof(BoxCollider),typeof(UIEventListener))]
    public class EmaiListItem : MonoBehaviour {
    	public Action<EmaiListItem> OnClickCallBack;
    	public Transform IconPoint;
        public UISprite BaseIcon;
    	public UISprite IsRead_spring;
    	public UILabel fromLable;
    	public UISprite FromSystem;
    	public UILabel TimeLable;
    	public UILabel TitleLable;
    	public SEmailSendUint _EamilItem;
        public UISprite SelectSpring;
        public long LeftSconds;
        private EmailEndTime mailEndTime;
        public SpriteSwith Background_spriteSwith;
    	void Awake()
    	{
    		GetComponent<UIEventListener>().onClick=OnItemClick;

    	}
    	
    	void OnItemClick(GameObject obj)
    	{
    		if(OnClickCallBack!=null)
    		{
    			OnClickCallBack(this);
    		}
    	}
    	
    	public  void BeSelected()
    	{
    		OnClickCallBack(this);
    	}
    	public  void OnGetFocus() 
    	{
            SelectSpring.gameObject.SetActive(true);
    	}
    	
    	public  void OnLoseFocus() 
    	{
            SelectSpring.gameObject.SetActive(false);	
    	}
       
        void RefreshTime()
        {

            float temptime =mailEndTime.ExpireTime-(Time.realtimeSinceStartup - mailEndTime.UpdateTime);
            if(temptime<=0)
            {
                List<long> list=new List<long>();
                list.Add(_EamilItem.llMailID);
                EmailDataManager.Instance.DeleteEmailFromLocalList(list);
                UI.MainUI.EmailInfoPanelManager.GetInstance().SC_EmailTabManager.Sc_EmailContainerList.StartRefreshList(false);

                TimeLable.text=string.Format(LanguageTextManager.GetString("IDS_I22_5"),NGUIColor.SetTxtColor("0",TextColor.ChatYellow));
            }
            else
            {
            int leftDay =((int)temptime) / 60 / 60 / 24;
            int leftHour = ((int)temptime )/ 60 / 60 % 24;
            int leftminute =((int) temptime )/60% 60;
            //long m_leftSconds = LeftSconds % 60;
            if(leftDay>0)
            {
                TimeLable.text=string.Format(LanguageTextManager.GetString("IDS_I22_4"),NGUIColor.SetTxtColor( leftDay.ToString(),TextColor.ChatYellow));

            }else if(leftHour>0)
            {
                TimeLable.text=string.Format(LanguageTextManager.GetString("IDS_I22_5"),NGUIColor.SetTxtColor( leftHour.ToString(),TextColor.ChatYellow));
            }
            else if(leftminute>0)
            {
                TimeLable.text=string.Format(LanguageTextManager.GetString("IDS_I22_6"),NGUIColor.SetTxtColor( leftminute.ToString(),TextColor.ChatYellow));
            }
            }
          
        }

        public void RefreshItemLocal()
        {
            _EamilItem=EmailDataManager.Instance.GetEamilFromLocal(_EamilItem.llMailID);
            RefreshItem();
        }
        public void RefreshItem()
        {
            Background_spriteSwith.ChangeSprite(5);
            IsRead_spring.gameObject.SetActive(!Convert.ToBoolean(_EamilItem.byState));
            IconPoint.ClearChild();
            fromLable.SetText(Encoding.UTF8.GetString(_EamilItem.szSendActorName));
            TitleLable.SetText(Encoding.UTF8.GetString(_EamilItem.szTitle));
            if(Convert.ToBoolean(_EamilItem.byIsSystem))
            {
                fromLable.gameObject.SetActive(false);
                FromSystem.gameObject.SetActive(true);
                var maildata=EmailInfoPanelManager.GetInstance().systemMailConfigDataBase.SystemMailConfigDataList.First(p=>p.MailType==(int)_EamilItem.wEmailType);
                TitleLable.SetText(LanguageTextManager.GetString(maildata.MailTitle));
               
            }
            else
            {
                fromLable.gameObject.SetActive(true);
                FromSystem.gameObject.SetActive(false);

            }
            if(Convert.ToInt32(_EamilItem.byGoodsType)==(int)emEMAIL_EXTRA_TYPE.EMAIL_NONE_EXTRA_TYPE)
            {

                BaseIcon.gameObject.SetActive(true);
            }
            else
            {
                BaseIcon.gameObject.SetActive(false);
                ItemData data=ItemDataManager.Instance.GetItemData(System.Convert.ToInt32( _EamilItem.dwGoodsID)) ;
                if(data!=null)
                {
                Background_spriteSwith.ChangeSprite(data._ColorLevel+1);
                NGUITools.AddChild(IconPoint.gameObject,data._picPrefab);
                }
            }

        }
    	public void InitItemData(SEmailSendUint EmailItem)
    	{
    		_EamilItem=EmailItem;
            mailEndTime= EmailDataManager.Instance.GetEmailEndTime(EmailItem.llMailID);
            CancelInvoke("RefreshTime");
            if(mailEndTime!=null)
            {
//                if(mailEndTime.UpdateTime!=0)
//                {
//                    float temp=Time.realtimeSinceStartup-mailEndTime.UpdateTime;
//                    if((long)temp>=mailEndTime.ExpireTime)
//                    {
//                        mailEndTime.ExpireTime=0;
//                    }
//                    else
//                    {
//                        mailEndTime.ExpireTime=(long)(mailEndTime.ExpireTime-temp);
//                    }
//                }

                InvokeRepeating("RefreshTime",0,1);
            }

            RefreshItem();
    	}
        public void CancelInvoke()
        {
            CancelInvoke("RefreshTime");
        }
        void OnDisable()
        {
            CancelInvoke();
        }

   }

}
