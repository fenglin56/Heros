using UnityEngine;
using System.Collections;
using System.Linq;
using System.IO;

namespace UI.MainUI
{

    public class RoleModelPanel : View
    {
    

        private GameObject m_hero;
        private int CurrentFashionID = 0;
        private long CurrentWeaponID = -1;
		private float speed = 1;
        private GameObject HeroEff;
        private bool HasCreated;
        private GameObject ItemInEff;
        private float ItemInEffDelay;
        private GameObject ItemOutEff;
        private float ItemOutEffDelay;
        PlayerGenerateConfigData playerGenerateConfigData;

        public Camera MyCamera;

        void Awake()
        {
            RegisterEventHandler();
            ShowHeroModelView();
            ItemInEff= Instantiate(playerGenerateConfigData.Item_InEff[0].Eff) as GameObject;

            ItemInEffDelay=playerGenerateConfigData.Item_InEff[0].StartTime/1000f;
            ItemOutEff=Instantiate( playerGenerateConfigData.Item_OutEff[0].Eff)as GameObject;
            ItemOutEffDelay=playerGenerateConfigData.Item_OutEff[0].StartTime/1000f;
         
            ItemInEff.transform.parent=m_hero.transform;
            ItemInEff.SetActive(false);
            ItemInEff.transform.localPosition=Vector3.zero;
            ItemOutEff.transform.parent=m_hero.transform;
            ItemOutEff.transform.localPosition=Vector3.zero;
            ItemOutEff.SetActive(false);
        }

        void ShowHeroModelView()
        {
            int VocationID = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
            gameObject.SetActive(true);
            if (m_hero == null)
            {
                AssemblyPlayer(VocationID);
                EffRePosition();
            }
            else
            {
                m_hero.SetActive(true);
            }
        }       
        public IEnumerator SetCameraPosition(Vector3 cameraPosition,float width)
        {

            Rect CameraRect = MyCamera.rect;
            CameraRect.x = cameraPosition.x;
            CameraRect.width = width;

            MyCamera.rect = CameraRect;
            yield return null;
        }
        public IEnumerator SetCameraPosition(Vector3 cameraPosition)
        {
            yield return null;
            //cameraPosition.z = 200;
            TraceUtil.Log("cameraPosition.x" + cameraPosition.x);
            var vpP = MyCamera.ScreenToViewportPoint(cameraPosition);
            TraceUtil.Log("vpP.x"+vpP.x);
            Rect CameraRect = MyCamera.rect;
            CameraRect.x = vpP.x - CameraRect.width / 2;
            CameraRect.y = vpP.y - CameraRect.height / 2;


            MyCamera.rect = CameraRect;
        }

        void EffRePosition()
        {
            if (HeroEff)
            {
                HeroEff.transform.localPosition = m_hero.transform.localPosition;
                SetCildLayer(HeroEff.transform, 25);
            }

        }

		public void AttachEffect(GameObject effectPrefab)
		{
            HeroEff = transform.InstantiateNGUIObj(effectPrefab);
            if (m_hero != null)
            {
               EffRePosition();
            }
		
		}
		
		public void OnDragRoleModel (Vector2 delta)
		{
			if (m_hero != null)
			{
				m_hero.transform.localRotation = Quaternion.Euler(0f, -0.5f * delta.x * speed, 0f) * m_hero.transform.localRotation;
			}
		}
        void AssemblyPlayer(int VocationID)
        {
            if (this.m_hero != null)
            {
                Destroy(m_hero);
            }
            playerGenerateConfigData = PlayerDataManager.Instance.GetUIItemData((byte)VocationID);
            this.m_hero = RoleGenerate.GenerateRole(playerGenerateConfigData.PlayerName, playerGenerateConfigData.DefaultAvatar, true);
            this.m_hero.transform.parent = transform;
			this.m_hero.transform.localScale = Vector3.one;
            this.m_hero.transform.localPosition = new Vector3(0,-8f,21);
            this.m_hero.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
            RoleGenerate.AttachAnimation(this.m_hero, playerGenerateConfigData.PlayerName, playerGenerateConfigData.DefaultAnim, playerGenerateConfigData.Animations);
            string[] weaponPosition = playerGenerateConfigData.Avatar_WeaponPos;
            this.m_hero.gameObject.AddComponent<RoleViewClickEvent>();
            RoleGenerate.AttachWeapon(this.m_hero, weaponPosition, playerGenerateConfigData.WeaponObj,null);
            ////SetCildLayer(m_hero.transform, 25);
            TweenFloat.Begin(1,0,1,null,AddRotateComponentForSeconds);
            ChangeHeroFashion();
            ChangeHeroWeapon(null,false);
			SetCildLayer(m_hero.transform,25);
           
        }

        void AddRotateComponentForSeconds(object obj)
        {
            this.m_hero.AddComponent<DragModel>();
        }

        public void ShowDefult()
        {
         
            gameObject.SetActive(true);
            effReset();
           m_hero.transform.position=new Vector3(1000,0,0);
            this.m_hero.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
            TweenFloat.Begin(1, 0, 1, null, AddRotateComponentForSeconds);
            ChangeHeroFashion();
            ChangeHeroWeapon(null,false);
            //PlayerIdleAnim();
            this.m_hero.transform.localPosition = new Vector3(0,-8f,21);
            PlayerChangeWaeponAnim();
            //PackInfoStateManager.Instance.StateChange(PackInfoStateType.InterPack);
        }
        public void Show()
		{
           
            //PlayerChangeWaeponAnim();
            gameObject.SetActive(true);
            effReset();
            this.m_hero.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
            ChangeHeroFashion();
            ChangeHeroWeapon(null,true);
           
        }

        public void Close()
        {
          foreach(var item in gameObject.GetComponents<TweenFloat>())
            {
                item.CancelInvoke();
            }
			gameObject.SetActive(false);
            DragModel dragModel = this.m_hero.GetComponent<DragModel>();
            if (dragModel != null)
            {
                Destroy(dragModel);
            }
        }

        public GameObject GetHeroModel
        {
            get { return m_hero; }
        }

        public void ChangeHeroWeapon(object obj,bool ShowAnm)
        {
			if(!gameObject.activeSelf)
				return;
            var WeaponInfo = ContainerInfomanager.Instance.GetSSyncContainerGoods_SCList(1).SingleOrDefault(P => P.nPlace == 0);
            //TraceUtil.Log("当前默认武器ID："+WeaponInfo.uidGoods);
            if (WeaponInfo.uidGoods != 0)
            {
                if (WeaponInfo.uidGoods != CurrentWeaponID)
                {
                    CurrentWeaponID = WeaponInfo.uidGoods;
                    string weapon = ContainerInfomanager.Instance.GetContainerGoodsInfo(WeaponInfo).LocalItemData._ModelId;
                    //StartCoroutine(ChangeWeapon(weapon,ShowAnm));
                    var WeaponEff=(ItemDataManager.Instance.GetItemData( ContainerInfomanager.Instance.GetContainerGoodsInfo(WeaponInfo).LocalItemData._goodID)as EquipmentData).WeaponEff;
                    ChangeWeapon(weapon,ShowAnm,WeaponEff);
                }
                else
                {

                }

            }
            else if (WeaponInfo.uidGoods != CurrentWeaponID)
            {
                if (CurrentWeaponID != 0)
                {
                    CurrentWeaponID = 0;
                    //StartCoroutine(ChangeDefulsWeapon(ShowAnm));
                    ChangeDefulsWeapon(ShowAnm);
                }
            }
        }

        public void ChangeHeroFashion()
        {
			if(!gameObject.activeSelf)
				return;
            int fashionID = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_FASHION;
            if (fashionID != CurrentFashionID)
            {
                CurrentFashionID = fashionID;
                if (fashionID == 0)
                {
                   // StartCoroutine(ChangeDefulsFashion());
                    ChangeDefulsFashion();
                }
                else
                {
                   // StartCoroutine(ChangeFashion(CurrentFashionID)); 
                    ChangeFashion(CurrentFashionID);
                }
            }
        }

        public void ChangeHeroFashion(int fashionID)
        {
           // StartCoroutine(ChangeFashion(fashionID)); 
            ChangeFashion(fashionID);
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

        /// <summary>
        /// 改变主角时装
        /// </summary>
        /// <param name="weaponName"></param>
        void  ChangeFashion(int fashionID)
        {
            var FashionData = ItemDataManager.Instance.GetItemData(fashionID);
            TraceUtil.Log("切换时装：" + FashionData._ModelId);
           // yield return null;
            RoleGenerate.GenerateRole(m_hero, FashionData._ModelId);
            SetCildLayer(m_hero.transform, 25);
        }

        /// <summary>
        /// 改变主角默认时装
        /// </summary>
        /// <param name="weaponName"></param>
        void ChangeDefulsFashion()
        {
            TraceUtil.Log("切换时装：默认时装" );
            //yield return null;
            RoleGenerate.GenerateRole(m_hero, playerGenerateConfigData.DefaultAvatar);
            SetCildLayer(m_hero.transform, 25);
        }

        /// <summary>
        /// 改变主角武器
        /// </summary>
        /// <param name="weaponName"></param>
        void ChangeWeapon(string Weapon,bool showAnm,GameObject WeaponEff)
        {
           // yield return null;
            string[] ItemWeaponPosition = playerGenerateConfigData.Item_WeaponPosition;
            var weaponObj = PlayerFactory.Instance.GetWeaponPrefab(Weapon);
            RoleGenerate.ChangeWeapon(PlayerManager.Instance.FindHero(), weaponObj,WeaponEff);
            RoleGenerate.AttachWeapon(this.m_hero,ItemWeaponPosition,weaponObj,WeaponEff);
            //RoleGenerate.ChangeWeapon(this.m_hero, weaponObj);
            SetCildLayer(m_hero.transform, 25);
            if(showAnm)
            {
            PlayerChangeWaeponAnim();
            }
            else
            {
                PlayerIdleAnim();
            }
            TraceUtil.Log("改变武器" + weaponObj);
        }

         
        /// <summary>
        /// 改变主角武器
        /// </summary>
        /// <param name="weaponName"></param>
        void  ChangeDefulsWeapon(bool showAnim)
        {
            //yield return null;
            byte Vocation = (byte)PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
            var weaponObj = PlayerFactory.Instance.GetDefulWeaponPrefab(Vocation);
            string[] weaponPosition = playerGenerateConfigData.Item_WeaponPosition;
            RoleGenerate.AttachWeapon(this.m_hero, weaponPosition, weaponObj,null);
            RoleGenerate.ChangeWeapon(PlayerManager.Instance.FindHero(), weaponObj,null);
            SetCildLayer(m_hero.transform, 25);
            if(showAnim)
            {
            PlayerChangeWaeponAnim();
            }
            else
            {
                PlayerIdleAnim();
            }
            TraceUtil.Log("改变武器" + weaponObj + " 职业:" + Vocation);
        }

        public void PlayerChangeWaeponAnim()
        {
            StopCoroutine("PlayerIdleAnimation");
            StopCoroutine("PlayerEquipAnimation");

            StartCoroutine(PlayerEquipAnimation(playerGenerateConfigData.ItemAniChange, 0));
        }

       public  void PlayerIdleAnim()
        {
           //StopCoroutine("PlayerIdleAnimation");
            StopCoroutine("PlayerEquipAnimation");
            var anim = playerGenerateConfigData.ItemAniIdle.Split('+');
            PlayerIdleAnimation(anim[0], float.Parse(anim[1]) / 1000);
        }

        public void PlayItemOutAnim()
        {
            //StopCoroutine("PlayerIdleAnimation");
            StopCoroutine("PlayerEquipAnimation");
            var anim = playerGenerateConfigData.Item_OutAni[0].Split('+');
            StartCoroutine(PlayerItemOutAnimation(anim[0],float.Parse(anim[1]) / 1000));
        }
        public void PlayItemInAnim()
        { 
            //StopCoroutine("PlayerIdleAnimation");
            StopCoroutine("PlayerEquipAnimation");
            StartCoroutine(ShowEff(ItemInEff,ItemInEffDelay));
            var anim = playerGenerateConfigData.Item_InAni[0].Split('+');

            StartCoroutine(PlayItemInAnimation(anim[0],float.Parse(anim[1]) / 1000));
           // PlayerIdleAnimation(anim[0], float.Parse(anim[1]) / 1000);
        }

        IEnumerator ShowEff(GameObject go,float time)
        {
            yield return new WaitForSeconds(time);
            go.SetActive(false);
            go.SetActive(true);
        }
        IEnumerator PlayerEquipAnimation(string[] AnimationName, int Step)
        {
            string[] anim = AnimationName[Step].Split('+');
            TraceUtil.Log("Player default anim at ContainerHeroView  PlayerEquipAnimation");
            this.m_hero.animation.CrossFade(anim[0]);
            this.m_hero.animation.wrapMode=WrapMode.Once;
            yield return new WaitForSeconds(float.Parse(anim[1]) / 1000);
            Step++;
            if (Step < AnimationName.Length)
            {
                StartCoroutine(PlayerEquipAnimation(AnimationName, Step));

            }
            else
            {
                PlayerIdleAnim();
            }
        }

        IEnumerator PlayItemInAnimation(string AnimationName, float AnimTime)
        {
            this.m_hero.animation.CrossFade(AnimationName);
            this.m_hero.animation.wrapMode=WrapMode.Once;
            yield return new WaitForSeconds(AnimTime);
            PackInfoStateManager.Instance.StateChange(PackInfoStateType.ShowFashion);
            PlayerIdleAnim();
        }

        IEnumerator PlayerItemOutAnimation(string AnimationName, float AnimTime)
        {
            this.m_hero.animation.CrossFade(AnimationName);
            this.m_hero.animation.wrapMode=WrapMode.Once ;
            yield return new WaitForSeconds(AnimTime);
            PackInfoStateManager.Instance.StateChange(PackInfoStateType.ShowFashionPanel);
            
        }

        void PlayerIdleAnimation(string AnimationName, float AnimTime)
        {
            this.m_hero.animation.CrossFade(AnimationName);
            this.m_hero.animation.wrapMode=WrapMode.Loop ;

        }


        public void PackgeStateChangeHandel(INotifyArgs args)
        {
            switch(PackInfoStateManager.Instance.CurrentState)
            {

                case PackInfoStateType.PrepareToFashion:
                    PlayItemOutAnim();
                    StartCoroutine(ShowEff(ItemOutEff,ItemOutEffDelay));
                
                    break;
                case PackInfoStateType.InterPack:
                    PlayItemInAnim();
                    StartCoroutine(ShowEff(ItemInEff,ItemInEffDelay));
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