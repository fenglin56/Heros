using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PopupTextController
{
    static Queue<PopUpInfo> PopUpInfoList = new Queue<PopUpInfo>();
    
    /// <summary>
    /// 弹出结算信息 PopupObjManager挂在在BattleUI场景中
    /// </summary>
    /// <param name="hostPosition"></param>
    /// <param name="content"></param>
    /// <param name="fightEffectType"></param>
	public static void SettleResult(Vector3 hostPosition, string content, FightEffectType fightEffectType, bool isHero)
    {  
        Vector3 uiPos = GetPopupPos(hostPosition, PopupObjManager.Instance.UICamera);
        //TraceUtil.Log("GetPopUPTitle:"+fightEffectType);
        switch (fightEffectType)
        {
            case FightEffectType.BATTLE_EFFECT_CRIT:
				GetPopupWidget(content, uiPos, PopupObjManager.Instance.PopUpCritObj, hostPosition, fightEffectType);
			break;
            case FightEffectType.BATTLE_EFFECT_HIT:
                GetPopupWidget(content, uiPos, PopupObjManager.Instance.PopUpHpObj, hostPosition, fightEffectType, isHero);
                //GetPopupWidget(content, uiPos, PopupObjManager.Instance.PopupObj, hostPosition, fightEffectType);
                break;
            case FightEffectType.BATTLE_ADDHP:
                GetPopupWidget(content, uiPos, PopupObjManager.Instance.AddHPPopupObj, hostPosition, fightEffectType);
                break;
            case FightEffectType.BATTLE_ADDMONEY:
            case FightEffectType.BATTLE_EFFECT_SHILIAN_EXPSHOW:
            case FightEffectType.BATTLE_EFFECT_SHILIAN_XIUWEI:
            case FightEffectType.BATTLE_EFFECT_EXPSHOW:
                GetPopupWidget(content, uiPos, PopupObjManager.Instance.PopUpIconObj, hostPosition, fightEffectType);
                break;
            case FightEffectType.BATTLE_ADDMP:
                GetPopupWidget(content, uiPos, PopupObjManager.Instance.AddMPPopupObj, hostPosition, fightEffectType);
                break;
            case FightEffectType.BATTLE_EFFECT_DODGE:
            //case FightEffectType.BATTLE_EFFECT_HIT:
            case FightEffectType.BATTLE_EFFECT_HP:           
                GetPopupWidget(content, uiPos, PopupObjManager.Instance.NormalPopupObj, hostPosition, fightEffectType);
                break;
            case FightEffectType.TOWN_EFFECT_ZHANLI:
                GetPopupWidget(content, uiPos, PopupObjManager.Instance.PopUpIconObj, hostPosition, fightEffectType);
                break;
        }

        
    }

    public static void SettleResultForTime(EntityModel entityModel, string content, FightEffectType fightEffectType)
    {
        PopUpInfoList.Enqueue(new PopUpInfo() { entityModel = entityModel, content = content, fightEffectType = fightEffectType });
        TweenFloat.Begin(1, 0, 1, null, PopUpForTime);
    }

    static void PopUpForTime(object obj)
    {
        if (PopUpInfoList.Count > 0)
        {
            PopUpInfo popUpInfo = PopUpInfoList.Dequeue();
            //SettleResult(popUpInfo.entityModel.GO.transform.position + UnityEngine.Vector3.up * 18,popUpInfo.content,popUpInfo.fightEffectType);
            SettleResult(getUIPosition(popUpInfo.entityModel), 
                popUpInfo.content, popUpInfo.fightEffectType, false);
            if (PopUpInfoList.Count > 0)
            {
                TweenFloat.Begin(1,0,1,null,PopUpForTime);
            }
        }
    }

    static Vector3 getUIPosition(EntityModel entityModel)
    {
        int index = UnityEngine.Random.Range(0, CommonDefineManager.Instance.CommonDefine.HitNumberPos_VectorX.Length);
        return entityModel.GO.transform.position + new Vector3(CommonDefineManager.Instance.CommonDefine.HitNumberPos_VectorX[index], CommonDefineManager.Instance.CommonDefine.HitNumberPos_VectorY[index], CommonDefineManager.Instance.CommonDefine.HitNumberPos_VectorZ[index]);
    }

    public static Vector3 GetPopupPos(Vector3 sPos, Camera uiCamera)
    {
       var worldPos= Camera.main.WorldToViewportPoint(sPos);
       var uipos = uiCamera.ViewportToWorldPoint(worldPos);

       uipos.z = 0;
       return uipos;
    }
    private static GameObject GetPopupWidget(string content, Vector3 position, GameObject popupLabelPrefab, Vector3 hostPosition, FightEffectType fightEffectType, bool isHero)
    {
       var m_popupWidget = GameObjectPool.Instance.AcquireLocal(popupLabelPrefab, position, Quaternion.identity);
       var scale = m_popupWidget.transform.localScale;
       m_popupWidget.transform.parent = PopupObjManager.Instance.UICamera.transform;
       //TraceUtil.Log("popUp：" + fightEffectType+","+content);
       m_popupWidget.transform.localScale = scale;
       switch (fightEffectType)
       {
           case FightEffectType.BATTLE_EFFECT_HIT:
           case FightEffectType.BATTLE_EFFECT_CRIT:
               m_popupWidget.GetComponent<CritPopUpObj>().Show(fightEffectType,content,isHero);
               //TraceUtil.Log("PopUPHP:" + content);
               break;
           case FightEffectType.BATTLE_ADDMONEY:
           case FightEffectType.BATTLE_EFFECT_SHILIAN_EXPSHOW:
           case FightEffectType.BATTLE_EFFECT_SHILIAN_XIUWEI:
           case FightEffectType.BATTLE_EFFECT_EXPSHOW:
               m_popupWidget.GetComponent<PopupIconObj>().Show(fightEffectType,content);
               var popBahaviour_icon = m_popupWidget.GetComponent<PopupFinish>();
               if (popBahaviour_icon != null)
               {
                   popBahaviour_icon.FightEffectType = fightEffectType;
                   popBahaviour_icon.HostPosition = hostPosition;           
               } 
               break;
           case FightEffectType.TOWN_EFFECT_ZHANLI:
               m_popupWidget.GetComponent<PopupIconObj>().Show(fightEffectType, content);
               var popBahaviour_icon2 = m_popupWidget.GetComponent<PopupFinish>();
               if (popBahaviour_icon2 != null)
               {
                   popBahaviour_icon2.FightEffectType = fightEffectType;
                   popBahaviour_icon2.HostPosition = hostPosition;
               }
               break;
           default:
               var uilabel = m_popupWidget.GetComponent<UILabel>();
               uilabel.text = content;
               var popBahaviour = m_popupWidget.GetComponent<PopupFinish>();
               if (popBahaviour != null)
               {
                   popBahaviour.FightEffectType = fightEffectType;
                   popBahaviour.HostPosition = hostPosition;           
               } 
               break;
       }
        return m_popupWidget;
    }

	private static GameObject GetPopupWidget(string content, Vector3 position, GameObject popupLabelPrefab, Vector3 hostPosition, FightEffectType fightEffectType)
	{
		return GetPopupWidget(content,position,popupLabelPrefab,hostPosition,fightEffectType,false);
	}


    public class PopUpInfo
    {
        public float waitTime;
        public EntityModel entityModel;
        public string content;
        public FightEffectType fightEffectType;
    }
}
