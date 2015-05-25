using UnityEngine;
using System.Collections;

namespace UI.MainUI
{

    public class ContainerBoxSlot_V2 : MonoBehaviour
    {
        public UISprite LockIcon;
        public UISprite Background;
        public GameObject ContainerBoxPrefab;//需要生成的物品格
        public Transform CreatContainerBoxPoint;

        private Vector3 Background_Scale;

        public SingleContainerBox MyContainerBox { get; private set; }
        public ContainerBoxSlotData     MyContainerBoxSlotData { get; private set; }
        private ContainerPackList_V2 myParent;
        public int m_guideBtnID;

        public void Start()
        {
            //TODO GuideBtnManager.Instance.RegGuideButton(gameObject, UI.MainUI.UIType.Package, SubType.PackageContainerBoxSlot, out m_guideBtnID);            
            Background_Scale = Background.transform.localScale;
        }

        void OnDestroy()
        {
            //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID);
        }

        public void Init(ContainerBoxSlotData containerBoxSlotData,ContainerPackList_V2 myParent)
        {
            //TraceUtil.Log("InitContainerBox:" + containerBoxSlotData.CurrentPlace);
            this.myParent = myParent;
            this.MyContainerBoxSlotData = containerBoxSlotData;
            LockIcon.enabled = containerBoxSlotData.IsLock;
            if (containerBoxSlotData.itemfileInfo != null)
            {
                CreatItem(containerBoxSlotData.itemfileInfo);
            }
            else
            {
                ClearUpItem();
            }           
        }

        public bool IsLock { get { return MyContainerBoxSlotData.IsLock; } }

        public void CreatItem(ItemFielInfo itemFielInfo)
        {
            //CreatContainerBoxPoint.BroadcastMessage("OnCustomerDestroy", SendMessageOptions.DontRequireReceiver);
            CreatContainerBoxPoint.ClearChild();
            GameObject creatObj = CreatObjectToNGUI.InstantiateObj(ContainerBoxPrefab, CreatContainerBoxPoint);
            MyContainerBox = creatObj.GetComponent<SingleContainerBox>();
            MyContainerBox.Init(itemFielInfo, SingleContainerBoxType.Container);
        }

        public void SetSelectStatusActive(bool flag)
        {
            if (MyContainerBox != null)
            {
                MyContainerBox.SetSelectStatus(flag);
            }
        }

        public void ClearUpItem()
        {
            //CreatContainerBoxPoint.BroadcastMessage("OnDestroy", SendMessageOptions.DontRequireReceiver);
            CreatContainerBoxPoint.ClearChild();
            MyContainerBox = null;
        }

        //public override void OnDragComponentHover()
        //{
        //    OnTouchSlot();
        //}

        /// <summary>
        /// 检测是否能够拖拽进来
        /// </summary>
        /// <param name="dragChild"></param>
        /// <returns></returns>
        //public override bool CheckIsPair(DragComponent dragChild)
        //{
        //    bool flag = false;
        //    if (!MyContainerBoxSlotData.IsLock)
        //    {
        //        if (MyContainerBoxSlotData.itemfileInfo == null)
        //        {
        //            flag = true; 
        //        }
        //        else if ((dragChild as SingleContainerBox).singleContainerBoxType == SingleContainerBoxType.Container)
        //        {
        //            flag = true; 
        //        }
        //    }
        //    return flag;
        //}

        /// <summary>
        /// 拖拽的物体移动到这里来
        /// </summary>
        /// <param name="enterComponent"></param>
        public void MoveToHere(DragComponent enterComponent)
        {
            var itemData = enterComponent as SingleContainerBox;

            //NewbieGuideManager_V2.Instance.IsDragGuide(m_guideBtnID, itemData.GuideID);

            if (MyContainerBox == itemData)
            {
                TraceUtil.Log("相同物品");
                //MyContainerBox.UpdatePos();
                //myParent.MyParent.CloseContainerTips();
                //TraceUtil.Log("MyContainerBox");
                return;
            }
            else
            {
                TraceUtil.Log("新物品,MyPlace:"+MyContainerBoxSlotData.CurrentPlace);
            }
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Equip");
            myParent.OnDragComponentToSlot(this, itemData);

        }

        void OnClick()
        {
            if (MyContainerBoxSlotData.IsLock == true)
            {
                myParent.OnUnlockContainerBtnClick();
            }
        }

        public void OnTouchSlot()
        {
            TweenScale.Begin(Background.gameObject, 0.1f, Background_Scale, Background_Scale + new Vector3(0, 10, 0), ScaleComplete);
        }

        void ScaleComplete(object obj)
        {
            TweenScale.Begin(Background.gameObject, 0.1f, Background_Scale);
        }

    }
}