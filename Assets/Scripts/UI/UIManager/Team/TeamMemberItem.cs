using UnityEngine;
using System.Collections;

namespace UI.Team
{
    /// <summary>
    /// 世界队伍成员
    /// </summary>
    public class TeamMemberItem : MonoBehaviour
    {
        public SMsgTeamPropMember_SC MsgTeamPropMember;

        public SpriteSwith AvatarSwitch;
        public UILabel Label_Profession;
        public UILabel LevelLabel;
        public UILabel NickNameLabel;
        

        public void InitInfo(Transform posTrans)
        {
            transform.parent = posTrans;
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
        }

        //public void InitInfo(Transform posTrans, SMsgTeamPropMember_SC sMsgTeamPropMember)
        //{
        //    transform.parent = posTrans;
        //    transform.localPosition = Vector3.zero;
        //    transform.localScale = Vector3.one;

            
        //}


        public void UpdateInfo(SMsgTeamPropMember_SC sMsgTeamPropMember)
        {
            this.MsgTeamPropMember = sMsgTeamPropMember;

            if (!gameObject.activeInHierarchy)
            {
                gameObject.SetActive(true);
            }

            AvatarSwitch.ChangeSprite(sMsgTeamPropMember.TeamMemberContext.byKind);

            //var professionData = PlayerDataManager.Instance.GetProfessionConfigData(sMsgTeamPropMember.TeamMemberContext.byKind);
            //Label_Profession.text = LanguageTextManager.GetString(professionData._professionName);

            LevelLabel.text = MsgTeamPropMember.TeamMemberContext.nLev.ToString();
            NickNameLabel.text = MsgTeamPropMember.TeamMemberContext.szName;

        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}

