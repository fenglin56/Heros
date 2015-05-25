using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UI.MainUI
{

    public class SingleGetAwardItem : MonoBehaviour
    {

        public Transform IconAnchor;
        public GameObject PopUpTitlePrefab;
        public TweenPosition tweenPositionComponent;

        Vector3 StartPosition;

        List<float> PositionXList = new List<float>();
        GetAwardAnimManager MyParent;
        int AddNumber;
        //int AddType;
        string ItemName;

        public void Show(Vector3 startPosition, int addNumber, int itemId, GetAwardAnimManager MyParent)
        {
            ItemData data = ItemDataManager.Instance.GetItemData(itemId);

            GameObject itemIcon = CreatObjectToNGUI.InstantiateObj(data._picPrefab, IconAnchor);

            ItemName = LanguageTextManager.GetString( data._szGoodsName );
            StartPosition = startPosition;

            //CostIcon.ChangeSprite(addType+1);
            this.MyParent = MyParent;
            this.AddNumber = addNumber;
            //this.AddType = 0;

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
            StartCoroutine(MoveItemForTime(Random.Range(1f,1.5f)));
        }

        IEnumerator MoveItemForTime(float waitTiem)
        {
            yield return new WaitForSeconds(waitTiem);
            TweenPosition.Begin(gameObject, 1f, transform.localPosition, MyParent.MyParent.LogPanelBtn.transform.localPosition, DestroyObj);
            TweenAlpha.Begin(gameObject, 2f, 1, 0, null);
            GameObject PopUpTitleObj = CreatObjectToNGUI.InstantiateObj(PopUpTitlePrefab,transform.parent);
            string addTitleStr = string.Format(LanguageTextManager.GetString( "IDS_I28_19" ), ItemName,  AddNumber);
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
}