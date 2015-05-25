using UnityEngine;
using System.Collections;

namespace UI.MainUI
{

    public class ContainerTipsManager_V2 : MonoBehaviour
    {

        public GameObject EquipmentContainerTipsPrefab;
        public GameObject MedicineConteinrTipsPrefab;
        public GameObject MaterialContainerTipsPrefab;

        private ContainerTips_Equipment equipmentTips;
        private ContainerTips_Medicine medicineTips;
        private ContainerTips_Materiel materialTips;

        public Transform TipsPos;

        public void Show(ItemFielInfo itemFielInfo)
        {
            //TraceUtil.Log("ShowTips");
            CloseTips();
            switch (itemFielInfo.LocalItemData._GoodsClass)
            {
                case 1:
                    if (equipmentTips == null) { equipmentTips = CreatObjectToNGUI.InstantiateObj(EquipmentContainerTipsPrefab, TipsPos).GetComponent<ContainerTips_Equipment>(); }
                    equipmentTips.Show(itemFielInfo);
                    break;
                case 2:
                    if (medicineTips == null) { medicineTips = CreatObjectToNGUI.InstantiateObj(MedicineConteinrTipsPrefab, TipsPos).GetComponent<ContainerTips_Medicine>(); }
                    medicineTips.Show(itemFielInfo);
                    break;
                case 3:
                    if (materialTips == null) { materialTips = CreatObjectToNGUI.InstantiateObj(MaterialContainerTipsPrefab, TipsPos).GetComponent<ContainerTips_Materiel>(); }
                    materialTips.Show(itemFielInfo);
                    break;
            }
        }

        public void CloseTips()
        {
            if (equipmentTips != null)
            {
                equipmentTips.Close();
            }
            if (medicineTips != null)
            {
                medicineTips.Close();
            }
            if (materialTips != null)
            {
                materialTips.Close();
            }
        }

    }
}