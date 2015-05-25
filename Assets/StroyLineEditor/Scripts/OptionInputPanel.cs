using UnityEngine;
using System.Collections;
namespace StroyLineEditor
{
    public class OptionInputPanel : MonoBehaviour
    {

        public UILabel LabelSign;
        public UILabel Title;

        private GroupType m_groupType;
        // Use this for initialization
        public void InitInput(string sign, string title, GroupType groupType)
        {
            LabelSign.text = sign;
            Title.text = title;
            m_groupType = groupType;
        }

        void OnSubmit(string param)
        {
            EditorDataManager.Instance.SubParams(m_groupType, param);
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}
