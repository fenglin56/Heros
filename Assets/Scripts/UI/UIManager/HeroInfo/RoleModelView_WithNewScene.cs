using UnityEngine;
using System.Collections;
using System.Linq;
using System.IO;

namespace UI.MainUI
{

    public class RoleModelView_WithNewScene : View
    {
     

        public enum PanelType {HeroInfoPanel,FashionInfoPanel}

        public Transform CreatHeroModelTransform;
        public Transform CameraTarget;
        public GameObject CameraObj;

        public PanelType MyPanelType { get; private set; }

        private GameObject m_hero;
        private int CurrentFashionID = 0;
        private long CurrentWeaponID = -1;

        private float DragSpeed = 1f;

        PlayerGenerateConfigData playerGenerateConfigData;
        private GameObject ItemInEff;
        private float ItemInEffDelay;
        private GameObject ItemOutEff;
        private float ItemOutEffDelay;
        private SSyncContainerGoods_SC WeaponInfo;
        public static readonly string ASSET_ITEM_DATA_ICON_FOLDER = @"Assets\Effects\Prefab";

      void Awake()
        {
            RegisterEventHandler();
        }

        public void Init(PanelType panelType)
        {
            DragSpeed = CommonDefineManager.Instance.CommonDefineFile._dataTable.RoleTurnSpeed;
            MyPanelType = panelType;
            ShowHeroModelView();
        }

        void ChangeFashionAndWeap()
        {
            ChangeHeroFashion();
            CurrentWeaponID = -1;
            ChangeHeroWeapon(null);
        }

        void ShowHeroModelView()
        {
            int VocationID = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
            gameObject.SetActive(true);
            if (m_hero == null)
            {
                AssemblyPlayer(VocationID);
            }
            else
            {
                m_hero.SetActive(true);
            }
            CreatEff();
            ChangeFashionAndWeap();
            DoForTime.DoFunForTime(0.3f,Show,null);
            //Show();
        }
        void CreatEff()
        {
            ItemInEff= Instantiate(playerGenerateConfigData.Avatar_InEff[0].Eff) as GameObject;
            ItemInEffDelay=playerGenerateConfigData.Item_InEff[0].StartTime/1000f;
            ItemOutEff=Instantiate( playerGenerateConfigData.Avatar_OutEff[0].Eff)as GameObject;
            ItemOutEffDelay=playerGenerateConfigData.Item_OutEff[0].StartTime/1000f;
            ItemInEff.transform.parent=m_hero.transform;
            ItemInEff.SetActive(false);
            ItemInEff.transform.localPosition=Vector3.zero;
            ItemOutEff.transform.parent=m_hero.transform;
            ItemOutEff.transform.localPosition=Vector3.zero;
            ItemOutEff.SetActive(false);
        }
        public void OnDragBtnDrag(Vector2 dragDelta)
        {
            //TraceUtil.Log(dragDelta);
            m_hero.transform.localRotation = Quaternion.Euler(0f, -0.5f * dragDelta.x * DragSpeed, 0f) * m_hero.transform.localRotation;
        }

        void AssemblyPlayer(int VocationID)
        {
            if (this.m_hero != null)
            {
                Destroy(m_hero);
            }
            playerGenerateConfigData = PlayerDataManager.Instance.GetUIItemData((byte)VocationID);
            this.m_hero = RoleGenerate.GenerateRole(playerGenerateConfigData.PlayerName, playerGenerateConfigData.DefaultAvatar, true);
            this.m_hero.transform.parent = CreatHeroModelTransform;
            this.m_hero.transform.localEulerAngles=Vector3.zero;
            this.m_hero.transform.localPosition=Vector3.zero;
            RoleGenerate.AttachAnimation(this.m_hero, playerGenerateConfigData.PlayerName, playerGenerateConfigData.DefaultAnim, playerGenerateConfigData.Animations);
			string[] weaponPosition =  GetWeaponPosition();
            this.m_hero.gameObject.AddComponent<RoleViewClickEvent>();
            RoleGenerate.AttachWeapon(this.m_hero, weaponPosition, playerGenerateConfigData.WeaponObj,null);

            var shadowEff = GameObject.Instantiate(playerGenerateConfigData.ShadowEffect) as GameObject;
            shadowEff.name = "shadow";
            shadowEff.transform.parent = m_hero.transform;
            shadowEff.transform.localPosition = new Vector3(0, 1, 0);

            CreatHeroModelTransform.transform.position = MyPanelType == PanelType.FashionInfoPanel ? playerGenerateConfigData.Avatar_CharPos : playerGenerateConfigData.RoleUI_CharPos;
            CameraObj.transform.position = m_hero.transform.position + (MyPanelType == PanelType.FashionInfoPanel ? playerGenerateConfigData.Avatar_CameraPos : playerGenerateConfigData.RoleUI_CameraPos);
            CameraTarget.transform.position = m_hero.transform.position + (MyPanelType == PanelType.FashionInfoPanel ? playerGenerateConfigData.Avatar_CameraTargetPos : playerGenerateConfigData.RoleUI_CameraTargetPos);
            CameraObj.transform.LookAt(CameraTarget);
            this.m_hero.transform.localPosition = new Vector3(1000,0,0);
            //m_hero.SetActive(false);
        }

		string[] GetWeaponPosition()
		{
			string[] getPos = playerGenerateConfigData.WeaponPosition;
			switch (MyPanelType)
			{
			case PanelType.FashionInfoPanel:
				getPos = playerGenerateConfigData.Avatar_WeaponPos;
				break;
			case PanelType.HeroInfoPanel:
				getPos = playerGenerateConfigData.RoleUI_WeaponPosition;
				break;
			default:
				break;
			}
			return getPos;
		}


        public void Show()
        {

            gameObject.SetActive(true);
            this.m_hero.transform.localPosition = Vector3.zero;
            m_hero.transform.localPosition=Vector3.zero;
            PackInfoStateManager.Instance.StateChange(PackInfoStateType.InterFashion);
          //  this.m_hero.transform.localRotation = Quaternion.Euler(Vector3.zero);

           // PlayItemToAvatarAnim();
        }

        public void Show(object obj)
        {
            Show();
        }

        void PlayItemToAvatarAnim()
        {
            StopAllCoroutines();
            //StartCoroutine( PlayItemToAvatarAnimation( playerGenerateConfigData.RoleUI_ItemToAvatar_Ani));
        }
        IEnumerator PlayItemToAvatarAnimation (string[] AnimationName)
        {
            string[] anim = AnimationName[0].Split('+');
            this.m_hero.animation.CrossFade(anim[0]);
            yield return new WaitForSeconds(float.Parse(anim[1]) / 1000);
            PlayerChangeWaeponAnim();
        }


        void PlayIdleAnim()
        {
			TraceUtil.Log("PlayIdleAnimation");
			StopCoroutine("PlayerIdleAnimation");
            //StopAllCoroutines();
            string[] idleAnim =null;
            switch (MyPanelType)
            {
                case PanelType.FashionInfoPanel:
                    idleAnim = playerGenerateConfigData.Avatar_DefaultAni;
                    break;
                case PanelType.HeroInfoPanel:
                    idleAnim = playerGenerateConfigData.RoleUI_Ani_Idle;
                    break;
            }
            StartCoroutine(PlayerIdleAnimation(idleAnim,0));
        }

        IEnumerator PlayerIdleAnimation(string[] AnimationName, int step)
        {
            string[] anim = AnimationName[step].Split('+');
            this.m_hero.animation.CrossFade(anim[0]);
            yield return new WaitForSeconds(float.Parse(anim[1]) / 1000);
            step++;
            if (step < AnimationName.Length)
            {
                StartCoroutine(PlayerIdleAnimation(AnimationName, step));
            }
            else
            {
                StartCoroutine(PlayerIdleAnimation(AnimationName, 0));
            }
        }

        public void ChangeHeroWeapon(object obj)
        {
             WeaponInfo = ContainerInfomanager.Instance.GetSSyncContainerGoods_SCList(1).SingleOrDefault(P => P.nPlace == 0);
            //TraceUtil.Log("当前默认武器ID："+WeaponInfo.uidGoods);
            if (WeaponInfo.uidGoods != 0)
            {
                if (WeaponInfo.uidGoods != CurrentWeaponID)
                {
                    CurrentWeaponID = WeaponInfo.uidGoods;
                    string weapon = ContainerInfomanager.Instance.GetContainerGoodsInfo(WeaponInfo).LocalItemData._ModelId;
                    //StartCoroutine(ChangeWeapon(weapon));
                    DoForTime.DoFunForTime(0.1f, ChangeWeapon, weapon);
                }
            }
            else if (WeaponInfo.uidGoods != CurrentWeaponID)
            {
                if (CurrentWeaponID != 0)
                {
                    CurrentWeaponID = 0;
                    //StartCoroutine(ChangeDefulsWeapon());
                    DoForTime.DoFunForTime(0.1f,ChangeDefulsWeapon, null);
                }
            }
        }

        public void ChangeHeroFashion()
        {
            int fashionID = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_FASHION;
            if (fashionID != CurrentFashionID)
            {
                CurrentFashionID = fashionID;
                if (fashionID == 0)
                {
                    StartCoroutine(ChangeDefulsFashion());
                }
                else
                {
                    //StartCoroutine(ChangeFashion(CurrentFashionID));
                    DoForTime.DoFunForTime(0.1f, ChangeFashion, CurrentFashionID);
                }
            }
        }

        /// <summary>
        /// 改变主角时装
        /// </summary>
        /// <param name="weaponName"></param>
        public void ChangeFashion(object obj)
        {
            int fashionID = (int)obj;
            var FashionData = ItemDataManager.Instance.GetItemData(fashionID);
            TraceUtil.Log("切换时装：" + FashionData._ModelId);
            //yield return null;
            RoleGenerate.GenerateRole(m_hero, FashionData._ModelId);
            SetCildLayer(m_hero.transform, 25);
            if (MyPanelType == PanelType.FashionInfoPanel)
            {
                PlayerChangeFashionAnim();
            }
        }

        /// <summary>
        /// 改变主角默认时装
        /// </summary>
        /// <param name="weaponName"></param>
        IEnumerator ChangeDefulsFashion()
        {
            TraceUtil.Log("切换时装：默认时装");
            yield return null;
            RoleGenerate.GenerateRole(m_hero, playerGenerateConfigData.DefaultAvatar);
            SetCildLayer(m_hero.transform, 25);
            if (MyPanelType == PanelType.FashionInfoPanel)
            {
                PlayerChangeFashionAnim();
            }
        }

        public void PlayerChangeFashionAnim()
        {
            //StopAllCoroutines();
			StopCoroutine("PlayerChangeFasionAnimation");
            string[] anim = null;
            switch (MyPanelType)
            {
                case PanelType.HeroInfoPanel:
                    //anim = playerGenerateConfigData.Avatar_Ani;
                    //break;
                    return;
                case PanelType.FashionInfoPanel:
                    anim = playerGenerateConfigData.Avatar_Ani;
                    break;
            }
            StartCoroutine(PlayerChangeFasionAnimation(anim, 0));
        }

        public void PlayFashionOutAnim()
        {
            //StopCoroutine("PlayerIdleAnimation");
            StopAllCoroutines();
            var anim = playerGenerateConfigData.Avatar_OutAni[0].Split('+');
            StartCoroutine(PlayerFashionOutAnimation(anim[0],float.Parse(anim[1]) / 1000));
        }
        public void PlayFashionInAnim()
        { 
            //StopCoroutine("PlayerIdleAnimation");
            StopAllCoroutines();
            StartCoroutine(ShowEff(ItemInEff,playerGenerateConfigData.Avatar_InEff[0].StartTime));
            var anim = playerGenerateConfigData.Avatar_InAni[0].Split('+');
            
            StartCoroutine(PlayFashionInAnimation(anim[0],float.Parse(anim[1]) / 1000));
            // PlayerIdleAnimation(anim[0], float.Parse(anim[1]) / 1000);
        }

        IEnumerator ShowEff(GameObject Eff ,float time)
        {
            Eff.SetActive(false);
            yield return new WaitForSeconds(time);
            Eff.SetActive(true);
        }
        IEnumerator PlayFashionInAnimation(string AnimationName, float AnimTime)
        {
            this.m_hero.animation.CrossFade(AnimationName);
            this.m_hero.animation.wrapMode=WrapMode.Once;
            yield return new WaitForSeconds(AnimTime);
            PackInfoStateManager.Instance.StateChange(PackInfoStateType.ShowFashion);
            PlayIdleAnim();
        }
        
        IEnumerator PlayerFashionOutAnimation(string AnimationName, float AnimTime) 
        {
            this.m_hero.animation.CrossFade(AnimationName);
            this.m_hero.animation.wrapMode=WrapMode.Once ;
            yield return new WaitForSeconds(AnimTime);
            PackInfoStateManager.Instance.StateChange(PackInfoStateType.ClosFashPanel);
            
        }
        IEnumerator PlayerChangeFasionAnimation(string[] AnimationName, int Step)
        {
            string[] anim = AnimationName[Step].Split('+');
            this.m_hero.animation.CrossFade(anim[0]);
            yield return new WaitForSeconds(float.Parse(anim[1]) / 1000);
            Step++;
            if (Step < AnimationName.Length)
            {
                StartCoroutine(PlayerChangeFasionAnimation(AnimationName, Step));
            }
            else
            {
                PlayIdleAnim();
            }
        }

        /// <summary>
        /// 改变主角武器
        /// </summary>
        /// <param name="weaponName"></param>
        void ChangeWeapon(object obj)
        {
            string Weapon = (string)obj;
			string[] ItemWeaponPosition = GetWeaponPosition();
            var weaponEff=(ItemDataManager.Instance.GetItemData(ContainerInfomanager.Instance.GetContainerGoodsInfo(WeaponInfo).LocalItemData._goodID) as EquipmentData).WeaponEff;
            var weaponObj = PlayerFactory.Instance.GetWeaponPrefab(Weapon);
            RoleGenerate.ChangeWeapon(PlayerManager.Instance.FindHero(), weaponObj,weaponEff);
            RoleGenerate.ChangeWeapon(this.m_hero, weaponObj,weaponEff);
            SetCildLayer(m_hero.transform, 25);
            PlayerChangeWaeponAnim();
            TraceUtil.Log("改变武器" + weaponObj);
        }

        /// <summary>
        /// 改变主角武器
        /// </summary>
        /// <param name="weaponName"></param>
        void ChangeDefulsWeapon(object obj)
        {
            byte Vocation = (byte)PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
            var weaponObj = PlayerFactory.Instance.GetDefulWeaponPrefab(Vocation);
			string[] weaponPosition = GetWeaponPosition();
            RoleGenerate.AttachWeapon(this.m_hero, weaponPosition, weaponObj,null);
            RoleGenerate.ChangeWeapon(PlayerManager.Instance.FindHero(), weaponObj,null);
            SetCildLayer(m_hero.transform, 25);
            PlayerChangeWaeponAnim();
            TraceUtil.Log("改变武器" + weaponObj + " 职业:" + Vocation);
        }

        public void PlayerChangeWaeponAnim()
        {
            //StopAllCoroutines();
			StopCoroutine("PlayerEquipAnimation");
            StartCoroutine(PlayerEquipAnimation(playerGenerateConfigData.ItemAniChange, 0));
        }

        IEnumerator PlayerEquipAnimation(string[] AnimationName, int Step)
        {
            string[] anim = AnimationName[Step].Split('+');
            this.m_hero.animation.CrossFade(anim[0]);
            yield return new WaitForSeconds(float.Parse(anim[1]) / 1000);
            Step++;
            if (Step < AnimationName.Length)
            {
                StartCoroutine(PlayerEquipAnimation(AnimationName, Step));

            }
            else
            {
                PlayIdleAnim();
            }
        }

        void SetCildLayer(Transform m_transform, int layer)
        {
            m_transform.gameObject.layer = layer;
            if (m_transform.childCount > 0)
            {
                foreach (Transform child in m_transform)
                {
                    SetCildLayer(child, layer);
                }
            }
        }

        public void Close()
        {
            StopAllCoroutines();
            DoForTime.stop();
            gameObject.SetActive(false);
        }

        public void PackgeStateChangeHandel(INotifyArgs args)
        {
            switch(PackInfoStateManager.Instance.CurrentState)
            {
                case PackInfoStateType.InterFashion:
                    PlayFashionInAnim();
                    effReset();
                    StartCoroutine(ShowEff(ItemInEff,ItemInEffDelay));
                    break;
                case PackInfoStateType.PrepareToOutFashion:
                    PlayFashionOutAnim();
                    effReset();
                    StartCoroutine(ShowEff(ItemOutEff,ItemOutEffDelay));
                    //ItemOutEff.SetActive(true);
                    break;
            }
        }
        void effReset()
        {
            ItemInEff.SetActive(false);
            ItemOutEff.SetActive(false);
        }

        #region implemented abstract members of View
        
        protected override void RegisterEventHandler()
        {
            AddEventHandler(EventTypeEnum.PackStateChange.ToString(),PackgeStateChangeHandel);
        }
        void OnDestroy()
        {
            RemoveEventHandler(EventTypeEnum.PackStateChange.ToString(),PackgeStateChangeHandel);
        }
        #endregion

    }

}