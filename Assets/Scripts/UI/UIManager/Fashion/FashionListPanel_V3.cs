using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.MainUI
{

    public class FashionListPanel_V3 : BaseTweenShowPanel
    {

//        public SingleButtonCallBack NextPageBtn;
//        public SingleButtonCallBack LastPageBtn;
//        public UILabel PageLabel;
		public GameObject SingleFashionBtnPrefab;
		public Transform Grid;

        public FashionPanel_V3 MyParent { get; private set; }
		public List<SingleFashionBtn> MyFashionButtonList{ get; private set; }
        private List<ItemData> CurrentFashionDataList;


        public void InitPanel(List<ItemData> fashionList, FashionPanel_V3 MyParent)
        {
            this.MyParent = MyParent;
            this.CurrentFashionDataList = fashionList;
			Grid.ClearChild();
			MyFashionButtonList = new List<SingleFashionBtn>();
			for(int i = 0;i<fashionList.Count;i++)
			{
				SingleFashionBtn newBtn = Grid.InstantiateNGUIObj(SingleFashionBtnPrefab).GetComponent<SingleFashionBtn>();
				newBtn.transform.localPosition = new Vector3(0,150-100*i,0);
				newBtn.Show(fashionList[i],this);
				MyFashionButtonList.Add(newBtn);
			}
        }

		public void OnMyBtnClick(ItemData selectData)
		{
			MyFashionButtonList.ApplyAllItem(C=>C.SetMySelectStatus(selectData));
			MyParent.SelectFashion(selectData);
		}

//        /// <summary>
//        /// 跳转到对应服装id的界面
//        /// </summary>
//        /// <param name="fashionID"></param>
//        public void TurningPage(ItemData fashionData)
//		{
//		}
    }
}