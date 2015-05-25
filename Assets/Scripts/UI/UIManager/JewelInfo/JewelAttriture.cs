using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace UI.MainUI
{
    public class JewelAttriture : MonoBehaviour
    {
        public UILabel Title;
        public UILabel SkillName;
        public UILabel SkillLevel;
        public GameObject AttributeContentProfab;
        public PassiveSkillDataBase  _PassiveSkillDataBase;
        public Transform ContentPoint;
        private AttributeContent AttributeContent;
        private PassiveSkillData passiveSkillData;
        private Jewel jewel;
        private int ItemId;
        private int skillId;
        private int skillLevel;

        void Awake()
        {

    
            AttributeContent = CreatObjectToNGUI.InstantiateObj(AttributeContentProfab, ContentPoint).GetComponent<AttributeContent>();
        }
        /// <summary>
        /// Init the specified jewel and SkillLevel.
        /// </summary>
        /// <param name="jewel">Jewel.</param>
        /// <param name="SkillLevel">Skill level.</param>
        public void Init(ItemFielInfo itemFielInfo, bool isSuit)
        {
                                   
            ItemId = itemFielInfo.LocalItemData._goodID;
            jewel = ItemDataManager.Instance.GetItemData(ItemId) as Jewel;

            if (isSuit)
            {
                Title.SetText("套装属性");
                skillId = jewel._activePassiveSkill.skillID;
                skillLevel = jewel._activePassiveSkill.skillLevel;
                                
            } else
            {
                Title.SetText("器魂属性");

                skillId = jewel.PassiveSkill;
                if(itemFielInfo.sSyncContainerGoods_SC.uidGoods!=0)
                {
                skillLevel = itemFielInfo.materiel.ESTORE_FIELD_LEVEL;
                }
                else
                {
                    EquiptSlotType type=(EquiptSlotType) JewelBesetManager.GetInstance().Sc_Container.SelectItemFileInfo.sSyncContainerGoods_SC.nPlace;
                    List<JewelInfo> jewelInfos= PlayerDataManager.Instance.GetJewelInfo(type);
                   foreach(var item in jewelInfos)
                    {
                        if(item.JewelID==jewel._goodID)
                        {
                            skillLevel=item.JewelLevel;
                        }
                    }
                }
                            
            }
            SkillLevel.SetText(string.Format(LanguageTextManager.GetString("IDS_I9_29"), skillLevel));
            if (skillLevel == 0)
                skillLevel++;
            passiveSkillData = _PassiveSkillDataBase._dataTable.First(c => (c.SkillID == skillId && c.SkillLevel == skillLevel));
            SkillName.SetText(LanguageTextManager.GetString(passiveSkillData.SkillName));
            AttributeContent.Init(passiveSkillData);

        }
        /// <summary>
        /// Init the specified jewel and SkillLevel.
        /// </summary>
        /// <param name="jewel">Jewel.</param>
        /// <param name="SkillLevel">Skill level.</param>
        public void Init(Jewel jewel, bool isSuit)
        {
          
            
            if (isSuit)
            {
                Title.SetText("套装属性");
                skillId = jewel._activePassiveSkill.skillID;
                skillLevel = jewel._activePassiveSkill.skillLevel;
                
            } else
            {
                Title.SetText("器魂属性");
                
                skillId = jewel.PassiveSkill;
                skillLevel = 1;
                
            }
            SkillLevel.SetText(string.Format(LanguageTextManager.GetString("IDS_I9_29"), skillLevel));
            if (skillLevel == 0)
                skillLevel++;
            passiveSkillData = _PassiveSkillDataBase._dataTable.First(c => (c.SkillID == skillId && c.SkillLevel == skillLevel));
            SkillName.SetText(LanguageTextManager.GetString(passiveSkillData.SkillName));
            AttributeContent.Init(passiveSkillData);
            
        }
    }
}
