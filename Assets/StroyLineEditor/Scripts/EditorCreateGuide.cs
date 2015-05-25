using UnityEngine;
using System.Collections;

namespace StroyLineEditor
{
    public class EditorCreateGuide : MonoBehaviour
    {

        public GameObject UIPopuplist;

        private GameObject m_curUIPanel;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void CreateGuide()
        {
            if (m_curUIPanel == null)
            {
                m_curUIPanel = Instantiate(UIPopuplist) as GameObject;
                m_curUIPanel.transform.parent = this.transform.parent;
                m_curUIPanel.transform.localScale = Vector3.one;
                //m_curUIPanel.GetComponent<UIPopupList>().eventReceiver = this.gameObject;
            }

            m_curUIPanel.transform.localPosition = Vector3.zero;
        }


    }
}
