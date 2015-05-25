using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace UI.MainUI
{
    public class JewelBeset_Hole : MonoBehaviour
    {
        private JewelBset_ContainerItem BsetItem;
        public SingleButtonCallBack buttonCallBack;
        private ItemFielInfo fielInfo;
        private HoleIndex _holeIndex;
        public GameObject BesetItem_prafab;
        public Transform Item_point;

        void Awake()
        {
            BsetItem = CreatObjectToNGUI.InstantiateObj(BesetItem_prafab, Item_point).GetComponent<JewelBset_ContainerItem>();
            BsetItem.CanCancel=true;
            buttonCallBack.SetCallBackFuntion(obj => {
                SoundManager.Instance.PlaySoundEffect("Sound_Button_Stone_Click");
                if (fielInfo != null)
                {
                    EquiptSlotType place=(EquiptSlotType)int.Parse((fielInfo.LocalItemData as EquipmentData)._vectEquipLoc);
                    List<JewelInfo> jewelInfo=PlayerDataManager.Instance.GetJewelInfo(place);
                    if (_holeIndex == HoleIndex.FirstHole)
                    {
                        if (jewelInfo[0].JewelID != 0)
                        {
                            if(ContainerInfomanager.Instance.PackIsFull())
                            {
                                MessageBox.Instance.ShowTips(3,LanguageTextManager.GetString("IDS_I9_20"),1);
                            }
                            else
                            {
                              NetServiceManager.Instance.EquipStrengthenService.SendRequestGoodsOperateRemoveCommand(fielInfo.sSyncContainerGoods_SC.uidGoods, 1);
                            }
                        
                            //JewelBesetManager.GetInstance().ShowRemoveJewel1Effect();
                            //JewelBesetManager.GetInstance().RemoveQueue.Enqueue(1);
                        } else
                        {
                        
                            //Debug.Log(place);
                            JewelBesetManager.GetInstance().UpdateChoseJewelList_Beset(fielInfo, 1);
                            JewelBesetManager.GetInstance().ChangeSubUistate(JewelState.JewelChose_Beset);
                           
                        }
                    
                    } else
                    {
                        if (jewelInfo[1].JewelID!= 0)
                        {
                            if(ContainerInfomanager.Instance.PackIsFull())
                            {
                                MessageBox.Instance.ShowTips(3,LanguageTextManager.GetString("IDS_I9_20"),1);
                            }
                            else
                            {
                                NetServiceManager.Instance.EquipStrengthenService.SendRequestGoodsOperateRemoveCommand(fielInfo.sSyncContainerGoods_SC.uidGoods, 2);
                            }

                            //JewelBesetManager.GetInstance().ShowRemoveJewel2Effect();
                            //JewelBesetManager.GetInstance().RemoveQueue.Enqueue(2);
                        } else
                        {
                        
                            JewelBesetManager.GetInstance().UpdateChoseJewelList_Beset(fielInfo, 2);
                            JewelBesetManager.GetInstance().ChangeSubUistate(JewelState.JewelChose_Beset);
                          
                        }
                       
                    }
                    JewelBesetManager.GetInstance().DisableAllHoleButton();
                }
            });
            buttonCallBack.SetPressCallBack(OnButtonPress);
        }

        public void SetButtonEnable()
        {
            buttonCallBack.SetMyButtonActive(true);
            buttonCallBack.SetButtonBackground(1);
        }

        public void SetButtonDisable()
        {
            buttonCallBack.SetMyButtonActive(false);
            buttonCallBack.SetButtonBackground(3);
        }

        void OnButtonPress(bool isPressed)
        {

            if (fielInfo.GetIfBesetJewel((int)_holeIndex))
            {
                buttonCallBack.spriteSwithList.ApplyAllItem(p => p.ChangeSprite(isPressed ? 4 : 3));
              

            } else
            {
                buttonCallBack.spriteSwithList.ApplyAllItem(p => p.ChangeSprite(isPressed ? 2 : 1));
            }
            buttonCallBack.BackgroundSwithList.ApplyAllItem(P => P.ChangeSprite(isPressed ? 2 : 1));
        }

        public void Init(ItemFielInfo selectedEquipItem, HoleIndex holeIndex)
        {
    
            _holeIndex = holeIndex;
            this.fielInfo = selectedEquipItem;
            ItemFielInfo JewelitemfileInfo;
            if (fielInfo != null)
            {
                BsetItem.OnItemClick=OnItemClick;
                JewelBesetManager.GetInstance().EnableAllHoleButton();
                 List<JewelInfo> jewelInfo=PlayerDataManager.Instance.GetJewelInfo((EquiptSlotType)fielInfo.sSyncContainerGoods_SC.nPlace);
                if (holeIndex == HoleIndex.FirstHole)
                {

                    if (fielInfo != null &&jewelInfo[0].JewelID!=0)
                    {
                   
                        JewelitemfileInfo = new ItemFielInfo(jewelInfo[0].JewelID);
                        BsetItem.InitItemData(JewelitemfileInfo);
                        buttonCallBack.spriteSwithList.ApplyAllItem(p => p.ChangeSprite(3));
                    } else
                    {
                        BsetItem.InitItemData(null);
                        buttonCallBack.spriteSwithList.ApplyAllItem(p => p.ChangeSprite(1));
                    }
                } else
                {
                    if (fielInfo != null && jewelInfo[1].JewelID!=0)
                    {
                  
                        JewelitemfileInfo = new ItemFielInfo(jewelInfo[1].JewelID);
                        BsetItem.InitItemData(JewelitemfileInfo);
                        buttonCallBack.spriteSwithList.ApplyAllItem(p => p.ChangeSprite(3));
                    } else
                    {
                  
                        BsetItem.InitItemData(null);
                        buttonCallBack.spriteSwithList.ApplyAllItem(p => p.ChangeSprite(1));
                    }
                }
            } else
            {
                BsetItem.InitItemData(null);
                SetButtonDisable();
            }
        }

        void OnItemClick(JewelBset_ContainerItem selectedEquipItem)
        {
            if(selectedEquipItem!=null&&selectedEquipItem.ItemFielInfo!=null)
            {
               
                    JewelBesetManager.GetInstance().InitBeset_Attribute(selectedEquipItem.ItemFielInfo,selectedEquipItem);
                
                selectedEquipItem.OnGetFocus(false);
            }
        }
        public void Active()
        {
            buttonCallBack.SetMyButtonActive(true);
        }

        public void DeActive()
        {
            buttonCallBack.SetMyButtonActive(false);
        }
    
    }

    public enum HoleIndex
    {
        FirstHole=1,
        SecondHole,
    }
}
