using UnityEngine;
using System.Collections;

namespace UI.Login
{
    public class LoginLoadingPanel : IUIPanel
    {

        public UISlider ProgressBar;
        public UILabel ProgressLabel;

        private GameObject DontDestroyObj;

        private LoadSceneData loadSceneData;

        void GetUIRoot(Transform Trs)
        {
            if (Trs.parent != null)
            {
                GetUIRoot(Trs.parent);
            }
            else
            {
                this.DontDestroyObj = Trs.gameObject;
                this.DontDestroyObj.AddComponent<DontDestroy>();
            }
        }

        void Awake()
        {
            TraceUtil.Log(this.name);
            Show();
        }

        public override void Show()
        {
            this.loadSceneData = GameDataManager.Instance.PeekData(DataType.LoadingSceneData) as LoadSceneData;
            transform.localPosition = Vector3.zero;
            this.enabled = true;
            GetUIRoot(transform);
        }

        void Update()
        {
            SetLoadingProgress();
        }

        void SetLoadingProgress()
        {
            ProgressBar.sliderValue = loadSceneData.Progress;
            ProgressLabel.text = string.Format("{0}%", (int)(loadSceneData.Progress * 100));
            if (loadSceneData.Progress == 1)
            {
                DestroyMyObject();
            }
            //TraceUtil.Log("加载场景，进度为："+Progress);
        }

        void DestroyMyObject()
        {
            if (DontDestroyObj != null)
            {
                Destroy(DontDestroyObj);
            }
        }

        public override void Close()
        {
            transform.localPosition = new Vector3(0, 0, -1000);
            this.enabled = false;
        }


        public override void DestroyPanel()
        {
            throw new System.NotImplementedException();
        }
    }
}