using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(UIGrid))]
public class LoopScrollView : View
{
  
    public UIDraggablePanel mScrollView;
    public UIPanel panel;
    private GameObject mReceiver;
    private string mFunctionName;
    private List<UIItem> mItemList = new List<UIItem>();
    private Vector4 mPosParam;
    private Transform mCachedTransform;
    private int mStartIndex;
    private int mMaxCount;
    private bool IsLoading;
    public bool AllPageEnd = false;
    public bool kaiguan = true;

    protected override void RegisterEventHandler()
    {
       
    }
    /// <summary>
    /// 初始化表
    /// </summary>
    /// <param name="go"></param>
    /// <param name="functionName"></param>
    public void Init(GameObject go)
    {
        mReceiver = go;
        mFunctionName = "OnChangeItem";
        mItemList.Clear();
        for (int i = 0; i < mCachedTransform.childCount; ++i)
        {
            Transform t = mCachedTransform.GetChild(i);
            UIItem uiw = t.GetComponent<UIItem>();
            if (uiw == null) uiw = t.gameObject.AddComponent<UIItem>();
            uiw.name = mItemList.Count.ToString();
            mItemList.Add(uiw);
        }
    }

    /// <summary>
    /// 刷新表
    /// </summary>
    public void UpdateList(int count)
    {
     //   Debug.Log(mMaxCount);
        //mStartIndex = 0;
        mMaxCount = count;
        IsLoading = false;
       // SceneManager.UINotice(UINoticeStatus.CloseLoading);
        //for (int i = 0; i < mItemList.Count; i++)
        //{
        //    UIItem item = mItemList[i];
        //    item.name = i.ToString();
        //   // item.Invalidate(true);
        //    item.gameObject.SetActive(i < count);
        //}
    }

    public List<UIItem> GetList()
    {
        return mItemList;
       
    }

    /// <summary>
    /// 发送消息到目标,告知需要更换内容
    /// </summary>
    /// <param name="go"></param>
    void SendMsg(GameObject go)
    {
        //mReceiver.SendMessage(mFunctionName, go, SendMessageOptions.DontRequireReceiver);
        UIEventManager.Instance.TriggerUIEvent(UIEventType.OnChangeItem, go);
    }

    void Awake()
    {
        RegisterEventHandler();
        mCachedTransform =transform;
//        mScrollView = mCachedTransform.parent.GetComponent<UIDraggablePanel>();
//        panel = mCachedTransform.parent.GetComponent<UIPanel>();
        UIGrid grid = this.GetComponent<UIGrid>();
        float cellWidth = grid.cellWidth;
        float cellHeight = grid.cellHeight;
        mPosParam = new Vector4(cellWidth, cellHeight, grid.arrangement == UIGrid.Arrangement.Horizontal ? 1 : 0, grid.arrangement == UIGrid.Arrangement.Vertical ? 1 : 0);
    }

    void LateUpdate()
    {
       
        if (kaiguan)
        {
            if (mItemList.Count <= 1) return;

            int sourceIndex = -1;
            int targetIndex = -1;
            int sign = 0;

            // 获得第一个和最后一个的visible
            bool firstVislable = panel.IsVisible(mItemList[0].transform.position);
            bool lastVisiable = panel.IsVisible(mItemList[mItemList.Count - 1].transform.position);

            // 如果都显示,那么返回
            if (firstVislable == lastVisiable)
            {
                if(mItemList[mItemList.Count - 1].transform.localPosition.y>panel.clipRange.y)
                {
                    mScrollView.restrictWithinPanel=true;
                }
                return;

            }
            // 得到需要替换的源和目标
            if (firstVislable)
            {
                sourceIndex = mItemList.Count - 1;
                targetIndex = 0;
                sign = 1;
            }
            else if (lastVisiable)
            {
                sourceIndex = 0;
                targetIndex = mItemList.Count - 1;
                sign = -1;
            }
            // 如果小于真正的初始索引或大于真正的结束索引,返回
            int realSourceIndex = int.Parse(mItemList[sourceIndex].gameObject.name);
            int realTargetIndex = int.Parse(mItemList[targetIndex].gameObject.name);
            if (realTargetIndex <= mStartIndex)
            {
                mScrollView.restrictWithinPanel = true;
                return;
            }
            else if (realTargetIndex >= (mMaxCount - 1))
            {
              //  Debug.Log("end" + mMaxCount);

                mScrollView.restrictWithinPanel = true;
                if (!IsLoading && !AllPageEnd)
                {
                    IsLoading = true;
                    //SceneManager.UINotice(UINoticeStatus.Loading);
                    //RaiseEvent(NotifyEventTypes.ActionItemEnd.ToString(), null);


                }
                return;
                //if (currentPageIndex < TotalPageSize)
                //{
                //   // 
                //    Debug.Log("end");
                //}
            }
            //if ( sourceIndex > -1 ) {
            mScrollView.restrictWithinPanel = false;
            UIItem movedWidget = mItemList[sourceIndex];
            Vector3 offset = new Vector3(sign * mPosParam.x * mPosParam.z, sign * mPosParam.y * mPosParam.w, 0);
            movedWidget.transform.localPosition = mItemList[targetIndex].transform.localPosition + offset;
            mItemList.RemoveAt(sourceIndex);
            mItemList.Insert(targetIndex, movedWidget);
            movedWidget.name = (realSourceIndex > realTargetIndex ? (realTargetIndex - 1) : (realTargetIndex + 1)).ToString();
            SendMsg(movedWidget.gameObject);
            //}
        }

    }
}
 