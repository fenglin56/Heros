using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace UI.Team
{
    /// <summary>
    /// 世界队伍信息
    /// </summary>
    public class TeamInfoItem : MonoBehaviour ,IPagerItem
    {
        private int mTeamID;
        public SMsgTeamProp_SC SMsgTeamProp;
        //public UILabel TranscriptName;  //副本
        //public UILabel DifficultyName;  //难度
        public Transform[] MemberTrans = new Transform[3];  //三个成员的位置
        public TeamMemberItem ATeamMemberItem;  //队伍成员	   
        public LocalButtonCallBack ButtonJoinTeam;  //加入队伍按键

        private List<TeamMemberItem> TeamMemberItemList = new List<TeamMemberItem>(); //储存回收list表

        //private uint m_guideBtnID;

        void Awake()
        {
            ////TODO GuideBtnManager.Instance.RegGuideButton(ButtonJoinTeam.gameObject, MainUI.UIType.TeamInfo, SubUIType.JoinTeam, out m_guideBtnID);
        }

        public void Start()
        {
            ButtonJoinTeam.SetCallBackFuntion(OnJoinTeamClick);
        }

        public void InitInfo(int teamID, Transform guidParent)
        {
            transform.parent = guidParent;
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;

            this.mTeamID = teamID;
        }

        //public void InitInfo(SMsgTeamProp_SC sMsgTeamProp, Transform guidParent)
        //{
        //    transform.parent = guidParent;
        //    transform.localPosition = Vector3.zero;
        //    transform.localScale = Vector3.one;

        //    this.SMsgTeamProp = sMsgTeamProp;
        //}

        public void UpdateInfo(SMsgTeamProp_SC sMsgTeamProp)
        {
            this.SMsgTeamProp = sMsgTeamProp;

            if (!gameObject.activeInHierarchy)
            {
                gameObject.SetActive(true);
            }
            //\read world team info
            
            //\读取队员信息   
            int memberNum = SMsgTeamProp.TeamMemberNum_SC.wMemberNum;   
            
            int listLength = TeamMemberItemList.Count;
            if (memberNum > TeamMemberItemList.Count)
            {
                for (int i = listLength; i < memberNum; i++)
                {
                    TeamMemberItem item = ((GameObject)Instantiate(ATeamMemberItem.gameObject)).GetComponent<TeamMemberItem>();
                    item.InitInfo(MemberTrans[i]);
                    TeamMemberItemList.Add(item);
                }                                
            }
            int memberNo = 0;
            TeamMemberItemList.ApplyAllItem(p => 
                {
                    if(memberNo<memberNum)
                    {
                        //p.UpdateInfo(SMsgTeamProp.);
                        p.UpdateInfo(SMsgTeamProp.TeamMemberNum_SC.SMsgTeamPropMembers[memberNo]);
                    }
                    else
                    {
                        p.Close();
                    }
                    memberNo++;
                });
            
            //var ectypeConfig = EctypeConfigManager.Instance.EctypeSelectConfigList.SingleOrDefault(p => p.Value._lEctypeID == SMsgTeamProp.TeamContext.dwEctypeId).Value;
            //if (ectypeConfig != null)
            //    TranscriptName.text = LanguageTextManager.GetString(ectypeConfig._szName);
            //\临时
            //string difficultyName = "简单";
            //switch (SMsgTeamProp.TeamContext.byEctypeDifficulty)
            //{
            //    case 1:
            //        difficultyName = "普通";
            //        break;
            //}
            //DifficultyName.text = difficultyName;


            transform.name = "Team" + (mTeamID+100).ToString();
        }

        public void PlayShutterAnimation(float delayTime)
        {
            StopAllCoroutines();
            //StartCoroutine(TweenPosition(targetTrans, startPos, endPos, duration));
            transform.localPosition = new Vector3(-2000, 0, 0);

            StartCoroutine(ShutterAnimation(delayTime));
        }

        IEnumerator ShutterAnimation(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            transform.localPosition = Vector3.zero;
            var tweenSceleArray = gameObject.GetComponentsInChildren<TweenScale>();

            tweenSceleArray.ApplyAllItem(p =>
                {
                    p.Reset();
                    p.Play(true);
                });

            //Vector3 from = tweenSceleArray[0].from;
            //Vector3 to = tweenSceleArray[0].to;

            yield return new WaitForSeconds(tweenSceleArray[0].duration);

            tweenSceleArray[0].Play(false);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        //IEnumerator TweenPosition(Transform targetTrans, Vector3 startPos, Vector3 endPos, float duration)
        //{
        //    float i = 0;
        //    float rate = 1.0f / duration;
        //    while (i < 1.0)
        //    {
        //        i += Time.deltaTime * rate;
        //        targetTrans.position = Vector3.Lerp(startPos, endPos, i);
        //        yield return null;
        //    }
        //}

        void OnDestroy()
        {
            ////TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID);
        }

        void OnJoinTeamClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            var playerData = PlayerManager.Instance.FindHeroDataModel();
            NetServiceManager.Instance.TeamService.SendTeamMemberJoinMsg(new SMsgTeamMemberJoin_CS()
            {
                byJoinType = 0,
                byJoinAnswer = 1,
                dwTeamID = SMsgTeamProp.TeamContext.dwId,
                dwActorID = (uint)playerData.ActorID,
            });
        }

        public void OnGetFocus()
        {
        }

        public void OnLoseFocus()
        {
        }

        public void OnBeSelected()
        {
        }

        public Transform GetTransform()
        {
            return transform;
        }
    }
}
