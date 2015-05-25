using UnityEngine;
using System.Collections;
using System;

namespace UI.MainUI
{
    public class JewelBset_ContainerItem : MonoBehaviour
    {
        public Transform EquipIconPoint;
        public UISprite FocusEff;
        public SpriteSwith EquipBg;
        public SpriteSwith EquipStar;
        public UISprite EquipedIcon;
        public GameObject LevelInfo;
        public Action<JewelBset_ContainerItem> OnItemClick;//点击处理委托
        public bool CanStrength { get; private set; }
        public bool CanCancel=false;
        public ItemFielInfo ItemFielInfo { get; private set; }

        [HideInInspector]
        public float DragAmount;  //拖动目的Amount
        public GameObject JewelChoseIcon;
        private UILabel EquipLevel;
        public GameObject BesetEff1_profab;
        public Transform Eff_point;
        private GameObject BesetEff1;
        public GameObject SwallowEff1_prefab;
        private GameObject SwallowEff1;
        private TweenPosition Besettween, swallowTween;
          
        void Awake()
        {
            EquipLevel = LevelInfo.transform.FindChild("EquipLevel").GetComponent<UILabel>();
            //监听点击事件
            GetComponent<UIEventListener>().onClick = (obj) =>
            {
              
                SoundManager.Instance.PlaySoundEffect("Sound_Button_Stone_Click");
                if (OnItemClick != null)
                {

                    OnItemClick(this);

                }
            };
            BesetEff1 = CreatObjectToNGUI.InstantiateObj(BesetEff1_profab, Eff_point)as GameObject;
            Besettween = BesetEff1.AddComponent<TweenPosition>();
            SwallowEff1 = CreatObjectToNGUI.InstantiateObj(SwallowEff1_prefab, Eff_point)as GameObject;
            swallowTween = SwallowEff1.AddComponent<TweenPosition>();
            SwallowEff1.SetActive(false);
            BesetEff1.SetActive(false);
            //  BesetEff2=CreatObjectToNGUI.InstantiateObj(BesetEff2_prefab,Eff_point)as GameObject;
        }
         
        public IEnumerator ShowSwallowEff(Vector3 Targetpos)
        {

            SwallowEff1.SetActive(true);
            swallowTween.PossionTweenUseWorldPos = true;
            Vector3 from = new Vector3(Eff_point.position.x, Eff_point.position.y, Eff_point.position.z);
            Vector3 to = new Vector3(Targetpos.x, Targetpos.y, Targetpos.z);
            TweenPosition.Begin(SwallowEff1, 1.5f, from, to);
            yield return new WaitForSeconds(1.75f);
            swallowTween.PossionTweenUseWorldPos = false;
            BesetEff1.SetActive(false);
            StartCoroutine(JewelBesetManager.GetInstance().ShowSwallowSuccseeEff());
        }

        public IEnumerator ShowBesetEff(Vector3 Targetpos)
        {

            BesetEff1.SetActive(true);
            BesetEff1.transform.localPosition = Vector3.zero;
            yield return new WaitForSeconds(1.0f);
            Besettween.PossionTweenUseWorldPos = true;
            TweenPosition.Begin(BesetEff1, 1f, Eff_point.position, Targetpos);
            yield return new WaitForSeconds(1);
            Besettween.PossionTweenUseWorldPos = false;
            yield return new WaitForSeconds(0.85f);
            BesetEff1.SetActive(false);
            StartCoroutine(JewelBesetManager.GetInstance().ShowBesetSuccseeEff());
        }

        #region IPagerItem implementation
        public void OnBeSelected()
        {
            OnItemClick(this);

          
        }
    
        /// <summary>
        /// 获得焦点
        /// </summary>
        public void OnGetFocus(bool ShowChoseLable)
        {
        
                if (ShowChoseLable)
                {
                    JewelChoseIcon.SetActive(true);
                }
            
                FocusEff.gameObject.SetActive(true);

        }
        /// <summary>
        /// 失去焦点
        /// </summary>
        public void OnLoseFocus()
        {
            JewelChoseIcon.SetActive(false);
            FocusEff.gameObject.SetActive(false);
        }
        #endregion
        public void InitItemData(ItemFielInfo itemFielInfo)
        {
            if (itemFielInfo != null)
            {
                HideOrShowIcon(false);
                ItemFielInfo = itemFielInfo;
                //装备图标
                SetEquipIcon();      
                EquipBg.ChangeSprite(itemFielInfo.LocalItemData._ColorLevel+1);
                if (itemFielInfo.LocalItemData._GoodsClass == 1)
                {
                                        
                                    
                    //已装备提示
                    if (EquipedIcon != null)
                    {
                        long equipId = itemFielInfo.equipmentEntity.SMsg_Header.uidEntity;
                        bool isEquipEd = ContainerInfomanager.Instance.sSyncHeroContainerGoods_SCs.Exists(item => item.uidGoods == equipId);
                        EquipedIcon.gameObject.SetActive(isEquipEd);
                    }            
                    //等级及星级
                    var equipLevel = EquipItem.GetItemInfoDetail(this.ItemFielInfo, EquipInfoType.EquipStrenLevel);
                    if (equipLevel != "+0")
                    {
                        LevelInfo.SetActive(true);
                        EquipLevel.text = equipLevel;
                    } else
                    {
                        LevelInfo.SetActive(false);
                    }
                    var equipStarIndex = EquipItem.GetItemInfoDetail(this.ItemFielInfo, EquipInfoType.EquipStarColorIndex);
                    EquipStar.ChangeSprite(int.Parse(equipStarIndex));

                  
                } 
            } else
            {
                HideOrShowIcon(false);
                EquipBg.ChangeSprite(5);
                StopAllCoroutines();
                EquipIconPoint.ClearChild();
                TraceUtil.Log(SystemModel.Rocky, TraceLevel.Error, "装备为空-- EquipItem->InitItemData");
            }
        }

        void HideOrShowIcon(bool hide)
        {
            EquipStar.gameObject.SetActive(hide);
            EquipedIcon.gameObject.SetActive(hide);
            LevelInfo.gameObject.SetActive(hide);
        }


        /// <summary>
        /// 设置装备图标
        /// </summary>
        void SetEquipIcon()
        {
            if (EquipIconPoint.childCount > 0)
            {
                StopAllCoroutines();
                EquipIconPoint.ClearChild();
            }
            //TraceUtil.Log(LanguageTextManager.GetString(this.ItemFielInfo.LocalItemData._szGoodsName) + "," + ItemFielInfo.LocalItemData._goodID);
            if (this.ItemFielInfo.LocalItemData._picPrefab != null)
            {
                var skillIcon = CreatObjectToNGUI.InstantiateObj(this.ItemFielInfo.LocalItemData._picPrefab, EquipIconPoint);
                //skillIcon.transform.localScale = new Vector3(59, 59, 1);
            }
        }

    }
}
