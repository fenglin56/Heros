
using UnityEngine;
using System.Collections;

public class ResetContainerButton : LocalButtonCallBack {

    //float CoolingTime = 10;//冷却时间
    //bool Enable = true;

    //float WaitTime = 0;
    public GameObject EffectPrefab;
    public Transform EffectPoint;

    public UIFilledSprite ProgressBar;//冷却进度条

    public SpriteSwith spritSwith;

    private uint m_guideBtnID;
    private bool IsColding=true;
    void Awake()
    {
        ////TODO GuideBtnManager.Instance.RegGuideButton(this.gameObject, UI.MainUI.UIType.PackInfo, SubUIType.NoSubType, out m_guideBtnID);
    }

    void Start()
    {
        RegisterEventHandler();
       
        UIEventManager.Instance.RegisterUIEvent(UIEventType.AddColdWork,AddColdWork);
    }

   public void OnShow()
    {
        //SetButtonActive(IsColding);
    }
    void OnDestroy()
    {
        ////TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID);
        //RemoveEventHandler(EventTypeEnum.ColdWork.ToString(), ColdMyself);
        UIEventManager.Instance.RemoveUIEventHandel(UIEventType.AddColdWork, AddColdWork);
    }


    void GetColdWork()
    {
        long targetUID = PlayerManager.Instance.FindHeroDataModel().UID;
        ColdWorkInfo myColdWork = ColdWorkManager.Instance.GetColdWorkInfo(targetUID, ColdWorkClass.ECold_ClassID_MODEL, (uint)ColdWorkType.ContainerColdWork);
        if (myColdWork != null)
        {
            SetButtonActive(false);
            TweenFloat.Begin(myColdWork.ColdTime / 1000, 1, 0, SetRecoverProgressBar);
            EffectPoint.ClearChild();
            EffectPoint.InstantiateNGUIObj(EffectPrefab);
        }
    }

    void AddColdWork(object obj)
    {
        ColdWorkInfo myColdWork = (ColdWorkInfo)obj;
        long targetUID = PlayerManager.Instance.FindHeroDataModel().UID;
        if (myColdWork.lMasterID == targetUID && myColdWork.ColdClass == ColdWorkClass.ECold_ClassID_MODEL && myColdWork.ColdID == (uint)ColdWorkType.ContainerColdWork)
        {
            SetButtonActive(false);
            TweenFloat.Begin(myColdWork.ColdTime/1000, 1, 0, SetRecoverProgressBar);
            EffectPoint.ClearChild();
            EffectPoint.InstantiateNGUIObj(EffectPrefab);
        }
    }

    //void ColdMyself(INotifyArgs inotifyArgs)
    //{
    //    SmsgActionColdWork smsgActionColdWork = (SmsgActionColdWork)inotifyArgs;
    //    if (smsgActionColdWork.sMsgActionColdWorkHead_SC.lMasterID == PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity)
    //    {
    //        foreach (SMsgActionColdWork_SC child in smsgActionColdWork.sMsgActionColdWork_SCs)
    //        {
    //            if (child.byClassID == 1&&child.dwColdID == 500001)//物品冷却
    //            {
    //                TweenFloat.Begin(child.dwColdTime/1000,1,0,SetRecoverProgressBar);
    //            }
    //        }
    //    }
    //}

    public override void SetButtonActive(bool Flag)
    {
        if (GetComponent<BoxCollider>())
            GetComponent<BoxCollider>().enabled = Flag;
        if (GetComponent<UIImageButton>())
            GetComponent<UIImageButton>().enabled = Flag;
        if(Flag)
        {
            spritSwith.ChangeSprite(1);
		//	EffectPoint.ClearChild();
        }
        else
		{
			spritSwith.ChangeSprite(2);
			
        }
	}

    void SetRecoverProgressBar(float Number)
    {
       // TraceUtil.Log("SetNumber:" + Number);
        ProgressBar.fillAmount = Number;
        IsColding=false;
        SetButtonActive(false);
        if (Number == 0)
        {
            SetButtonActive(true);
            IsColding=true;
            //StartCoroutine(FlashBtn());
        }
    }

//    IEnumerator FlashBtn()
//    {
//        UI.CreatObjectToNGUI.InstantiateObj(FlashEffectPrefab,CreatFlashEffectPoint);
//        yield return new WaitForSeconds(1);
//        CreatFlashEffectPoint.ClearChild();
//    }

    void OnClick()
    {
        if (base.BtnCallBack != null)
        {
            base.BtnCallBack(null);
            SetButtonActive(false);
        }
    }


     protected override void RegisterEventHandler()
     {
         //AddEventHandler(EventTypeEnum.ColdWork.ToString(),ColdMyself);
     }
}
