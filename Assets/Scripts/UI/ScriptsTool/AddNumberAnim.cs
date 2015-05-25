using UnityEngine;
using System.Collections;

public class AddNumberAnim:MonoBehaviour {

    float AddTime = 0;

    float FromeNumber = 0;
    float ToNumber = 10;

    float time = 0;

    private float addDistance = 0;

    public ButtonCallBack FinishCallBack;
    

    public static void StartFloatNumAnim(float Time, float FromeNum, float ToNum,ButtonCallBack finishCallBack)
    {
        GameObject FloatAnimObj = new GameObject();
        AddNumberAnim addNumScripit = FloatAnimObj.AddComponent<AddNumberAnim>();
        addNumScripit.AddTime = Time;
        addNumScripit.FromeNumber = FromeNum;
        addNumScripit.ToNumber = ToNum;
        addNumScripit.FinishCallBack = finishCallBack;
    }

    void Start()
    {
        addDistance = (ToNumber - FromeNumber)/(AddTime*Time.deltaTime);
        time = FromeNumber;
    }

    void Update()
    {
        if (time < ToNumber)
        {
            time += addDistance;
        }else
        {
            FinishCallBack(null);
            Destroy(gameObject);
        }
    }


}
