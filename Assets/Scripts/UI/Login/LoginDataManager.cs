using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UI.Login
{

    public class LoginDataManager : MonoBehaviour
    {
        private static LoginDataManager m_instance;
        public static LoginDataManager Instance { get { return m_instance; } }

        /// <summary>
        /// 生成角色配表
        /// </summary>
        public PlayerGenerateConfigDataBase playerGenerateConfigDataBase;
        private Dictionary<int, PlayerGenerateConfigData> PlayerGenerateConfigDataList = new Dictionary<int, PlayerGenerateConfigData>();
        /// <summary>
        /// 新建角色配表
        /// </summary>
        public NewCharacterConfigDataBase newCharacterConfigDataBase;
        private Dictionary<int, NewCharacterConfigData> NewCharacterConfigDataList = new Dictionary<int, NewCharacterConfigData>();

        public CreateRoleUIDataBase CreateRoleUIDataBase;
        private List<CreateRoleUIData> m_createRoleUIList = new List<CreateRoleUIData>();
        /// <summary>
        /// 语言配置表
        /// </summary>
        public LanguageDataBase LanguageTextDataBase;
        private Dictionary<string, LanguageTextEntry> LanguageTextDataList = new Dictionary<string, LanguageTextEntry>();
        /// <summary>
        /// 随机名字
        /// </summary>
        public CharacterNameDataBase characterNameDataBase;

        public LoginUIManagerFor91 LocalLoginUI;
        public PlatformLoginBehaviour PlatformLoginUI;

        void Awake()
        {
            m_instance = this;
            InitNewCharacterConfigData();
            InitPlayerGenerateConfigData();
            InitLanguageConfigData();
            InitCreateRoleUIData();
            InitLoginUIViaPlatformType();
        }
        void InitLoginUIViaPlatformType()
        {
            //如果不是登录本地，则禁用此脚本
            var isLocalLogin = GameManager.Instance.PlatformType == PlatformType.Local;
            PlatformLoginUI.enabled = !isLocalLogin;
            LocalLoginUI.enabled = isLocalLogin;
        }
        //void OnGUI()
        //{
        //    if (GUILayout.Button("Notify",GUILayout.Width(200),GUILayout.Height(50)))
        //    {
        //        JHPlatformConnManager.Instance.Notify("你有一个小消息", "江湖", "你的等级已经提升到15级",10);
        //    }
        //}
        void OnDestroy()
        {
            m_instance = null;
        }
       
        void InitLanguageConfigData()
        {
            foreach (LanguageTextEntry child in LanguageTextDataBase.stringTable)
            {
                if (LanguageTextDataList.ContainsKey(child.key))
                {
                    TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"相同ID：" + child.key);
                }
                else
                {
                    LanguageTextDataList.Add(child.key, child);
                }
            }
        }

        void InitNewCharacterConfigData()
        {
            foreach (NewCharacterConfigData child in newCharacterConfigDataBase.NewCharacterConfigDataList)
            {
                NewCharacterConfigDataList.Add(child.VocationID, child);
            }
        }

        void InitCreateRoleUIData()
        {
            foreach (var child in CreateRoleUIDataBase._dataTable)
            {
                m_createRoleUIList.Add(child);
            }
        }

        void InitPlayerGenerateConfigData()
        {
            foreach (PlayerGenerateConfigData child in playerGenerateConfigDataBase._dataTable)
            {
                PlayerGenerateConfigDataList.Add(child.PlayerId, child);
            }
        }

        public string GetLanguageTextData(string LanguageKey)
        {
            if (LanguageTextDataList.ContainsKey(LanguageKey))
            {
                return LanguageTextDataList[LanguageKey].text;
            }
            else return "null";
        }

        public NewCharacterConfigData GetNewCharacterConfigData(int PlayerID)
        {
            if (NewCharacterConfigDataList.ContainsKey(PlayerID))
            {
                return NewCharacterConfigDataList[PlayerID];
            }
            else
            {
                return null;
            }
        }

        public List<CreateRoleUIData> GetCreateRoleUIData
        {
            get { return m_createRoleUIList; }
        }

        public PlayerGenerateConfigData GetPlayerGenerateConfigData(int PlayerID)
        {
            return PlayerGenerateConfigDataList[PlayerID];
        }

    }

    public enum LoginUIType
    {
        CreatRole,
        SelectRole,
        Login,
        Loaing,
        JoinGame,
        ServerList,
        LoginPlatformFail,
    }
}