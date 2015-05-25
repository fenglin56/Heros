using UnityEngine;
using System.Collections;
using System.Linq;

namespace UI.MainUI
{

    public class ShopSingleGoodsBox_V2 : MonoBehaviour
    {
        public UISprite ScaleBackground;
        public SpriteSwith Background;
        public UILabel NameLabel;
        public UILabel OverlayLabel;//叠加数量
        public SingleButtonCallBack AllowLvLabel;

        public SingleButtonCallBack Effect01;
        public SingleButtonCallBack Effect02;
        public Transform CreatItemIconPoint;

        public SMsgTradeOpenShopGoodsInfo_SC sMsgTradeOpenShopGoodsInfo_SC { get; private set; }
        public ItemData LocalGoodsData { get; private set; }

        private ShopGoodsListPanel_V2 MyParent;

        private Vector3 Background_Scale;

        //private int m_guideBtnID = 0;

        void Awake()
        {
            if (ScaleBackground != null)
            {
                Background_Scale = ScaleBackground.transform.localScale;
            }
        }

        public void Init(SMsgTradeOpenShopGoodsInfo_SC sMsgTradeOpenShopGoodsInfo_SC, ShopGoodsListPanel_V2 MyParent)
        {

            ////TODO GuideBtnManager.Instance.RegGuideButton(this.gameObject, UIType.ShopInfo, SubType.ShopInfoGoodItem, out m_guideBtnID);
            OverlayLabel.SetText("");
            this.MyParent = MyParent;
            this.sMsgTradeOpenShopGoodsInfo_SC = sMsgTradeOpenShopGoodsInfo_SC;
            Effect01.gameObject.SetActive(false);
            Effect02.gameObject.SetActive(false);
            this.LocalGoodsData = ItemDataManager.Instance.GetItemData((int)sMsgTradeOpenShopGoodsInfo_SC.dGoodsID);
            NameLabel.text = GetGoodsName(LocalGoodsData);
            AllowLvLabel.SetButtonText(LocalGoodsData._AllowLevel.ToString());
            CreatItemIconPoint.ClearChild();
            CreatObjectToNGUI.InstantiateObj(LocalGoodsData._picPrefab, CreatItemIconPoint);
            //var AddString = LocalGoodsData('+');
            //var EfferData = ItemDataManager.Instance.EffectDatas._effects.FirstOrDefault(P => P.m_SzName == AddString[0]);
            switch ((BoxItemType)LocalGoodsData._GoodsClass)
            {
                case BoxItemType.Equipment:
                    var AddString = (LocalGoodsData as EquipmentData)._vectEffects.Split('|');
                    for (int i = 0; i < AddString.Length; i++)
                    {
                        var efferData = ItemDataManager.Instance.EffectDatas._effects.FirstOrDefault(P => P.m_SzName == AddString[i].Split('+')[0]);
                        if (i == 0)
                        {
                            Effect01.gameObject.SetActive(true);
                            Effect01.SetButtonText(HeroAttributeScale.GetScaleAttribute(efferData, int.Parse(AddString[i].Split('+')[1])).ToString());
                            Effect01.SetButtonBackground(efferData.EffectRes);
                        }
                        else if (i == 1)
                        {
                            Effect02.gameObject.SetActive(true);
                            Effect02.SetButtonText(HeroAttributeScale.GetScaleAttribute(efferData,int.Parse(AddString[i].Split('+')[1])).ToString());
                            Effect02.SetButtonBackground(efferData.EffectRes);
                        }
                    }
                    break;
                case BoxItemType.Materien:
                    OverlayLabel.SetText(string.Format("x{0}", LocalGoodsData._PileQty));
                    break;
                case BoxItemType.Medicament:
                    OverlayLabel.SetText(string.Format("x{0}", LocalGoodsData._PileQty));
                    break;
            }
            FlashBackground();
        }

        void OnDestroy()
        {
            ////TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID);
        }

        public void FlashBackground()
        {
            if (ScaleBackground != null)
            {
                TweenScale.Begin(ScaleBackground.gameObject, 0.1f, Background_Scale, Background_Scale + new Vector3(0, 10, 0), ScaleComplete);
            }
        }

        void ScaleComplete(object obj)
        {
            if (ScaleBackground != null)
            {
                TweenScale.Begin(ScaleBackground.gameObject, 0.1f, Background_Scale);
            }
        }

        protected string GetGoodsName(ItemData GoodsData)
        {
            TextColor NameTextColor = TextColor.white;
            switch (GoodsData._ColorLevel)//物品品质颜色
            {
                case 0:
                    NameTextColor = TextColor.EquipmentGreen;
                    break;
                case 1:
                    NameTextColor = TextColor.EquipmentBlue;
                    break;
                case 2:
                    NameTextColor = TextColor.EquipmentMagenta;
                    break;
                case 3:
                    NameTextColor = TextColor.EquipmentYellow;
                    break;
                default:
                    break;
            }
            string str = NGUIColor.SetTxtColor(LanguageTextManager.GetString(GoodsData._szGoodsName), NameTextColor);
            return str;
        }

        public void OnClick()
        {
            if (gameObject.active)
            {
                MyParent.OnShopGoodsSelect(this);
            }
        }

        public void SetSelectStatus(bool flag)
        {
            Background.ChangeSprite(flag?2:1);
        }

    }
}