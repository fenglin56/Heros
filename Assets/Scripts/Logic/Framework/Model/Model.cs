using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public abstract class Model : Notifier 
{
    //模型名是只读的，只在构造函数处生成
    private static int m_InstanceId = 0;
    protected string ModelName;
    

    public Model()
    {
        MakeModelName();
        //在初始化的时候将自己添加到模型管理器中
        ModelManager.AddModel(this);
    }

    ~Model()
    {
        //在析构的时候，将自己从模型管理器中删除
        ModelManager.DelModel(GetModelName());
    }

    //设置自己的模型名，多个模型实例可用静态变量来区分ID，也可以子类构造函数传参
    protected virtual void MakeModelName()
    {
        var instanceId=Model.m_InstanceId++;
        this.ModelName = this.ModelName + instanceId;
    }

    public string GetModelName()
    {
        return ModelName;
    }

    public bool IsModelNameValide()
    {
        //如果不存在该模型名，该模型名可用
        return !NotifyManager.HasEvent(ModelName);
    }

    //监听控制层的事件
    protected abstract void RegisterEventHandler();

    //将一个object注入到该数据模型
    public void Inject(object obj)
    {
        //1.获取obj的所有属性
        FieldInfo[] fields = obj.GetType().GetFields();
        foreach (FieldInfo field in fields)
        {
            //2.检查自己是否拥有该属性
            FieldInfo myfield = this.GetType().GetField(field.Name);
            if (null != myfield)
            {
                //3.设置值
                myfield.SetValue(this, field.GetValue(obj));
            }
        }
    }

    protected void Refresh(string attribute, INotifyArgs e)
    {
        //触发事件并将被改变的属性作为参数传入
        RaiseEvent(ModelName + attribute, e);
    }
}
