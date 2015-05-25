//using UnityEngine;
//using System.Collections;

//namespace UI
//{

//    public class Flag : MonoBehaviour
//    {
//        //************************************ 用于闪烁**********************************

//        UISprite FlagComponent;
//        Color Mycolor;

//        float FlagSpeed = 3;//闪烁速度
//        float FlagNum = 2;//闪烁次数

//        float flagnumber = 0;
//        float time;
//        bool Up = true, Down = false;
//        bool Playing = false;

//        void Start()
//        {
//            Mycolor = Color.green;
//            setEnable(false);
//        }

//        public void StartTwinkling(int FlagNumber)//传入闪烁次数
//        {
//            //TraceUtil.Log("CompleteFlag");
//            Playing = true;
//            FlagComponent = gameObject.GetComponent<UISprite>();
//            this.FlagNum = FlagNumber;
//            setEnable(true);
//        }

//        public void StopFlag()
//        {
//            if (Playing == false)
//                return;
//            Playing = false;
//            Mycolor.a = 0;
//            FlagComponent.color = Mycolor;
//            setEnable(false);
//            flagnumber = 0;
//        }

//        void Update()
//        {
//            if (Up && time < 1.3f)
//            {
//                time += Time.deltaTime * FlagSpeed;
//            }
//            else if (Up && time > 1.3f)
//            {
//                Up = false;
//                Down = true;
//            }
//            if (Down && time > -0.3f)
//            {
//                time -= Time.deltaTime * FlagSpeed;
//            }
//            else if (Down && time < -0.3f)
//            {
//                Down = false;
//                Up = true;

//                flagnumber++;
//                if (flagnumber > FlagNum)
//                {
//                    StopFlag();
//                }
//            }

//            Mycolor.a = Mathf.Lerp(0, 1, time);
//            FlagComponent.color = Mycolor;
//        }

//        void setEnable(bool flag)
//        {
//            this.enabled = flag;
//        }

//    }
//}
