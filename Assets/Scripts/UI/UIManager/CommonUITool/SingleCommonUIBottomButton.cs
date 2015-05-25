using UnityEngine;
using System.Collections;

namespace UI.MainUI
{
    public class SingleCommonUIBottomButton : MonoBehaviour
    {
        public CommonBtnInfo commonBtnInfo;

        public UISprite BackgroundSprite;
        public UISprite ClickSprite;

        CommonUIBottomButtonTool MyParent;

        void Awake()
        {
            ClickSprite.enabled = false;
        }

        public void InitButton(CommonBtnInfo commonBtnInfo)
        {
            if (!string.IsNullOrEmpty(commonBtnInfo.BtnNormalSprite)) BackgroundSprite.name = commonBtnInfo.BtnNormalSprite;
            if (!string.IsNullOrEmpty(commonBtnInfo.BtnPressedSprite)) ClickSprite.name = commonBtnInfo.BtnPressedSprite;
            ClickSprite.enabled = false;
            this.commonBtnInfo = commonBtnInfo;
        }

        public void InitButton(CommonBtnInfo commonBtnInfo,CommonUIBottomButtonTool myParent)
        {
            MyParent = myParent;
            if(!string.IsNullOrEmpty(commonBtnInfo.BtnNormalSprite))SetBackgroundSprite(commonBtnInfo.BtnNormalSprite);
            if (!string.IsNullOrEmpty(commonBtnInfo.BtnPressedSprite)) SetClickSprite(commonBtnInfo.BtnPressedSprite);
            ClickSprite.enabled = false;
            this.commonBtnInfo = commonBtnInfo;
        }
        public void SetBackgroundSprite(string spriteName)
        {
            this.BackgroundSprite.atlas = MyParent.AtlasDictionary[spriteName];
            this.BackgroundSprite.spriteName = spriteName;
        }

        public void SetClickSprite(string spriteName)
        {
            this.ClickSprite.atlas = MyParent.AtlasDictionary[spriteName];
            this.ClickSprite.spriteName = spriteName;
        }

		void OnClick()
		{
			if (commonBtnInfo != null)
			{
				if (commonBtnInfo.OnClickCallBack != null)
				{
					commonBtnInfo.OnClickCallBack(null);
				}
			}
		}

        void OnPress(bool isPressed)
        {
			ClickSprite.enabled = isPressed;
//			if (commonBtnInfo != null)
//			{
//				if (!isPressed && commonBtnInfo.OnClickCallBack != null)
//				{
//					commonBtnInfo.OnClickCallBack(null);
//				}
//			}
        }


    }
}