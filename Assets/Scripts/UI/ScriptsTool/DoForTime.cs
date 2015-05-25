using UnityEngine;
using System.Collections;

public class DoForTime : View
{
    public delegate void DoDelegate(object obj);

    private static DoForTime instance;

    void Awake()
    {
        AddEventHandler(EventTypeEnum.SceneChange.ToString(),DestroyMySelf);
    }

    void DestroyMySelf(INotifyArgs inotifyArgs)
    {
        RemoveEventHandler(EventTypeEnum.SceneChange.ToString(), DestroyMySelf);
        instance = null;
        Destroy(gameObject);
    }

    public static void DoFunForTime(float waitTime,DoDelegate doDelegate,object Value)
    {
        if (instance == null)
        {
            InistInstance();
        }
        instance.AddDelegate(waitTime, doDelegate, Value);
    }

    public static void DoFunForFrame(int farmeNumber,DoDelegate doDelegate, object value)
    {
        if (instance == null)
        {
            InistInstance();
        }
        instance.BeginDelegateForFarme(farmeNumber,doDelegate,value);
    }

    static void InistInstance()
    {
        GameObject InstanceGameObj = new GameObject();
        InstanceGameObj.name = "DoforTimeGameObj";
        instance = InstanceGameObj.AddComponent<DoForTime>();
        InstanceGameObj.AddComponent<DontDestroy>();
    }

    void AddDelegate(float waitTime, DoDelegate doDelegate, object Value)
    {
        StartCoroutine(Dofun(waitTime,doDelegate,Value));
    }

    void BeginDelegateForFarme(int farmeNumber, DoDelegate doDelegate, object value)
    {
        StartCoroutine(DoDelegateForFrame(farmeNumber, doDelegate, value));
    }

    IEnumerator DoDelegateForFrame(int farmeNumber, DoDelegate doDelegate, object Value)
    {
        yield return null;
        farmeNumber--;
        for (int i = 0; i < farmeNumber; i++)
        {
            yield return null;
        }
        doDelegate(Value);
    }

    IEnumerator Dofun(float waitTime, DoDelegate doDelegate, object Value)
    {
        yield return new WaitForSeconds(waitTime);
        doDelegate(Value);
    }

    public static void stop()
    {
        instance.StopAllCoroutines();
    }
    protected override void RegisterEventHandler()
    {
        throw new System.NotImplementedException();
    }
}
