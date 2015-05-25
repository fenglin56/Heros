using UnityEngine;
using System.Collections;
using UI.MainUI;
namespace UI.Forging
{
public class ForgingPanelManager : BaseUIPanel {
        public ForgingContainList Sc_ForgingContainList;
        public ForgingResult Sc_ForgingResult;
        public SingleButtonCallBack BackButton;
        public SingleButtonCallBack ForgeButton;
        public GameObject Prefab_ForgeButtonEff;
        private GameObject Eff_ForgeButton;
        private static ForgingPanelManager instance;
        public GameObject EffProfab;
        public Transform EffPoint;
        public SelectButtonPanel selectButtonPanel;
        [HideInInspector]
        public ForgeRecipeData CurrentData;
        public ForgingType DefultType=ForgingType.ForgEquipment;
        public ForgingType CurrentForingType {get;private set;}

        public void SetCurrentForingType(ForgingType type)
        {
            CurrentForingType=type;
        }


        public static ForgingPanelManager GetInstance ()
        {
            if (!instance) {
                instance = (ForgingPanelManager)GameObject.FindObjectOfType (typeof(ForgingPanelManager));
                if (!instance)
                    Debug.LogError ("没有附加JewelBesetManager脚本的gameobject在场景中");
            }
            return instance;
        }
        
     
        void Awake()
        {
            Eff_ForgeButton=CreatObjectToNGUI.InstantiateObj(Prefab_ForgeButtonEff,ForgeButton.transform);

            CurrentForingType=DefultType;
            BackButton.SetCallBackFuntion(OnBackButtonClick);
            ForgeButton.SetCallBackFuntion(OnForgeButtonClick);
            RegUIEvent();

            TaskGuideBtnRegister();
        }
        /// <summary>
        /// 引导按钮注入代码
        /// </summary>
        private void TaskGuideBtnRegister()
        {
            BackButton.gameObject.RegisterBtnMappingId(UIType.Forging, BtnMapId_Sub.Forging_Back);
            ForgeButton.gameObject.RegisterBtnMappingId(UIType.Forging, BtnMapId_Sub.Forging_Forging);
			selectButtonPanel.gameObject.RegisterBtnMappingId(UIType.Forging, BtnMapId_Sub.Forging_MainBtn);
        }
        public override void Show(params object[] value)
        {
            base.Show(value);

			if(value.Length>0)
			{
				CurrentForingType=(ForgingType)value[0];
			}
			else
			{
            	CurrentForingType=ForgingType.ForgEquipment;
			}
            selectButtonPanel.UpdateSelectButton();
            InitList();
        }
      
        public void InitList()
        {
            int id;
        
            Sc_ForgingContainList.InitListItem();
        }
        public void UpdateList()
        {
         
            Sc_ForgingContainList.UpdateList();
        }

        public void CheckButtonEff()
        {
            int id;
            if(CheckMatetiralEnough(out id))
            {
                Eff_ForgeButton.SetActive(true);
            }
            else
            {
                Eff_ForgeButton.SetActive(false);
            }
        }

        public override void Close()
        {
            Sc_ForgingContainList.SelectedItem=null;
            CurrentForingType=DefultType;
            CloseSelectPanel();
            CurrentData=null;
            base.Close();

        }
        void OnForgeButtonClick(object obj)
        {
            CloseSelectPanel();
            if(CurrentData!=null)
            {
                SoundManager.Instance.PlaySoundEffect("Sound_Button_Forge_Click");
                if(JudgeConditions())
                {
                    NetServiceManager.Instance.EquipStrengthenService.SendSEquipMakeMsg(System.Convert.ToUInt32(CurrentData.ForgeID));
                    LoadingUI.Instance.Show();
                    ForgeButton.SetButtonColliderActive(false);
                }
            }

        }

        bool  CheckMatetiralEnough(out int ItemId)
        {
            bool can =true;
            ItemId=0;
            for (int i = 0; i < CurrentData.ForgeCost.Length; i++)
            {
                if (ForgingRecipeConfigDataManager.Instance.GetOwnMaterialCount(CurrentData.ForgeCost[i].RecipeID) < CurrentData.ForgeCost[i].count)
                {
                    ItemId=CurrentData.ForgeCost[i].RecipeID;
                    can=false;
                    break;
                }
            }
            return can;
        }

        bool JudgeConditions()
        {
            //bool conditions=true;
            //背包是否已满
            if(ContainerInfomanager.Instance.PackIsFull())
            {
                MessageBox.Instance.ShowTips(2,LanguageTextManager.GetString("IDS_I12_7"),1);
                return false;
            }

            int id;
            if(!CheckMatetiralEnough(out id))
            {
                
                SoundManager.Instance.PlaySoundEffect("Sound_Button_Forge_Warning");
                MessageBox.Instance.ShowTips(4, string.Format(LanguageTextManager.GetString("IDS_I12_8"), ForgingRecipeConfigDataManager.Instance.GetGoodsName(id)), 1f);
                return false;
            }
      

            if(ContainerInfomanager.Instance.GetEmptyPackBoxNumber()<0)
            {
                MessageBox.Instance.ShowTips(4,LanguageTextManager.GetString("IDS_I12_7"),1f);
                return false;
            }
            else
            {
                return true;
            }

        }
        void OnBackButtonClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect ("Sound_Button_Equipment_Cancel");
            this.Close ();
        }
        public void CloseSelectPanel()
        {
            selectButtonPanel.btn_ChoseForgingType.CancelSelect();
        }

        IEnumerator ShowEff()
        {
            CreatObjectToNGUI.InstantiateObj(EffProfab,EffPoint);
            yield return new WaitForSeconds(2);
            EffPoint.ClearChild();
            ForgeButton.SetButtonColliderActive(true);
        }
        void  ForgingComHandle(object obj)
        {
            UpdateList();
            SMsgGoodsOperateEquipMake_SC sMsgGoodsOperateEquipMake_SC=(SMsgGoodsOperateEquipMake_SC)obj;
            LoadingUI.Instance.Close();
            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_Forge_Success");
            Sc_ForgingResult.RefreshCurrentResult();
            //teList();
            if(sMsgGoodsOperateEquipMake_SC.bySucess==1)
            {
                StartCoroutine(ShowEff());
            }
        }
        void RegUIEvent()
        {
            UIEventManager.Instance.RegisterUIEvent(UIEventType.Forging,ForgingComHandle);
        }
        void OnDestroy()
        {
            instance = null;
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.Forging,ForgingComHandle);
        }
}
    public enum ForgingType
    {
        ForgEquipment=1,
        ForgGiftBox,
        ForgMaterial,
    }
}