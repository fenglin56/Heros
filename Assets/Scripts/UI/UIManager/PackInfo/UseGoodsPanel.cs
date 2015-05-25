using UnityEngine;
using System.Collections;
using UI.MainUI;
public class UseGoodsPanel : MonoBehaviour {
    public UILabel Title;
    public UILabel Des;
    public UILabel CountDes;
    public SingleButtonCallBack UseBtn;
    public SingleButtonCallBack UseAllBtn;
    public SingleButtonCallBack CloseBtn;
    private int m_GoodsId;
    private static UseGoodsPanel instance;
    public static UseGoodsPanel Instance
    {
        get
        {
            if (!instance) {
                instance = (UseGoodsPanel)GameObject.FindObjectOfType (typeof(UseGoodsPanel));
                if (!instance)
                    Debug.LogError ("没有附加JewelBesetManager脚本的gameobject在场景中");
            }
            return instance;
        }
    }
     void Awake()
    {
        UseBtn.SetCallBackFuntion(OnUseGoodsBtnClick);
        UseAllBtn.SetCallBackFuntion(OnUseAllGoodsBtnClick);
        CloseBtn.SetCallBackFuntion(OnClosePanelBtnClick);
        Close();
    }
	public void Show(int goodsID)
    {
        ItemData item=ItemDataManager.Instance.GetItemData(goodsID);
        if(item==null)
        {
            TraceUtil.Log(SystemModel.Common,"goodsID"+goodsID+"不存在");
            return ;
        }
        if(item._GoodsClass!=2||item._GoodsSubClass!=3)
        {
            TraceUtil.Log(SystemModel.Common,"物品：goodsID="+goodsID+"不可使用");
            return ;
        }
        else
        {
            transform.localPosition=Vector3.zero;
            m_GoodsId=goodsID;
            string Name=LanguageTextManager.GetString(item._szGoodsName);
            string TitleStr=string.Format(LanguageTextManager.GetString("IDS_I1_51"),Name);
            Title.SetText(TitleStr);
            Des.SetText(LanguageTextManager.GetString(item._szDesc));
            int count=ContainerInfomanager.Instance.GetOwnMaterialCount(goodsID);
            string countdesStr=string.Format(LanguageTextManager.GetString("IDS_I1_52"),count,Name);
            CountDes.SetText(countdesStr);
        }
      

    }
    void OnUseGoodsBtnClick(object obj)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_MedicamentUse");
        UseGoods();
        Close();
    }
    void UseGoods()
    {
        ContainerInfomanager.Instance.UseUsableGoods(m_GoodsId,false);
    }

    void UseAllGoods()
    {
        ContainerInfomanager.Instance.UseUsableGoods(m_GoodsId,true);
    }

    void OnUseAllGoodsBtnClick(object obj)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_MedicamentAllUse");
        UseAllGoods();
        Close();
    }
    void OnClosePanelBtnClick(object obj)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_MedicamentClose");
        Close();
    }
    public void Close()
    {
        transform.localPosition=new Vector3(0,0,-1000);
    }
}
