using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UI.MainUI
{

    public class CommonUIBottomButtonTool : MonoBehaviour
    {

        public GameObject ButtonPrefab;

        public UIAtlas[] UIAtlasList;

        public Dictionary<string, UIAtlas> AtlasDictionary = new Dictionary<string,UIAtlas>();

        void Awake()
        {
            UIAtlasList.ApplyAllItem(P=>P.spriteList.ApplyAllItem(C=>AtlasDictionary.Add(C.name,P)));
        }

        private Dictionary<CommonBtnInfo, SingleCommonUIBottomButton> m_btnList = new Dictionary<CommonBtnInfo, SingleCommonUIBottomButton>();

        public void Show(List<CommonBtnInfo> btnInfo)
        {
            transform.ClearChild();
            foreach (var child in btnInfo)
            {
                GameObject CreatBtn = CreatObjectToNGUI.InstantiateObj(ButtonPrefab,transform);
                SingleCommonUIBottomButton btnScript = CreatBtn.GetComponent<SingleCommonUIBottomButton>();
                btnScript.InitButton(child,this);
                m_btnList.Add(child, btnScript);
            }
            ShowAnim();
        }

        public void ShowAnim()
        {
            foreach (var child in m_btnList)
            {
                TweenPosition.Begin(child.Value.gameObject, 0.3f, Vector3.zero, new Vector3(100 * child.Key.BtnPosition, 0, 0));
                TweenScale.Begin(child.Value.gameObject, 0.5f, new Vector3(0.1f, 0.1f, 0.1f), ButtonPrefab.transform.localScale, null);
            }
        }

        public SingleCommonUIBottomButton GetButtonComponent(CommonBtnInfo key)
        {
            if (m_btnList.ContainsKey(key))
            {
                return m_btnList[key];
            }
            return null;
        }

        public void ChangeButtonInfo(CommonBtnInfo commonBtnInfo, string normalSprite, string pressedSprite, ButtonCallBack onClickCallBack)
        {
            if (m_btnList.ContainsKey(commonBtnInfo))
            {
                m_btnList[commonBtnInfo].commonBtnInfo.OnClickCallBack = onClickCallBack;
                m_btnList[commonBtnInfo].SetBackgroundSprite(normalSprite);
                m_btnList[commonBtnInfo].SetClickSprite(pressedSprite);
            }
        }
    }


    public class CommonBtnInfo
    {
        public int BtnPosition;
        public string BtnNormalSprite;
        public string BtnPressedSprite;
        public ButtonCallBack OnClickCallBack;

        public CommonBtnInfo(ButtonCallBack onClickCallBack)
        {
            this.OnClickCallBack = onClickCallBack;
        }
        public CommonBtnInfo(int btnPosition,string normalSprite,string pressedSprite,ButtonCallBack onClickCallBack)
        {
            this.BtnPosition = btnPosition;
            this.BtnNormalSprite = normalSprite;
            this.BtnPressedSprite = pressedSprite;
            this.OnClickCallBack = onClickCallBack;
        }
    }

}