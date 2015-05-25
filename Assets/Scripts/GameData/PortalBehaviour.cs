using UnityEngine;
using System.Collections;

public class PortalBehaviour :View , IEntityDataManager{

    private Transform m_preTrans;
    //private SMSGEctypeRequestCreate_CS sMSGEctypeRequestCreate_CS = new SMSGEctypeRequestCreate_CS();

    private IEntityDataStruct m_portalDataModel;
    public IEntityDataStruct PortalDataModel
    {
        get { return this.m_portalDataModel; }
        set { this.m_portalDataModel = value; }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    void Awake()
    {
        RegisterEventHandler();
    }
   
     protected override void RegisterEventHandler()
    {
        AddEventHandler(EventTypeEnum.OnTouchInvoke.ToString(), OnTouchDown);
    }

    public void OnTouchDown(INotifyArgs e)
    {
        TouchInvoke touchInvoke = (TouchInvoke)e;
        if (touchInvoke.TouchGO == gameObject)
        {
            RaiseEvent(EventTypeEnum.TargetSelected.ToString(), new TargetSelected() { Target = transform, Type = ResourceType.Portal });
            m_preTrans = transform;
        }
    }
   
    void Destroy()
    {
        RemoveAllEvent();
    }

    void OnTriggerEnter (Collider collidObject)
    {
        if(GameManager.Instance.UseJoyStick)
        {
            PlayerBehaviour pb = collidObject.GetComponent<PlayerBehaviour>();
            if( null != pb)
            {
                if(pb.IsHero)
                {
                    SMsgPropCreateEntity_SC_Channel dataModel = (SMsgPropCreateEntity_SC_Channel)this.PortalDataModel;
                    
                    NetServiceManager.Instance.EntityService.SendEnterPortal(dataModel.SMsg_Header.uidEntity);
                }
            }
        }
    }



    public IEntityDataStruct GetDataModel()
    {
        return this.PortalDataModel;
    }
}
