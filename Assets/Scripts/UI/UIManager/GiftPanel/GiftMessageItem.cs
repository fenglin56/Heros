using UnityEngine;
using System.Collections;


namespace UI.Gift
{
	public class GiftMessageItem : MonoBehaviour 
	{

		public UILabel Label_Content;

		private Vector3 m_startPos;
		private Vector3 m_endPos;
		private float m_MoveSpeed; //分米每秒
		private float m_PushSpeed;
		private float m_MoveTime = 0.5f;
		private float m_PushTime = 1f;
		private bool m_isPushing = false;
		private float m_PushDistance;
		private Vector3 m_pushEndPos;

		private int m_goodID = 0;

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

		public void Show(string smg,Vector3 startPos)
		{
			m_MoveSpeed = CommonDefineManager.Instance.CommonDefine.ItemMsgSpeedVertical;
			m_PushSpeed = CommonDefineManager.Instance.CommonDefine.ItemMsgSpeedHorizontal;
			m_PushTime = CommonDefineManager.Instance.CommonDefine.ItemMsgTimeHorizontal;

			Label_Content.text = smg;
			m_endPos = startPos;
			m_startPos = m_endPos - Vector3.right* m_MoveSpeed * m_MoveTime;
			transform.localPosition = m_startPos;
			m_pushEndPos = m_endPos;
			
			StartCoroutine("Showing");
		}

		void Update()
		{
			if(m_isPushing)
			{
				if(transform.localPosition.y - m_pushEndPos.y > 0 )
				{
					transform.localPosition += Vector3.down * m_PushSpeed * Time.fixedDeltaTime;
				}
			}
		}
	
		IEnumerator Showing()
		{
			float i = 0;
			float rate = 1.0f/m_MoveTime;
			while (i < 1.0) {
				i += Time.deltaTime * rate;
				Vector3 newPos = Vector3.Lerp(m_startPos, m_endPos, i);
				transform.localPosition = newPos;
				yield return null;
			}
			m_isPushing = true;
		}

		public void Push()
		{

			//m_endPos = m_endPos + Vector3.down * m_PushSpeed * m_PushTime;

			m_pushEndPos += Vector3.down * m_PushSpeed * m_PushTime;
			//StartCoroutine("Pushing");
		}

		IEnumerator Pushing()
		{
			float i = 0;
			float rate = 1.0f/m_PushTime;
			while (i < 1.0) {
				i += Time.deltaTime * rate;
				transform.localPosition -= Vector3.up * m_PushSpeed * Time.deltaTime;
				yield return null;
			}
		}

		public void Disappear()
		{
			Destroy(gameObject);
		}
	}
}
