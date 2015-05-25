using UnityEngine;
using System.Collections.Generic;

//武学类型
public enum MartialType
{
	XINFA = 1,
	NEIGONG = 2,
	DANYAO = 3,
}

public class MartialItemListPanel_V2 : MonoBehaviour {
	
	public SingleButtonCallBack XinfaButton;	//心法按钮
	public SingleButtonCallBack NeigongButton;	//内功按钮
	public SingleButtonCallBack DanYaoButton;	//丹药按钮

	public UISprite Background;					//武学类型说明背景
	public GameObject MartialItemAreaPrefab;	//每一行的预制
	private List<QinglongMartialItemArea> MartialItemAreaList;	

	public UIGrid Grid;	//挂载武学图标的grid
	private UIDraggablePanel DraggablePanel;
	private int PrevLineCount;	//上一个行数
	private MartialType CurrentShowType;	//当前显示的武学类型

	[HideInInspector]
	public List<SSendWuXueData> MyMartialDataList;
	[HideInInspector]
	public Transform ThisParent;
	
	void Awake()
	{
		//CurrentShowList = new List<MartialItem>();
		MartialItemAreaList = new List<QinglongMartialItemArea>();
		ThisParent = this.transform.parent;
		DraggablePanel = Grid.transform.parent.GetComponent<UIDraggablePanel>();
		
		XinfaButton.SetCallBackFuntion(OnShowMartialTypeButtonClick, MartialType.XINFA);
		NeigongButton.SetCallBackFuntion(OnShowMartialTypeButtonClick, MartialType.NEIGONG);
		DanYaoButton.SetCallBackFuntion(OnShowMartialTypeButtonClick, MartialType.DANYAO);
	}
	
	
	//初始化显示
	public void show(MartialType type)
	{
		//更新三个按钮状态
		
		//更新武学列表
		OnShowMartialTypeButtonClick(MartialType.XINFA);
		CurrentShowType = MartialType.XINFA;
	}
	
	/// <summary>
	/// 武学类型按钮点击回调
	/// </summary>
	/// <param name="arg">Argument.</param>
	void OnShowMartialTypeButtonClick(object arg)
	{
		//SoundManager.Instance.PlaySoundEffect("Sound_Button_PlayerMartialArts_Change");
		MartialType btnType = (MartialType) arg;
		if(CurrentShowType == btnType)	//重复点击同一个类型按钮，不刷新
			return;

		Background.GetComponent<SpriteSwith>().ChangeSprite((int)btnType);
		XinfaButton.spriteSwithList.ApplyAllItem(P=>P.ChangeSprite(btnType == MartialType.XINFA?2:1));
		NeigongButton.spriteSwithList.ApplyAllItem(P=>P.ChangeSprite(btnType == MartialType.NEIGONG?2:1));
		DanYaoButton.spriteSwithList.ApplyAllItem(P=>P.ChangeSprite(btnType == MartialType.DANYAO?2:1));
		ShowMartialList(btnType);
	}
	
	/// <summary>
	/// 根据武学类型显示武学列表.
	/// </summary>
	/// <param name="martialType">Martial type.</param>
	void ShowMartialList(MartialType martialType)
	{
		//初始化列表里的每个元素
		//在Grid里生成所有的武学prefab(MartialItem)，并给每个prefab添加点击事件回调
		CurrentShowType = martialType;
		//获取当前要显示的所有武学信息
		MyMartialDataList = PlayerMartialDataManager.Instance.GetMartialListByType(martialType);
		
		//武学排序
		MyMartialDataList.Sort((left, right) =>
		                       {
			if (left.dwWuXueID < right.dwWuXueID)
			{
				return -1;
			}
			else if (left.dwWuXueID == right.dwWuXueID)
			{
				return 0;
			}
			else return 1;
			
		});

		MartialItemAreaList.ApplyAllItem(p=>Destroy(p.gameObject));
		MartialItemAreaList.Clear();

		int lineCount = Mathf.CeilToInt(MyMartialDataList.Count / 4);	//行数
		lineCount = lineCount <= 1 ? 2 : lineCount;	//至少两行
		PrevLineCount = lineCount;
		for(int i = 0; i < lineCount; i++)
		{
			GameObject itemLine = NGUITools.AddChild(Grid.gameObject, MartialItemAreaPrefab);
			QinglongMartialItemArea itemArea = itemLine.GetComponent<QinglongMartialItemArea>(); 
			int perLineCount;	//每行显示多少个图标
			int remainCount;	//剩余的武学图标

			if(MyMartialDataList.Count <= 4)
			{
				perLineCount = (lineCount-1-i)*MyMartialDataList.Count;
			}
			else
			{
				remainCount = MyMartialDataList.Count - (i*4);
				if(remainCount >= 4)
				{
					perLineCount = 4;
				}
				else
				{
					perLineCount = remainCount % 4; 
				}
			}
			MartialItemAreaList.Add(itemArea);
			itemArea.Init(i, perLineCount, this);
			itemLine.transform.localPosition = Vector3.zero;
		}
		Grid.repositionNow = true;
	}
	
	//如果有新武学解锁，刷新当前列表
	private void RefreshList(int unLockMartialID)
	{
		int newMartialLevel = PlayerMartialDataManager.Instance.GetMartialLevelByID(unLockMartialID);
		SSendWuXueData newMartialData = new SSendWuXueData(){dwWuXueID = unLockMartialID, byWuXueLevel = (byte)newMartialLevel};
		MyMartialDataList.Add(newMartialData);
		int lineCount = Mathf.CeilToInt(MyMartialDataList.Count / 4);	//行数
		if(MyMartialDataList.Count % 4 == 0)
			lineCount -= 1;

		//新添加的武学下标
		int index = (MyMartialDataList.Count) % 4 == 0 ? 3 : ((MyMartialDataList.Count) % 4 - 1);	

		//如果新行数大于旧的行数，添加新的一行
		if(lineCount <= PrevLineCount)
		{
			MartialItemAreaList[lineCount].AddItem(lineCount, index, this);
		}
		else
		{
			GameObject itemLine = NGUITools.AddChild(Grid.gameObject, MartialItemAreaPrefab);
			QinglongMartialItemArea itemArea = itemLine.GetComponent<QinglongMartialItemArea>(); 
			itemArea.AddItem(lineCount, index, this);
			MartialItemAreaList.Add(itemArea);
			Grid.repositionNow = true;
			PrevLineCount += 1;
		}

	}
	
	/// <summary>
	/// 武学图标被选中.
	/// </summary>
	/// <param name="martialId">Martial identifier.</param>
	public void OnSelectItemCallback(object arg)
	{
		Debug.Log("OnSelectItemCallback id = " + (int)arg);
		//SoundManager.Instance.PlaySoundEffect("Sound_Button_PlayerMartialArts_Select");
		MartialItemAreaList.ApplyAllItem(p=>{
			p.OnSelectItemCallback(arg);
		});
	}
	
	/// <summary>
	/// 当武学被学习或升级后，更新武学图标
	/// 如果有新武学解锁，刷新当前列表 
	/// </summary>
	/// <param name="martialId">Martial identifier.</param>
	public void UpdateMartialList(int martialID)
	{
		//解锁新武学，添加新的武学图标
		int unLockID = PlayerMartialDataManager.Instance.UnlockNewMartialId(martialID);
		if(unLockID != 0)
		{
			RefreshList(unLockID);
		}
		
		MartialItemAreaList.ApplyAllItem(p=>{
			p.OnLearnOrUpdateCallback(martialID);
		});
	}
}

