using UnityEngine;
using System.Collections;

/// <summary>
/// 技能升级按钮
/// </summary>
public class UpgradeSkillPanel : MonoBehaviour {
    public enum UpgradeSkillType { True, NeedMoney, NeedItem,NeedLevel,FullLevel }
    public GameObject Prefab_SkillItem;
    public SingleButtonCallBack UpgradeButton;
    public UILabel NeedLabel;
    public UILabel MightRightLabel;
    public UILabel MightLeftLabel;
    public UILabel ConsumeLabel;
    public UILabel DescribeLabel;
    public UILabel UpgradeBtnLabel;
    public UILabel InadequateLevelLabel;

    public UILabel NeedTitleLabel;
    public UILabel MightTitleLabel;
    public UILabel ConsumeTitleLabel;
    public UISprite TitleSprite;
    public UISprite ArrowSprite;
    //public UILabel TitleLabel;
    public GameObject UpgradeEffect;

    private GameObject m_skillItem;
    private SkillsItem m_skillItemData;
    private GameObject m_upgradeEffect;

    private int m_guideBtnID;

    void Awake()
    {
        UpgradeButton.SetCallBackFuntion(UpgradeSkillEvent);
        ResetUpgradePanel();

        //TODO GuideBtnManager.Instance.RegGuideButton(UpgradeButton.gameObject, UI.MainUI.UIType.SkillMain, SubType.SkillMainUpgrade, out m_guideBtnID);
    }

    void OnDestroy()
    {
        //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID);
    }

    public void ResetUpgradePanel()
    {
        //SetButtonState(false);
        SetButtonState(UpgradeSkillType.True);

        if (m_skillItem != null)
        {
            DestroyImmediate(m_skillItem);
        }

        if(PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION == 1)
            TitleSprite.spriteName = "JH_UI_Typeface_1100_01";
        else
            TitleSprite.spriteName = "JH_UI_Typeface_1100_02";

        UpgradeBtnLabel.text = LanguageTextManager.GetString("IDS_H2_31");
        NeedLabel.text = "";
        MightLeftLabel.text = "";
        MightRightLabel.text = "";
        ConsumeLabel.text = "";
        DescribeLabel.text = "";

        NeedTitleLabel.text = "";
        MightTitleLabel.text = "";
        ConsumeTitleLabel.text = "";
        TitleSprite.enabled = true;
        ArrowSprite.enabled = false;
    }

    private void UpgradeSkillEvent(object obj)
    {

		if(m_skillItemData == null)
			return;
		
        var selectedSkill = m_skillItemData.ItemFielInfo;

        //if (!IsSkillCanUpgrade(selectedSkill))
        //{
        //    return;
        //}
        if (selectedSkill != null)
        {
            switch (GetSkillCanUpgradeType(selectedSkill))
            {
                case UpgradeSkillType.FullLevel:
                    break;
                case UpgradeSkillType.NeedItem:
                    UI.MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H2_3"), 1);
                    break;
                case UpgradeSkillType.NeedLevel:
                    UI.MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H2_6"), 1);
                    break;
                case UpgradeSkillType.NeedMoney:
                    UI.MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H2_35"), 1);
                    break;
                case UpgradeSkillType.True:
                    SoundManager.Instance.PlaySoundEffect("Sound_Button_Pay");
                    NetServiceManager.Instance.EntityService.SendSkillUpgrade((byte)selectedSkill.localSkillData.m_skillId);
                    UI.LoadingUI.Instance.Show();
                    break;
                default:
                    break;
            }
        }
        
    }
	
	
    //private bool IsSkillCanUpgrade(SingleSkillInfo selectedSkillInfo)
    //{
    //    bool canUpgrade = selectedSkillInfo.CanUpgrade;

    //    int consumeMoneyValue = selectedSkillInfo.localSkillData.m_upgradeMoneyParams.Length < selectedSkillInfo.SkillLevel ? selectedSkillInfo.localSkillData.m_upgradeMoneyParams[selectedSkillInfo.SkillLevel - 1] : 0;//升级所需钱币

    //    int levelNeed = selectedSkillInfo.localSkillData.m_unlockLevel + selectedSkillInfo.SkillLevel * selectedSkillInfo.localSkillData.m_UpdateInterval;

    //    if (levelNeed > PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL||//等级判断
    //        consumeMoneyValue>PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HOLDMONEY)//消耗钱币判读
    //    {
    //        //UI.MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H2_6"), 1); //提示等级不足
    //        canUpgrade = false;
    //    }
    //    return canUpgrade;
    //}

    private UpgradeSkillType GetSkillCanUpgradeType(SingleSkillInfo selectedSkillInfo)
    {
        UpgradeSkillType upgradeSkillType = UpgradeSkillType.True;

        int UpgradeItemCount = selectedSkillInfo.localSkillData.m_upgradeItemCount;

        if (UpgradeItemCount > 0)
        {
            upgradeSkillType = UI.MainUI.ContainerInfomanager.Instance.GetItemNumber(selectedSkillInfo.localSkillData.m_upgradeItemId) >= UpgradeItemCount ? upgradeSkillType : UpgradeSkillType.NeedItem;
            //canUpgrade &= ContainerInfomanager.Instance.GetItemNumber(selectedSkillInfo.localSkillData.m_upgradeItemId) >= UpgradeItemCount;
        }
        //int UpgradeMoney = SkillValue.GetSkillValue(selectedSkillInfo.SkillLevel, selectedSkillInfo.localSkillData.m_upgradeMoneyParams);
        int UpgradeMoney = selectedSkillInfo.localSkillData.m_upgradeMoneyParams.Length >= selectedSkillInfo.SkillLevel ? selectedSkillInfo.localSkillData.m_upgradeMoneyParams[selectedSkillInfo.SkillLevel - 1] : 0;//升级所需钱币
        if (UpgradeMoney > 0)
        {
            upgradeSkillType = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HOLDMONEY >= UpgradeMoney ? upgradeSkillType : UpgradeSkillType.NeedMoney;
            //canUpgrade &= PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HOLDMONEY > UpgradeMoney;
        }

        int UpgradeLevel = selectedSkillInfo.localSkillData.m_unlockLevel + selectedSkillInfo.SkillLevel * selectedSkillInfo.localSkillData.m_UpdateInterval;
        upgradeSkillType = UpgradeLevel > PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL?UpgradeSkillType.NeedLevel:upgradeSkillType;

        upgradeSkillType = selectedSkillInfo.SkillLevel >= selectedSkillInfo.localSkillData.m_maxLv ? UpgradeSkillType.FullLevel : upgradeSkillType;

        return upgradeSkillType;
    }

    void UpgradeSucessEffect()
    {
        SoundManager.Instance.PlaySoundEffect("Sound_UIEff_SkillUpgrade");

        if (m_upgradeEffect != null)
        {
            DestroyImmediate(m_upgradeEffect);
        }

        m_upgradeEffect = Instantiate(UpgradeEffect) as GameObject;
        m_upgradeEffect.transform.parent = this.transform;
        m_upgradeEffect.transform.localScale = Vector3.one * 2.05f;
        m_upgradeEffect.transform.localPosition = new Vector3(0, 0, -25);
    }

    public void UpgradeSkillData(SingleSkillInfo selectedSkillInfo)
    {
        UpgradeSucessEffect();

        SetSpriteState(selectedSkillInfo);

        if (m_skillItem != null)
        {
            //m_skillItem.GetComponent<SkillsItem>().UpdateSkillItem(selectedSkillInfo);
        }

        SetButtonState(GetSkillCanUpgradeType(selectedSkillInfo));

        //if (!IsSkillCanUpgrade(selectedSkillInfo))
        //if (GetSkillCanUpgradeType(selectedSkillInfo) == UpgradeSkillType.FullLevel)
        //{
        //    UpgradeBtnLabel.text = LanguageTextManager.GetString("IDS_H2_75");
        //    SetButtonState(false);
        //}
    }

    private void SetSpriteState(SingleSkillInfo selectedSkillInfo)
    {
        //int consumeMoneyValue = SkillValue.GetSkillValue(selectedSkillInfo.SkillLevel, selectedSkillInfo.localSkillData.m_upgradeMoneyParams);
        int consumeMoneyValue = selectedSkillInfo.localSkillData.m_upgradeMoneyParams.Length>=selectedSkillInfo.SkillLevel?selectedSkillInfo.localSkillData.m_upgradeMoneyParams[selectedSkillInfo.SkillLevel-1]:0;
        int skillHurtValue = SkillValue.GetSkillValue(selectedSkillInfo.SkillLevel, selectedSkillInfo.localSkillData.m_mightParams);
        int nextSkillHurtValue = SkillValue.GetSkillValue(selectedSkillInfo.SkillLevel + 1, selectedSkillInfo.localSkillData.m_mightParams);
        //int needLv = selectedSkillInfo.SkillLevel + 1;
        int needLv = selectedSkillInfo.localSkillData.m_unlockLevel + selectedSkillInfo.SkillLevel * selectedSkillInfo.localSkillData.m_UpdateInterval ;
        if (consumeMoneyValue > 0)
        {
            if (PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HOLDMONEY < consumeMoneyValue)
                ConsumeLabel.color = Color.red;
            else
                ConsumeLabel.color = Color.white;
        }

        if (needLv > 0)
        {
            if (PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL < needLv)
            {
                NeedLabel.color = Color.red;
                ArrowSprite.enabled = false;
                MightLeftLabel.enabled = false;
                MightRightLabel.color = Color.white;
                MightRightLabel.text = skillHurtValue.ToString();
            }
            else
            {
                NeedLabel.color = Color.white;
                MightLeftLabel.enabled = true;
                ArrowSprite.enabled = true;
                MightRightLabel.color = Color.green;
                MightRightLabel.text = nextSkillHurtValue.ToString();
            }
        }


        NeedLabel.text = needLv.ToString();
        MightLeftLabel.text = skillHurtValue.ToString();
        ConsumeLabel.text = consumeMoneyValue.ToString();
    }

    private void SetButtonState(UpgradeSkillType upgradeSkillType)
    {
        switch (upgradeSkillType)
        {
            case UpgradeSkillType.True:
                InadequateLevelLabel.enabled = false;
                UpgradeButton.gameObject.SetActive(true);
                break;
            case UpgradeSkillType.NeedMoney:
                InadequateLevelLabel.enabled = false;
                UpgradeButton.gameObject.SetActive(true);
                break;
            case UpgradeSkillType.NeedLevel:
                InadequateLevelLabel.enabled = true;
                InadequateLevelLabel.SetText(LanguageTextManager.GetString("IDS_H1_515"));
                UpgradeButton.gameObject.SetActive(false);
                break;
            case UpgradeSkillType.NeedItem:
                InadequateLevelLabel.enabled = false;
                UpgradeButton.gameObject.SetActive(true);
                break;
            case UpgradeSkillType.FullLevel:
                InadequateLevelLabel.enabled = true;
                InadequateLevelLabel.SetText(LanguageTextManager.GetString("IDS_H1_516"));
                UpgradeButton.gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }

    //private void SetButtonState(bool isFlag)
    //{
        
    //    if (isFlag)
    //    {
    //        UpgradeButton.SetTextColor(Color.white);
    //    }
    //    else
    //    {
    //        UpgradeButton.SetTextColor(Color.gray);
    //    }

    //    this.UpgradeButton.GetComponent<BoxCollider>().enabled = isFlag;
    //}

	// Use this for initialization
	public void InitUpgradeInfo (SkillsItem skillItem) 
    {
        SetPanelItemActive(!skillItem.ItemFielInfo.Lock);
        m_skillItemData = skillItem;
        SetSpriteState(skillItem.ItemFielInfo);

        if (m_skillItem != null)
        {
            DestroyImmediate(m_skillItem);
        }

        m_skillItem = GameObject.Instantiate(Prefab_SkillItem) as GameObject;
        m_skillItem.transform.parent = this.transform;
        m_skillItem.transform.localPosition = new Vector3(0, 152, -2);
        m_skillItem.transform.localScale = Vector3.one;
        m_skillItem.GetComponent<SkillsItem>().InitItemData(skillItem.ItemFielInfo);
        m_skillItem.GetComponent<SkillsItem>().IsUpgradeState(false);

        DescribeLabel.text = LanguageTextManager.GetString(skillItem.ItemFielInfo.localSkillData.m_desc);

        NeedTitleLabel.text = LanguageTextManager.GetString("IDS_H1_407");
        MightTitleLabel.text = LanguageTextManager.GetString("IDS_H1_405");
        ConsumeTitleLabel.text = LanguageTextManager.GetString("IDS_H1_408");
        TitleSprite.enabled = false;

        SetButtonState(GetSkillCanUpgradeType(skillItem.ItemFielInfo));
        //if (IsSkillCanUpgrade(skillItem.ItemFielInfo))
        //if (GetSkillCanUpgradeType(skillItem.ItemFielInfo) != UpgradeSkillType.FullLevel)
        //{
        //    UpgradeBtnLabel.text = LanguageTextManager.GetString("IDS_H2_31");
        //    SetButtonState(true);
        //}
        //else
        //{
        //    UpgradeBtnLabel.text = LanguageTextManager.GetString("IDS_H2_75");
        //    SetButtonState(false);
        //}
	}

    /// <summary>
    /// 是否显示面板属性，如果技能未解锁就不显示
    /// </summary>
    /// <param name="flag"></param>
    public void SetPanelItemActive(bool flag)
    {
        if (m_skillItem != null)
        {
            DestroyImmediate(m_skillItem);
        }
        InadequateLevelLabel.gameObject.SetActive(flag);
        UpgradeButton.gameObject.SetActive(flag);
        NeedLabel.SetText("");
        MightLeftLabel.SetText("");
        MightRightLabel.SetText("");
        ConsumeLabel.SetText("");
        DescribeLabel.SetText("");
        TitleSprite.gameObject.SetActive(flag);
    }
	
}
