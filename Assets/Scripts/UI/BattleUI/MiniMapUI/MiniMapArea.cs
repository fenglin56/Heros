using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UI.Battle
{
    public class MiniMapArea : View
    {

        private SceneConfigData sceneConfigData;
        public GameObject BackagroundObj;
        public Transform ShowPointPosition;

        public GameObject PortalUIPrefab;
        public GameObject PointTeammateUIPrefab;
        public GameObject PointHeroUIPrefab;
        public GameObject PointMonsterUIPreafab;

        void Start()
        {
            UIEventManager.Instance.RegisterUIEvent(UIEventType.CreatMoster,AddMonsterMiniMapUI);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.CreatPortal,AddPortalMiniMapUI);
            RegisterEventHandler();
        }

        protected override void RegisterEventHandler()
        {
            AddEventHandler(EventTypeEnum.EntityCreate.ToString(),AddRoleMiniMapUI);
        }

        void OnDestroy()
        {
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.CreatMoster, AddMonsterMiniMapUI);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.CreatPortal, AddPortalMiniMapUI);
            RemoveEventHandler(EventTypeEnum.EntityCreate.ToString(), AddRoleMiniMapUI);
        }

        public void SetMapAreaSize(SceneConfigData sceneConfigData)
        {
            this.sceneConfigData = sceneConfigData;
            float mapWidth = sceneConfigData._mapWidth;
            float mapHeight = sceneConfigData._mapHeight;
            float XSize = mapHeight <= mapWidth ? 100 : mapWidth * 100 / mapHeight;
            float YSize = mapWidth <= mapHeight ? 100 : mapHeight * 100 / mapWidth;
            BackagroundObj.transform.localScale = new Vector3(XSize, YSize, 1);
        }

        public void ShowMiniMapUIPoint()
        {
            ShowHeroMiniMapUI();
            List<EntityModel> entityModelList = MonsterManager.Instance.GetMonstersList();
            foreach (EntityModel child in entityModelList)
            {
                ShowMonsterMiniMapUI(child.GO);
            }
        }

        void AddRoleMiniMapUI(INotifyArgs iNotifyArgs)
        {
            SMsgPropCreateEntity_SC_Header sMsgPropCreateEntity_SC_Header = (SMsgPropCreateEntity_SC_Header)iNotifyArgs;
            if (sMsgPropCreateEntity_SC_Header.uidEntity!=0) 
            {
                GameObject CreatObj = PlayerManager.Instance.FindPlayer(sMsgPropCreateEntity_SC_Header.uidEntity);
                ShowTeammateMiniMapUI(CreatObj);
            }
        }

        void AddMonsterMiniMapUI(object obj)//添加新怪物UI
        {
            EntityModel entityModel = (EntityModel)obj;
            ShowMonsterMiniMapUI(entityModel.GO);
        }

        void AddPortalMiniMapUI(object obj)
        {
            EntityModel portalData = (EntityModel)obj;
            ShowPortalManagerUI(portalData.GO);
        }

        void ShowHeroMiniMapUI()
        {
            GameObject HeroUIObj = CreatObjectToNGUI.InstantiateObj(PointHeroUIPrefab, ShowPointPosition);
            MinimapPointBlick miniMapPointSetting = HeroUIObj.GetComponent<MinimapPointBlick>();
            GameObject PlayerObj = PlayerManager.Instance.FindHero();
            miniMapPointSetting.SetPosition(PlayerObj,this.sceneConfigData);
            miniMapPointSetting.BeginBlik();
        }

        void ShowTeammateMiniMapUI(GameObject TeammateObj)
        {
            GameObject RoleObjUI = CreatObjectToNGUI.InstantiateObj(PointTeammateUIPrefab, ShowPointPosition);
            RoleObjUI.GetComponent<MiniMapPointSetting>().SetPosition(TeammateObj, this.sceneConfigData);
            
        }

        void ShowPortalManagerUI(GameObject obj)
        {
            GameObject PortalObjUI = CreatObjectToNGUI.InstantiateObj(PortalUIPrefab, ShowPointPosition);
            MinimapPointBlick minimapUIsetting = PortalObjUI.GetComponent<MinimapPointBlick>();
            minimapUIsetting.SetPosition(obj, this.sceneConfigData);
            minimapUIsetting.BeginBlik();
        }

        void ShowMonsterMiniMapUI(GameObject MonsterObj)
        {
            GameObject RoleObjUI = CreatObjectToNGUI.InstantiateObj(PointMonsterUIPreafab, ShowPointPosition);
            RoleObjUI.GetComponent<MiniMapPointSetting>().SetPosition(MonsterObj, this.sceneConfigData);
            
        }


    }
}