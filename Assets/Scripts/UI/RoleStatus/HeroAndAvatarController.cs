using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class HeroAndAvatarController : View {

    public Camera UIHeroCamera;
    public PlayerGenerateConfigDataBase PlayerGenerateConfigData_UI;
    public GameObject[] Weapons;
   

    private GameObject m_heroGo;
    private int m_currentRandValue;
    private PlayerGenerateConfigData m_configData;
    private GameObject m_bloodGO;

    private Vector3 sPos;

    void Awake()
    {
        RegisterEventHandler();
    }
	// Use this for initialization
	void Start () {
        if (!UIHeroCamera)
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"Hero camera can not be empty!");
        }
	}
    void Update()
    {
        GetHitPoint();
    }
    void OnGUI()
    {
        if (GUILayout.Button("CraeteHero"))
        {
            this.CreateHero("1");
        }
        if (GUILayout.Button("1"))
        {
            var so = this.m_heroGo.animation.Play("Atk01");
            //this.ChangeWeapon("JH_MOD_weapon_00001");
        }
        if (GUILayout.Button("2"))
        {
            var so = this.m_heroGo.animation.Play("Atk02");
            //this.ChangeWeapon("JH_MOD_weapon_00101");
        }
        if (GUILayout.Button("3"))
        {
            var so = this.m_heroGo.animation.Play("Atk03");
            //this.ChangeWeapon("JH_MOD_weapon_00301");
        }
        if (GUILayout.Button("加血100"))
        {           
            var so=this.m_heroGo.transform.position+Vector3.up*18;
            //PopupTextController.RoleSettleResult(so, "HP+100");
        }       
        if (GUILayout.Button("减血100"))
        {
            var so = this.m_heroGo.transform.position + Vector3.up * 18;
            //PopupTextController.RoleSettleResult(so, "HP-100");
        }
        if (GUILayout.Button("闪避"))
        {
            var so = this.m_heroGo.transform.position + Vector3.up * 18;
            //PopupTextController.RoleSettleResult(so, "闪避");
        }
        if (GUILayout.Button("爆击"))
        {
            var so = this.m_heroGo.transform.position + Vector3.up * 18;
            //PopupTextController.RoleSettleResult(so, "爆击");
        }
        if (GUILayout.Button("被击飞"))
        {
            
        }
    }
    void GetHitPoint()
    {
        bool hasTarget = false;
        //暂时实现角色走到点击处
        //应该把获得的点击坐标发送到服务器，然后根据服务器返回的路点进行客户端寻路
        Vector3 clickPoint = Vector3.zero;
#if (UNITY_EDITOR||(!UNITY_ANDROID&&!UNITY_IPHONE))
        if (Input.GetMouseButtonUp(0))
        {
            clickPoint = Input.mousePosition;
        }
#else
        if ((InputUtil.Instance.ClickAmount==1))
        {           
            clickPoint = new Vector3(InputUtil.Instance.TouchPosition.x, InputUtil.Instance.TouchPosition.y);
        }
#endif

        if (clickPoint != Vector3.zero)
        {           
            var ray = UIHeroCamera.ScreenPointToRay(clickPoint);
            RaycastHit raycastHit;
            
            if (Physics.Raycast(ray, out raycastHit, 10000))
            {
                string rayTag = raycastHit.transform.tag.ToLower();
                if (rayTag == "uihero")
                {
                    RandomPlayHeroAnim();
                }
            }
            else
            {
                //TraceUtil.Log("RayCast nothing!");
            }
        }
    }   
    /// <summary>
    /// 创建主角模型
    /// </summary>
    /// <param name="heroName"></param>
    /// <param name="avatarName"></param>
    public void CreateHero(string heroName)
    {
       AssemblyPlayer(heroName,true);
    }
    private string GetPlayerName(byte playerKind)
    {
        string playerName;
        switch (playerKind)
        {
            case 1:
                playerName = "daoke";
                break;
            case 2:
                playerName = "cike";
                break;
            case 3:
                playerName = "tianshi";
                break;
            case 4:
                playerName = "daoke";
                break;
            default:
                playerName = "daoke";
                break;
        }
        return playerName;
    }

    private void AssemblyPlayer(byte playerKind, bool isHero)
    {
        var playerName = GetPlayerName(playerKind);

        AssemblyPlayer(playerName, isHero);       
    }
    private void AssemblyPlayer(string playerName, bool isHero)
    {       
        Dictionary<string, PlayerGenerateConfigData> m_UIItems = new Dictionary<string, PlayerGenerateConfigData>();
        foreach (PlayerGenerateConfigData data in PlayerGenerateConfigData_UI._dataTable)
        {
            m_UIItems.Add(data.PlayerName, data);
        }
        m_configData = m_UIItems[playerName];

        var player = RoleGenerate.GenerateRole(playerName, m_configData.DefaultAvatar, isHero);
        player.name = playerName;

        RoleGenerate.AttachAnimation(player, playerName, m_configData.DefaultAnim, m_configData.Animations);// );

        player.transform.Rotate(Vector3.up, 180);

        player.tag = "UIHero";

        //player.animation.wrapMode = WrapMode.Loop;      

        this.m_heroGo = player;
        this.ChangeWeapon(m_configData.DefaultWeapon,null);

        float aniTime = player.animation["Atk01"].length;

        float singleF = aniTime / 75;

        var heroBehaviour = player.AddComponent<UIHeroBehaviour>();

        //找到heroBehaviour的FSMSystem。然后把处理方法传入SkillBase委托中。
		/*
        RoleGenerate.AttachSkill(player, "ATK01"
            , new SkillEvent[]
            {
                new SkillEvent(){ EventTimeAfterLaunch=0f, Type=SkillEventType.PlayViewEffect, Param=0}
                ,new SkillEvent(){ EventTimeAfterLaunch=singleF*20f, Type=SkillEventType.PlayViewEffect, Param=1}
                ,new SkillEvent(){ EventTimeAfterLaunch=singleF*33f, Type=SkillEventType.PlayViewEffect, Param=2}
            }
            , string.Empty
            , null
            , new GameObject[] 
            { 
                (GameObject)Resources.Load("Effects/prefab/JH_Eff_Char01_Atk01")
                ,(GameObject)Resources.Load("Effects/prefab/JH_Eff_Char01_Atk02")
                ,(GameObject)Resources.Load("Effects/prefab/JH_Eff_Char01_Atk03")
            });
        RoleGenerate.AttachSkill(player, "skill05"
           , new SkillEvent[]{new SkillEvent(){ EventTimeAfterLaunch=0.3f, Type=SkillEventType.PlayViewEffect, Param=0}}
           , string.Empty
           , null
           , new GameObject[] { (GameObject)Resources.Load("Effects/prefab/JH_Eff_Char01_SkillUse02") });
        RoleGenerate.AttachSkill(player, "SKILL01"
           , new SkillEvent[]{new SkillEvent(){ EventTimeAfterLaunch=0.3f, Type=SkillEventType.PlayViewEffect, Param=0}}
           , string.Empty
           , null
           , new GameObject[] { (GameObject)Resources.Load("Effects/prefab/JH_Eff_Char01_SkillUse03") });
        RoleGenerate.AttachSkill(player, "skill06"
           , new SkillEvent[]{new SkillEvent(){ EventTimeAfterLaunch=0.3f, Type=SkillEventType.PlayViewEffect, Param=0}}
           , string.Empty
           , null
           , new GameObject[] { (GameObject)Resources.Load("Effects/prefab/JH_Eff_Char01_SkillUse04") });
           */

       


    }
    /// <summary>
    /// 随机播放主角动画
    /// </summary>
    private void RandomPlayHeroAnim()
    {

        var behaviour = this.m_heroGo.GetComponent<UIHeroBehaviour>();

        behaviour.animation.Play("Atk01");

        //int clipCount=this.m_heroGo.animation.GetClipCount();
        //int randValue=Random.Range(0,clipCount);
        //while (true)    //避免两次随机动画一样
        //{
        //    if (m_currentRandValue != randValue)
        //    {
        //        m_currentRandValue = randValue;

        //        break;
        //    }
        //    else
        //    {
        //        randValue = Random.Range(0, clipCount);
        //    }
        //}        
        //if (this.m_heroGo != null)
        //{
        //    int i = 0;
        //    foreach (AnimationState stat in this.m_heroGo.animation)
        //    {
        //        if(i==randValue)
        //        {
        //            //TraceUtil.Log(stat.name);
        //            var aniStr = stat.name;
        //            //var aniStr = "skill05";
        //            var behaviour = this.m_heroGo.GetComponent<UIHeroBehaviour>();
        //            SkillBase skill= behaviour.GetSkillByAniName(aniStr);
        //            if (skill != null)
        //            {
        //                skill.Fire();
        //            }
        //            else
        //            {
        //                this.m_heroGo.animation.CrossFade(aniStr);
        //            }

        //            return;
        //        }
        //        else
        //        {
        //            i++;
        //        }
        //    }
        //}
    }
    /// <summary>
    /// 改变主角服装
    /// </summary>
    /// <param name="avatarName"></param>
    public void ChangeAvatar(string avatarName)
    {
        if (this.m_heroGo)
        {
            RoleGenerate.GenerateRole(this.m_heroGo, avatarName);
        }
        else
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"主角对象不存在");
        }
    }
    /// <summary>
    /// 改变主角武器
    /// </summary>
    /// <param name="weaponName"></param>
    public void ChangeWeapon(string weaponName,GameObject weaponEff)
    {
        var weapon = Weapons.SingleOrDefault(P => P.name == weaponName);

        //configData.WeaponPosition.ApplyAllItem(P => //TraceUtil.Log("WeaponPosition:" + P));

        RoleGenerate.AttachWeapon(this.m_heroGo, m_configData.WeaponPosition, weapon,weaponEff);
    }
    protected override void RegisterEventHandler()
    {
    }
}
