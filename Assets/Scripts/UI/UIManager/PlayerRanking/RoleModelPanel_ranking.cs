using UnityEngine;
using System.Collections;
using UI.MainUI;
public class RoleModelPanel_ranking : MonoBehaviour
{

    private GameObject m_hero;
    private int CurrentFashionID = 0;
    private long CurrentWeaponID = -1;
    private float speed = 1;
    private SMsgInteract_GetPlayerRanking_SC m_data;
    PlayerGenerateConfigData playerGenerateConfigData;

    public Camera MyCamera;

    void Awake()
    {
        // ShowHeroModelView();
    }

    public  void ShowHeroModelView()
    {
        int VocationID = PlayerRankingDataManager.Instance.RankingDetail.byKind;
        gameObject.SetActive(true);
        if (m_hero == null)
        {
            AssemblyPlayer(VocationID);
        } else
        {
            m_hero.SetActive(true);
        }
    }       
    public IEnumerator SetCameraPosition(Vector3 cameraPosition, float width)
    {
        yield return null;
        Rect CameraRect = MyCamera.rect;
        CameraRect.x = cameraPosition.x;
        CameraRect.width = width;

        MyCamera.rect = CameraRect;
    }
    public IEnumerator SetCameraPosition(Vector3 cameraPosition)
    {
        yield return null;
        //cameraPosition.z = 200;
        TraceUtil.Log("cameraPosition.x" + cameraPosition.x);
        var vpP = MyCamera.ScreenToViewportPoint(cameraPosition);
        TraceUtil.Log("vpP.x" + vpP.x);
        Rect CameraRect = MyCamera.rect;
        CameraRect.x = vpP.x - CameraRect.width / 2;
        CameraRect.y = vpP.y - CameraRect.height / 2;


        MyCamera.rect = CameraRect;
    }

    public void AttachEffect(GameObject effectPrefab)
    {
        GameObject newObj = transform.InstantiateNGUIObj(effectPrefab);
        newObj.transform.localPosition = m_hero.transform.localPosition;
        SetCildLayer(newObj.transform, 25);
    }
        
    public void OnDragRoleModel(Vector2 delta)
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
        this.m_hero.transform.localPosition = new Vector3(0, -8f, 21);
        this.m_hero.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
        RoleGenerate.AttachAnimation(this.m_hero, playerGenerateConfigData.PlayerName, playerGenerateConfigData.DefaultAnim, playerGenerateConfigData.Animations);
        string[] weaponPosition = playerGenerateConfigData.Avatar_WeaponPos;
        m_hero.GetComponent<CharacterController>().enabled=false;
        //this.m_hero.gameObject.AddComponent<RoleViewClickEvent>();
        RoleGenerate.AttachWeapon(this.m_hero, weaponPosition, playerGenerateConfigData.WeaponObj,null);
        ////SetCildLayer(m_hero.transform, 25);
        //Invoke("AddRotateComponentForSeconds",1);
       
        ChangeHeroFashion();
        //ChangeHeroWeapon(null);
        SetCildLayer(m_hero.transform, 25);
    }

//     void   AddRotateComponentForSeconds()
//    {
//           this.m_hero.AddComponent<DragModel>();
//    }

    public void Show(SMsgInteract_GetPlayerRanking_SC data)
    {
        m_data = data;
        gameObject.SetActive(true);
        this.m_hero.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
        //PlayerChangeWaeponAnim();
        //Invoke("AddRotateComponentForSeconds",1);
        ChangeHeroFashion();
        ChangeHeroWeapon(null);
        PlayerIdleAnim();
    }

    public void Close()
    {
        CurrentWeaponID=0;
        CurrentFashionID=0;
        gameObject.SetActive(false);
        transform.ClearChild();
        if (m_hero)
        {
            DragModel dragModel = this.m_hero.GetComponent<DragModel>();
            if (dragModel != null)
            {
                Destroy(dragModel);
            }
        }
        StopAllCoroutines();
        //CancelInvoke("AddRotateComponentForSeconds");
    }

    public GameObject GetHeroModel
    {
        get { return m_hero; }
    }

    public void ChangeHeroWeapon(object obj)
    {
        if (!gameObject.activeSelf)
            return;
        var WeaponInfo = m_data.dwGoods [0];
        //TraceUtil.Log("当前默认武器ID："+WeaponInfo.uidGoods);
        if (WeaponInfo.dwGoodsID != 0)
        {
            if (WeaponInfo.dwGoodsID != CurrentWeaponID)
            {
                CurrentWeaponID = WeaponInfo.dwGoodsID;
                EquipmentData weapdata=ItemDataManager.Instance.GetItemData((int)WeaponInfo.dwGoodsID) as EquipmentData;
                string weapon = weapdata._ModelId;
                var weaponEff=weapdata.WeaponEff;
                StartCoroutine(ChangeWeapon(weapon,weaponEff));
            }
        }
//         else if (WeaponInfo.dwGoodsID != CurrentWeaponID)
//        {
////            if (CurrentWeaponID != 0)
////            {
////                CurrentWeaponID = 0;
////                StartCoroutine(ChangeDefulsWeapon());
////            }
//        }
    }

    public void ChangeHeroFashion()
    {
        if (!gameObject.activeSelf)
            return;
        int fashionID = (int)m_data.dwFashionID;
            if (fashionID == 0)
            {
                StartCoroutine(ChangeDefulsFashion());
            } else
            {
            StartCoroutine(ChangeFashion(fashionID)); 
            }

    }

    public void ChangeHeroFashion(int fashionID)
    {
        StartCoroutine(ChangeFashion(fashionID)); 
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
    IEnumerator ChangeFashion(int fashionID)
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
    IEnumerator ChangeDefulsFashion()
    {
        TraceUtil.Log("切换时装：默认时装");
        yield return null;
        RoleGenerate.GenerateRole(m_hero, playerGenerateConfigData.DefaultAvatar);
        SetCildLayer(m_hero.transform, 25);
    }

    /// <summary>
    /// 改变主角武器
    /// </summary>
    /// <param name="weaponName"></param>
    IEnumerator ChangeWeapon(string Weapon ,GameObject weaponEff)
    {
        yield return null;
        string[] ItemWeaponPosition = playerGenerateConfigData.Item_WeaponPosition;
        var weaponObj = PlayerFactory.Instance.GetWeaponPrefab(Weapon);
        //RoleGenerate.ChangeWeapon(PlayerManager.Instance.FindHero(), weaponObj);
        //RoleGenerate.AttachWeapon(this.m_hero, ItemWeaponPosition);
        RoleGenerate.ChangeWeapon(this.m_hero, weaponObj,weaponEff);
        SetCildLayer(m_hero.transform, 25);
        //PlayerChangeWaeponAnim();
        TraceUtil.Log("改变武器" + weaponObj);
    }



//    public void PlayerChangeWaeponAnim()
//    {
//        StopAllCoroutines();
//        StartCoroutine(PlayerEquipAnimation(playerGenerateConfigData.ItemAniChange, 0));
//    }

    void PlayerIdleAnim()
    {
       
        var anim = playerGenerateConfigData.ItemAniIdle.Split('+');
        StartCoroutine(PlayerIdleAnimation(anim [0], float.Parse(anim [1]) / 1000));
    }

//    IEnumerator PlayerEquipAnimation(string[] AnimationName, int Step)
//    {
//        string[] anim = AnimationName [Step].Split('+');
//        TraceUtil.Log("Player default anim at ContainerHeroView  PlayerEquipAnimation");
//        this.m_hero.animation.CrossFade(anim [0]);
//        yield return new WaitForSeconds(float.Parse(anim [1]) / 1000);
//        Step++;
//        if (Step < AnimationName.Length)
//        {
//            StartCoroutine(PlayerEquipAnimation(AnimationName, Step));
//
//        } else
//        {
//            PlayerIdleAnim();
//        }
//    }

    IEnumerator PlayerIdleAnimation(string AnimationName, float AnimTime)
    {
        this.m_hero.animation.CrossFade(AnimationName);
        yield return new WaitForSeconds(AnimTime);

        StartCoroutine(PlayerIdleAnimation(AnimationName, AnimTime));

    }

}
