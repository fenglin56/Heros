using UnityEngine;
using System.Collections;
using System.Linq;

namespace UI.MainUI
{

    public class OperateSophisticationMsgPanel : MonoBehaviour
    {
        public UILabel MsgLabel;
        public SingleButtonCallBack SureBtn;
        public SingleButtonCallBack CancelBtn;

        public GoodsOperateArtificePanel MyParent{ get; private set; }
        public ItemFielInfo SelectItem { get; private set; }

        private int[] m_guideID;

        void Start()
        {
            MsgLabel.SetText(LanguageTextManager.GetString("IDS_H1_522"));
            //洗炼会重置装备的技能，但不会影响技能等级，确认要洗炼吗？
            Close();
            SureBtn.SetCallBackFuntion(OnSureBtnClick);
            CancelBtn.SetCallBackFuntion(OnCancelBtnCLick);

            m_guideID = new int[2];
            //TODO GuideBtnManager.Instance.RegGuideButton(SureBtn.gameObject, UIType.EquipStrengthen, SubType.EquipStrenMsg, out m_guideID[0]);
            //TODO GuideBtnManager.Instance.RegGuideButton(CancelBtn.gameObject, UIType.EquipStrengthen, SubType.EquipStrenMsg, out m_guideID[1]);
        }

        void OnDestroy()
        {
            for (int i = 0; i < m_guideID.Length; i++)
            {
                //TODO GuideBtnManager.Instance.DelGuideButton(m_guideID[i]);
            }
        }

        public void Show(ItemFielInfo selectItem,GoodsOperateArtificePanel myParent)
        {
            gameObject.SetActive(true);
            transform.localPosition = new Vector3(290,-86,-50);
            this.SelectItem = selectItem;
            this.MyParent = myParent;
        }

        void OnSureBtnClick(object obj)
        {
            SendGoodsOperateSophisticationToSever();
        }

        void SendGoodsOperateSophisticationToSever()
        {
            int shortcutItemid = CommonDefineManager.Instance.CommonDefineFile._dataTable.ShortcutItem_PassiveSkill;
            var shopData = ShopDataManager.Instance.GetShopData(shortcutItemid);
            var containerData = ContainerInfomanager.Instance.itemFielArrayInfo.FirstOrDefault(P => P.LocalItemData._goodID == shopData.GoodsID);
            if (containerData == null)
            {
                UIEventManager.Instance.TriggerUIEvent(UIEventType.QuickPurchase, shortcutItemid);
            }
            else
            {
                MyParent.SendResetPassiveSkillToSever(SelectItem);
            }
            Close();
        }

        void OnCancelBtnCLick(object obj)
        {
            Close();
        }

        void Close()
        {
            gameObject.SetActive(false);
        }
    }
}