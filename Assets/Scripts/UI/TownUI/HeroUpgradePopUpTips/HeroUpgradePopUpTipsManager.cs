using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace UI.Town
{

    public class HeroUpgradePopUpTipsManager : MonoBehaviour
    {

        public class AddEffectData
        {
            public EffectData effectData;
            public int AddNumber;            
        }

        public GameObject UpgradeTipsPrefab;
        public GameObject TipsPrefab;
        public Transform CreatTipsPoint;

        public List<AddEffectData> AddEffectList { get; private set; }

        Transform HeroTransform;
        bool IsShow = false;

        void Awake()
        {
            AddEffectList = new List<AddEffectData>();
        }


        IEnumerator Start()
        {
            yield return null;
            //TraceUtil.Log("检测是否升级");
            TypeID entityType = TypeID.TYPEID_BOX;
            Int64 uid = PlayerManager.Instance.FindHeroDataModel().UID;
            EntityModel HeroEntityModel = EntityController.Instance.GetEntityModel(uid, out entityType);
            if (HeroEntityModel != null &&HeroEntityModel.GO != null && HeroUpgradeLevelData.Instance.GetLevel()!=0)
            {
                HeroTransform = HeroEntityModel.GO.transform;
                int LastLevel = HeroUpgradeLevelData.Instance.GetLevel();
                int currentLevel = PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
                //int currentLevel = LastLevel + 5;

                int showUpdateLevel = CommonDefineManager.Instance.CommonDefineFile._dataTable.UpgradeAnimationStartLevel;

                if (currentLevel != LastLevel&&currentLevel>=showUpdateLevel)
                {
                    UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowMissionFailPanelLate,null);
                    GetRoleBaseValue(LastLevel, currentLevel);
                    StartCoroutine(ShowUpGradeTipsTitle());
                }
            }
        }

        IEnumerator ShowUpGradeTipsTitle()
        {
            IsShow = true;
            GameObject UpgradeTipsObj = CreatObjectToNGUI.InstantiateObj(UpgradeTipsPrefab, CreatTipsPoint);
            yield return new WaitForSeconds(2.5f);
            Destroy(UpgradeTipsObj);
            InvokeRepeating("ShowUpgradeTips", 0.1f, 0.3f);
        }

        void SaveLevel()
        {
            int currentLevel = PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
            HeroUpgradeLevelData.Instance.SaveLevel(currentLevel);
            //GameDataManager.Instance.ResetData(DataType.LastRoleLevel, currentLevel);
            TraceUtil.Log("保存当前等级:"+currentLevel);
        }

        void OnDestroy()
        {
            SaveLevel();
        }

        void GetRoleBaseValue(int lastLevel, int currentLevel)
        {
            AddEffectList.Clear();
            var m_HeroDataModel = PlayerManager.Instance.FindHeroDataModel();
            int vocation = m_HeroDataModel.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
			var configData = PlayerDataManager.Instance.PlayerBasePropConfigData.playerBasePropDatalist.Where(P=>P.PlayerKind == vocation);
				foreach (var child in configData)
            {
                int lastLevelRoleBaseValue = GetRoleBaseValue(vocation,lastLevel,child);
                int currentLevelRolebaseValue = GetRoleBaseValue(vocation,currentLevel,child);
                if (lastLevelRoleBaseValue < currentLevelRolebaseValue)
                { 
                    EffectData addEffectData = ItemDataManager.Instance.EffectDatas._effects.First(P=>P.m_SzName == child.BaseProp);
                    AddEffectList.Add(new AddEffectData() { effectData = addEffectData, AddNumber = currentLevelRolebaseValue - lastLevelRoleBaseValue });
                }
            }
        }

        void ShowUpgradeTips()
        {
            if (AddEffectList.Count > 0)
            {
                AddEffectData addData = AddEffectList[0];
                AddEffectList.RemoveAt(0);
                SingleUpgradetips singleUpgradetips = CreatObjectToNGUI.InstantiateObj(TipsPrefab, CreatTipsPoint).GetComponent<SingleUpgradetips>();
                singleUpgradetips.Show(addData.effectData, addData.AddNumber);
            }
            else
            {
                CancelInvoke();
                StartCoroutine(StopShowForTime(2));
            }
        }

        IEnumerator StopShowForTime(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            IsShow = false;
        }

        void Update()
        {
            //if (Input.GetKeyDown(KeyCode.C))
            //{
            //    StartCoroutine(Start());
            //}
            //if (Input.GetKeyDown(KeyCode.S))
            //{
            //    SaveLevel();
            //}
            if(null == HeroTransform)
            {
                TypeID entityType = TypeID.TYPEID_BOX;
                Int64 uid = PlayerManager.Instance.FindHeroDataModel().UID;
                EntityModel HeroEntityModel = EntityController.Instance.GetEntityModel(uid, out entityType);
                if(HeroEntityModel != null && HeroEntityModel.GO != null)
                {
                    HeroTransform = HeroEntityModel.GO.transform;
                }
            }

            if (IsShow && null != HeroTransform )
            {
                Vector3 ShowPosition = PopupTextController.GetPopupPos(HeroTransform.position+Vector3.up*18,UICamera.mainCamera);
                CreatTipsPoint.position = ShowPosition;
            }
        }

        int GetRoleBaseValue(int Vocation, int Level,PlayerBasePropData PlayerData)
        {
            int m_value = (int)(Mathf.FloorToInt((PlayerData.ParamA * Mathf.Pow(Level, 2) + PlayerData.ParamB * Level + PlayerData.ParamC) / PlayerData.ParamD) * PlayerData.ParamD);
            //TraceUtil.Log("获取基础值:" + PlayerData.BaseProp + ":" + string.Format("A:{0},B:{1},C:{2},D:{3},Value:{4}", PlayerData.ParamA, PlayerData.ParamB, PlayerData.ParamC, PlayerData.ParamD, m_value));
            return m_value;
        }
    }

}