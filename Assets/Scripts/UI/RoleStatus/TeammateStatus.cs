using UnityEngine;
using System.Collections;

namespace UI.Battle
{
    public class TeammateStatus : MonoBehaviour
    {
        //**************组队后显示队友的头像及血量各种状态的UI控制代码************************

        public SpriteSwith HeadIcon;//头像

        public SpriteSwith Profession;//职业
        public UILabel TeammateName;//名字
        public UILabel TeammateLv;//等级
        public UISlider TeammateHP;//血值
        public UISlider TeammateMP;//真气值


        public SMsgTeamPropMember_SC sMsgTeamPropMember_SC;

        public SpriteSwith TeamleaderIcon;
        public GameObject RecoverTemmateBtnPrefab;
        public Transform CreatBtnPoint;

        private RecoverTemmateBtn recoverTemmateBtnScripts;

        private int RecoverTeamateMoney = 10;
        private float currentHP = -1;
        private float currentMP =-1;

        //bool Isalive = false;
        
        public void SetPanelAttribute(SMsgTeamPropMember_SC sMsgTeamPropMember_SC)
        {
            //TraceUtil.Log("显示队友状态：" + sMsgTeamPropMember_SC.TeamMemberContext.szName);
            //Debug.LogWarning("HP" + sMsgTeamPropMember_SC.TeamMemberContext.nCurHP + "," + sMsgTeamPropMember_SC.TeamMemberContext.nMaxHP);            
            this.sMsgTeamPropMember_SC = sMsgTeamPropMember_SC;
            int vocation = sMsgTeamPropMember_SC.TeamMemberContext.byKind;            
            string TeammateName = sMsgTeamPropMember_SC.TeamMemberContext.szName;
            float TeammateHP = sMsgTeamPropMember_SC.TeamMemberContext.nCurHP;
            float TeammateMP = sMsgTeamPropMember_SC.TeamMemberContext.nCurMP;
            int Level = sMsgTeamPropMember_SC.TeamMemberContext.nLev;
            //HeadIcon.ChangeSprite(vocation);
            Profession.ChangeSprite(vocation);
            this.TeammateName.text = TeammateName;
            this.TeammateLv.text = Level.ToString();
            if (currentHP != TeammateHP)
            {
                TweenFloat.Begin(1, currentHP, TeammateHP, SetTeammateHP);
                currentHP = TeammateHP;
            }
            if (currentMP != TeammateMP)
            {
                TweenFloat.Begin(1, currentMP, TeammateMP, SetTeammateMP);
                currentMP = TeammateMP;
            }
            SetRoleStatus(currentHP <= 0 ? false : true);
        }

        //public void SetPanelAttribute(SMsgPropCreateEntity_SC_MainPlayer TeammateEntityMode)
        //{
        //    this.TeammateEntityMode = TeammateEntityMode;
        //    int vocation = TeammateEntityMode.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
        //    string TeammateName =TeammateEntityMode.Name;
        //    float TeammateHP = TeammateEntityMode.UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_CURHP;
        //    float TeammateMP = TeammateEntityMode.UnitValues.sMsgPropCreateEntity_SC_UnitInvisibleValue.UNIT_FIELD_CURMP;
        //    HeadIcon.ChangeSprite(vocation);
        //    this.TeammateName.text = TeammateName;
        //    if (currentHP != TeammateHP) 
        //    {
        //        TweenFloat.Begin(1, currentHP, TeammateHP, SetTeammateHP);
        //        currentHP = TeammateHP;
        //    }
        //    if (currentMP != TeammateMP) 
        //    {
        //        TweenFloat.Begin(1, currentMP, TeammateMP, SetTeammateMP);
        //        currentMP = TeammateMP;
        //    }
        //    if (currentHP <= 0)
        //    {
        //        SetRoleStatus(false);
        //    }
        //    else
        //    {
        //        SetRoleStatus(true);
        //    }
        //    //SetTeammateHP(TeammateHP);
        //    //SetTeammateMP(TeammateMP);
        //}

        public void SetTeamleader(int ID)//队长标志
        {
            TeamleaderIcon.ChangeSprite(ID);
        }

        void SetTeammateHP(float HP)
        {
            this.TeammateHP.sliderValue = HP / sMsgTeamPropMember_SC.TeamMemberContext.nMaxHP;
        }

        void SetTeammateMP(float MP)
        {
            this.TeammateMP.sliderValue = MP / sMsgTeamPropMember_SC.TeamMemberContext.nMaxMP;
        }

        void SetRoleStatus(bool isalive)
        {
            //if (Isalive == isalive)
            //    return;
            //Isalive = isalive;
            if (isalive)
            {
                CreatBtnPoint.ClearChild();
            }
            else
            {
                CreatBtnPoint.ClearChild();
                recoverTemmateBtnScripts = CreatObjectToNGUI.InstantiateObj(RecoverTemmateBtnPrefab,CreatBtnPoint).GetComponent<RecoverTemmateBtn>();
                recoverTemmateBtnScripts.SetCallBackFuntion(RecoverCallBack);
                recoverTemmateBtnScripts.ShowBtn(RecoverTeamateMoney, "");
            }
        }

        void RecoverCallBack(object obj)
        {
            int currentMoney = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BINDPAY;
            if (currentMoney > RecoverTeamateMoney)
            {
                int myActorID = PlayerManager.Instance.FindHeroDataModel().ActorID;
                int TargetActor = (int)this.sMsgTeamPropMember_SC.TeamMemberContext.dwActorID;
                NetServiceManager.Instance.EntityService.SendActionRelivePlayer(myActorID, TargetActor, (byte)EctypeRevive.ER_PREFECT);
                TraceUtil.Log("发送复活队员请求,m_UID:" + myActorID + ",TargetUID:" + TargetActor);
            }
            else
            {
                //recoverTemmateBtnScripts.GrayButton(LanguageTextManager.GetString("IDS_H2_44"));//元宝不足
                MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H2_44"), 1);
            }
            //if(currentMoney>)
        }

        public void DestroyMySelf()
        {
            Destroy(this.gameObject);
        }
    }
}
