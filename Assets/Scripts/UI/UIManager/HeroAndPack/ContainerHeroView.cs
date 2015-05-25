using UnityEngine;
using System.Collections;
using System.Linq;

namespace UI.MainUI
{
    public class ContainerHeroView : MonoBehaviour
    {
		public GameObject Eff_HeroHoloPrefab;
		private GameObject m_heroHolo;
		//public GameObject TestObj;
        private GameObject m_hero;
        
        public string UICameraPath;
        Camera UICamera;
        public Camera ModelCamera;
        public Transform CameraPoint;
        public Rect ScreenPoint;

        PlayerGenerateConfigData playerGenerateConfigData;

		public int ActorID{set;get;}
        private int CurrentVocationID = 0;
        private int CurrentFashionID = 0;
        private long CurrentWeaponID = 0;

        void Awake()
        {
            this.UICamera = GameObject.Find(UICameraPath).camera;
        }

        public void ShowHeroModelView(int VocationID)
        {
            gameObject.SetActive(true);
            if (m_hero == null)
            {
                AssemblyPlayer(VocationID);
            }
            else
            {
                gameObject.SetActive(true);
                PlayerIdleAnim();

                //edit by lee
                //PlayerChangeWaeponAnim();
                //ChangeHeroFashion();
                //ChangeHeroWeapon(null);
                StartCoroutine(SetModelPosition());
            }          
            //SetCameraPosition();
        }

        public void ShowHeroModelView(int actorID, int VocationID, int FashionID, int WeaponID)
        {
			this.ActorID = actorID;
            gameObject.SetActive(true);
            if (m_hero == null)
            {
                AssemblyPlayer(VocationID, FashionID, WeaponID);
                PlayerIdleAnim();
            }
            else
            {
                gameObject.SetActive(true);
                PlayerIdleAnim();

                //edit by lee
                //PlayerChangeWaeponAnim();
                ChangeHeroFashion(FashionID);
                ChangeHeroWeapon(WeaponID);
                StartCoroutine(SetModelPosition());
            }            
        }


        public void CloseHeroModelView()
        {
            StopAllCoroutines();
            gameObject.SetActive(false);
        }

		public bool IsCreateObj()
		{
			return m_hero!=null;
		}


        public void DeleteHeroModeView()
        {
            StopAllCoroutines();
            if (m_hero != null)
            {
                Destroy(m_hero);
				Destroy(m_heroHolo);
                m_hero = null;
                //值要复位
                CurrentFashionID = 0;
                CurrentWeaponID = 0;
            }                                    
            gameObject.SetActive(false);
        }

        void SetModelPisitionImmediate()
        {
            Vector3 CameraPosition = UICamera.WorldToScreenPoint(CameraPoint.position);
            CameraPosition.z = 200;
            m_hero.transform.position = ModelCamera.ScreenToWorldPoint(CameraPosition);
			m_heroHolo.transform.position = m_hero.transform.position;
        }

        IEnumerator SetModelPosition()
        {         
            yield return new WaitForEndOfFrame();
            if (m_hero != null)
            {
                Vector3 CameraPosition = UICamera.WorldToScreenPoint(CameraPoint.position);
                CameraPosition.z = 200;
                m_hero.transform.position = ModelCamera.ScreenToWorldPoint(CameraPosition);
				m_heroHolo.transform.position = m_hero.transform.position;
            }
            else
            {
                TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"m_hero == null");
            }
        }

        void AssemblyPlayer(int VocationID)
        {
            /*
            if (this.m_hero != null)
            {
                Destroy(m_hero);
            }
            playerGenerateConfigData = PlayerDataManager.Instance.GetUIItemData((byte)VocationID) ;
            this.m_hero = RoleGenerate.GenerateRole(playerGenerateConfigData.PlayerName, playerGenerateConfigData.DefaultAvatar, true);
            this.m_hero.transform.parent = transform;
            //this.m_hero.transform.localPosition = new Vector3(0, -8, 170);
            this.m_hero.transform.localRotation = Quaternion.Euler(new Vector3(0,180,0));
            
            RoleGenerate.AttachAnimation(this.m_hero, playerGenerateConfigData.PlayerName, playerGenerateConfigData.DefaultAnim, playerGenerateConfigData.Animations);

            string[] weaponPosition = playerGenerateConfigData.Item_WeaponPosition;            
            //RoleGenerate.AttachWeapon(this.m_hero, weaponPosition, playerGenerateConfigData.WeaponObj);

            ChangeHeroWeapon(null);

            PlayerIdleAnim();
            SetCildLayer(m_hero.transform, 25);
            */
            playerGenerateConfigData = PlayerDataManager.Instance.GetUIItemData((byte)VocationID);
            this.m_hero = RoleGenerate.GenerateRole(playerGenerateConfigData.PlayerName, playerGenerateConfigData.DefaultAvatar, true);
            this.m_hero.transform.parent = transform;
            this.m_hero.transform.localPosition = new Vector3(0, -8, 170);
            this.m_hero.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
            RoleGenerate.AttachAnimation(this.m_hero, playerGenerateConfigData.PlayerName, playerGenerateConfigData.DefaultAnim, playerGenerateConfigData.Animations);

            string[] weaponPosition = playerGenerateConfigData.Avatar_WeaponPos;            
            RoleGenerate.AttachWeapon(this.m_hero, weaponPosition, playerGenerateConfigData.WeaponObj,null);

			//光环
			this.m_heroHolo = (GameObject)Instantiate(Eff_HeroHoloPrefab);
			m_heroHolo.transform.parent = transform;
			this.m_heroHolo.transform.Rotate(Vector3.left * 20, Space.Self);
			SetCildLayer(this.m_heroHolo.transform , 25);
            //ChangeHeroWeapon(null);
            ////SetCildLayer(m_hero.transform, 25);
            //TweenFloat.Begin(1,0,1,null,AddRotateComponentForSeconds);
            

            ChangeHeroFashion();
            ChangeHeroWeapon(null);

            SetCildLayer(m_hero.transform, 25);            

            SetModelPisitionImmediate();
        }

        void AssemblyPlayer(int VocationID, int FashionID, int WeaponID)
        {
            playerGenerateConfigData = PlayerDataManager.Instance.GetUIItemData((byte)VocationID);
            this.m_hero = RoleGenerate.GenerateRole(playerGenerateConfigData.PlayerName, playerGenerateConfigData.DefaultAvatar, true);
            this.m_hero.transform.parent = transform;
            this.m_hero.transform.localPosition = new Vector3(0, -8, 170);
            this.m_hero.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
            RoleGenerate.AttachAnimation(this.m_hero, playerGenerateConfigData.PlayerName, playerGenerateConfigData.DefaultAnim, playerGenerateConfigData.Animations);
            ChangeHeroWeapon(WeaponID);
            ChangeHeroFashion(FashionID);
			//光环
			this.m_heroHolo = (GameObject)Instantiate(Eff_HeroHoloPrefab);
			m_heroHolo.transform.parent = transform;
			this.m_heroHolo.transform.Rotate(Vector3.left * 20, Space.Self);
			SetCildLayer(this.m_heroHolo.transform , 25);

            SetModelPisitionImmediate();
        }

        private void ChangeHeroWeapon(int weaponID)
        {        
            CurrentWeaponID = weaponID;
            if (CurrentWeaponID == 0)
            {
                StartCoroutine(ChangeDefulsWeapon());
            }
            else
            {
                StartCoroutine(ChangeWeapon(CurrentWeaponID));
            }
        }

        public void ChangeHeroWeapon(object obj)
        {            
            var WeaponInfo = ContainerInfomanager.Instance.GetSSyncContainerGoods_SCList(1).SingleOrDefault(P => P.nPlace == 0);
            if (WeaponInfo.uidGoods != 0)
            {
                if (WeaponInfo.uidGoods != CurrentWeaponID)
                {
                    CurrentWeaponID = WeaponInfo.uidGoods;
                    string weapon = ContainerInfomanager.Instance.GetContainerGoodsInfo(WeaponInfo).LocalItemData._ModelId;
                    var weaponEff=(ItemDataManager.Instance.GetItemData(ContainerInfomanager.Instance.GetContainerGoodsInfo(WeaponInfo).LocalItemData._goodID)as EquipmentData).WeaponEff;
                    StartCoroutine(ChangeWeapon(weapon,weaponEff));
                }
                else
                {
                    TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"WeaponInfo.uidGoods == CurrentWeaponID");
                }
            }
            else if (WeaponInfo.uidGoods != CurrentWeaponID)
            {
                if (CurrentWeaponID != 0)
                {
                    CurrentWeaponID = 0;
                    StartCoroutine(ChangeDefulsWeapon());
                }
                else
                {
                    TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"CurrentWeaponID == 0");
                }
                
            }
        }

        /// <summary>
        /// 改变主角武器
        /// </summary>
        /// <param name="weaponName"></param>
        public IEnumerator ChangeWeapon(string Weapon,GameObject WeaponEff)
        {
            /* //旧方法
            yield return null;
            string[] ItemWeaponPosition = playerGenerateConfigData.Item_WeaponPosition;
            var weaponObj = PlayerFactory.Instance.GetWeaponPrefab(Weapon);
            //RoleGenerate.AttachWeapon(this.m_hero, ItemWeaponPosition, weaponObj);
            RoleGenerate.ChangeWeapon(this.m_hero, weaponObj);
            //string[] weaponPosition = playerGenerateConfigData.WeaponPosition;
            //RoleGenerate.AttachWeapon(PlayerManager.Instance.FindHero(), weaponPosition, weaponObj);
            RoleGenerate.ChangeWeapon(PlayerManager.Instance.FindHero(),weaponObj);
            SetCildLayer(m_hero.transform, 25);
            PlayerChangeWaeponAnim();
            TraceUtil.Log("改变武器" + weaponObj);
             */
            yield return null;
            string[] ItemWeaponPosition = playerGenerateConfigData.Item_WeaponPosition;
            var weaponObj = PlayerFactory.Instance.GetWeaponPrefab(Weapon);
            RoleGenerate.ChangeWeapon(this.m_hero, weaponObj,WeaponEff);
            RoleGenerate.ChangeWeapon(PlayerManager.Instance.FindHero(), weaponObj,WeaponEff);
            SetCildLayer(m_hero.transform, 25);
            PlayerChangeWaeponAnim();            
        }

        IEnumerator ChangeWeapon(long weaponID)
        {
            yield return null;
			if(this.m_hero!=null)
			{
				var item = ItemDataManager.Instance.GetItemData((int)weaponID);
				if (item != null)
				{
					var weaponGO = PlayerFactory.Instance.GetWeaponPrefab(item._ModelId);
					string[] weaponPosition = playerGenerateConfigData.Item_WeaponPosition;
					var WeaponEff=(ItemDataManager.Instance.GetItemData(item._goodID)as EquipmentData).WeaponEff;
					RoleGenerate.AttachWeapon(this.m_hero, weaponPosition, weaponGO,WeaponEff);
					RoleGenerate.ChangeWeapon(PlayerManager.Instance.FindHero(), weaponGO,WeaponEff);
					SetCildLayer(m_hero.transform, 25);
				}
				else
				{
					
				}
			}                       
        }
        /// <summary>
        /// 改变主角武器
        /// </summary>
        /// <param name="weaponName"></param>
        public IEnumerator ChangeDefulsWeapon()
        {
            /*
            yield return null;
            byte Vocation = (byte)PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
            //string[] ItemWeaponPosition = playerGenerateConfigData.Item_WeaponPosition;
            //string[] weaponPosition = playerGenerateConfigData.WeaponPosition;
            var weaponObj = PlayerFactory.Instance.GetDefulWeaponPrefab(Vocation);
            //RoleGenerate.AttachWeapon(this.m_hero, ItemWeaponPosition, weaponObj);
            //RoleGenerate.AttachWeapon(PlayerManager.Instance.FindHero(), weaponPosition, weaponObj);
            RoleGenerate.ChangeWeapon(this.m_hero, weaponObj);
            RoleGenerate.ChangeWeapon(PlayerManager.Instance.FindHero(), weaponObj);
            SetCildLayer(m_hero.transform, 25);
            PlayerChangeWaeponAnim();
            TraceUtil.Log("改变武器" + weaponObj);
             */
            yield return null;
            byte Vocation = (byte)PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
            var weaponObj = PlayerFactory.Instance.GetDefulWeaponPrefab(Vocation);
            string[] weaponPosition = playerGenerateConfigData.Item_WeaponPosition;
            RoleGenerate.AttachWeapon(this.m_hero, weaponPosition, weaponObj,null);
            RoleGenerate.ChangeWeapon(PlayerManager.Instance.FindHero(), weaponObj,null);
            SetCildLayer(m_hero.transform, 25);
            PlayerChangeWaeponAnim();
            //TraceUtil.Log("改变武器" + weaponObj + " 职业:" + Vocation);
        }

        /// <summary>
        /// 改变主角时装
        /// </summary>
        /// <param name="weaponName"></param>
        IEnumerator ChangeFashion(int fashionID)
        {
            var FashionData = ItemDataManager.Instance.GetItemData(fashionID);
            //TraceUtil.Log("切换时装：" + FashionData._ModelId);
            yield return null;
            RoleGenerate.GenerateRole(m_hero, FashionData._ModelId);
            SetCildLayer(m_hero.transform, 25);
        }
        /// <summary>
        /// 改变主角默认时装
        /// </summary>
        /// <param name="weaponName"></param>
        IEnumerator ChangeDefulsFashion()
        {
            //TraceUtil.Log("切换时装：默认时装");
            yield return null;
            RoleGenerate.GenerateRole(m_hero, playerGenerateConfigData.DefaultAvatar);
            SetCildLayer(m_hero.transform, 25);
        }

        public void ChangeHeroFashion()
        {
            int fashionID = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_FASHION;
       
            CurrentFashionID = fashionID;
            if (fashionID == 0)
            {
                StartCoroutine(ChangeDefulsFashion());
            }
            else
            {
                StartCoroutine(ChangeFashion(CurrentFashionID));
            }
         
        }

        public void ChangeHeroFashion(int FashionID)
        {
            CurrentFashionID = FashionID;
            if (CurrentFashionID == 0)
            {
                StartCoroutine(ChangeDefulsFashion());
            }
            else
            {
                StartCoroutine(ChangeFashion(CurrentFashionID));
            }    
        }
		/// <summary>
		/// 
		/// </summary>
		public void PlayJoinInAnimation()
		{
			var anim = playerGenerateConfigData.Team_Ani_Join;
			StartCoroutine(PlayerIdleAnimation(anim[0],float.Parse(anim[1])/1000));
		}

        void PlayerChangeWaeponAnim()
        {
            //StopAllCoroutines();
            StartCoroutine(PlayerEquipAnimation(playerGenerateConfigData.ItemAniChange,0));
        }

        public void PlayerIdleAnim()
        {
            //StopAllCoroutines();
            var anim = playerGenerateConfigData.ItemAniIdle.Split('+');
            StartCoroutine(PlayerIdleAnimation(anim[0],float.Parse(anim[1])/1000));
        }

        #region edit by lee
        public void PlayRandomAnim(string[] animationArray)
        {
            //StopAllCoroutines();
            int length = animationArray.Length;
            //int step = Random.Range(0, length);
            //StartCoroutine(PlayerEquipAnimation(animationArray, step));
            StartCoroutine(PlayerEquipAnimation(animationArray, 0));
        }
        #endregion

        IEnumerator PlayerEquipAnimation(string[] AnimationName, int Step)
        {
            string[] anim = AnimationName[Step].Split('+');
            //TraceUtil.Log("Player default anim at ContainerHeroView  PlayerEquipAnimation");
            this.m_hero.animation.CrossFade(anim[0]);
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

        IEnumerator PlayerIdleAnimation(string AnimationName, float AnimTime)
        {
            //TraceUtil.Log("Player default anim at ContainerHeroView  PlayerIdleAnimation");
            this.m_hero.animation.CrossFade(AnimationName);
            yield return new WaitForSeconds(AnimTime);
            //StartCoroutine(PlayerIdleAnimation(AnimationName, AnimTime));            
			PlayerIdleAnim();
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

    }
}