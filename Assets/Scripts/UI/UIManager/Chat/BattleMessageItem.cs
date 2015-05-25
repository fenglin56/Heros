using UnityEngine;
using System.Collections;


namespace UI.Battle
{
	public class BattleMessageItem : MonoBehaviour 
	{
		
		public UILabel Label_Num;
		public UILabel Label_Name;
		public UIPanel Panel;

	
		private int m_goodID = 0;

		private int m_frame = 0;
		private bool m_isBegin = false;

		/*旧方法,先保留
		public void Show(int goodID, int num, Vector3 startPos)
		{
			m_goodID = goodID;

			m_MoveSpeed = CommonDefineManager.Instance.CommonDefine.ItemMsgSpeedVertical;
			m_PushSpeed = CommonDefineManager.Instance.CommonDefine.ItemMsgSpeedHorizontal;
			m_PushTime = CommonDefineManager.Instance.CommonDefine.ItemMsgTimeHorizontal;
			//m_MoveTime = CommonDefineManager.Instance.CommonDefine.ItemMsgTimeGap;

			var goodData = ItemDataManager.Instance.GetItemData(goodID);
			if(goodData == null)
				return;
			TextColor textColor = TextColor.white;
			switch(goodData._ColorLevel)
			{
				case 0:
					textColor = TextColor.EquipmentGreen;
					break;
				case 1:
					textColor = TextColor.EquipmentBlue;
					break;
				case 2:
					textColor = TextColor.EquipmentMagenta;
					break;
				case 3:
					textColor = TextColor.EquipmentYellow;
					break;
				default:
					break;
			}
			string content = string.Format(LanguageTextManager.GetString("IDS_H1_568"),
			                               NGUIColor.SetTxtColor(LanguageTextManager.GetString(goodData._szGoodsName),textColor), num.ToString());
			Label_Content.text = content;
			m_endPos = startPos;
			m_startPos = m_endPos - Vector3.right* m_MoveSpeed * m_MoveTime;
			transform.localPosition = m_startPos;
			m_pushEndPos = m_endPos;

			StartCoroutine("Showing");

		}
		*/
		void Awake()
		{
			Panel.alpha = 0;
		}

		void FixedUpdate()
		{
			if(!m_isBegin)
				return;
			m_frame++;
			if(m_frame <= 10)
			{
				Panel.alpha += 0.1f;
				transform.localPosition += Vector3.up * 5;
			}
			else if(m_frame <= 30)
			{
			}
			else if(m_frame <= 90)
			{
				Panel.alpha -= 0.017f;
				transform.localPosition += Vector3.up * 1.16f;
			}
			else
			{
				m_isBegin = false;
			}
		}

		public void Show(string numStr, string nameStr)
		{			
			Label_Num.text = "x" + numStr;
			Label_Name.text = nameStr;
			
			m_isBegin = true;
		}
		
		public void Disappear()
		{
			Destroy(gameObject);
		}
	}
}
