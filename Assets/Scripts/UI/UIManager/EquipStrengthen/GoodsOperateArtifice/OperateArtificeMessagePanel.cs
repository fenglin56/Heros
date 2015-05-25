using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UI.MainUI
{
    /// <summary>
    /// 确认炼化装备面板
    /// </summary>
    public class OperateArtificeMessagePanel : MonoBehaviour
    {

        public UIPanel MyPanel;
        public GameObject SingleContainerBoxPrefab;
        public Transform[] CreatContainerBoxTransform;

        public SingleButtonCallBack SureBtn;
        public SingleButtonCallBack CancelBtn;

        public GameObject EffectPrefab;
        public Transform EffectTransform;

        public SelectArtificeGoodsPanel MyParent { get; private set; }

        Vector3 DisCardScale = new Vector3(0.8f,0.8f,0.8f);
        Vector3 ShowScale = Vector3.one;

        private int[] m_guideID;

        void Start()
        {
            SureBtn.SetCallBackFuntion(OnSureBtnClick);
            CancelBtn.SetCallBackFuntion(OnCancelBtnClick);
            gameObject.transform.localScale = DisCardScale;
            gameObject.SetActive(false);

            m_guideID = new int[2];
            //TODO GuideBtnManager.Instance.RegGuideButton(SureBtn.gameObject, UIType.EquipStrengthen, SubType.EquipStrenButton, out m_guideID[0]);
            //TODO GuideBtnManager.Instance.RegGuideButton(CancelBtn.gameObject, UIType.EquipStrengthen, SubType.EquipStrenButton, out m_guideID[1]);
        }

        public void TweenShow(List<ItemFielInfo> selectItemFileInfo, SelectArtificeGoodsPanel myParent)
        {
            TweenScale.Begin(gameObject,0.1f,transform.localScale,ShowScale,null);
            Show(selectItemFileInfo,myParent);
        }

        public void TweenClose()
        {
            TweenScale.Begin(gameObject,0.1f,transform.localScale,DisCardScale,TweenCloseComplete);
        }

        public void TweenCloseComplete(object obj)
        {
            gameObject.SetActive(false);
        }

        void Show(List<ItemFielInfo> selectItemFileInfo, SelectArtificeGoodsPanel myParent)
        {
            transform.localPosition = new Vector3(0, 0, -50);
            gameObject.SetActive(true);
            MyParent = myParent;
            CreatContainerBoxTransform.ApplyAllItem(P=>P.ClearChild());
            for (int i = 0; i < selectItemFileInfo.Count; i++)
            {
                GameObject creatObj = CreatObjectToNGUI.InstantiateObj(SingleContainerBoxPrefab,CreatContainerBoxTransform[i]);
                creatObj.GetComponent<SingleContainerBox>().Init(selectItemFileInfo[i], SingleContainerBoxType.HeroEquip);
                creatObj.GetComponent<BoxCollider>().enabled = false;
            }
        }

        void OnSureBtnClick(object obj)
        {
            ShowEffect();
        }

        void ShowEffect()
        {
            GameObject creatEffect = CreatObjectToNGUI.InstantiateObj(EffectPrefab,EffectTransform);
            SureBtn.SetButtonColliderActive(false);
            CancelBtn.SetButtonColliderActive(false);
            CreatContainerBoxTransform.ApplyAllItem(P => P.ClearChild());
            DoForTime.DoFunForTime(2, ShowEffectComplete, creatEffect);
            DoForTime.DoFunForTime(0.5f, HidePanel, null);
        }

        void HidePanel(object obj)
        {
            MyPanel.alpha = 0.01f;
        }

        void ShowEffectComplete(object obj)
        {
            GameObject effect = obj as GameObject;
            if (effect != null)
            {
                Destroy(effect);
            }
            transform.localScale = DisCardScale;
            gameObject.SetActive(false);
            MyPanel.alpha = 1;
            SureBtn.SetButtonColliderActive(true);
            CancelBtn.SetButtonColliderActive(true);
            SendOperateArtificeToSever();
        }

        void SendOperateArtificeToSever()
        {
            long equipUID = MyParent.SelectItemFileInfo.sSyncContainerGoods_SC.uidGoods;
            var selectItemList = MyParent.GetSelectItemList();
            uint artificeNum = (uint)selectItemList.Count;
            List<long> equipUidList = new List<long>();
            selectItemList.ApplyAllItem(P => equipUidList.Add(P.sSyncContainerGoods_SC.uidGoods));
            NetServiceManager.Instance.EquipStrengthenService.SendGoodsOperateArtificeCommoand(equipUID, artificeNum, equipUidList);
            TraceUtil.Log("发送洗练到服务器");
        }

        void OnCancelBtnClick(object obj)
        {
            TweenClose();
        }

        void OnDestroy()
        {
            if (m_guideID != null)
            {
                for (int i = 0; i < m_guideID.Length; i++)
                {
                    //TODO GuideBtnManager.Instance.DelGuideButton(m_guideID[i]);
                }
            }
        }
    }
}