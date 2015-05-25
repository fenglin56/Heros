using UnityEngine;
using System.Collections;

namespace UI.MainUI
{

    public class SingleArtificeGoodsSlot : MonoBehaviour
    {
        public SpriteSwith SelectStatusIcon;
        public GameObject SingleContainerBoxPrefab;
        public GameObject SirenItemPrefab;//妖女内丹面板
        public Transform CreatContainerBoxTransform;

        public SingleContainerBox singleContainerBox { get; private set; }
        public SingleSirenItem singleSirenItem { get; private set; }
        public SelectArtificeGoodsPanel MyParent { get; private set; }
        public ArtificeItemFielInfo MyData { get; private set; }
        public bool IsSirenItem { get; private set; }

        private int m_guideID = 0;

        void Awake()
        {
            SelectStatusIcon.ChangeSprite(IsSelect() ? 1 : 0);
            //TODO GuideBtnManager.Instance.RegGuideButton(gameObject, UIType.EquipStrengthen, SubType.EquipStrenOperateItem, out m_guideID);
        }

        void OnDestroy()
        {
            //TODO GuideBtnManager.Instance.DelGuideButton(m_guideID);
        }

        //public void Init(SingleSelectArtificeGoodsDragPanel myParent)
        //{
        //    this.MyParent = myParent;
        //}

        public void Show(ArtificeItemFielInfo itemFielInfo, SelectArtificeGoodsPanel MyParent)
        {
            this.MyParent = MyParent;
            IsSirenItem = false;


            CreatContainerBoxTransform.ClearChild();

            this.MyData = itemFielInfo;
            SelectStatusIcon.ChangeSprite(IsSelect() ? 1 : 0);
            if (itemFielInfo == null)
            {
                return;
            }
            
            //TraceUtil.Log("显示物品："+LanguageTextManager.GetString(itemFielInfo.MyItemfielInfo.LocalItemData._szGoodsName));
            singleContainerBox = CreatObjectToNGUI.InstantiateObj(SingleContainerBoxPrefab, CreatContainerBoxTransform).GetComponent<SingleContainerBox>();

            singleContainerBox.Init(itemFielInfo.MyItemfielInfo, SingleContainerBoxType.HeroEquip);
            singleContainerBox.GetComponent<BoxCollider>().enabled = false;
        }
        /// <summary>
        /// 显示妖女内丹
        /// </summary>
        public void ShowSirenItem(SelectArtificeGoodsPanel MyParent)
        {
            this.MyParent = MyParent;
            IsSirenItem = true;
            CreatContainerBoxTransform.ClearChild();
            singleSirenItem = CreatObjectToNGUI.InstantiateObj(SirenItemPrefab,CreatContainerBoxTransform).GetComponent<SingleSirenItem>();
            singleSirenItem.Show();
        }

        public bool IsSelect()
        {
            return MyData != null && MyData.IsSelect;
        }

        void OnClick()
        {
            if (MyData == null)
                return;
            if (IsSirenItem)
            {
                //显示快捷购买妖女内丹面板
            }
            else
            {
                if (MyParent.CheckCanSelect()||MyData.IsSelect)
                {
                    MyData.IsSelect = !MyData.IsSelect;
                    SelectStatusIcon.ChangeSprite(IsSelect() ? 1 : 0);
                    MyParent.MyParent.UpdateSkillProgressBar();
                    //TraceUtil.Log("刷新进度条");
                }
                else
                {
                    //MessageBox.Instance.ShowTips(3, "每次只能炼化6件装备", 1);
                    MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H1_520"), 1);
                    //TraceUtil.Log("选择已满");
                }
            }
        }

        public void Cleanup()
        {
            CreatContainerBoxTransform.ClearChild();
        }

    }
}