using UnityEngine;
using System.Collections;
namespace UI.PlayerTitle
{
	public class PlayerTitleItem : MonoBehaviour 
	{
		public SpriteSwith Swith_Icon; 
		public LocalButtonCallBack Button_Click;

		public Transform IconTrans;

		public GameObject LockMark;

		public int m_TitleID{get;private set;}
		public bool m_isUnlock{get; private set;}
		private ButtonCallBack m_CallBack;
		private UISprite m_IconSprite;

		public void Init(int titleID, bool isUnlock,  GameObject spritePrefab, ButtonCallBack btnCB)
		{
			this.m_TitleID = titleID;
			this.m_isUnlock = m_isUnlock;
			//Button_Click.ButtonBackground.spriteName = spriteName;

			GameObject titleGameObj = (GameObject)UI.CreatObjectToNGUI.InstantiateObj(spritePrefab, IconTrans);
			titleGameObj.transform.localScale = spritePrefab.transform.localScale;
			m_IconSprite = titleGameObj.GetComponent<UISprite>();

			m_CallBack = btnCB;
			//Button_Click.SetCallBackFuntion(btnCB);

			UpdateInfo(isUnlock);
		}

		public void UpdateInfo(bool isUnlock)
		{
			if(isUnlock)
			{
				m_IconSprite.color = Color.white;
				LockMark.SetActive(false);
			}
			else
			{
				m_IconSprite.color = Color.gray;
				LockMark.SetActive(true);
			}
		}

		public void Reset(bool isFlag)
		{
			int no = isFlag == true ? 1 : 0;
			Swith_Icon.ChangeSprite(no);
		}

		void OnClick()
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Title_Change");
			m_CallBack(m_TitleID);
		}


	}
}