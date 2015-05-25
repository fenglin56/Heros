using UnityEngine;
using System.Collections;
using UI.MainUI;
using UI;

public class PVPBattleUI : View 
{
    public GameObject GO_BattleUIButtonExit;

    public GameObject Mark_SkillButtons;    //按钮挡板

    public LocalButtonCallBack Button_Surrender;
    public GameObject GO_VS;
    public GameObject GO_ReadyStart;
    public UILabel Label_ReadyTime;
    public UILabel Label_BattleTime;

    public GameObject GO_PVPCompetitorStatus;
    public GameObject GO_PVPHeroStatus;
    //settle
    public GameObject PVPSettle;
    public UILabel Label_BackTownTime;

    public GameObject PVPHeroViewPrefab;
    public Transform HeroViewCameraTrans;

    public GameObject GO_DialogBoard;
    public GameObject GO_SettleInfo;
    public GameObject GO_PlayerNames;

    public UILabel Label_myDialog;
    public UILabel Label_competitorDialog;

    public UILabel Label_myName;
    public UILabel Label_competitorName;
    //public UILabel Label_myResult;
    //public UILabel Label_competitorResult;
    public UILabel Label_myPrestige;
    public UILabel Label_competitorPrestige;

    public Transform Trans_myResultTip;
    public Transform Trans_competitorResultTip;
    public GameObject GO_winnerTip;
    public GameObject GO_loserTip;

    private ContainerHeroView HeroView;          //英雄view
    private ContainerHeroView CompetitorView;    //对手view

    private int m_readyTime;
    private int m_battleTime;
    private int m_backTownTime;

    private const int time_backTown = 6;
    private const float scale_speed = 0.5f;
    private const float scale_time = 2f;

    private SMSGEctypePvpSettleAccounts_SC m_pvpSettleMsg;
    private bool m_isWin = false;
	void Awake () 
    {
        //        
        if (PVPBattleManager.Instance.IsPVPBattle)
        {
            Button_Surrender.SetCallBackFuntion(OnSurrenderClick, null);
            SetTure();
            this.RegisterEventHandler();
        }
	}

    //void OnDestroy()
    //{
    //    Time.timeScale = 1f;
    //}

    public void SetTure()
    {
        GO_BattleUIButtonExit.SetActive(false);
        GO_VS.SetActive(true);
        GO_PVPCompetitorStatus.SetActive(true);
        GO_PVPHeroStatus.SetActive(true);
        Button_Surrender.gameObject.SetActive(true);
        Mark_SkillButtons.SetActive(true);
    }

    //投降
    void OnSurrenderClick(object obj)
    {
        if (PVPBattleManager.Instance.CurrentState == PVPBattleManager.PVPBattleState.settle)
        {
            this.Surrender();
        }
        else
        {
            MessageBox.Instance.Show(4, "", LanguageTextManager.GetString("IDS_H1_355"), LanguageTextManager.GetString("IDS_H2_55"),
            LanguageTextManager.GetString("IDS_H2_28"), Surrender, null);        
        }
        
    }
    void Surrender()
    {
        Time.timeScale = 1f;//避免死亡慢镜情况下退出pvp
        NetServiceManager.Instance.EctypeService.SendEctypeRunAway();
    }
    //private void InitHeroViewCamera()
    //{
    //    StartCoroutine("AddCamera");
    //}
    private void AddCamera()
    {
        //yield return new WaitForSeconds(0.1f);
        //yield return new WaitForEndOfFrame();
        int Vocation = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
        if (HeroView == null)
        {
            GameObject heroViewCamera = (GameObject)Instantiate(PVPHeroViewPrefab);
            heroViewCamera.transform.parent = HeroViewCameraTrans;
            heroViewCamera.transform.localScale = Vector3.one;
            ContainerHeroView[] containerHeroViews = heroViewCamera.GetComponentsInChildren<ContainerHeroView>();
            int length = containerHeroViews.Length;
            if (length >= 2)
            {
                HeroView = containerHeroViews[0];
                CompetitorView = containerHeroViews[1];
            }

        }
        HeroView.ShowHeroModelView(Vocation);
        var competitorData = PVPBattleManager.Instance.GetPVPPlayerData();
        CompetitorView.ShowHeroModelView(competitorData.byKind);
    }


    void PVPReadyHandle(INotifyArgs arg)
    {
        SMSGEctypePvpReady_SC readyData = (SMSGEctypePvpReady_SC)arg;
        Label_ReadyTime.gameObject.SetActive(true);
        m_readyTime = (int)readyData.dwReadyTime;
        Label_ReadyTime.text = m_readyTime.ToString();
        InvokeRepeating("ReadyTiming", 1f, 1f);
    }
    private void ReadyTiming()
    {
        m_readyTime--;
        if (m_readyTime > 0)
        {
            Label_ReadyTime.text = m_readyTime.ToString();
        }
        else if (m_readyTime == 0)
        {
            //Label_ReadyTime.text = LanguageTextManager.GetString("IDS_H2_64");
            Label_ReadyTime.gameObject.SetActive(false);
            GO_ReadyStart.SetActive(true);
        }
        else if (m_readyTime < 0)
        {            
            GO_ReadyStart.SetActive(false);
            CancelInvoke("ReadyTiming");
        }        
    }

    void PVPFightingHandle(INotifyArgs arg)
    {        
        SMSGEctypePvpFighting_SC fightingData = (SMSGEctypePvpFighting_SC)arg;
        //very important
        PVPBattleManager.Instance.SetPVPState(PVPBattleManager.PVPBattleState.battle);
        Mark_SkillButtons.SetActive(false);

        Label_BattleTime.gameObject.SetActive(true);
        m_battleTime = (int)fightingData.dwFightingTime;
        TimingCount();
        InvokeRepeating("BattleTiming", 1f, 1f);
    }
    private void BattleTiming()
    {
        if (m_battleTime <= 0)
        {
            return;
        }
        m_battleTime--;
        TimingCount();
    }
    private void TimingCount()
    {
        int min = m_battleTime / 60;
        int sec = m_battleTime % 60;

        Label_BattleTime.text = "00:" + ToTimeString(min) + ":" + ToTimeString(sec);
    }
    private string ToTimeString(int value)
    {
        if (value < 10)
        {
            return "0" + value.ToString();
        }
        return value.ToString();
    }

    void PVPSettleAccountHandle(INotifyArgs arg)
    {        
        SMSGEctypePvpSettleAccounts_SC settleData = (SMSGEctypePvpSettleAccounts_SC)arg;
        m_pvpSettleMsg = settleData;

        PVPBattleManager.Instance.SetPVPState(PVPBattleManager.PVPBattleState.settle);

        m_isWin = settleData.dwWinerActorID == PlayerManager.Instance.FindHeroDataModel().ActorID;

        m_backTownTime = time_backTown;

        Label_BattleTime.gameObject.SetActive(false);
        CancelInvoke("BattleTiming");

        StartCoroutine("SettleAnimation");
        //PVPSettle.SetActive(true);
        //Label_BackTownTime.text = string.Format(LanguageTextManager.GetString("IDS_H1_356"), m_backTownTime.ToString());
        //this.AddCamera();
        //InvokeRepeating("BackTownTiming", 1f, 1f);
    }
    
    IEnumerator SettleAnimation()
    {        
        yield return new WaitForSeconds(2f);
        PVPSettle.SetActive(true);
        yield return new WaitForSeconds(1f);
        //Label_BackTownTime.text = string.Format(LanguageTextManager.GetString("IDS_H1_356"), m_backTownTime.ToString());
        this.AddCamera();
        GO_PlayerNames.SetActive(true);
        Label_myName.text = PlayerManager.Instance.FindHeroDataModel().Name;
        Label_competitorName.text =  PVPBattleManager.Instance.GetPVPPlayerData().szName;        
        InvokeRepeating("BackTownTiming", 1f, 1f);
    }
    private void BackTownTiming()
    {
        m_backTownTime--;
        if (m_backTownTime == 5)
        {
            GO_DialogBoard.SetActive(true);
            //随机语言
            var winLanguage = PlayerDataManager.Instance.GetPlayerPrestigeList()[0]._WinWord;
            var loseLanguage = PlayerDataManager.Instance.GetPlayerPrestigeList()[0]._LoseWord;
            int maxWinNum = winLanguage.Length;
            int loseWinNum = loseLanguage.Length;
            if (m_isWin)
            {                
                Label_myDialog.text = LanguageTextManager.GetString(winLanguage[UnityEngine.Random.Range(0, maxWinNum)]);
                Label_competitorDialog.text = LanguageTextManager.GetString(loseLanguage[UnityEngine.Random.Range(0, loseWinNum)]);
            }
            else
            {
                Label_myDialog.text = LanguageTextManager.GetString(loseLanguage[UnityEngine.Random.Range(0, loseWinNum)]);
                Label_competitorDialog.text = LanguageTextManager.GetString(winLanguage[UnityEngine.Random.Range(0, maxWinNum)]);
            }
        }
        if (m_backTownTime == 3)
        {
            GO_DialogBoard.SetActive(false);
            GO_SettleInfo.SetActive(true);

            //动作
            //我方玩家
            byte kind = (byte)PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
            var playerGenerate = PlayerDataManager.Instance.GetUIItemData(kind);
            //敌方
            var competitorData = PVPBattleManager.Instance.GetPVPPlayerData();
            var competitorGenerate = PlayerDataManager.Instance.GetUIItemData(competitorData.byKind);

            if (m_isWin)
            {
                //获胜
                GO_winnerTip.transform.position = Trans_myResultTip.position;
                //Label_myResult.text = "获胜";
                //Label_myResult.color = new Color(1f, 0.5f, 0);
                Label_myPrestige.text = "+" + m_pvpSettleMsg.dwPrestigeReward.ToString();
                GO_loserTip.transform.position = Trans_competitorResultTip.position;
                //Label_competitorResult.text = "战败";
                //Label_competitorResult.color = Color.white;
                Label_competitorPrestige.text = "-" + m_pvpSettleMsg.dwLoserPrestigeDeduct.ToString();

                if (playerGenerate != null)
                {
                    HeroView.PlayRandomAnim(playerGenerate.PVP_Success);
                }
                if (competitorGenerate != null)
                {
                    CompetitorView.PlayRandomAnim(competitorGenerate.PVP_Fail);
                }
            }
            else
            {
                //战败
                GO_loserTip.transform.position = Trans_myResultTip.position;
                //Label_myResult.text = "战败";
                //Label_myResult.color = Color.white;
                Label_myPrestige.text = "-" + m_pvpSettleMsg.dwLoserPrestigeDeduct.ToString();
                GO_winnerTip.transform.position = Trans_competitorResultTip.position;
                //Label_competitorResult.text = "获胜";
                //Label_competitorResult.color = new Color(1f, 0.5f, 0);
                Label_competitorPrestige.text = "+" + m_pvpSettleMsg.dwPrestigeReward.ToString();

                if (playerGenerate != null)
                {
                    HeroView.PlayRandomAnim(playerGenerate.PVP_Fail);
                }
                if (competitorGenerate != null)
                {
                    CompetitorView.PlayRandomAnim(competitorGenerate.PVP_Success);
                }
            }
            Label_BackTownTime.gameObject.SetActive(true);

            
            
        }
        if (m_backTownTime > 0)
        {
            Label_BackTownTime.text = string.Format(LanguageTextManager.GetString("IDS_H1_356"), m_backTownTime.ToString());
        }
        else
        {            
            CancelInvoke("BackTownTiming");
            NetServiceManager.Instance.EctypeService.SendEctypeChallengeComplete_CS();
        }        
    }


    void PlayerDieHandle(INotifyArgs arg)
    {
        StartCoroutine("PlayerDieAnimation");
    }
    IEnumerator PlayerDieAnimation()
    {
        Time.timeScale = scale_speed;
        yield return new WaitForSeconds(1 / scale_speed * scale_time);
        Time.timeScale = 1f;
    }

    protected override void RegisterEventHandler()
    {
        AddEventHandler(EventTypeEnum.PVPReady.ToString(), PVPReadyHandle);
        AddEventHandler(EventTypeEnum.PVPFighting.ToString(), PVPFightingHandle);
        AddEventHandler(EventTypeEnum.PVPSettleAccount.ToString(), PVPSettleAccountHandle);
        AddEventHandler(EventTypeEnum.EntityDie.ToString(), PlayerDieHandle);
    }
}
