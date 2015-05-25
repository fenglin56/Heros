using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI
{
    public class SysBtnPanel : MonoBehaviour
    {
        /// <summary>
        /// UI系统按钮下的ui按钮排控制代码
        /// </summary>
        //public GameObject ButtonObject;//需要被实例出来的预置物体
        public List<LocalSysButton> btnArray { get; private set; }
       
        bool Showing = false;//展开的状态

        private List<int> m_guideBtnID = new List<int>();
        private GameObject m_btnEnableEffect;
        private float[] m_insertLeftUpPos_x=new float[]{0,0,0,0,0,0,0,0,0,0},m_insertRightUpPos_x=new float[]{0,0,0,0,0,0,0,0,0,0}, m_insertRightDownPos_x=new float[]{0,0,0,0,0,0,0,0,0,0};
        private float[] m_insertLeftUpRadio=new float[]{0,0,0,0,0,0,0,0,0,0},m_insertRightUpRadio=new float[]{0,0,0,0,0,0,0,0,0,0}, m_insertRightDownRadio=new float[]{0,0,0,0,0,0,0,0,0,0};
        private bool[]  m_insertLeftUpIFirst=new bool[]{true,true,true,true,true,true,true,true,true,true}, m_insertRightUpFirst=new bool[]{true,true,true,true,true,true,true,true,true,true}, m_insertRightDownFirst=new bool[]{true,true,true,true,true,true,true,true,true,true};
        private float m_insertLeftUpPos_y = 0, m_insertRightUpPos_y = 0, m_insertRightDownPos_y = 0;

        void Awake()
        {
            //if (ButtonObject == null)
            //{
            //    TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"没有关联按钮预置体!");
            //}
            btnArray = new List<LocalSysButton>();
        }

        public void RestPanel()
        {
            for(int i=0;i<10;i++)
            {
                m_insertLeftUpPos_x[i]=0;
                m_insertRightUpPos_x[i]=0;
                m_insertRightDownPos_x[i]=0;
                m_insertLeftUpRadio[i]=0;
                m_insertRightUpRadio[i]=0;
                m_insertRightDownRadio[i]=0;
                m_insertLeftUpIFirst[i]=true;
                m_insertRightUpFirst[i]=true;
                m_insertRightDownFirst[i]=true;


            }
//            m_insertLeftUpPos_x.ApplyAllItem(c=>c=0);
//            m_insertRightUpPos_x.ApplyAllItem(c=>c=0);
//            m_insertRightDownPos_x.ApplyAllItem(c=>c=0);
//            m_insertLeftUpRadio.ApplyAllItem(c=>c=0);
//            m_insertRightUpRadio.ApplyAllItem(c=>c=0);
//            m_insertRightDownRadio.ApplyAllItem(c=>c=0);
//            m_insertLeftUpIFirst.ApplyAllItem(c=>c=true);
//            m_insertLeftUpIFirst.ApplyAllItem(c=>c=true);
//            m_insertLeftUpIFirst.ApplyAllItem(c=>c=true);
            ResetSystemPanel();
        }
        public void InsertBtn(MainTownButtonConfigData item)//MainUI.UIType sysBtnType, MainBtnArea btnPos, int btnNum)//插入按钮，按钮位置，按钮序号，按钮序号从0开始
        {
            //print("原预添加位置："+btnNum);
//            if (item._ButtonIndex > btnArray.Count)
//            {
//                item._ButtonIndex = btnArray.Count;
//            }
            //print("实际添加位置：" +btnNum);
            LocalSysButton localBtnController = CreatObjectToNGUI.InstantiateObj(item.ButtonPrefab, transform).GetComponent<LocalSysButton>();
            localBtnController.gameObject.RegisterBtnMappingId(item.ButtonFunc, BtnMapId_Sub.Empty);

            //new 
            switch (item.ButtonArea)
            {
                case MainTownButtonArea.LeftUp:
                    for(int i=0;i<10;i++)
                    {
                        if(i==item.Button_Row-1)
                        {
                            if(m_insertLeftUpIFirst[i])
                            {
                                m_insertLeftUpPos_x[i]=CommonDefineManager.Instance.CommonDefine.TownstartPoint1.BaseOffset.x;
                                m_insertLeftUpIFirst[i]=false;

                                //m_insertLeftUpPos_y=CommonDefineManager.Instance.CommonDefine.TownstartPoint1.BaseOffset.y;
                            }
                            else{
                                m_insertLeftUpPos_x[i] += (item.ButtonRadius +m_insertLeftUpRadio[i])*CommonDefineManager.Instance.CommonDefine.TownstartPoint1.Direction.x;
                               
                            }
                            m_insertLeftUpPos_y= item.Button_RowIndex*CommonDefineManager.Instance.CommonDefine.TownstartPoint1.Direction.y;
                            m_insertLeftUpRadio[i]=item.ButtonRadius;
                            localBtnController.SetBtnAtrribute(item.ButtonFunc, new Vector3(m_insertLeftUpPos_x[i], m_insertLeftUpPos_y, 0), this.Showing,item.DefaultEnable==1,item.ButtonSound);
                        }
                    }
                   
                    break;
                case MainTownButtonArea.RightUp:
                    for(int i=0;i<10;i++)
                    {
                        if(i==item.Button_Row-1)
                        {
                            if(m_insertRightUpFirst[i])
                            {
                                m_insertRightUpPos_x[i]=CommonDefineManager.Instance.CommonDefine.TownstartPoint2.BaseOffset.x;
                                m_insertRightUpFirst[i]=false;
                               // m_insertRightUpPos_y=CommonDefineManager.Instance.CommonDefine.TownstartPoint2.BaseOffset.y;
                            }
                            else{
                                m_insertRightUpPos_x[i] += (item.ButtonRadius + m_insertRightUpRadio[i])*CommonDefineManager.Instance.CommonDefine.TownstartPoint2.Direction.x;
                               
                            }
                            m_insertRightUpPos_y= item.Button_RowIndex*CommonDefineManager.Instance.CommonDefine.TownstartPoint2.Direction.y;
                            m_insertRightUpRadio[i]=item.ButtonRadius;
                            localBtnController.SetBtnAtrribute(item.ButtonFunc, new Vector3(m_insertRightUpPos_x[i], m_insertRightUpPos_y, 0), this.Showing,item.DefaultEnable==1,item.ButtonSound);
                        }
                    }
                    break;
                case MainTownButtonArea.RightDown:

                    for(int i=0;i<10;i++)
                    {
                        if(i==item.Button_Row-1)
                        {
                            if(m_insertRightDownFirst[i])
                            {
                                if(i==0)
                                {
                                 m_insertRightDownPos_x[i]=CommonDefineManager.Instance.CommonDefine.TownstartPoint3.BaseOffset.x;
                                   
                                }
                                else
                                {
                                m_insertRightDownPos_x[i]=0;
                                }
                                m_insertRightDownFirst[i]=false;
                               // m_insertRightDownPos_y=CommonDefineManager.Instance.CommonDefine.TownstartPoint2.BaseOffset.y;
                            }
                            else{
                            m_insertRightDownPos_x[i] += (item.ButtonRadius + m_insertRightDownRadio[i])*CommonDefineManager.Instance.CommonDefine.TownstartPoint3.Direction.x;
                            }
                            m_insertRightDownPos_y= item.Button_RowIndex*CommonDefineManager.Instance.CommonDefine.TownstartPoint3.Direction.y;
                            m_insertRightDownRadio[i]=item.ButtonRadius;
                            localBtnController.SetBtnAtrribute(item.ButtonFunc, new Vector3(m_insertRightDownPos_x[i], m_insertRightDownPos_y, 0), this.Showing,item.DefaultEnable==1,item.ButtonSound);
                        }
                    }
                    break;
                default:
                    break;
            }

            //localBtnController.SetBtnAtrribute(item.ButtonFunc, new Vector3(item.ButtonPositionOffset.x, item.ButtonPositionOffset.y, 0), this.Showing,item.DefaultEnable==1);
            //引导注入
           
            #region old
//            switch (item._ButtonArea)
//            {
//                case MainBtnArea.TopLeft:
//                   // m_insertBottomPos += -(item._ButtonRadius * 2);
//                    localBtnController.SetBtnAtrribute(item._ButtonFunc, new Vector3(item._ButtonIndex.x, item._ButtonIndex.y, 0), this.Showing);
//                    break;
//                case MainBtnArea.TopRight:
//                    m_insertRightPos += (item._ButtonRadius * 2);
//                    localBtnController.SetBtnAtrribute(item._ButtonFunc, new Vector3(0, m_insertRightPos, 0), this.Showing);
//                    break;
//                default:
//                    break;
//            }
            #endregion
            ///ssss
            btnArray.Add(localBtnController);
            //ResetBtnPosition();

            int guideID=0;
            //TODO GuideBtnManager.Instance.RegGuideButton(localBtnController.gameObject, item._ButtonFunc, SubType.MainButton, out guideID);
            m_guideBtnID.Add(guideID);
        }


        public void ShowEnableEffect(GameObject effect, MainUI.UIType funcType)
        {
            if (m_btnEnableEffect != null)
                Destroy(m_btnEnableEffect);

            LocalSysButton btnComponent = null;// = btnArray.Single(P => P.ButtonFunc == funcType);
			
			for (int i = 0; i < btnArray.Count; i++) {
				if(btnArray[i].ButtonFunc == funcType)
					btnComponent = btnArray[i];
			}
			
			if(btnComponent != null)
			{
	            m_btnEnableEffect = Instantiate(effect) as GameObject;
	            m_btnEnableEffect.transform.parent = btnComponent.transform;
	            m_btnEnableEffect.transform.localScale = Vector3.one;
                m_btnEnableEffect.transform.localPosition = Vector3.zero;
			}
        }

        public void DelEnableEffect()
        {
            if (m_btnEnableEffect != null)
                Destroy(m_btnEnableEffect);
        }

        public void ResetSystemPanel()
        {
            foreach (LocalSysButton child in btnArray)
            {
                Destroy(child.gameObject);
            }
            btnArray.Clear();
        }

        void OnDestroy()
        {
            for (int i = 0; i < m_guideBtnID.Count; i++)
            {
                //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID[i]);
            }

            if (m_btnEnableEffect != null)
                Destroy(m_btnEnableEffect);
        }

        public void ShowPanelBtn()
        {
            Showing = true;
            foreach (LocalSysButton child in btnArray)
            {
                if (child != null)
                {
                    if(!child.m_DefaultEnable)
                    child.ShowBtn(true);
                }
            }
        }

        public void ClosePanleBtn()
        {
            Showing = false;
            foreach (LocalSysButton child in btnArray)
            {
                if (child != null)
                {
                    if(!child.m_DefaultEnable)
                    child.CloseBtn();
                }
            }
        }

        void ResetBtnPosition()//重置按钮位置
        {
            foreach (LocalSysButton child in btnArray)
            {
                child.ResetPosition(btnArray.IndexOf(child));
            }
        }

        public void RemoveButton(MainUI.UIType sysBtnType)//移除功能按钮
        {
            foreach (LocalSysButton child in btnArray)
            {
                if (child.ButtonFunc == sysBtnType)
                {
                    btnArray.Remove(child);
                    child.DestroyMyself();
                    ResetBtnPosition();
                    break;
                }
            }
        }

        public bool ContainsButton(MainUI.UIType ButtonType)
        {
            foreach (LocalSysButton child in btnArray)
            {
                if (child.ButtonFunc == ButtonType)
                {
                    return true;
                }
            }
            return false;
        }

        public void ShowBtnEffect(MainUI.UIType buttonType)
        {

            btnArray.ApplyAllItem(P=>P.ShowBtnEff(buttonType));
        }
        public void HideBtnEffect(MainUI.UIType buttonType)
        {
           
            btnArray.ApplyAllItem(P=>P.HideBtnEff(buttonType));
        }

        public void PlayBtnAnimation(MainUI.UIType buttonType)
        {
            //TraceUtil.Log("播放按钮动画:"+buttonType+","+btnArray.Count);
            btnArray.ApplyAllItem(P=>P.PlayBtnAnim(buttonType));
        }

        public void StopBtnAnimation(MainUI.UIType buttonType)
        {
            btnArray.ApplyAllItem(P => P.StopBtnAnim(buttonType));
        }
    }
}
