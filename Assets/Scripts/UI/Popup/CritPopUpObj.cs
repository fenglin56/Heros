using UnityEngine;
using System.Collections;

public class CritPopUpObj : MonoBehaviour {


    public GameObject[] PopUpObjPrefab;

    //public GameObject OneNumberPopUpObj;
    //public GameObject TowNumberPopUpObj;
    //public GameObject ThreeNumberPopUpObj;
    //public GameObject FourNumberPopUPObj;
    //public GameObject FiveNumberPopUpObj;

    //public UILabel OneNumberLabel;
    //public UILabel[] TowNumberLabel;
    //public UILabel[] ThreeNumberLabel;
    //public UILabel[] FourNumberLabel;
    //public UILabel[] FiveNumberLabel;

    //public UIFont NorMalFont;
    //public UIFont CritFont;
    //Vector3 NorMalFontScale = new Vector3(1,1,1);
    //Vector3 CritFontScale = new Vector3(2,2,2);

	public void Show(FightEffectType fightEffectType, string showNumber, bool isHero)
    {
        int prefabIndex = Random.Range(0, PopUpObjPrefab.Length);
        UILabel textLabel = UI.CreatObjectToNGUI.InstantiateObj(PopUpObjPrefab[prefabIndex],transform).GetComponentInChildren<UILabel>();
        textLabel.text = showNumber;
		if(isHero)
		{
			textLabel.color = new Color(1f,0.184f,0.184f);
		}
		else
		{
			textLabel.color = Color.white;
		}
        StartCoroutine(DestroyObjForTime(1));
    }

    IEnumerator DestroyObjForTime(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        transform.ClearChild();
        GameObjectPool.Instance.Release(gameObject);
    }

    //public void Show(FightEffectType fightEffectType, string showNumber)
    //{
    //    int scaleNumber = HeroAttributeScale.GetScaleAttribute(HeroAttributeScaleType.Display_CurHp, int.Parse(showNumber));
    //    scaleNumber = scaleNumber < 1 ? 1 : scaleNumber;
    //    showNumber = scaleNumber.ToString();
    //    //TraceUtil.Log("PopUPHP:" + showNumber+","+fightEffectType);
    //    //string showNumber = showString.Split('-')[1];
    //    OneNumberPopUpObj.SetActive(false);
    //    TowNumberPopUpObj.SetActive(false);
    //    ThreeNumberPopUpObj.SetActive(false);
    //    FourNumberPopUPObj.SetActive(false);
    //    FiveNumberPopUpObj.SetActive(false);
    //    switch (showNumber.Length)
    //    {
    //        case 1:
    //            OneNumberPopUpObj.SetActive(true);
    //            OneNumberPopUpObj.transform.localScale = fightEffectType == FightEffectType.BATTLE_EFFECT_CRIT ? CritFontScale : NorMalFontScale;
    //            SetLabelFont(fightEffectType,OneNumberLabel);
    //            OneNumberLabel.SetText(showNumber);
    //            break;
    //        case 2:
    //            TowNumberPopUpObj.SetActive(true);
    //            TowNumberPopUpObj.transform.localScale = fightEffectType == FightEffectType.BATTLE_EFFECT_CRIT ? CritFontScale : NorMalFontScale;
    //            SetLabelFont(fightEffectType,TowNumberLabel);
    //            for (int i = 0; i < 2; i++)
    //            {
    //                TowNumberLabel[i].SetText(showNumber.Substring(i,1));
    //            }
    //                break;
    //        case 3:
    //            ThreeNumberPopUpObj.SetActive(true);
    //            ThreeNumberPopUpObj.transform.localScale = fightEffectType == FightEffectType.BATTLE_EFFECT_CRIT ? CritFontScale : NorMalFontScale;
    //            SetLabelFont(fightEffectType,ThreeNumberLabel);
    //            for (int i = 0; i <3; i++)
    //            {
    //                ThreeNumberLabel[i].SetText(showNumber.Substring(i, 1));
    //            }
    //            break;
    //        case 4:
    //            FourNumberPopUPObj.SetActive(true);
    //            FourNumberPopUPObj.transform.localScale = fightEffectType == FightEffectType.BATTLE_EFFECT_CRIT ? CritFontScale : NorMalFontScale;
    //            SetLabelFont(fightEffectType,FourNumberLabel);
    //            for (int i = 0; i <3; i++)
    //            {
    //                FourNumberLabel[i].SetText(showNumber.Substring(i, 1));
    //            }
    //            break;
    //        case 5:
    //            FiveNumberPopUpObj.SetActive(true);
    //            FiveNumberPopUpObj.transform.localScale = fightEffectType == FightEffectType.BATTLE_EFFECT_CRIT ? CritFontScale : NorMalFontScale;
    //            SetLabelFont(fightEffectType,FiveNumberLabel);
    //            for (int i = 0; i <3; i++)
    //            {
    //                FiveNumberLabel[i].SetText(showNumber.Substring(i, 1));
    //            }
    //            break;
    //        default:
    //            break;
    //    }
    //    StartCoroutine(DestroyObjForTime(1));
    //}

    //public void SetLabelFont(FightEffectType fightEffectType,params UILabel[] labelList)
    //{
    //    foreach (UILabel child in labelList)
    //    {
    //        child.font = fightEffectType == FightEffectType.BATTLE_EFFECT_CRIT? CritFont:NorMalFont;
    //    }
    //}

}
