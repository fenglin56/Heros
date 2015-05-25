using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UI.MainUI;
using UI;
using System.Linq;
using UI.Forging;
using System;


public class ForgingRecipeConfigDataManager : View {

    public ForgeRecipeDataBase ForgeRecipeDataBase;
    private List<ForgeRecipeData> m_ForgeRecipeData_Equip=new List<ForgeRecipeData>();
    private List<ForgeRecipeData> m_ForgeRecipeData_GiftBox=new List<ForgeRecipeData>();
    private List<ForgeRecipeData> m_ForgeRecipeData_Material=new List<ForgeRecipeData>();
    private static ForgingRecipeConfigDataManager m_instance;
    private readonly string[] PositionDic=new string[]{"物资","武器","时装","头饰","衣服","靴子","饰品","徽章"};
    private readonly Dictionary<string,string> ProfessionDic=new Dictionary<string, string>{{"1","剑客"},{"4","刺客"}};
    public static ForgingRecipeConfigDataManager Instance
    {
        get { return m_instance; }
    }
    
    void OnDestroy()
    {
        m_instance = null;
       
        RemoveEventHandler(EventTypeEnum.PlayerLevelUpdate.ToString(), OnPlayerLvOrMoneyUpdate);
        UIEventManager.Instance.RemoveUIEventHandel(UIEventType.TownUIBtnLoadComplete, TownUISceneLoadComplete);
        UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ResetContainerGoods, OnContainerChange);
    }
    protected override void RegisterEventHandler()
    {
        AddEventHandler(EventTypeEnum.PlayerLevelUpdate.ToString(), OnPlayerLvOrMoneyUpdate);
        UIEventManager.Instance.RegisterUIEvent(UIEventType.TownUIBtnLoadComplete, TownUISceneLoadComplete);
        UIEventManager.Instance.RegisterUIEvent(UIEventType.ResetContainerGoods,OnContainerChange);
    }

    bool CheckhasRepiceCanForging()
    {
        bool can = false;
        // throw new System.NotImplementedException();
        foreach (ForgingType type in Enum.GetValues(typeof( ForgingType)))
        {
            if (can)
            {
                break;
            }
            List<ForgeRecipeData> list = ForgeRecipeDataList(type);
            foreach (var item in list)
            {
                if (IsCanForging(item))
                {
                    can = true;
                    break;
                }
            }
        }
        return can;
    }

    void TownUISceneLoadComplete(object uiEventInsatance)
    {
       if( CheckhasRepiceCanForging())
        {
            UIEventManager.Instance.TriggerUIEvent(UIEventType.PlayMainBtnAnim, UIType.Forging);
        }
        else
        {
            UIEventManager.Instance.TriggerUIEvent(UIEventType.StopMainBtnAnim, UIType.Forging);
        }
    }

    void OnContainerChange(object uiEventInsatance)
    {
        if( CheckhasRepiceCanForging())
        {
            UIEventManager.Instance.TriggerUIEvent(UIEventType.PlayMainBtnAnim, UIType.Forging);
        }
        else
        {
            UIEventManager.Instance.TriggerUIEvent(UIEventType.StopMainBtnAnim, UIType.Forging);
        }
    }

    void OnPlayerLvOrMoneyUpdate(INotifyArgs e)
    {
        if( CheckhasRepiceCanForging())
        {
            UIEventManager.Instance.TriggerUIEvent(UIEventType.PlayMainBtnAnim, UIType.Forging);
        }
        else
        {
            UIEventManager.Instance.TriggerUIEvent(UIEventType.StopMainBtnAnim, UIType.Forging);
        }
    }


 
    void Awake()
    {
        RegisterEventHandler();
        m_instance = this;
        if (ForgeRecipeDataBase != null)
        {
            foreach (var item in ForgeRecipeDataBase.ForgeRecipeDataList)
            {
                if(item.ForgeType==ForgingType.ForgEquipment)
                {
                    m_ForgeRecipeData_Equip.Add(item);
                }
                else if(item.ForgeType==ForgingType.ForgGiftBox)
                {
                    m_ForgeRecipeData_GiftBox.Add(item);
                }
                else if(item.ForgeType==ForgingType.ForgMaterial)
                {
                    m_ForgeRecipeData_Material.Add(item);
                }


            }
        }
        else
            TraceUtil.Log(SystemModel.wanglei,TraceLevel.Error,"铸造配方配置文件为空！");
        
    
    }
    /// <summary>
    /// Determines whether this instance is own the specified data.
    /// </summary>
    /// <returns><c>true</c> if this instance is own the specified data; otherwise, <c>false</c>.</returns>
    /// <param name="data">Data.</param>
    public bool IsOwn(ForgeRecipeData data)
    {
        bool isOwn=false;
        ItemData Itemdata=ItemDataManager.Instance.GetItemData(data.ForgeEquipmentID);
        if(Itemdata._GoodsClass==1)
        {
            EquipmentData qu=Itemdata as EquipmentData ;
            List<ItemFielInfo> list= ContainerInfomanager.Instance.itemFielArrayInfo.Where(p=>p.LocalItemData._GoodsSubClass==qu._GoodsSubClass).ToList();
            foreach(var item in list)
            {
                if(item.equipmentEntity.EQUIP_FIELD_QAULITY>=qu._ColorLevel)
                {
                    isOwn=true;
                }
                else if(item.equipmentEntity.EQUIP_FIELD_QAULITY==qu._ColorLevel&&item.LocalItemData._AllowLevel>=qu._AllowLevel)
                {
                    isOwn=true;
                }
            }
          
        }
        else
        {

            isOwn=false;
         
        }
        return isOwn; 
       
    }
    /// <summary>
    /// 获取已拥有的材料的数量
    /// </summary>
    /// <returns>The own material count.</returns>
    /// <param name="materialID">Material I.</param>
    public int GetOwnMaterialCount(int materialID)
    {
      List<ItemFielInfo> items=  ContainerInfomanager.Instance.GetPackItemList().Where(p=>p.LocalItemData._goodID==materialID).ToList();
        int count=0;
        foreach(var item in items )
        {
          count+= item.sSyncContainerGoods_SC.byNum;

        }
        return count;
    }
    public bool IsCanForging(ForgeRecipeData data)
    {
        bool Canforg=true;
        foreach(var item in data.ForgeCost)
        {
            int count=GetOwnMaterialCount(item.RecipeID);
            if(count<item.count)
            {
            Canforg=false;
                break;
            }
        }
        return Canforg;
    }
    /// <summary>
    /// Gets the name of the goods.
    /// </summary>
    /// <returns>The goods name.</returns>
    /// <param name="GoodsID">Goods I.</param>
    public string GetGoodsName(int GoodsID)
    {
        ItemData data=ItemDataManager.Instance.GetItemData(GoodsID);
        return NGUIColor.SetTxtColor( LanguageTextManager.GetString(data._szGoodsName),(TextColor)data._ColorLevel);
    }

    public string GetGoodsLevel(int GoodsID)
    {
        ItemData data=ItemDataManager.Instance.GetItemData(GoodsID);
        TextColor color;
        if(data._AllowLevel>PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL)
        {
            color=TextColor.red;
        }
        else
        {
            color=TextColor.green;
        }
        return NGUIColor.SetTxtColor( "Lv"+data._AllowLevel,color);
    }
    /// <summary>
    /// Gets the goods profab.
    /// </summary>
    /// <returns>The goods profab.</returns>
    /// <param name="GoodsID">Goods I.</param>
    public GameObject GetGoodsProfab(int GoodsID)
    {
      return  ItemDataManager.Instance.GetItemData(GoodsID)._picPrefab;
    }

    public string GetPosition(int GoodsID)
    {
       // 0=武器、(1+2+3+4+5+6+7+8+9+10)=时装（不包括括号）、11=头饰、12=衣服、13=鞋子、14=饰品、15=徽章

        return PositionDic[((EquipmentData)ItemDataManager.Instance.GetItemData(GoodsID))._GoodsSubClass];

    }
    public string GetProfession(int GoodsID)
    {
        // 0=武器、(1+2+3+4+5+6+7+8+9+10)=时装（不包括括号）、11=头饰、12=衣服、13=鞋子、14=饰品、15=徽章
        string str="";
        string[] tem=((EquipmentData)ItemDataManager.Instance.GetItemData(GoodsID))._AllowProfession.Split('+');
        foreach(string item in tem)
        {
            if(int.Parse(item)==PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION)
            {
                str+=NGUIColor.SetTxtColor(ProfessionDic[item],TextColor.ChatYellow)+" ";
            }
            else
            {
                str+=NGUIColor.SetTxtColor(ProfessionDic[item],TextColor.red)+" ";
            }
        }
        return str;
        
        
    }
    /// <summary>
    ///城镇主界面功能按钮配置数据 
    /// </summary>
    public List<ForgeRecipeData> ForgeRecipeDataList(ForgingType type)
    {

      switch(type)
        {
            case ForgingType.ForgEquipment:
                return m_ForgeRecipeData_Equip;
            case ForgingType.ForgGiftBox:
                return m_ForgeRecipeData_GiftBox;
            case ForgingType.ForgMaterial:
                return m_ForgeRecipeData_Material;
            default :
                return null;
        }
    }
    
  
}
