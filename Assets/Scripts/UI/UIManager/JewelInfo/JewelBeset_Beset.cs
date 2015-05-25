using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace UI.MainUI
{
    public class JewelBeset_Beset :JewelBesetUIBase {
	private JewelBeset_Hole FirstHolePos;
	private JewelBeset_Hole SecondHolePos;
	public GameObject JewelBeset_Hole_prefab;
	public Transform FirstHolePoint;
	public Transform SecondHolePoint;
	public GameObject jewelAttriture_prefab;
	public Transform jewelAttriturePoint,suitAttriturePoint;
	private JewelAttriture jewelAttriture,suitAttriture;
	public GameObject NoneAttriture;
	public GameObject SuitActiv_Effect_prefab;
    public Transform SuitActiv_Effect_point;
	private GameObject SuitActiv_Effect;
	public GameObject RemoveJewelEffect1_profab,RemoveJewelEffect2_profab;
	public Transform RemoveJewelEffect1_Point,RemoveJewelEffect2_point;
	private GameObject RemoveJewelEffect1,RemoveJewelEffect2;
	private ItemFielInfo CurrentItemfielInfo;
    private JewelBset_ContainerItem CurrentSelectJewelItem;
	void Awake()
		{
			RegisterEventHandler();
			FirstHolePos=CreatObjectToNGUI.InstantiateObj(JewelBeset_Hole_prefab,FirstHolePoint).GetComponent<JewelBeset_Hole>();
			SecondHolePos=CreatObjectToNGUI.InstantiateObj(JewelBeset_Hole_prefab,SecondHolePoint).GetComponent<JewelBeset_Hole>();
			jewelAttriture=CreatObjectToNGUI.InstantiateObj(jewelAttriture_prefab,jewelAttriturePoint).GetComponent<JewelAttriture>();
			suitAttriture=CreatObjectToNGUI.InstantiateObj(jewelAttriture_prefab,suitAttriturePoint).GetComponent<JewelAttriture>();
			SuitActiv_Effect=CreatObjectToNGUI.InstantiateObj(SuitActiv_Effect_prefab,SuitActiv_Effect_point)as GameObject;
			SuitActiv_Effect.SetActive(false);
			RemoveJewelEffect1=CreatObjectToNGUI.InstantiateObj(RemoveJewelEffect1_profab,RemoveJewelEffect1_Point)as GameObject;
			RemoveJewelEffect2=CreatObjectToNGUI.InstantiateObj(RemoveJewelEffect2_profab,RemoveJewelEffect2_point)as GameObject;
			RemoveJewelEffect1.SetActive(false);
			RemoveJewelEffect2.SetActive(false);
            NoneAttriture.GetComponentInChildren<UILabel>().SetText(LanguageTextManager.GetString("IDS_I9_16"));
            //Init(null);
            TaskGuideBtnRegister();
        }
    /// <summary>
    /// ������ťע�����
    /// </summary>
    private void TaskGuideBtnRegister()
    {
        FirstHolePos.buttonCallBack.gameObject.RegisterBtnMappingId(UIType.Gem, BtnMapId_Sub.Gem_JewelBesetPanel_JewlBasetHoleLeft);
        SecondHolePos.buttonCallBack.gameObject.RegisterBtnMappingId(UIType.Gem, BtnMapId_Sub.Gem_JewelBesetPanel_JewlBasetHoleRight);
    }

		public IEnumerator ShowRemoveJewelEffect1()
		{
			RemoveJewelEffect1.SetActive(true);
			JewelBesetManager.GetInstance().UpdateBesetPanel(CurrentItemfielInfo);
			yield return new WaitForSeconds(2.5f);
			RemoveJewelEffect1.SetActive(false);
			JewelBesetManager.GetInstance().ChangeSubUistate(JewelState.JewelBeset);
			JewelBesetManager.GetInstance().UPdateContain(JewelState.JewelBeset,CurrentItemfielInfo);
		}
		public IEnumerator ShowRemoveJewelEffect2()
		{
			RemoveJewelEffect2.SetActive(true);
			yield return new WaitForSeconds(2.5f);
			RemoveJewelEffect2.SetActive(false);
			JewelBesetManager.GetInstance().ChangeSubUistate(JewelState.JewelBeset);
			JewelBesetManager.GetInstance().UPdateContain(JewelState.JewelBeset,CurrentItemfielInfo);
		}

		public  void Init(ItemFielInfo selectedEquipItem)
		{
            FirstHolePos.Init(selectedEquipItem,HoleIndex.FirstHole);
            SecondHolePos.Init(selectedEquipItem,HoleIndex.SecondHole);
            if(selectedEquipItem!=null)
            {
                CurrentItemfielInfo=selectedEquipItem;
                List<JewelInfo> jewelInfos=PlayerDataManager.Instance.GetJewelInfo((EquiptSlotType)CurrentItemfielInfo.sSyncContainerGoods_SC.nPlace);
                if(jewelInfos[0].JewelID!=0&&jewelInfos[1].JewelID!=0&&jewelInfos[0].JewelID!=jewelInfos[1].JewelID)
			  {
                    Jewel Jewel1=ItemDataManager.Instance.GetItemData(jewelInfos[0].JewelID)as Jewel;
                    Jewel Jewel2=ItemDataManager.Instance.GetItemData(jewelInfos[1].JewelID) as Jewel;
                    if(Jewel1.StoneGrop!=0&&Jewel1.StoneGrop==Jewel2.StoneGrop)
				  {
					  SuitActiv_Effect.SetActive(true);
				  }
			
			  }
			  else
			  {
				  SuitActiv_Effect.SetActive(false);
			  }
            }
          
		}

		public void InitAttribute(ItemFielInfo itemFileInfo,JewelBset_ContainerItem item)
		{
            if(CurrentSelectJewelItem!=null&&CurrentSelectJewelItem.ItemFielInfo.LocalItemData._GoodsClass==3)
            {
                if(CurrentSelectJewelItem.CanCancel)
                {
                CurrentSelectJewelItem.OnLoseFocus();
                }
              
            }
            CurrentSelectJewelItem=item;
            if(itemFileInfo!=null&&itemFileInfo.LocalItemData._GoodsClass==3&&itemFileInfo.LocalItemData._GoodsSubClass==3)
			{
				NoneAttriture.SetActive(false);
				jewelAttriture.gameObject.SetActive(true);
				suitAttriture.gameObject.SetActive(true);
				jewelAttriture.Init(itemFileInfo,false);
                //if(itemFileInfo.)
                
                var ItemId = itemFileInfo.LocalItemData._goodID;
               var jewel = ItemDataManager.Instance.GetItemData(ItemId) as Jewel;
                if(jewel.StoneGrop==0)
                {
                    suitAttriture.gameObject.SetActive(false);
                }
                else
                {
				    suitAttriture.Init(itemFileInfo,true);
                    suitAttriture.gameObject.SetActive(true);
                }
			}
			else
			{
				NoneAttriture.SetActive(true);
				jewelAttriture.gameObject.SetActive(false);
				suitAttriture.gameObject.SetActive(false);
			}
		}


		public void ReceiveRemoveJewelHandel(object arg)
		{
			SMsgGoodsOperateRemove_SC s= (SMsgGoodsOperateRemove_SC)arg  ;
			if(System.Convert.ToBoolean( s.bySucess))
			{
                SoundManager.Instance.PlaySoundEffect("Sound_UIEff_Stone_Remove");
				int m=s.Place;
				if(m==1)
				{

					StartCoroutine(ShowRemoveJewelEffect1());

				}
				else
				{
                  
					StartCoroutine(ShowRemoveJewelEffect2());
				}
			}
		}

        public void DisableAllHoleButton()
        {
            FirstHolePos.SetButtonDisable();
            SecondHolePos.SetButtonDisable();
        }

        public void EnableAllHoleButton()
        {
            FirstHolePos.SetButtonEnable();
            SecondHolePos.SetButtonEnable();
        }
        void OnDestroy()
        {
            DeRegisterEventHandler();
        }


        void DeRegisterEventHandler()
        {
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ReceiveRemoveJewel,ReceiveRemoveJewelHandel);
        }


		protected override  void RegisterEventHandler()
		{
		
			UIEventManager.Instance.RegisterUIEvent(UIEventType.ReceiveRemoveJewel,ReceiveRemoveJewelHandel);
		}
}
}
