using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace UI.MainUI
{
    public class DailyTaskRewardPanel : MonoBehaviour
    {
        //public DailyTaskRewardConfigDataBase RewardConfigDataTable;
        public LocalButtonCallBack Button_Sure;
        public GameObject RewardItemPrefab;
        public UIDraggablePanel DraggablePanel;
        public UIGrid Grid;
        public TweenScale TweenScale;


        void Awake()
        {
            Button_Sure.SetCallBackFuntion(OnSureHandle, null);

            CreateChests();
        }

        void Start()
        {
            Grid.Reposition();
            DraggablePanel.ResetPosition();

            SetActive(false);
        }

        public void SetActive(bool isFalg)
        {
            if (isFalg)
            {
                DraggablePanel.ResetPosition();
                TweenScale.Play(true);
                transform.localPosition = Vector3.zero;
            }
            else
            {
                //TweenScale.Reset();
                TweenScale.Play(false);
                StartCoroutine(ClosePanel(TweenScale.duration));                
            }
        }

        IEnumerator ClosePanel(float time)
        {
            yield return new WaitForSeconds(time);
            transform.localPosition = new Vector3(0, 0, -800);
        }

        void OnSureHandle(object obj)
        {
            SetActive(false);
        }        

        //private void UpdateRewardInfo()
        //{
        //    if (m_chestProcess == 0)
        //        return;

        //    var playerData = PlayerManager.Instance.FindHeroDataModel();
        //    for (int i = 0; i < RewardConfigDataTable._dataTable.Length; i++)
        //    {
        //        if (playerData.PlayerValues.PLAYER_FIELD_ACTIVE_VALUE < RewardConfigDataTable._dataTable[i]._requirementActiveValue)
        //        {
        //            if (m_chestProcess < RewardConfigDataTable._dataTable[i]._boxSequence)
        //            {
        //                CreateChests(RewardConfigDataTable._dataTable[i]._boxSequence);
        //            }
        //            break;
        //        }
        //    }
        //}

        private void CreateChests()
        {         
			DailyTaskDataManager.Instance.GetDailyTaskRewardConfigArray().ApplyAllItem(p =>
                {
                    GameObject item = (GameObject)Instantiate(RewardItemPrefab);
                    item.transform.parent = Grid.transform;
                    item.transform.localPosition = Vector3.zero;
                    item.transform.localScale = Vector3.one;

                    var itemScript = item.GetComponent<DailyTaskRewardItem>();
                    itemScript.Init(p);
                });
            
        }      
    }
}