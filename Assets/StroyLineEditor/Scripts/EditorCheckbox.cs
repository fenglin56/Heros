using UnityEngine;
using System.Collections;

namespace StroyLineEditor
{
    public class EditorCheckbox : MonoBehaviour
    {
        public UILabel UILabel;
        private int m_curID;

        //use only for type CheckboxType.StroyLine
        private int m_vocation;
		private int triggerCondition;

        private CheckboxType m_type;

        // Use this for initialization
        public void InitCheckbox(int id,int condition, string text, CheckboxType type, int vocation)
        {
            m_curID = id;
			triggerCondition = condition;
            UILabel.text = text;
            m_type = type;
            m_vocation  = vocation;
        }

        void OnActivate(bool isActive)
        {
            if (isActive)
            {
                switch (m_type)
                {
                    case CheckboxType.StroyLine:
					EditorDataManager.Instance.SetCurSelectEctypeID(m_curID,triggerCondition, m_vocation);
						//int vocation = 1;//PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
					StroyLineDataManager.Instance.SetCurEctypeID(new StroyLineKey { VocationID = m_vocation,ConditionID = triggerCondition, EctypeID = m_curID});
                        break;
                    case CheckboxType.NpcID:
                        StroyEditorUIManager.Instance.SetActionUiState(m_curID);
                        break;
                    case CheckboxType.CameraID:
                        StroyEditorUIManager.Instance.SetCameraUiState(m_curID);
                        break;
                        default:
                        break;
                }
                
            }

        }
    }
}
