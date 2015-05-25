using UnityEngine;
using System.Collections;

namespace UI.MainUI{

	public class SelectNumPanel : MonoBehaviour {
		public delegate void SelectNumDelegate(int num);

		public UIInput InputLabel;
		//public UIScrollBar DragScrollBar;
        public UISlider DragScrollBar;
		public SingleButtonCallBack AddBtn;
		public SingleButtonCallBack CountDownBtn;
		public SingleButtonCallBack  SureBtn;
		public SingleButtonCallBack CancelBtn;

		int MinNum = 1;
		int DefultMaxNum = 99;
		int DragMaxNum = 99;
		int CurrentNum = 99;
		bool IsShow = false;

		float DragMinScrollValue;
		float DragMaxScrollvalue;

		SelectNumDelegate m_SelectNumCallBack;


		void Awake()
		{
			AddBtn.SetCallBackFuntion(OnAddBtnClick);
			CountDownBtn.SetCallBackFuntion(OnCountDownBtnClick);
			SureBtn.SetCallBackFuntion(OnSureBtnClick);
			CancelBtn.SetCallBackFuntion(OnCancelBtnClick);
			DragScrollBar.onValueChange = OnScoreBarChange;
			Show(1,55,null);

            TaskGuideBtnRegister();
        }
        /// <summary>
        /// 引导按钮注入代码
        /// </summary>
        private void TaskGuideBtnRegister()
        {
            AddBtn.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_SellItemConfirmPanel_SelectNumPanel_AddAmount);
            CountDownBtn.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_SellItemConfirmPanel_SelectNumPanel_SubtractAmount);
            SureBtn.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_SellItemConfirmPanel_SelectNumPanel_AmountConfirm);
            CancelBtn.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_SellItemConfirmPanel_SelectNumPanel_AmountCancel);
        }
		/// <summary>
		/// 显示数量选择面板
		/// </summary>
		/// <param name="minNum">Minimum number.</param>
		/// <param name="dragMaxNum">Drag max number.</param>
		/// <param name="defultMaxNum">Defult max number.</param>
		public void Show(int minNum,int dragMaxNum,SelectNumDelegate selectCallBack)
		{
			gameObject.SetActive(true);
            CurrentNum = dragMaxNum;
			MinNum = minNum;
			DragMaxNum = dragMaxNum;
            DefultMaxNum = dragMaxNum;
            CurrentNum = dragMaxNum;
            DragScrollBar.numberOfSteps = dragMaxNum+1;
            DragMinScrollValue = (float)minNum/(float)DefultMaxNum;
            DragMaxScrollvalue = (float)dragMaxNum/(float)DefultMaxNum;
			m_SelectNumCallBack =  selectCallBack;
            DragScrollBar.sliderValue = DragMaxScrollvalue;
			InputLabel.text = CurrentNum.ToString();
			IsShow = true;
		}

		void OnScoreBarChange(float value)
		{
//			if(value>DragMaxScrollvalue)
//			{
//                value = DragMaxScrollvalue;
           if(value<DragMinScrollValue)
			{
			  DragScrollBar.sliderValue = DragMinScrollValue;
                value=DragMinScrollValue;
			}
//			dragBar.ProgressBackground.fillAmount = dragBar.scrollValue;
			CurrentNum = Mathf.RoundToInt(value*DragMaxNum);
			InputLabel.label.text = CurrentNum.ToString();
		}

		void OnAddBtnClick(object obj)
		{
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Click");
			if(CurrentNum<DragMaxNum)
			{
				CurrentNum++;
				DragScrollBar.sliderValue = (float)CurrentNum/(float)DefultMaxNum;
			}
		}

		void OnCountDownBtnClick(object obj)
		{
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Click");
			if(CurrentNum>MinNum)
			{
				CurrentNum--;
				DragScrollBar.sliderValue = (float)CurrentNum/(float)DefultMaxNum;
			}
		}

		void OnSureBtnClick(object obj)
		{
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Confirm");
			if(m_SelectNumCallBack!=null)
			{
				m_SelectNumCallBack(CurrentNum);
			}
			Close();
		}
		
		void OnCancelBtnClick(object obj)
		{
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Cancel");
			Close();
		}

		void Close()
		{
			IsShow = false;
			gameObject.SetActive(false);
		}

//		void LateUpdate()
//		{
//			if(!IsShow)
//				return;
//			DragScrollBar.scrollValue = DragScrollBar.scrollValue>DragMaxScrollvalue?DragMaxScrollvalue:DragScrollBar.scrollValue;
//			DragScrollBar.scrollValue = DragScrollBar.scrollValue<DragMinScrollValue?DragMinScrollValue:DragScrollBar.scrollValue;
//			DragScrollBar.ProgressBackground.fillAmount = DragScrollBar.scrollValue;
//		}
	}
}