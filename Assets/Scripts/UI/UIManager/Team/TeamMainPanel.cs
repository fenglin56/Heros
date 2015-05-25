using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace UI.Team
{
    /// <summary>
    /// 组队主面板
    /// </summary>
    public class TeamMainPanel : MonoBehaviour
    {
        public UIGrid MyUIGrid; //队伍信息UI的父体    
        public TeamInfoItem ATeamInfoItem;  //队伍信息项       
        public LocalButtonCallBack ButtonRefreshWorldTeamInfo;  //刷新按钮
        
        private List<TeamInfoItem> TeamInfoItemList = new List<TeamInfoItem>(); //储存回收list表


        //\假设网络过来的信息:
        private int mItemNum = 5;        


        void Start()
        {
            ButtonRefreshWorldTeamInfo.SetCallBackFuntion(OnRefreshWorldTeamInfoClick);
        }

        public void ShowTeamMainPanel()
        {
            CreateTeamInfoItems();
            transform.localPosition = Vector3.zero;
        }
      

        public void CloseTeamMainPanel()
        {            
            transform.localPosition = new Vector3(0, 0, -800);
        }


        void OnRefreshWorldTeamInfoClick(object obj)
        {
            //NetServiceManager.Instance.TeamService.SendClickTeamButton(new STeamButton_CS { beTeam = 0, nCmd = 1, dwPlayerID = 1234 });
            ShowTeamMainPanel();
        }


        private void CreateTeamInfoItems()
        {
            //\read world team info
            mItemNum = 3;
            if (mItemNum > TeamInfoItemList.Count)
            {
                int addNum = mItemNum - TeamInfoItemList.Count;
                for (int i = 0; i < addNum; i++)
                {
                    GameObject obj = ((GameObject)Instantiate(ATeamInfoItem.gameObject));
                    TeamInfoItem item = obj.GetComponent<TeamInfoItem>();
                    item.InitInfo(i, MyUIGrid.transform);
                    TeamInfoItemList.Add(item);
                }
                //TeamInfoItemList.ApplyAllItem(p => p.UpdateInfo());
            }
            else
            {
                int num = 0;
                TeamInfoItemList.ApplyAllItem(p =>
                {
                    if (num < mItemNum)
                    {
                        //p.UpdateInfo();
                    }
                    else
                    {
                        p.gameObject.SetActive(false);
                    }
                    num++;
                });
            }
            //排序
            MyUIGrid.repositionNow = true;

        }


    }
}
