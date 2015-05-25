using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UI.MainUI;

public class EquipStrengthEffect : MonoBehaviour {
    const float FIGHTING_VALUE_CHANGE_DURATION = 1f;
    public GameObject FaildEff;
    public GameObject Glint;
    public GameObject Light;
    public GameObject Success;
    public GameObject Refine_Reset;      //成功洗练动画
    public GameObject CombatPowerUp;      //战力增加动画
    public UILabel PlayerFightingLbl;   //战力标签
    public float CombatPowerLifeTime=2;  //战力特效时间

    private ItemFielInfo m_strengthEquip;
    private float playerFightingBeforeStrength;  //强化前玩家战力
    private float m_successLength;   //强化成功特效长度
    private float m_refineLength;   //洗练成功特效长度
    private List<PlayDataStruct<Animation>> m_animations;

    void Awake()
    {
        m_successLength = InitAnimationData(Success.transform);
        m_refineLength = InitAnimationData(Success.transform);
        CombatPowerUp.SetActive(false);
    }
    public void Init(ItemFielInfo strengthEquip)
    {
        Success.SetActive(false);
        Light.SetActive(false);
        Glint.SetActive(false);
        FaildEff.SetActive(false);
        Refine_Reset.SetActive(false);
        m_strengthEquip = strengthEquip;
        playerFightingBeforeStrength = PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_FIGHTING;

    }  
    public void StrengthResultEff(byte result)
    {        
        if (result == 1)
        {
           StartCoroutine(StrengthSuccessHandler());
        }
        else
        {
            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_StrengthenFail");
            Success.SetActive(false);
            Light.SetActive(false);
            Glint.SetActive(false);
            FaildEff.SetActive(true);
            Refine_Reset.SetActive(false);
            CombatPowerUp.SetActive(false);

            UI.MainUI.ContainerInfomanager.Instance.GetEquiptItemList();
        }
    }

    private IEnumerator StrengthSuccessHandler()
    {
        SoundManager.Instance.PlaySoundEffect("Sound_UIEff_StrengthenSuccess");
        FaildEff.SetActive(false);
        Success.SetActive(true);
        Light.SetActive(true);
        Glint.SetActive(true);
        yield return new WaitForSeconds(m_successLength);  //等待成功动画播放完成后，播放洗练动画
        Refine_Reset.SetActive(true);

        yield return new WaitForSeconds(m_refineLength);  //等待洗练动画播放完成后，判断是否播放战力增加动画

        if (UI.MainUI.ContainerInfomanager.Instance.GetEquiptItemList().Exists(P => P == m_strengthEquip))
        {
            CombatPowerUp.SetActive(true);
            var currentFighting = PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_FIGHTING;
            TweenFloat.Begin(FIGHTING_VALUE_CHANGE_DURATION, playerFightingBeforeStrength, currentFighting, (value) =>
                {
                    PlayerFightingLbl.text = ((int)value).ToString();
                });
            StartCoroutine(DestroyCombatPowerUp(CombatPowerUp));
        }
    }
    private IEnumerator DestroyCombatPowerUp(GameObject combatPowerUp)
    {
        yield return new WaitForSeconds(CombatPowerLifeTime);
        combatPowerUp.SetActive(false);
    }
    private float InitAnimationData(Transform trans)
    {
        float animTime = 0;
        trans.transform.RecursiveGetComponent<Animation>("Animation", out m_animations);

        if (m_animations != null)
            m_animations.ApplyAllItem(ani => ani.AnimComponent.Stop());
       
        if (m_animations != null)
        {
            m_animations.ApplyAllItem(animation =>
            {
                animation.PlayTimeLength = animation.AnimComponent.clip.length;
                animTime = animTime >= animation.PlayTimeLength ? animTime : animation.PlayTimeLength;
            });
        }

        return animTime;
    }
}
