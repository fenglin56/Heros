using UnityEngine;
using System.Collections;
namespace UI.Siren
{
	public class SirenUnlockBox : MonoBehaviour 
	{
		public UILabel Label_Title;
		public UILabel Label_Value;
		public LocalButtonCallBack Button_Cancle;
		public LocalButtonCallBack Button_Sure;

		private int m_SirenID = 0;
		private int m_SirenPrice = 0;
		private ButtonCallBack m_SirenUnlockCallBack;

		void Start()
		{
			Button_Sure.SetCallBackFuntion(OnSureClick,null);
			Button_Cancle.SetCallBackFuntion(OnCancleClick, null);
		}

		public void Show(int sirenID, int price, ButtonCallBack unlockCallBack)
		{
			Label_Title.text = LanguageTextManager.GetString("IDS_I2_6");
			this.m_SirenID = sirenID;
			this.m_SirenPrice = price;
			this.m_SirenUnlockCallBack = unlockCallBack;
			Label_Value.text = price.ToString();
			transform.localPosition = Vector3.zero;
			if(m_SirenPrice > PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BINDPAY)
			{
				Label_Value.color = Color.red;
			}
			else
			{
				Label_Value.color = new Color(1f,0.98f,0.435f);
			}
		}
		public void Close()
		{
			transform.localPosition = Vector3.forward * -800;
		}
		void OnSureClick(object obj)
		{
			Close();

			if(m_SirenPrice > PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BINDPAY)
			{
				MessageBox.Instance.ShowTips(3,LanguageTextManager.GetString("IDS_I2_8"),2f);
				return;
			}
			NetServiceManager.Instance.EntityService.SendLianHua(m_SirenID, 
			                                                    EntityService.YaoNvOpType.unlockByMoney,m_SirenPrice);
			m_SirenUnlockCallBack(null);
		}
		void OnCancleClick(object obj)
		{
			Close();
		}

	}
}
