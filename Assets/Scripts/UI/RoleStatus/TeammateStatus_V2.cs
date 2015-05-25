using UnityEngine;
using System.Collections;
using System;
using UI.Battle;
using UI;
using System.Linq;

public class TeammateStatus_V2 : MonoBehaviour {

	public enum TeammateStatus
	{
		Alive,
		Dead,
	}

    public UISprite HeadIcon;
	public SpriteSwith Swith_CostIcon;
    public RecoverTemmateBtn RecoveBtn;

    //const int RECOVER_TEAMATE_MONEY = 10;//复活队员需要金钱
	private int m_costMoney = 0;

	private TeammateStatus m_Stutas = TeammateStatus.Alive;

    void Awake()
    {
        RecoveBtn.SetCallBackFuntion(RecoverCallBack);        
    }

    public void InitMemberIcon(int vocation,int fashion, uint actorID,  bool isDead)
    {
        UpdateHeadIcon(vocation,fashion);
        ActorID = (int)actorID;
		if(m_Stutas == TeammateStatus.Dead)
		{
			StopAllCoroutines();
		}
		m_Stutas = isDead ? TeammateStatus.Dead : TeammateStatus.Alive;
        if (isDead)
        {
			int reTime = EctypeManager.Instance.GetTeammateReliveTimes((int)actorID)+1;
			//金钱公式 
			var pefectPrice = EctypeManager.Instance.GetCurrentEctypeData().PefectRevivePrice;
			m_costMoney = (int)((pefectPrice.Parma1*reTime*reTime+pefectPrice.Parma2*reTime+pefectPrice.Parma3)/pefectPrice.Parma4)*pefectPrice.Parma4;
			RecoveBtn.ShowBtn(m_costMoney, "");
			RecoveBtn.RecoverMoneyLabel.color = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BINDPAY >= m_costMoney? Color.white:Color.red;
        }

		if(m_Stutas == TeammateStatus.Dead)
		{
			StartCoroutine("RecoverCutDownTime");
		}

        RecoveBtn.gameObject.SetActive(isDead);
        HeadIcon.gameObject.SetActive(!isDead);
    }


    void UpdateHeadIcon(int vocationID,int fashionID)
    {
        var m_HeroDataModel = PlayerManager.Instance.FindHeroDataModel();
        //int fashionID = m_HeroDataModel.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_FASHION;
        var resData = CommonDefineManager.Instance.CommonDefineFile._dataTable.HeroIcon_TownAndTeam.FirstOrDefault(P => P.VocationID == vocationID && P.FashionID == fashionID);
        if (resData == null) { TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"找不到角色头像,fashionID:" + fashionID); }
        HeadIcon.spriteName = resData.ResName;
    }


    void RecoverCallBack(object obj)
    {
        int currentMoney = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BINDPAY;
		if (currentMoney >= m_costMoney)
        {            
            int myActorID = PlayerManager.Instance.FindHeroDataModel().ActorID;
            NetServiceManager.Instance.EntityService.SendActionRelivePlayer(myActorID, ActorID, (byte)EctypeRevive.ER_PREFECT);
        }
        else
        {
            MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H2_44"), 1);
        }
    }

	IEnumerator RecoverCutDownTime()
	{
		yield return new WaitForSeconds(EctypeManager.Instance.GetCurrentEctypeData().ReviveTime);
		var uiImageBtn = RecoveBtn.GetComponent<UIImageButton>();
		uiImageBtn.isEnabled = false;
	}

    public int ActorID { set; get; }
    //private long TeammateUID;
	
}
