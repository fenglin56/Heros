using UnityEngine;
using System.Collections;


public class SkillUIBoxSlot : DragComponentSlot
{
    public int Index;
    public GameObject AssemblyEffect;
    private int m_skillID;
    private SkillsItem m_curEquipSkill;
    private bool m_isNewEquipSkill = true;
    private GameObject m_assemblyEffect;

    private int m_guideBtnID;

   /// <summary>
    /// 检测被拖拽的物体能否移动到该槽位,判断条件由继承的子类自定
    /// </summary>
    /// <param name="checkChild">拖拽的物体</param>
    /// <returns></returns>
    public override bool CheckIsPair(DragComponent dragChild)
    {
        //TraceUtil.Log("#############+CheckIsPair");
        return true;
    }
    private Vector3 Background_Scale;

    public override void Start()
    {
        base.Start();
        //TODO GuideBtnManager.Instance.RegGuideButton(gameObject, UI.MainUI.UIType.SkillMain, SubType.SkillMainAssembly, out m_guideBtnID);
        Background_Scale = transform.localScale;
    }
    /// <summary>
    ///手指悬放到这里的时候
    /// </summary>
    public override void OnDragComponentHover()
    {
        //TraceUtil.Log("#############+OnDragComponentHover");
        OnTouchSlot();
    }

    public void OnTouchSlot()
    {
        TweenScale.Begin(gameObject, 0.1f, Background_Scale, Background_Scale + new Vector3(0, 10, 0), ScaleComplete);
    }

    void ScaleComplete(object obj)
    {
        TweenScale.Begin(gameObject, 0.1f, Background_Scale);
    }

    /// <summary>
    /// 某个物件要放到这里了
    /// </summary>
    /// <param name="enterComponent"></param>
    public override void MoveToHere(DragComponent enterComponent)
    {
        var itemData = enterComponent.gameObject.GetComponent<SkillsItem>();
        //NewbieGuideManager_V2.Instance.IsDragGuide(m_guideBtnID, itemData.GetGuideID);
        m_isNewEquipSkill = true;

        CloneSkillItem(itemData.ItemFielInfo, enterComponent.gameObject);

        enterComponent.transform.localPosition = new Vector3(0, 0, -2);
        enterComponent.GetComponent<TweenPosition>().to = new Vector3(0, 0, -2);
        AssemblySucessEffect();
    }

    private void CloneSkillItem(SingleSkillInfo skillInfo, GameObject item)
    {
        var transPos = this.transform.localPosition;
        transPos.z = -1;

        var cloneGo = Instantiate(item.gameObject) as GameObject;
        cloneGo.transform.parent = this.transform.parent;
        cloneGo.transform.localScale = Vector3.one;
        cloneGo.GetComponent<SkillUIDragBox>().enabled = false;

        SkillsItem cloneItem = cloneGo.GetComponent<SkillsItem>();
        cloneItem.InitGuideID(2);
        cloneItem.OnLoseFocus();
        
        TweenPosition tweenComponent = cloneItem.GetComponent<TweenPosition>();
        if (tweenComponent != null)
        {
            tweenComponent.from = transPos;
            tweenComponent.to = transPos;
        }
        else
        {
            cloneGo.transform.localPosition = transPos;
        }

        cloneItem.IsUpgradeState(false);
        cloneItem.InitItemData(skillInfo);

        EquipSkill(cloneItem);
    }

    //public void UpgradeSkill(SingleSkillInfo singleSkill)
    //{
    //    m_curEquipSkill.InitItemData(singleSkill);
    //}

    
    public void InitEquipSkill(SingleSkillInfo skillInfo, GameObject item)
    {
        m_isNewEquipSkill = false;
        CloneSkillItem(skillInfo, item);
    }

    void EquipSkill(SkillsItem skillItem)
    {
        int skillID = skillItem.ItemFielInfo.localSkillData.m_skillId;
        SingleSkillInfo RemovesingleSkillInfo = null;
        SingleSkillInfo AddsingleSkillInfo = null;

        if (m_curEquipSkill != null)
        {
            RemovesingleSkillInfo = m_curEquipSkill.ItemFielInfo;
            Destroy(m_curEquipSkill.gameObject);
            //AssemblySkillPanel.Instance.m_skillListForID.Remove(m_curEquipSkill.ItemFielInfo.localSkillData.m_skillId);
        }

        m_curEquipSkill = skillItem;

//        if (AssemblySkillPanel.Instance.m_skillListForID.ContainsKey(skillID))
//        {
//            RemovesingleSkillInfo = AssemblySkillPanel.Instance.m_skillListForID[skillID].ItemFielInfo;
//            DestroyImmediate(AssemblySkillPanel.Instance.m_skillListForID[skillID].gameObject);
//            AssemblySkillPanel.Instance.m_skillListForID.Remove(skillID);
//        }

        //AssemblySkillPanel.Instance.m_skillListForID.Add(skillID, skillItem);
        AddsingleSkillInfo = skillItem.ItemFielInfo;

        if (AddsingleSkillInfo == null) { return; }
        
        if(m_isNewEquipSkill)
            SetChuangeButtonToSever(RemovesingleSkillInfo, AddsingleSkillInfo);
        
    }

    void OnDestroy()
    {
        //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID);
        if (m_curEquipSkill != null)
        {
            Destroy(m_curEquipSkill.gameObject);
            m_curEquipSkill = null;
        }
    }

    void AssemblySucessEffect()
    {
        SoundManager.Instance.PlaySoundEffect("Sound_UIEff_SkillEquip");

        if (m_assemblyEffect != null)
        {
            DestroyImmediate(m_assemblyEffect);
        }
        var localPos = this.transform.localPosition;
        localPos.z = -5;

        m_assemblyEffect = Instantiate(AssemblyEffect) as GameObject;
        m_assemblyEffect.transform.parent = this.transform.parent;
        m_assemblyEffect.transform.localScale = Vector3.one * 2.05f;
        m_assemblyEffect.transform.localPosition = localPos;
    }



    void SetChuangeButtonToSever(SingleSkillInfo RemovesingleSkillInfo, SingleSkillInfo AddsingleSkillInfo)
    {
        SkillEquipEntity skillEquipEntity = new SkillEquipEntity();
        skillEquipEntity.Skills = new System.Collections.Generic.Dictionary<byte, ushort>();
        SingleSkillInfo[] equipSkillsList = SkillsPanelManager.m_playerSkillList.EquipSkillsList;
        if (RemovesingleSkillInfo != null)
        {
            for (int i = 0; i < equipSkillsList.Length; i++)
            {
                if (RemovesingleSkillInfo == equipSkillsList[i])
                {
                    equipSkillsList[i] = null;
                }
            }
        }
        if (AddsingleSkillInfo != null)
        {
            equipSkillsList[Index] = AddsingleSkillInfo;
        }

        for (int i = 0; i < equipSkillsList.Length; i++)
        {
            if (equipSkillsList[i] != null)
            {
                skillEquipEntity.Skills.Add((byte)i, (byte)equipSkillsList[i].localSkillData.m_skillId);
            }
            else
            {
                skillEquipEntity.Skills.Add((byte)i, 0);
            }
        }

        NetServiceManager.Instance.EntityService.SendSkillEquip(skillEquipEntity);
        //SoundManager.Instance.PlaySoundEffect("sound0048");
    }
}
