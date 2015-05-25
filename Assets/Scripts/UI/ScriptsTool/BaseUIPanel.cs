using UnityEngine;
using System.Collections;

namespace UI.MainUI
{

    public abstract class BaseUIPanel : View
    {

        public bool IsShow { get; protected set; }

        protected override void RegisterEventHandler()
        {   
           
        }

        public virtual void Show(params object[] value)
        {
            IsShow = true;
            transform.localPosition = Vector3.zero;
        }

        public virtual void Close()
        {
        
            IsShow = false;
            transform.localPosition = new Vector3(0, 0, -1000);
        }

        public void CleanUpUIStatus()
        {
            if (MainUIController.Instance != null)
            {
                MainUIController.Instance.CurrentUIStatus = UIType.Empty;
				MainUIController.Instance.NextUIStatus = UIType.Empty;
            }
        }

    }
}