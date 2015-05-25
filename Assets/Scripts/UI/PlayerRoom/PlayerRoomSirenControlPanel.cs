using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace UI.PlayerRoom
{
    public class PlayerRoomSirenControlPanel : MonoBehaviour
    {
        public YaoNvPracticeInfoItem YaoNvPracticeInfoItemPrefab;
        public ItemPagerManager ItemPagerManager_YaoNv;
        public SingleButtonCallBack Button_Close;
        public UILabel Label_YaoNvNum;
        public PlayerRoomAccoutConfigDataBase PlayerRoomAccoutConfigDataBase;

        private List<YaoNvPracticeInfoItem> m_InfoList = new List<YaoNvPracticeInfoItem>();
        private Dictionary<int, GameObject> m_SirenDict = new Dictionary<int, GameObject>();
        private List<SirenPosInfo> m_SirenPosInfoList = new List<SirenPosInfo>();

        private bool m_IsInit = false;

        void Awake()
        {
            ItemPagerManager_YaoNv.OnPageChanged += ItemPageChangedHandle;
            Button_Close.SetCallBackFuntion(ClosePanelHandle, null);            
        }

        public void ShowPanel(bool isHomer)
        {
            transform.localPosition = Vector3.zero;
            if (m_IsInit == false)
            {
                InitItems(isHomer);
                m_IsInit = true;
            }
        }

        public void HidePanel()
        {
            transform.localPosition = new Vector3(0, 0, -1000);
        }

        void ClosePanelHandle(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            HidePanel();
        }

        public void CreateSirens()
        {
            m_SirenPosInfoList = PlayerRoomAccoutConfigDataBase._dataTable[0].SirenPosInfoList;
            var sirenData = PlayerRoomDataManager.Instance.GetPlayerSirenList();
            int dataLength = sirenData.Count;
            for (int i = 0; i < dataLength; i++)
            {
                int maxlevel = sirenData[i]._growthMaxLevel;
                var sirenConfigData = sirenData[i]._sirenConfigDataList.SingleOrDefault(p => p._growthLevels == maxlevel);
                //创建妖女模型
                CreateSiren(sirenData[i]._sirenID, sirenConfigData._dzPrefab);                
            }
        }


        private void InitItems(bool isHomer)
        {
            var sirenData = PlayerRoomDataManager.Instance.GetPlayerSirenList();
            int dataLength = sirenData.Count;
            //判断妖女是否炼化完成
            var lianHuaList = SirenManager.Instance.GetYaoNvList();
            //妖女展现
            var yaonvUpdateInfo = PlayerRoomManager.Instance.GetYaoNvUpdateInfo();
            int itemNum = 0;
            for (int i = 0; i < dataLength; i++)
            {
                var lianHuaInfo = lianHuaList.SingleOrDefault(p => p.byYaoNvID == sirenData[i]._sirenID);
                var sirenConfigData = sirenData[i]._sirenConfigDataList.SingleOrDefault(p => p._growthLevels == lianHuaInfo.byLevel);
                //如果是房主，初始化妖女展示面板
                if (isHomer)
                {
                    if (lianHuaInfo.byLevel == sirenData[i]._growthMaxLevel)//炼化完成
                    {
                        itemNum++;
                        GameObject item = (GameObject)Instantiate(YaoNvPracticeInfoItemPrefab.gameObject);
                        YaoNvPracticeInfoItem itemScript = item.GetComponent<YaoNvPracticeInfoItem>();
                        item.transform.parent = ItemPagerManager_YaoNv.transform;
                        item.transform.localScale = Vector3.one;

                        itemScript.Set(sirenData[i]._sirenID, sirenData[i]._name, yaonvUpdateInfo.dwYaoNvList[i], sirenConfigData._sitEffect.ToString(), RecoverSirenCallBack, ReleaseSirenCallBack);
                        m_InfoList.Add(itemScript);
                    }
                }
                //创建妖女模型
                //CreateSiren(sirenData[i]._sirenID, sirenConfigData._prefab);
            }
            //妖女展示数量            
            int sirenNum = sirenData.Count;
            Label_YaoNvNum.text = itemNum.ToString() + "/" + sirenNum.ToString();

            //初始化页码           
            if (m_InfoList.Count == 0)
            {
                ItemPagerManager_YaoNv.InitPager(1, 1, 0);
            }
            else
            {
                ItemPagerManager_YaoNv.InitPager(m_InfoList.Count, 1, 0);
            }          

			//更新按钮状态
			UpdateSirenModel(yaonvUpdateInfo.dwYaoNvList);

        }

        void ItemPageChangedHandle(PageChangedEventArg pageSmg)
        {           
            m_InfoList.ApplyAllItem(p =>
                {
                    p.gameObject.SetActive(false);
                });
            int size = ItemPagerManager_YaoNv.PagerSize;
            var showYaonvArray = m_InfoList.Skip((pageSmg.StartPage - 1) * size).Take(size).ToArray();
            showYaonvArray.ApplyAllItem(p =>
                {
                    p.gameObject.SetActive(true);
                });
            ItemPagerManager_YaoNv.UpdateItems(showYaonvArray, "yaoNvList");
        }

        void RecoverSirenCallBack(int sirenID)
        {
            SendSynchronizationUpdateYaoNv();
        }
        void ReleaseSirenCallBack(int sirenID)
        {
            SendSynchronizationUpdateYaoNv();
        }

        private void CreateSiren(int sirenID, GameObject sirenPrefab)
        {
            var posInfo = m_SirenPosInfoList.SingleOrDefault(p => p.sirenID == sirenID);
            if (posInfo == null)
                return;
            GameObject siren = (GameObject)Instantiate(sirenPrefab);
            siren.transform.position = posInfo.sirenPos;
            siren.gameObject.SetActive(false);
            m_SirenDict.Add(sirenID, siren);
        }
        //房主同步妖女显示
        private void SendSynchronizationUpdateYaoNv()
        {
            uint[] array = new uint[5] { 0, 0, 0, 0, 0 };
            for (int i = 0; i < array.Length; i++)
            {
                if (i < m_InfoList.Count)
                {
                    if (m_InfoList[i].IsRelease)
                    {
                        array[i] = (uint)m_InfoList[i].SirenID;
                    }
                }                
            }
            NetServiceManager.Instance.EctypeService.SendYaoNvUpdate(array);
        }

        /// <summary>
        /// 更新妖女展示
        /// </summary>
        /// <param name="sirenIDArray">妖女id组</param>
        public void UpdateSirenModel(uint[] sirenIDArray)
        {           
            m_SirenDict.ApplyAllItem(p =>
                {
                    if (sirenIDArray.Any(k => k == p.Key))
                    {
                        p.Value.SetActive(true);                        

                        //
                        var infoItem = m_InfoList.SingleOrDefault(m => m.SirenID == p.Key);
                        if (infoItem != null)
                        {
                            infoItem.UpdateState(true);
                        }
                    }
                    else
                    {
                        p.Value.SetActive(false);

                        //
                        var infoItem = m_InfoList.SingleOrDefault(m => m.SirenID == p.Key);
                        if (infoItem != null)
                        {
                            infoItem.UpdateState(false);
                        }                        
                    }
                });

			int num = 0;
			for(int i=0;i<m_InfoList.Count;i++)
			{
				if(m_InfoList[i].IsRelease)
				{
					num++;
				}
			}

            var sirenData = PlayerRoomDataManager.Instance.GetPlayerSirenList();
            int sirenNum = sirenData.Count;
            Label_YaoNvNum.text = num.ToString() + "/" + sirenNum.ToString();
        }
    }
}