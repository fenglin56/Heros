using UnityEngine;
using System.Collections;
using System.Linq;

namespace UI.Login
{

    public class HeroModelManager : MonoBehaviour
    {

        private GameObject m_hero;
        private GameObject m_effect;

        private NewCharacterConfigData m_configData;
        private PlayerGenerateConfigData playerGenerateConfigData;

        public Camera UICamera;
        public Camera ModelCamera;
        public Transform CameraPoint;

        private int AnimationID = 0;
        private int animSoundID = 0;
        public string UICameraPath;

        void Awake()
        {
            UICamera = GameObject.Find(UICameraPath).camera;
        }

        //public void SetCameraPosition()
        //{
        //    Vector3 CameraPosition = UICamera.WorldToViewportPoint(CameraPoint.position);
        //    Rect CameraRect = ModelCamera.rect;
        //    CameraRect.x = CameraPosition.x-CameraRect.width/2;
        //    CameraRect.y = CameraPosition.y - CameraRect.height / 2;
        //    ModelCamera.rect = CameraRect;
        //}

        IEnumerator SetModelPosition()
        {
            yield return new WaitForEndOfFrame();
            if (m_hero != null)
            {
                Vector3 CameraPosition = UICamera.WorldToScreenPoint(CameraPoint.position);
                CameraPosition.z = 70;
                m_hero.transform.position = ModelCamera.ScreenToWorldPoint(CameraPosition);
                //Debug.LogWarning(CameraPoint.position+","+CameraPosition+","+m_hero.transform.position);
            }
        }

        public void SetModelEnable(bool flag)
        {
            m_hero.SetActive(flag);
        }

        public void ShowRoleModel(NewCharacterConfigData configData)
        {
            if (this.m_hero != null)
            {
                StopAllCoroutines();
                Destroy(m_hero);
            }
            m_configData = configData;
            AssemblyPlayer(configData);
            //StartCoroutine(PlayerCreatRoleAnimation());
            StartCoroutine(SetModelPosition());
            //SetModelPosition();
        }


        public void ShowLastRoleModel()
        {
            ShowRoleModel(m_configData);
        }

        public void ClearModel()
        {
            if (this.m_hero != null)
            {
                StopAllCoroutines();
                Destroy(m_hero);
                //Destroy(m_effect);
            }
        }

        void AssemblyPlayer(NewCharacterConfigData configData)
        {
            //特效
            if (configData.EffectPrefab != null)
            {
                //this.m_effect = GameObjectPool.Instance.AcquireLocal(configData.EffectPrefab, Vector3.zero, Quaternion.identity);                
            }
            else
            {
                //this.m_effect = GameObject.CreatePrimitive(PrimitiveType.Cube);
            }
            //英雄
            playerGenerateConfigData = LoginDataManager.Instance.GetPlayerGenerateConfigData(configData.VocationID);
            this.m_hero = RoleGenerate.GenerateRole(playerGenerateConfigData.PlayerName, playerGenerateConfigData.DefaultAvatar, true);
            this.m_hero.transform.parent = transform;
            this.m_hero.transform.localPosition = new Vector3(0, 0, 180);
            this.m_hero.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
            RoleGenerate.AttachAnimation(this.m_hero, playerGenerateConfigData.PlayerName, playerGenerateConfigData.DefaultAnim, playerGenerateConfigData.Animations);
            //GameObject weapon = GameManager.Instance.PlayerFactory.Weapons.FirstOrDefault(P => P.name == configData.WeaponName);
            GameObject weapon = configData.Weapon;
            TraceUtil.Log("weapon:" + weapon);
            this.ChangeWeapon(weapon,null);

            //this.m_effect.transform.parent = m_hero.transform;
            //this.m_effect.transform.localPosition = Vector3.zero;
            //this.m_effect.transform.localScale = Vector3.one;
        }

        public void PlayerSelectRoleAnimation()
        {
            AnimationID = 0;
            StartCoroutine(PlayerSelectRoleAnim());
        }

        public void PlayerCreatRoleAnimation()
        {
            AnimationID = 0;
            StartCoroutine(PlayerCreatRoleAnim());

            animSoundID = 0;
            StartCoroutine(PlayCreatRoleSound());
        }

        IEnumerator PlayCreatRoleSound()
        {
            string[] soundInfo = m_configData.SelectAniSound.Split('|');
            string[] currentSoundInfo = soundInfo[animSoundID].Split('+');
            float waitTime = float.Parse(currentSoundInfo[1]);
            yield return new WaitForSeconds(waitTime/1000);
            SoundManager.Instance.PlaySoundEffect(currentSoundInfo[0]);
            animSoundID++;
            if (animSoundID < soundInfo.Length)
            {
                StartCoroutine(PlayCreatRoleSound());
            }
        }

        IEnumerator PlayerSelectRoleAnim()
        {
            this.m_hero.animation.Stop();
            this.m_hero.animation.CrossFade(m_configData.SelectAni[AnimationID]);
            this.m_hero.animation.wrapMode = WrapMode.Once;
            float WaitTime = m_configData.SelectAnimTime[AnimationID] / 1000f;
            //TraceUtil.Log("[PlayerSelectRoleAnim]");
            StartCoroutine(PlayEffect(m_configData.EffectTime));
            yield return new WaitForSeconds(WaitTime);
            if (AnimationID < this.m_configData.SelectAni.Length - 1)
            {
                AnimationID++;
            }
            else
            {
                AnimationID = 0;
            }
            StartCoroutine(PlayerSelectRoleAnim());
        }

        IEnumerator PlayerCreatRoleAnim()
        {
            //TraceUtil.Log("Player default anim at HeroModelManaher  PlayerCreatRoleAnim");
            this.m_hero.animation.Stop();
            this.m_hero.animation.CrossFade(m_configData.Animations[AnimationID]);
            this.m_hero.animation.wrapMode = WrapMode.Once;
            float WaitTime = m_configData.AnimationsTime[AnimationID] / 1000f;
            StartCoroutine(PlayEffect(m_configData.EffectTime));
            yield return new WaitForSeconds(WaitTime);            
            if (AnimationID < this.m_configData.Animations.Length - 1)
            {
                AnimationID++;
            }
            else
            {
                AnimationID = 0;
            }
            StartCoroutine(PlayerCreatRoleAnim());
        }

        IEnumerator PlayEffect(float delayTime, float destroyTime, GameObject prefab)
        {            
            yield return new WaitForSeconds(delayTime);
            /*
            if (prefab != null)
            {
                GameObject effect = (GameObject)Instantiate(prefab);
                effect.transform.parent = m_hero.transform;
                effect.transform.localPosition = Vector3.zero;
                effect.transform.localScale = Vector3.one;
                var destroyScript = effect.AddComponent<DestroySelf>();
                destroyScript.Time = destroyTime;
            }            
            */
        }

        IEnumerator PlayEffect(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);

            /*
            var animations = m_effect.GetComponentsInChildren<Animation>();
            animations.ApplyAllItem(p =>
                {
                    p.Stop();
                    p.Play();
                });
            var particles = m_effect.GetComponentsInChildren<ParticleSystem>();
            particles.ApplyAllItem(p =>
                {
                    p.Stop();                    
                    p.Play();
                });
                */
        }

        /// <summary>
        /// 改变主角武器
        /// </summary>
        /// <param name="weaponName"></param>
        public void ChangeWeapon(GameObject weapon,GameObject weaponEff)
        {
            string[] weaponPosition = m_configData.WeaponPosition.Split('+') ;
            RoleGenerate.AttachWeapon(this.m_hero, weaponPosition, weapon,weaponEff);
            SetCildLayer(m_hero.transform, 25);
        }

        /// <summary>
        /// 改变主角时装
        /// </summary>
        /// <param name="weaponName"></param>
        public IEnumerator ChangeFashion(int fashionID)
        {
            var FashionData = ItemDataManager.Instance.GetItemData(fashionID);
            TraceUtil.Log("切换时装：" + FashionData._ModelId);
            yield return null;
            RoleGenerate.GenerateRole(m_hero, FashionData._ModelId);
            SetCildLayer(m_hero.transform, 25);
        }

        /// <summary>
        /// 改变主角默认时装
        /// </summary>
        /// <param name="weaponName"></param>
        public IEnumerator ChangeDefulsFashion()
        {
            TraceUtil.Log("切换时装：默认时装");
            yield return null;
            RoleGenerate.GenerateRole(m_hero, playerGenerateConfigData.DefaultAvatar);
            SetCildLayer(m_hero.transform, 25);
        }


        void SetCildLayer(Transform m_transform,int layer)
        {
            m_transform.gameObject.layer = layer;
            if (m_transform.childCount > 0)
            {
                foreach (Transform child in m_transform)
                {
                    SetCildLayer(child,layer);
                }
            }
        }
    }
}