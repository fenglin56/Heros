
using UnityEngine;
using System.Collections;

namespace UI
{

    public class LoadingUI : MonoBehaviour
    {

        private static LoadingUI m_Instance;
        public static LoadingUI Instance { get { return m_Instance; } }

        public GameObject LoadingPanelPrefab;

        private GameObject InstanceObj;
		private float loadingStartTime = -1;
        void Awake()
        {
            m_Instance = this;
        }
        public void Show()
        {
            ClearLoadingPanel();
            if (LoadingPanelPrefab != null)
            {
                InstanceObj = CreatObjectToNGUI.InstantiateObj(LoadingPanelPrefab, transform);
                LoadingUIPanel UIPanelScripts =InstanceObj .GetComponent<LoadingUIPanel>();
            }
        }
		/*public void StartMarkTime()
		{
			loadingStartTime = Time.realtimeSinceStartup;
		}
		public void StartShowDownTime()
		{
			if (loadingStartTime < 0) {
				return;
			}
			float loadedTime = Time.realtimeSinceStartup - loadingStartTime;
			loadingStartTime = -1;
		}*/
        public void Close()
        {
            ClearLoadingPanel();
			loadingStartTime = -1;
        }

        void ClearLoadingPanel()
        {
            if (transform.childCount > 0)
            {
                transform.ClearChild();
            }
        }

        void OnDestroy()
        {
            m_Instance = null;
        }

    }
}