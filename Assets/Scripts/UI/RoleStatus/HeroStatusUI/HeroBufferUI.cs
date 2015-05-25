using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UI.Battle
{

    public class HeroBufferUI : MonoBehaviour
    {

        public UISprite BufferSprit;

        //private List<BufferUIInfo> BufferUIList;
        //private List<SMsgActionWorldObjectAddBuff_SC> CreatBufferList;

        void Awake()
        {
            //BufferSprit.gameObject.SetActive(false);
            //BufferUIList = new List<BufferUIInfo>();
            //CreatBufferList = new List<SMsgActionWorldObjectAddBuff_SC>();
            //UIEventManager.Instance.RegisterUIEvent(UIEventType.CreatBuffer, CreatBuffer);
            //UIEventManager.Instance.RegisterUIEvent(UIEventType.RemoveBuffer, RemoveBuffer);
            GameDataManager.Instance.dataEvent.RegisterEvent(DataType.GameBufferData,ResetBuffer);
            ResetBuffer(null);
        }

        void OnDestroy()
        {
            //UIEventManager.Instance.RemoveUIEventHandel(UIEventType.CreatBuffer, CreatBuffer);
            //UIEventManager.Instance.RemoveUIEventHandel(UIEventType.RemoveBuffer, RemoveBuffer);
            GameDataManager.Instance.dataEvent.RemoveEventHandel(DataType.GameBufferData, ResetBuffer);
        }

        void ResetBuffer(object obj)
        {
            if (BattleConfigManager.Instance == null)
                return;
            RoleBuffList roleBuffList = GameDataManager.Instance.PeekData(DataType.GameBufferData) as RoleBuffList;
            if (roleBuffList == null) { return;  }
            TraceUtil.Log("刷新Buffer:"+roleBuffList.BufferList.Count);
            if (roleBuffList == null)
                return;
            transform.ClearChild();
            for (int i = 0; i < roleBuffList.BufferList.Count; i++)
            {
                GameObject CreatObj = CreatBuffer(roleBuffList.BufferList[i]);
                SetBufferListPosition(i,CreatObj.transform);
                TraceUtil.Log("收到buffer:" + roleBuffList.BufferList[i].dwBuffId);
            }
        }

        GameObject CreatBuffer(SMsgActionWorldObjectAddBuff_SC sMsgActionWorldObjectAddBuff_SC)
        {
            string IconName = BattleConfigManager.Instance.BuffConfigList[sMsgActionWorldObjectAddBuff_SC.dwBuffId]._iconID;
            UISprite CreatUISprite = CreatObjectToNGUI.InstantiateObj(BufferSprit.gameObject, transform).GetComponent<UISprite>();
            CreatUISprite.enabled = true;
            CreatUISprite.spriteName = IconName;
            return CreatUISprite.gameObject;
        }

        //void CreatBuffer(object obj)
        //{
        //    SMsgActionWorldObjectAddBuff_SC sMsgActionWorldObjectAddBuff_SC = (SMsgActionWorldObjectAddBuff_SC)obj;
        //    TraceUtil.Log("角色获得Buffid：" + sMsgActionWorldObjectAddBuff_SC.dwBuffId+",Index:"+sMsgActionWorldObjectAddBuff_SC.dwIndex);
        //    string IconName = BattleConfigManager.Instance.BuffConfigList[sMsgActionWorldObjectAddBuff_SC.dwBuffId]._iconID;
        //    foreach (var child in BufferUIList)
        //    {
        //        if (child.BufferInfo.dwBuffId == sMsgActionWorldObjectAddBuff_SC.dwBuffId)
        //            return;
        //    }
        //    //CreatBufferList.Add(sMsgActionWorldObjectAddBuff_SC);
        //    UISprite CreatUISprite = CreatObjectToNGUI.InstantiateObj(BufferSprit.gameObject,transform).GetComponent<UISprite>();
        //    CreatUISprite.gameObject.SetActive(true);
        //    CreatUISprite.spriteName = IconName;
        //    BufferUIList.Add(new BufferUIInfo(sMsgActionWorldObjectAddBuff_SC,CreatUISprite));
        //    ResetBufferListPosition();
        //}

        //void RemoveBuffer(int Index)
        //{
        //    //var RemoveBufferStruct = (SMsgActionWorldObjectRemoveBuff_SC)obj;
        //    //if (RemoveBufferStruct.SMsgActionSCHead.uidEntity != PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity)
        //    //    return;
        //    TraceUtil.Log("删除的IndexID：" + Index);
        //    foreach (var child in BufferUIList)
        //    {
        //        if (child.BufferInfo.dwIndex == Index)
        //        {
        //            BufferUIList.Remove(child);
        //            Destroy(child.SpriteInfo.gameObject);
        //            break;
        //        }
        //    }
        //    ResetBufferListPosition();
        //}

        void SetBufferListPosition(int id, Transform child)
        {
            if (id < 5)
                {
                    child.localPosition = new Vector3(child.localScale.x * id, 0, 0);
                }
            else if (id < 10)
                {
                    child.localPosition = new Vector3(child.localScale.x * (id -5), -child.localScale.y, 0);
                }
                else
                {
                    child.localPosition = new Vector3(0,1000,0);
                }
            
        }

    }

    public class BufferUIInfo
    {
        public SMsgActionWorldObjectAddBuff_SC BufferInfo;
        public UISprite SpriteInfo;
        public BufferUIInfo(SMsgActionWorldObjectAddBuff_SC Info, UISprite uiSprite)
        {
            this.BufferInfo = Info;
            this.SpriteInfo = uiSprite;
        }


    }

}