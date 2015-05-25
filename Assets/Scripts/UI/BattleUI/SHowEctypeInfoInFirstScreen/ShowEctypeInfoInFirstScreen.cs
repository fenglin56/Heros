using UnityEngine;
using System.Collections;
using System.Linq;

namespace UI.Battle
{
    public class ShowEctypeInfoInFirstScreen : MonoBehaviour
    {

        public GameObject IconPrefab;//封魔副本开始标题

        void Start()
        {
            CheckIsHardScenece();
        }

        void CheckIsHardScenece()
        {
            LoadSceneData loadSceneData = GameDataManager.Instance.PeekData(DataType.LoadingSceneData) as LoadSceneData;
            var LoadSceneInfo = (SMsgActionNewWorld_SC)loadSceneData.LoadSceneInfo;
            var HardSceneList = EctypeConfigManager.Instance.EctypeContainerConfigFile.ectypeContainerDataList.Where(P => P.MapType == 1).ToArray();
            TraceUtil.Log("CurrentMapID:" + LoadSceneInfo.dwMapId);
            if (HardSceneList.Length > 0)
            {
                TraceUtil.Log("HardSceneList.cout:"+HardSceneList.Length);
                var CurrentHardScenece = HardSceneList.SingleOrDefault(P => P.vectMapID.Split('+')[0] == LoadSceneInfo.dwMapId.ToString());
                if (CurrentHardScenece != null)
                {
                    TraceUtil.Log("ShowIcon");
                    StartCoroutine( ShowHardSceneceIcon());
                }
            }
        }

        IEnumerator ShowHardSceneceIcon()//显示封魔副本
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Voice_HardEctype");
            GameObject CreatIcon = CreatObjectToNGUI.InstantiateObj(IconPrefab, transform);
            yield return new WaitForSeconds(3);
            DestroyObj(CreatIcon);
        }


        void DestroyObj(GameObject obj)
        {
            Destroy((GameObject)obj);
        }


    }
}