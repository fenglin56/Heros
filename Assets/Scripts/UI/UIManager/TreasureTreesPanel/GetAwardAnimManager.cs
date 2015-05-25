using UnityEngine;
using System.Collections;
using System.Linq;

namespace UI.MainUI
{

    public class GetAwardAnimManager : MonoBehaviour
    {

        public Transform TargetTransform;
        public GameObject SingleItemPreafab;


        public TreasureTreesPanelManager MyParent { get; private set; }

        public void Init(TreasureTreesPanelManager myParent)
        {
            MyParent = myParent;
        }

        public void PickUpFruit(byte fruitPosition, int itemId, int itemCount)
        {

            var fruitPointData = MyParent.TreasureTreesFruitPointList.First(P=>P.MyPositionID == fruitPosition);
            Vector3 creatPosition = fruitPointData.transform.parent.localPosition;
            int allNumber = itemCount;
            int popUpNumber = 0;
            if(itemCount > 5)
            {
                popUpNumber  = Random.Range(2, 5);
            }
            else
            {
                popUpNumber = 1;
            }

            for (int i = 0; i < popUpNumber; i++)
            {
                int AddNumber = 0;
                if (i == (popUpNumber - 1))
                {
                    AddNumber = allNumber / popUpNumber+allNumber%popUpNumber;
                }
                else
                {
                    AddNumber = allNumber / popUpNumber;
                }
                SingleGetAwardItem singleGetAwardItem = CreatObjectToNGUI.InstantiateObj(SingleItemPreafab, transform).GetComponent<SingleGetAwardItem>();
                singleGetAwardItem.Show(creatPosition,AddNumber,itemId,this);
            }
        }

    }
}