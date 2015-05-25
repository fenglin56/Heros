using UnityEngine;
using System.Collections;
using System;

public class ItemPagerManager : View {

    public int PagerSize = 5;
    public Transform[] ItemBgs;
    public SingleButtonCallBack PreviousPage;
    public SingleButtonCallBack NextPage;
    public UILabel PagerCaption;

    private bool m_isPagerInited = false;
    private int m_pagerSize=0;
    private int m_currentPager = 1;
	public int CurrentPage{get{return m_currentPager;}}
    private SingleButtonCallBack[] m_itemClick;
    private IPagerItem[] m_pagerItems;
    [HideInInspector]
    public int SelectedIndex;
	// Use this for initialization

    public event PageItemSelectedHandler OnPageItemSelected;
    public Action<PageChangedEventArg> OnPageChanged;

    private int[] m_guideBtnID;

	void Start () {

        m_guideBtnID = new int[2];
        if(null != PreviousPage)
        {
            //TODO GuideBtnManager.Instance.RegGuideButton(PreviousPage.gameObject, UI.MainUI.UIType.Empty, SubType.TopCommon,out m_guideBtnID[0]);
        }
        if(null != NextPage)
        {
            //TODO GuideBtnManager.Instance.RegGuideButton(NextPage.gameObject, UI.MainUI.UIType.Empty, SubType.TopCommon, out m_guideBtnID[1]);
        }

        if (ItemBgs == null || ItemBgs.Length != PagerSize)
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"请先关联列表背景物件(Prefab->Prefab_EquipList)");
        }
        RegisterEventHandler();
	}

    void OnDestroy()
    {
        if (m_guideBtnID != null)
        {
            for (int i = 0; i < m_guideBtnID.Length; i++)
            {
                //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID[i]);
            }
        }       
    }
    /// <summary>
    /// 选择指定项
    /// </summary>
    /// <param name="index"></param>
    public void SelectedItem(int index)
    {
        SelectedIndex = index;
        ItemClickCallBack(index);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    public IPagerItem[] GetCurPageItem
    {
        get { return m_pagerItems; }
    }
    /// <summary>
    /// 初始化分页
    /// </summary>
    /// <param name="pagerItemCount"></param>
    /// <param name="currentPager"></param>
    /// 
    public void InitPager(int pagerItemCount)
    {
        InitPager(pagerItemCount, m_currentPager);
    }

    public void InitPager(int pagerItemCount, int currentPager,int selectedIndex)
    {
        SelectedIndex = selectedIndex;
		//没有任何装备时，pageSize算1
		m_pagerSize =pagerItemCount==0?1:( pagerItemCount / PagerSize + ((pagerItemCount % PagerSize) == 0 ? 0 : 1));
        if (currentPager > m_pagerSize)
        {
            //TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"页码设置错误,当前页：" + currentPager + "  页码数：" + m_pagerSize);
            return;
        }
        m_currentPager = currentPager;
        m_isPagerInited = true;

        if(null != PagerCaption)
        {
            PagerCaption.text = string.Format("{0}/{1}", m_currentPager, m_pagerSize);
        }
        if (PreviousPage != null &&PreviousPage.BackgroundSprite != null)
        {
            PreviousPage.BackgroundSprite.alpha = m_currentPager <= 1 ? 0.5f : 1f;
        }
        if (NextPage != null &&NextPage.BackgroundSprite != null)
        {
            NextPage.BackgroundSprite.alpha = m_currentPager >= m_pagerSize ? 0.5f : 1f;
        }
        //TraceUtil.Log(PagerCaption.text + "  " + pagerItemCount + " 分页项：" + m_pagerSize + " " + currentPager);
        ChangePage(m_currentPager);
        
    }
    public void InitPager(int pagerItemCount,int currentPager)
    {
        InitPager(pagerItemCount, m_currentPager,SelectedIndex);
    }
    /// <summary>
    /// 传入Item项
    /// </summary>
    /// <param name="pagerItems"></param>
    public void UpdateItems(IPagerItem[] pagerItems,string itemName)
    {
        //TraceUtil.Log("传入Item：" + pagerItems.Length);
        if (!m_isPagerInited)
        {
            //TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"请先传入Item数量初始化分页");
			//重置页码
			InitPager(0,1,0);
        }
        m_pagerItems=pagerItems;
        SetItemStatus(itemName);
    }
    private void SetItemStatus(string itemName)
    {
        ItemBgs.ApplyAllItem(P => P.ClearChild(itemName));
        if (this.m_pagerItems != null && this.m_pagerItems.Length>0)
        {
            if(this.m_pagerItems.Length<=SelectedIndex)
            {
                SelectedIndex=0;
            }
            for (int i = 0; i < m_pagerItems.Length; i++)
            {
                m_pagerItems[i].GetTransform().parent = ItemBgs[i];
                m_pagerItems[i].GetTransform().localPosition = new Vector3(0,0,-2);
                m_pagerItems[i].GetTransform().localScale = new Vector3(1, 1, 1);
            }
            ItemClickCallBack(SelectedIndex);
        }
    }
    private void PagerClickHandle(object obj)
    {
        bool isForward= (bool)obj;
        bool pagerChanged = false;
        if (isForward)
        {
            if (m_currentPager == 1)
            {
                return;
            }
            else
            {
                m_currentPager--;
                pagerChanged = true;
            }
        }
        else
        {
            if (m_currentPager == m_pagerSize)
            {
                return;
            }
            else
            {
                m_currentPager++;
                pagerChanged = true;
            }
        }
        if (pagerChanged)
        {
            ChangePage(m_currentPager);
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Page");
        }
    }
    private void ChangePage(int currentPage)
    {
        if(null != PagerCaption)
        {
            PagerCaption.text = string.Format("{0}/{1}", currentPage, m_pagerSize);
        }

        if (PreviousPage != null &&PreviousPage.BackgroundSprite != null)
        {
            PreviousPage.BackgroundSprite.alpha = currentPage <= 1 ? 0.5f : 1f;
			PreviousPage.SetButtonColliderActive(currentPage > 1);
        }
        if (NextPage != null &&NextPage.BackgroundSprite != null)
        {
            NextPage.BackgroundSprite.alpha = currentPage >= m_pagerSize ? 0.5f : 1f;
			NextPage.SetButtonColliderActive(currentPage < m_pagerSize);
        }
        //发出页面改变消息
        //int startPage = (currentPage - 1) * ITEM_SIZE_IN_PAGER;
        int count = PagerSize;
        PageChangedEventArg pageChangedEventArg = new PageChangedEventArg(currentPage, count);
        if (OnPageChanged != null)
        {
            OnPageChanged(pageChangedEventArg);
        }
        //RaiseEvent(EventTypeEnum.PageChanged.ToString(), pageChangedEventArg);
    }
    private void ItemClickCallBack(object obj)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
        int itemIndex=(int)obj;
        if(m_pagerItems!=null&&m_pagerItems.Length>itemIndex)
        {
            for(int i=0;i<m_pagerItems.Length;i++)
            {
                m_pagerItems[i].OnLoseFocus();
            }
            if (itemIndex < 0)
                return;
            m_pagerItems[itemIndex].OnGetFocus();
            m_pagerItems[itemIndex].OnBeSelected();

            SelectedIndex = itemIndex;
            //触发Item选择事件
            if (OnPageItemSelected != null)
            {
                OnPageItemSelected(m_pagerItems[itemIndex]);
            }
        }
    }
    private void SetItemClickHandle()
    {
        m_itemClick = new SingleButtonCallBack[PagerSize];

        for (int i = 0; i < PagerSize; i++)
        {
            m_itemClick[i] = ItemBgs[i].GetComponent<SingleButtonCallBack>();
            if (m_itemClick[i] != null)
            {
                m_itemClick[i].SetCallBackFuntion(ItemClickCallBack, i);
            }            
        }
    }
    private void SetPagerClickHandle()
    {
        if(PreviousPage != null )
        {
            this.PreviousPage.SetCallBackFuntion(PagerClickHandle, true);//向前翻页
        }
        if(NextPage != null)
        {
            this.NextPage.SetCallBackFuntion(PagerClickHandle, false);//向后翻页
        }
    }

    protected override void RegisterEventHandler()
    {
        SetItemClickHandle();
        SetPagerClickHandle();
    }
}
