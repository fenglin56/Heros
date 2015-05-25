using UnityEngine;
using System.Collections;
using System.Linq;

namespace UI.MainUI
{

    public class SingleRoleAtrribute : MonoBehaviour
    {
        public RoleAttributeType roleAttributeType = RoleAttributeType.MaxHP;

        public UILabel NameLabel;
        public UILabel NumberLabel;
        public UISprite IconSprite;

        public int EffectBasePropID { get; private set; }

        void Awake()
        {
            EffectBasePropID = GetEffectBasePropID(roleAttributeType);            
            EffectData effectData = ItemDataManager.Instance.EffectDatas._effects.First(P => P.BasePropView == EffectBasePropID);
            this.IconSprite.spriteName = effectData.EffectRes;
            this.NameLabel.SetText(LanguageTextManager.GetString(effectData.IDS));
        }

        public void ResetInfo(string showInfo)
        {

            this.NumberLabel.SetText(showInfo);

        }

        #region edit by lee
        private int GetEffectBasePropID(RoleAttributeType attributeType)
        {
            int EffectBasePropID = 0;
            switch (attributeType)
            {
                case RoleAttributeType.MaxHP:
                    EffectBasePropID = 10;
                    break;
                case RoleAttributeType.MaxMP:
                    EffectBasePropID = 20;
                    break;
                case RoleAttributeType.ATK:
                    EffectBasePropID = 40;
                    break;
                case RoleAttributeType.DEF:
                    EffectBasePropID = 50;
                    break;
                case RoleAttributeType.HIT:
                    EffectBasePropID = 30;
                    break;
                case RoleAttributeType.EVA:
                    EffectBasePropID = 60;
                    break;
                case RoleAttributeType.Crit:
                    EffectBasePropID = 100;
                    break;
                case RoleAttributeType.ResCrit:
                    EffectBasePropID = 110;
                    break;
                case RoleAttributeType.Force:
                    EffectBasePropID = 130;
                    break;
            }
            return EffectBasePropID;
        }

        public void ResetInfo(RoleAttributeType type, string showInfo)
        {
            int EffectBasePropID = GetEffectBasePropID(type);
            EffectData effectData = ItemDataManager.Instance.EffectDatas._effects.First(P => P.BasePropView == EffectBasePropID);
            this.IconSprite.spriteName = effectData.EffectRes;
            this.NumberLabel.SetText(showInfo);
        }

        public void ResetInfo(string effectRes, string name_ids, string showInfo)
        {
            this.IconSprite.spriteName = effectRes;
            this.NameLabel.SetText(LanguageTextManager.GetString(name_ids));
            this.NumberLabel.SetText(showInfo);
        }
        #endregion

    }

    public enum RoleAttributeType
    {
        MaxHP,//生命
        MaxMP,//真气
        ATK,//攻击
        DEF,//防御
        HIT,//命中
        EVA,//闪避
        Crit,//暴击
        ResCrit,//抗暴
        Force,//战力
    }

}