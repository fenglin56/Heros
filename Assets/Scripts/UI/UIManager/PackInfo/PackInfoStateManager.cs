using UnityEngine;
using System.Collections;

public class PackInfoStateManager :Controller,ISingletonLifeCycle {
    #region implemented abstract members of Controller
    protected override void RegisterEventHandler()
    {
       
    }
    #endregion

    private static PackInfoStateManager instance;
    public static PackInfoStateManager Instance
    {
        get
        {
            if(instance==null)
            {
                instance=new PackInfoStateManager();
                SingletonManager.Instance.Add(instance);
            }
            return instance;
        }
    }
    #region ISingletonLifeCycle implementation
    public void Instantiate()
    {

    }
    public void LifeOver()
    {
        instance=null;
    }
    #endregion

    public PackInfoStateType CurrentState;
   
    public void StateChange(PackInfoStateType state)
    {
        CurrentState=state;

        RaiseEvent(EventTypeEnum.PackStateChange.ToString(),null);
    }
}
public enum PackInfoStateType
{
    InterPack,
    Showpack,
    PrepareToFashion,
    ShowFashionPanel,
    InterFashion,
    ShowFashion,
    PrepareToOutFashion,
    ClosFashPanel,
  
}
