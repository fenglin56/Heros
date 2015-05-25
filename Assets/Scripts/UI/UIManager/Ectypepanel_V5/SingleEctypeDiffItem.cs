using UnityEngine;
using System.Collections;

namespace UI.MainUI{

	public class SingleEctypeDiffItem : MonoBehaviour {

		public UISprite LockIcon;
		public GameObject SelectStatus;
		public Transform EctypeIconPos;
		public SingleButtonCallBack NameLabel;

		public EctypeSelectConfigData MyConfigData{get;private set;}
		public EctypeDiffListPanel MyParent{get;private set;}
		public bool IsLock{get{return MyConfigData==null;}}
		[HideInInspector]
		public bool isPointEctype;
        /// <summary>
        ///  副本难度所在的DragUnit在List的UIGrid中的位置百分比。用于代码控制DragablePanel的滚动
        /// </summary>
        [HideInInspector]
        public float DragAmount;
		private UISprite m_IconSprite;

		public void Init(EctypeSelectConfigData configData,EctypeDiffListPanel myParent)
		{
			MyParent = myParent;
			MyConfigData = configData;
			LockIcon.gameObject.SetActive(IsLock);
			NameLabel.gameObject.SetActive(!IsLock);
			if(!IsLock)
			{
				m_IconSprite = CreatObjectToNGUI.InstantiateObj(configData._EctypeIconPrefab,EctypeIconPos).GetComponent<UISprite>();
				NameLabel.SetButtonText(LanguageTextManager.GetString(configData._szName));
			}
		}
		private Transform childPanel;
		public void SetSelectStatus(SingleEctypeDiffItem selectItem)
		{
			bool isSelect = selectItem == this;
			if (childPanel == null)
				childPanel = transform.Find ("Panel");
			childPanel.localScale = isSelect?new Vector3(1.8f,1.8f,1.8f):Vector3.one;
			if(!IsLock)
			{
				m_IconSprite.color = isSelect?Color.white:Color.gray;
				SelectStatus.gameObject.SetActive(isSelect);
				NameLabel.gameObject.SetActive(isSelect&&!IsLock);
			}
		}

		void OnClick()
		{
			if(IsLock)
				return;
			SoundManager.Instance.PlaySoundEffect("Sound_Button_EctypeTabChoice");
			MyParent.OnMyChildItemClick(this);
		}

	}
}