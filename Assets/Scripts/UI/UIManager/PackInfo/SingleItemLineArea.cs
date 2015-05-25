using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace UI.MainUI
{

    public class SingleItemLineArea : MonoBehaviour
    {

        public GameObject SinglePackItemPrefab;
        public GameObject PackLockPrefab;
        public GameObject UnLockPackEffectPrefab;

		public ContainerItemListPanel MyParent{get;private set;}
		public int MyIndex{get;private set;}
		public bool IsLock{get;private set;}
		private GameObject UnlockPackEffect;
		private SingleButtonCallBack PackLockCallBack;

		List<SinglePackItemSlot> m_SingleItemList = new List<SinglePackItemSlot>();
        private float m_dragAmount;
        private Action<float> m_dragAmountSlerpAct;

        public void Init(int lineIndex, float itemLineAreaNum, bool islock, ContainerItemListPanel myParent
            , Action<float> dragAmountSlerpAct)
        {
			MyIndex = lineIndex;
            this.MyParent = myParent;
			transform.localPosition = new Vector3(0, 140 - (94 * lineIndex), 0);
			IsLock = islock;
            m_dragAmount=lineIndex/itemLineAreaNum;
            m_dragAmountSlerpAct = dragAmountSlerpAct;

            UpdateStatus();
        }
        /// <summary>
        /// 是否有引导箭头
        /// </summary>
        /// <returns></returns>
        public bool HasGuideArrow
        {
            get
            {
                foreach (var item in m_SingleItemList)
                {
                    var btnGuideBehaviour = item.GetComponent<GuideBtnBehaviour>();
                    if (btnGuideBehaviour != null)
                    {
                        if (btnGuideBehaviour.BtnFrame != null)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }
        //
        public int GetActiveEnergyHaveGold(int UnlockBox)
        {
            // (向下取整 ((参数1×（购买次数）^2+参数2×购买次数+参数3)/参数4)×参数4)
            //CommonDefineManager.Instance.CommonDefine.BuyEnergyConsumption1
            int a = CommonDefineManager.Instance.CommonDefine.PackageUnlockConsumption1;
            int b = CommonDefineManager.Instance.CommonDefine.PackageUnlockConsumption2;
            int c = CommonDefineManager.Instance.CommonDefine.PackageUnlockConsumption3;
            int d = CommonDefineManager.Instance.CommonDefine.PackageUnlockConsumption4;
            //int times = PlayerDataManager.Instance.GetenergyPurchaseTimes() - PlayerManager.Instance.FindHeroDataModel ().PlayerValues.PLAYER_FIELD_CANBUYACTIVE_NUM;
            int val = Mathf.FloorToInt ( ((a*UnlockBox*UnlockBox + b*UnlockBox + c)/(float)d) )*d;
            return val;
        }
        void UpdateStatus()
		{
			if(IsLock)
			{
				PackLockCallBack = CreatObjectToNGUI.InstantiateObj(PackLockPrefab,transform).GetComponent<SingleButtonCallBack>();
				PackLockCallBack.transform.localScale = new Vector3(1.05f, 0.85f, 1);
				PackLockCallBack.transform.localPosition = Vector3.zero;
				PackLockCallBack.SetCallBackFuntion(OnLockBtnClick);
				// 计算解锁的格数
				int UnlockBox = (ContainerInfomanager.Instance.GetContainerClientContsext(2).wMaxSize/ 4)- 4;
                int tackGold = GetActiveEnergyHaveGold(UnlockBox);
				PackLockCallBack.SetButtonText(tackGold.ToString());

                // 引导注入代码
                PackLockCallBack.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_PackUnLock);
			}else
			{
				List<ItemFielInfo> allPackItemList = MyParent.MyPackItemList;
				for(int i = MyIndex*4, iMax = (MyIndex+1)*4;	i < iMax;	i++)
				{
					//ItemFielInfo currentItemFile = allPackItemList.FirstOrDefault(P=>P.sSyncContainerGoods_SC.nPlace == i);//按后台位置排序
					ItemFielInfo currentItemFile = allPackItemList.Count>i? allPackItemList[i]: null;
					ItemFielInfo bestItem = null;
					EquiptSlotType myItemType = EquiptSlotType.Null;
					if(currentItemFile!=null)
					{
						myItemType = getEquiptType(currentItemFile);
					}
					MyParent.BestItemList.TryGetValue(myItemType,out bestItem);
					bool isBestItem = bestItem!=null&&bestItem == currentItemFile;
					SinglePackItemSlot newItemSlot = CreatObjectToNGUI.InstantiateObj(SinglePackItemPrefab,transform).GetComponent<SinglePackItemSlot>();
					newItemSlot.gameObject.transform.localScale =new  Vector3(0.85f, 0.85f, 1);
					newItemSlot.transform.localPosition = new Vector3(-136 + 90 * (i - MyIndex * 4), 0, 0);
					newItemSlot.Init(currentItemFile,isBestItem,SinglePackItemSlot.ItemStatus.PackItem,MyParent.MyParent.OnItemClick);
					m_SingleItemList.Add(newItemSlot);

                    //引导注入
                    if (currentItemFile != null)
                    {
                        newItemSlot.gameObject.RegisterBtnMappingId(currentItemFile.LocalItemData._goodID, UIType.Package, BtnMapId_Sub.Package_Cell
                            , m_dragAmountSlerpAct, m_dragAmount);
                    }
				}
			}
		}

		public void SetItemSelectStatus(object obj)
		{
			m_SingleItemList.ApplyAllItem(P=>P.SetSelectStatus(obj as ItemFielInfo));
		}

		EquiptSlotType getEquiptType(ItemFielInfo itemInfo)
		{
			EquiptSlotType  getType = EquiptSlotType.Null;
			switch (itemInfo.LocalItemData._GoodsSubClass)
			{
			case 1:
				getType = EquiptSlotType.Weapon;
				break;
			case 2:
				break;
			case 3:
				getType = EquiptSlotType.Heard;
				break;
			case 4:
				getType = EquiptSlotType.Body;
				break;
			case 5:
				getType = EquiptSlotType.Shoes;
				break;
			case 6:
				getType = EquiptSlotType.Accessories;
				break;
			case 7:
				getType = EquiptSlotType.Medicine;
				break;
			default:
				break;
			}
			return getType;
		}

		/// <summary>
		/// 当点击了解锁背包按钮
		/// </summary>
		void OnLockBtnClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_PackageUnlockButton");
			MyParent.OnUnLockContainerBoxBtnClick();
		}

		public void UnLockMySelf()
		{
			PackLockCallBack.SetCallBackFuntion(null);
			GameObject unLockEffect = CreatObjectToNGUI.InstantiateObj(UnLockPackEffectPrefab,	transform);
			unLockEffect.transform.localScale = new Vector3(1.05f, 0.85f, 1);
			unLockEffect.transform.localPosition +=new Vector3(0,0,-10);
			DoForTime.DoFunForTime(2.5f,DestroyObj,unLockEffect);
			DoForTime.DoFunForTime(1f,CreatNewItemSlot,null);
		}
		/// <summary>
		/// 创建出新的背包格
		/// </summary>
		/// <param name="obj">Object.</param>
		void CreatNewItemSlot(object obj)
		{
			IsLock = false;
			UpdateStatus();
			DestroyObj(PackLockCallBack.gameObject);
		}

		void DestroyObj(object obj)
		{
			GameObject destroyObj = obj as GameObject;
			if(destroyObj!=null)
			{
				Destroy(destroyObj);
			}
		}

    }
}