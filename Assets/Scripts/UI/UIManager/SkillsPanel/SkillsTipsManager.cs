using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UI.MainUI;
//无用了、可删除
namespace UI.MainUI
{
    public class SkillsTipsManager : MonoBehaviour
    {
        public GameObject TipsGo;
		public Vector3 newSkillCenterPos;
        private GetNewSkillTips m_newSkillTips;
        private SingleSkillInfoList m_playerSkillList;//装备技能列表

        private List<SingleSkillInfo> m_curUnlockList = new List<SingleSkillInfo>();
        private List<SingleSkillInfo> m_curEquipList = new List<SingleSkillInfo>();
        // Use this for initialization
        

        void Start()
        {
            //Invoke("InitSkillData", 0.5f);
        }

        //void OnGUI()
        //{
        //    if (GUILayout.Button("EnablePvp"))
        //    {
        //        ShowAtkInfo();
        //    }
        //}

        //int i = 1;
        ///// <summary>
        ///// 显示战力信息
        ///// </summary>


        void InitSkillData()
        {
			var playerLv = PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
            if (playerLv <= CommonDefineManager.Instance.CommonDefine.SkillTipsStartLevel)  //当玩家等级小于指定等级，不弹出新技能面板
                return;
			
            m_playerSkillList = new SingleSkillInfoList();

            foreach (SingleSkillInfo item in m_playerSkillList.singleSkillInfoList)
            {
                if (!item.Lock)
                {
                    m_curUnlockList.Add(item);
                }
            }

            foreach (SingleSkillInfo item in m_playerSkillList.EquipSkillsList)
            {
                if (item != null)
                {
                    m_curEquipList.Add(item);
                }
            }

            NewSkillTipsCheck();
        }

        private void NewSkillTipsCheck()//设置各种属性
        {
			if (m_curEquipList.Count < 4 && m_curEquipList.Count < m_curUnlockList.Count && PlayerDataManager.Instance.CanPopTip(EViewType.ENewSkillType))
            {
                if (m_newSkillTips == null)
                {
                    m_newSkillTips = UI.CreatObjectToNGUI.InstantiateObj(TipsGo, transform).GetComponent<GetNewSkillTips>();
                }
				m_newSkillTips.transform.localPosition = newSkillCenterPos;
                SoundManager.Instance.PlaySoundEffect("Sound_Voice_GetNewQuest");
                m_newSkillTips.SetEquipSkillList = m_playerSkillList.EquipSkillsList;
				
				for (int i = 0; i < m_curUnlockList.Count; i++) {

                    bool isInclude = m_curEquipList.Exists(P => P.localSkillData.m_skillId == m_curUnlockList[i].localSkillData.m_skillId);

                    if (!isInclude)
                    {
                        m_newSkillTips.Show(m_curUnlockList[i]);
                    }
                    
                    //for (int j = 0; j < m_curEquipList.Length; j++) {
                    //    if(m_curUnlockList[i].localSkillData.m_skillId )
                    //}
				}
                
            }
        }

    }

}