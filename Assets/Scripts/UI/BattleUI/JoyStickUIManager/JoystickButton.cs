using UnityEngine;
using System.Collections;


namespace UI.Battle
{
    public class JoystickButton : JoyStick
    {
        public UISprite BackgroundSprite;
        public UISprite DragSpriteIcon;
        float DragDistance = 80;

        public Transform LeftPos;
        public Transform CenterPos;

        void Start()
        {
			selfCollider = GetComponent<BoxCollider>();
			Invoke ("NextFun",0.05f);
        }
		//因为有时本start先走，求出centerPos，又NGUIScreenPosition中的start再走，改变了position，center就不准确了！//
		void NextFun()
		{
			GetUICamera(transform);
			base._transform = transform;
			base.SetGUICenter(base._uiCamera.WorldToScreenPoint(transform.position), GetTouchRange());
		}
        void GetUICamera(Transform transform)
        {
            Camera uiCamera = transform.parent.GetComponent<Camera>();
            if (uiCamera != null)
            {
                base._uiCamera = uiCamera;
            }
            else
            {
                GetUICamera(transform.parent);
            }
        }

        float GetTouchRange()
        {
            Vector3 leftScreenPos = base._uiCamera.WorldToScreenPoint(LeftPos.position);
            Vector3 centerScreenPos = base._uiCamera.WorldToScreenPoint(CenterPos.position);
            return Mathf.Abs(centerScreenPos.x - leftScreenPos.x);
        }

        //public override void ShowDragPanel()
        //{
        //    //TraceUtil.Log("active");
        //    SetItemActive(true);
        //}

        //public override void HideDragPanel()
        //{
        //    //TraceUtil.Log("activeFalse");
        //    SetItemActive(false);
        //}

        void SetItemActive(bool flag)
        {
            BackgroundSprite.enabled = flag;
            DragSpriteIcon.enabled = flag; 
        }

        public override void MovePad(bool isPress, Vector3 dir)
        {
            base.MovePad(isPress, dir);
            Vector3 UIPosition = new Vector3(dir.x,dir.z,0);
            float distance = Vector3.Distance(UIPosition,Vector3.zero);
            UIPosition.Normalize();
            DragSpriteIcon.transform.localPosition = UIPosition * (distance<DragDistance?distance:DragDistance);
        }

        void OnDoubleClick()
        {
        }

    }
}