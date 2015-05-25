using UnityEngine;
using System.Collections;


namespace UI.Battle
{

    public class SingleTrialsSettlementComponent : MonoBehaviour
    {

        public Transform[] ItemCreatIconPoint;
        public UILabel[] ItemInfoLabel;
        public Animation AnimationComponent;
        public int ComponentID = 0;

        public void Init(int componentID)
        {
            this.ComponentID = componentID;
        }

        public void Show(SMSGEctypeTrialsTotalResult_SC sMSGEctypeTrialsTotalResult_SC)
        {
            int showComponentID = ComponentID * 2;
            for (int i = 0; i < 2; i++)
            {
                if ((showComponentID + i) < sMSGEctypeTrialsTotalResult_SC.dwEquipReward.Count)
                {
                    SEquipRewardInfo sEquipRewardInfo = sMSGEctypeTrialsTotalResult_SC.dwEquipReward[showComponentID + i];
                    ItemData creatItemData = ItemDataManager.Instance.GetItemData((int)sEquipRewardInfo.dwEquipId);
                    ItemCreatIconPoint[i].ClearChild();
                    CreatObjectToNGUI.InstantiateObj(creatItemData._picPrefab, ItemCreatIconPoint[i]);
                    ItemInfoLabel[i].SetText(creatItemData._GoodsClass == 3 ? sEquipRewardInfo.dwEquipNum.ToString() : LanguageTextManager.GetString(creatItemData._szGoodsName));
                }
                else
                {
                    ItemCreatIconPoint[i].ClearChild();
                    ItemInfoLabel[i].SetText("");
                }
            }
            AnimationComponent.Stop();
            AnimationComponent.Play();
        }
    }
}
