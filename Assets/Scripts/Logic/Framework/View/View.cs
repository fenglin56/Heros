using UnityEngine;
using System.Collections;

public abstract class View : ViewNotifier
{
    //绑定模型的某个属性
    public void BindModel(Model m, string Attribute, NotifyManager.StandardDelegate fun)
    {
        AddEventHandler(m.GetModelName() + Attribute, fun);
    }

    //解绑模型的某个属性
    void UnBindModel(Model m, string Attribute, NotifyManager.StandardDelegate fun)
    {
        RemoveEventHandler(m.GetModelName() + Attribute, fun);
    }
    protected abstract void RegisterEventHandler();
}
