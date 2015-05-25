using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.Battle
{

    public class TeammateUIManager : View
    {

        public GameObject TeammateUIPrefab;
        public TeammateStatus[] teammateScripts;

        private Vector3 vFirstTeammatePos = new Vector3(6, 32, 0);
        private const float fTeammateSpacing = 240;

        public Transform FaucetTrans;
        private Vector3 vFaucetPos = new Vector3(250, 68, 0);

        void Awake()
        {
            //RegisterEventHandler();
            UIEventManager.Instance.RegisterUIEvent(UIEventType.ReasetTeammateStatus, ResetStatus);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.TeamMemberLeave, ShowTeammate);
            AddEventHandler(EventTypeEnum.EctypeNoQualification.ToString(), TeammateQuitMessage);
        }

        void Start()
        {
            if (PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_ISGROUP == 0)
            {
                TraceUtil.Log("无组队状态:" + PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_ISGROUP);
                return;
            }
            ShowTeammate(null);

            SMsgTeamPropMember_SC[] SMsgTeamPropMembers = TeamManager.Instance.MyTeamProp.TeamMemberNum_SC.SMsgTeamPropMembers;
            
        }

        void OnDestroy()
        {
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ReasetTeammateStatus, ResetStatus);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.TeamMemberLeave, ShowTeammate);
            RemoveEventHandler(EventTypeEnum.EctypeNoQualification.ToString(), TeammateQuitMessage);

        }

        void TeammateQuitMessage(INotifyArgs inotifyArgs)
        {
            var ReceiveMsg = (SMSGEctypeResult_SC)inotifyArgs;
            if (ReceiveMsg.byResult == (ushort)SMSGEctypeResult_SC.ErrorType.PLAYERLEAVE)
            {
                SMsgTeamPropMember_SC[] SMsgTeamPropMembers = TeamManager.Instance.MyTeamProp.TeamMemberNum_SC.SMsgTeamPropMembers;
                foreach (var child in SMsgTeamPropMembers)
                {
                    TraceUtil.Log("收到离队消息：" + ReceiveMsg.dwActorIds[0] + "," + child.TeamMemberContext.dwActorID);
                    TraceUtil.Log("本人ID：" + PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity);
                }
                var TeammateInfo = SMsgTeamPropMembers.SingleOrDefault(P => P.TeamMemberContext.dwActorID == ReceiveMsg.dwActorIds[0]);
                string Name = TeammateInfo.TeamMemberContext.szName;
                string Msg = string.Format(LanguageTextManager.GetString("IDS_H1_159"),Name);
                MessageBox.Instance.ShowTips(3,Msg ,1);
            }
        }

        public void ShowTeammate(object obj)
        {
            //transform.ClearChild();

            if (!TeamManager.Instance.IsTeamExist())
                return;

            SMsgTeamPropMember_SC[] SMsgTeamPropMembers = TeamManager.Instance.MyTeamProp.TeamMemberNum_SC.SMsgTeamPropMembers;
            List<SMsgTeamPropMember_SC> NewSMsgTeamPropMembers = new List<SMsgTeamPropMember_SC>();;
            foreach (SMsgTeamPropMember_SC child in SMsgTeamPropMembers)
            {
                //if (child.TeamMemberContext.uidEntity != PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity)
                //{
                //    NewSMsgTeamPropMembers.Add(child);
                //}
                if (child.TeamMemberContext.dwActorID != PlayerManager.Instance.FindHeroDataModel().ActorID)
                {
                    NewSMsgTeamPropMembers.Add(child);
                }
            }
            teammateScripts.ApplyAllItem(p =>
            {
                Destroy(p.gameObject);
            });
            teammateScripts = new TeammateStatus[NewSMsgTeamPropMembers.Count];
            
            if (NewSMsgTeamPropMembers.Count <= 0)
            {
                FaucetTrans.gameObject.SetActive(false);
            }
            else
            {
                FaucetTrans.gameObject.SetActive(true);
                FaucetTrans.localPosition = new Vector3(vFaucetPos.x + fTeammateSpacing * (NewSMsgTeamPropMembers.Count - 1), vFaucetPos.y, vFaucetPos.z);
            }
            for (int i = 0; i < NewSMsgTeamPropMembers.Count; i++)
            {
                //SMsgPropCreateEntity_SC_MainPlayer sMsgPropCreateEntity_SC_MainPlayer = (SMsgPropCreateEntity_SC_MainPlayer)PlayerManager.Instance.GetEntityMode(teammateID[i]).EntityDataStruct;
                
                GameObject creatObj = CreatObjectToNGUI.InstantiateObj(TeammateUIPrefab, transform);
                creatObj.transform.localPosition = new Vector3(vFirstTeammatePos.x + fTeammateSpacing * i, vFirstTeammatePos.y, vFirstTeammatePos.z);                
                teammateScripts[i] = creatObj.GetComponent<TeammateStatus>();
                teammateScripts[i].SetPanelAttribute(NewSMsgTeamPropMembers[i]);
                teammateScripts[i].SetTeamleader(NewSMsgTeamPropMembers[i].TeamMemberContext.dwActorID == TeamManager.Instance.MyTeamProp.TeamContext.dwCaptainId ? 1 : 0);
            }
        }

        void ResetStatus(object obj)//更新单个属性
        {
            uint dwActorID = (uint)obj;
            SMsgTeamPropMember_SC[] SMsgTeamPropMembers = TeamManager.Instance.MyTeamProp.TeamMemberNum_SC.SMsgTeamPropMembers;

            foreach (SMsgTeamPropMember_SC child in SMsgTeamPropMembers)
            {
                if (child.TeamMemberContext.dwActorID == dwActorID)
                {
                    foreach (TeammateStatus UIChild in teammateScripts)
                    {
                        //Debug.LogWarning("teammateScripts.Legth:"+teammateScripts.Length+",TeammateID:"+UIChild.sMsgTeamPropMember_SC.TeamMemberContext.dwActorID+",CurrentID:"+dwActorID);
                        if (UIChild.sMsgTeamPropMember_SC.TeamMemberContext.dwActorID == dwActorID)
                        {
                            Debug.LogWarning("刷新队友状态:"+child.TeamMemberContext.szName);
                            UIChild.SetPanelAttribute(child);
                            return;
                        }
                    }
                }
            }

        }


        protected override void RegisterEventHandler()
        {
            throw new System.NotImplementedException();
        }
    }
}