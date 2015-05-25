using UnityEngine;
using System.Collections;
using System;
using System.Text;
using UI;

namespace UI.Ranking
{
    [RequireComponent(typeof(BoxCollider),typeof(UIEventListener))]
    public class SirenRankingItem : MonoBehaviour {
        public Action<SirenRankingItem> OnClickCallBack;
        public GameObject RankingUint_prefab;
        public Transform RankingUnitPoint;
        private RankingUnit SC_RankingUnit;

        public GameObject SirenListUnit_prefab;
        public Transform  SirenListUnitPoint;
        private  SirenListUnit SC_SirenListUnit;

        public GameObject PlayerHeadUnit_prefab;
        public Transform  PlayerHeadUnitPoint;
        private  PlayerHeadUnit SC_PlayerHeadUnit;
        
        public GameObject ForceUnit_prefab;
        public Transform  ForceUnitPoint;
        private ForceUnit SC_ForceUnit;
        public SingleButtonCallBack LookdetailBtn;
        private uint otherid;
        void Awake()
        {
            GetComponent<UIEventListener>().onClick=OnItemClick;
            SC_RankingUnit=CreatObjectToNGUI.InstantiateObj(RankingUint_prefab,RankingUnitPoint).GetComponent<RankingUnit>();
            SC_PlayerHeadUnit=CreatObjectToNGUI.InstantiateObj(PlayerHeadUnit_prefab,PlayerHeadUnitPoint).GetComponent<PlayerHeadUnit>();
            SC_SirenListUnit=CreatObjectToNGUI.InstantiateObj(SirenListUnit_prefab,SirenListUnitPoint).GetComponent<SirenListUnit>();
            SC_ForceUnit=CreatObjectToNGUI.InstantiateObj(ForceUnit_prefab,ForceUnitPoint).GetComponent<ForceUnit>();
            LookdetailBtn.SetCallBackFuntion(DetailBtnClick);
        }
        
        void DetailBtnClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Ranking_Detail");
            PlayerRankingPanelManger.GetInstance().ShowDetailePanel(RankingType.SirenRanking,otherid);
        }
        void OnItemClick(GameObject obj)
        {
            if(OnClickCallBack!=null)
            {
                OnClickCallBack(this);
            }
        }
        
        public  void BeSelected()
        {
            OnClickCallBack(this);
        }
        public  void OnGetFocus() 
        {
            //SelectSpring.gameObject.SetActive(true);
        }
        
        public  void OnLoseFocus() 
        {
            //SelectSpring.gameObject.SetActive(false);   
        }
        public void  InitItemData(RankingYaoNvFightData data)
        {
            otherid=data.dwActorID;
            SC_RankingUnit.InitData(data.wRankingIndex);
            SC_SirenListUnit.InitData(data);
            SC_PlayerHeadUnit.InitData(data.byKind,Encoding.UTF8.GetString(data.szName),(int)data.byLevel,(int)data.byVipLevel,data.dwFashionID);
            SC_ForceUnit.InitData("JH_UI_Typeface_1338",data.dwYaoNvFight.ToString());
            
        }
    }
}
