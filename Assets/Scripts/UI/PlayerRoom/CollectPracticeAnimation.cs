using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UI;

public class CollectPracticeAnimation : MonoBehaviour
{
    public GameObject PopUpTitlePrefab;
    public TweenPosition tweenPositionComponent;

    Vector3 StartPosition;

    List<float> PositionXList = new List<float>();
    Vector3 EndPosition;
    int AddNumber;
    int AddType;

    public void Show(Vector3 startPosition, int addNumber, Vector3 EndPosition)
    {
        StartPosition = startPosition;
        this.EndPosition = EndPosition;
        this.AddNumber = addNumber;

        float TargetPositionX = Random.Range(-100, 100);
        PositionXList.Add(TargetPositionX / 3);
        PositionXList.Add(TargetPositionX / 3);
        PositionXList.Add(TargetPositionX / 6);
        PositionXList.Add(TargetPositionX / 6);
        MoveUpStep1();
    }

    void MoveUpStep1()
    {
        Vector3 fromPosition = StartPosition;
        Vector3 toPosition = fromPosition + new Vector3(PositionXList[0], 80, 0);
        tweenPositionComponent.method = UITweener.Method.EaseOut;
        TweenPosition.Begin(gameObject, 0.3f, fromPosition, toPosition, MoveDownStep2);
    }

    void MoveDownStep2(object obj)
    {
        Vector3 fromPosition = transform.localPosition;
        Vector3 toPosition = fromPosition + new Vector3(PositionXList[1], -150, 0);
        tweenPositionComponent.method = UITweener.Method.EaseIn;
        TweenPosition.Begin(gameObject, 0.3f, fromPosition, toPosition, MoveUpStep3);
    }

    void MoveUpStep3(object obj)
    {
        Vector3 fromPosition = transform.localPosition;
        Vector3 toPosition = fromPosition + new Vector3(PositionXList[2], 20, 0);
        tweenPositionComponent.method = UITweener.Method.EaseOut;
        TweenPosition.Begin(gameObject, 0.1f, fromPosition, toPosition, MoveDownStep4);
    }

    void MoveDownStep4(object obj)
    {
        Vector3 fromPosition = transform.localPosition;
        Vector3 toPosition = fromPosition + new Vector3(PositionXList[3], -20, 0);
        tweenPositionComponent.method = UITweener.Method.EaseIn;
        TweenPosition.Begin(gameObject, 0.1f, fromPosition, toPosition, MoveToTarget);
    }

    void MoveToTarget(object obj)
    {
        StartCoroutine(MoveItemForTime(Random.Range(1f, 1.5f)));
    }

    IEnumerator MoveItemForTime(float waitTiem)
    {
        yield return new WaitForSeconds(waitTiem);
        TweenPosition.Begin(gameObject, 1f, transform.localPosition, EndPosition, DestroyObj);
        TweenAlpha.Begin(gameObject, 2f, 1, 0, null);
        GameObject PopUpTitleObj = CreatObjectToNGUI.InstantiateObj(PopUpTitlePrefab, transform.parent);
        string addTitleStr = string.Format(LanguageTextManager.GetString("IDS_H1_471"), AddNumber);
        PopUpTitleObj.GetComponent<UILabel>().SetText(addTitleStr);
        TweenPosition.Begin(PopUpTitleObj, 1f, transform.localPosition, transform.localPosition + new Vector3(0, 100, 0), DestroyObj);
        //TweenAlpha.Begin(PopUpTitleObj,0.5f,1,0,DestroyObj);
    }

    void DestroyObj(object obj)
    {
        GameObject desObj = obj as GameObject;
        if (desObj != null)
        {
            Destroy(desObj);
        }
    }
}
