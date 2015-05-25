using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace UI.MainUI{
	public class EctypeDiffListPanel : MonoBehaviour {

		public GameObject SingleEctypeDiffItemPrefab;
		public Transform Grid;
		public UIPanel m_UIDraggablePanel;
		public UISprite ConstraintSprite_Up;
		public UISprite ConstraintSprite_Down;		
		public UISprite DragBackground;

		//public SMSGEctypeSelect_SC UnlockEctypeData{get;private set;}
		public List<SingleEctypeDiffItem> MyEctypeDiffItemList{get;private set;}
		public EctypePanel_V5 MyParent{get;private set;}

		float animTime = 0.3f;
		GameObject TweenFloatObj;
		Vector3 ConstraintSpriteDefaultScale;
		bool isRead = false;
        //引导列表滚动到指定项
        private float m_noticeToDragAmount;
		void Init()
		{
			if (isRead)
				return;
			isRead = true;
			ConstraintSpriteDefaultScale = ConstraintSprite_Up.transform.localScale;
			MyEctypeDiffItemList = new List<SingleEctypeDiffItem>();
			m_UIDraggablePanel.gameObject.GetComponent<UIDraggablePanel>().ConstraintCallBack = SetPanelConstraint;
			originalPos = m_UIDraggablePanel.transform.localPosition;
		}

		public IEnumerator Init(SMSGEctypeSelect_SC unlockEctypeData,EctypePanel_V5 myParent,int ectypeID)
		{
			Init ();
			MyParent = myParent;
			ResetItemListPosition();
			MyEctypeDiffItemList.ApplyAllItem(C=>Destroy(C.gameObject));
			float startMarkDragpanelPosY = m_UIDraggablePanel.transform.localPosition.y;
			m_UIDraggablePanel.transform.localPosition = new Vector3(m_UIDraggablePanel.transform.localPosition.x,startPosY,m_UIDraggablePanel.transform.localPosition.z);
			MyEctypeDiffItemList.Clear();
			//指定跳转区域副本ID
			int singleItemIndex = 0;
			int areaEctypeID = -1;
			if(ectypeID != -1)
			{
				areaEctypeID = EctypeConfigManager.Instance.GetSelectContainerID (ectypeID);
			}
            yield return null;
			//UnlockEctypeData = unlockEctypeData;
			List<int>ectypeDiffID = new List<int>();
			SingleEctypeDiffItem tweenDiffItem = null;
			foreach(var child in unlockEctypeData.sMSGEctypeData_SCs)
			{
				//获取区域副本ID
				int diffID = myParent.GetEctypeDiffID((int)child.dwEctypeContaienrID);
				if(!ectypeDiffID.Contains(diffID))
				{
					ectypeDiffID.Add(diffID);
				}
			}
			ectypeDiffID.Sort((left,right)=>
			                  {
				if(left>right){return 1;}
				else if(left==right){return 0;}
				else return -1;
			});
			for(int i = 0;i<=ectypeDiffID.Count;i++)
			{
				if(i<ectypeDiffID.Count){//显示解锁的副本
					if(areaEctypeID != -1 && areaEctypeID == ectypeDiffID[i])
					{
						singleItemIndex = i;
					}
					SingleEctypeDiffItem newItem = CreatObjectToNGUI.InstantiateObj(SingleEctypeDiffItemPrefab,Grid).GetComponent<SingleEctypeDiffItem>();
					newItem.Init(EctypeConfigManager.Instance.EctypeSelectConfigList[ectypeDiffID[i]],this);
					newItem.transform.localPosition = new Vector3(0,200-126*i,0);

                    MyEctypeDiffItemList.Add(newItem);
				}else if(i<EctypeConfigManager.Instance.EctypeSelectConfigList.Count)//显示未解锁的副本,选中最后一个解锁副本
				{
					SingleEctypeDiffItem newItem = CreatObjectToNGUI.InstantiateObj(SingleEctypeDiffItemPrefab,Grid).GetComponent<SingleEctypeDiffItem>();
					newItem.Init(null,this);
					newItem.transform.localPosition = new Vector3(0,200-126*i,0);
					MyEctypeDiffItemList.Add(newItem);
					//tweenDiffItem = MyEctypeDiffItemList[i-1];//SelectAndMoveToEctypeDiffItem(MyEctypeDiffItemList[i-1]);
					if(areaEctypeID == -1)
					{
						singleItemIndex = i-1;
					}
					tweenDiffItem = MyEctypeDiffItemList[singleItemIndex];
				}

			}
			DragBackground.transform.localScale = new Vector3(40,126*(ectypeDiffID.Count),0);
            yield return null;
            for (int i = 0; i < MyEctypeDiffItemList.Count; i++)
            {
                var newItem = MyEctypeDiffItemList[i];
                if (newItem.MyConfigData != null)
                {
                    newItem.DragAmount = (i + 1) / (float)MyEctypeDiffItemList.Count;
                    newItem.gameObject.RegisterBtnMappingId(newItem.MyConfigData._lEctypeID, UIType.Battle, BtnMapId_Sub.Battle_EctypeTab
                        , NoticeToDragSlerp, newItem.DragAmount);
                }
            }            
			if (m_noticeToDragAmount != 0) {
				m_UIDraggablePanel.transform.localPosition = new Vector3(m_UIDraggablePanel.transform.localPosition.x,startMarkDragpanelPosY,m_UIDraggablePanel.transform.localPosition.z);
				StartCoroutine (DragAmountSlerp (m_noticeToDragAmount));
				m_noticeToDragAmount = 0;
				if(tweenDiffItem != null)
					OnMyChildItemClick(tweenDiffItem);
			} else if(tweenDiffItem != null){
				SelectAndMoveToEctypeDiffItem(tweenDiffItem,singleItemIndex);			
			}
		}
        /// <summary>
        /// 记下要自动滚动到的位置
        /// </summary>
        /// <param name="targetAmount"></param>
        private void NoticeToDragSlerp(float targetAmount)
        {
            m_noticeToDragAmount = targetAmount;
        }
        public IEnumerator DragAmountSlerp(float targeAmount)
        {
            yield return null;
            var dragPanelComp = m_UIDraggablePanel.GetComponent<UIDraggablePanel>();
            if (dragPanelComp.shouldMove)
            {
                float smoothTime = 0.3f, currentSmoothTime = 0; ;
                float currentAmount = 0;
                while (true)
                {
                    currentSmoothTime += Time.deltaTime;
                    currentAmount = Mathf.Lerp(currentAmount, targeAmount, Time.deltaTime * 20);
                    dragPanelComp.SetDragAmount(0, currentAmount, false);
                    yield return null;
                    if ((targeAmount - currentAmount) <= float.Epsilon || currentSmoothTime >= smoothTime)
                    {
						Debug.Log("currentSmoothTime==="+currentSmoothTime+"fff"+Time.deltaTime);
                        break;
                    }
                }
            }
        }
		private float startPosY = 800;
		public void TweeenClose()
		{			
			if(TweenFloatObj!=null){DestroyImmediate(TweenFloatObj);}
			TweenFloatObj = TweenFloat.Begin(animTime,m_UIDraggablePanel.transform.localPosition.y,startPosY,SetDraggablePanelPosition);
		}

		void SelectAndMoveToEctypeDiffItem(SingleEctypeDiffItem item,int index)
		{
			OnMyChildItemClick(item);
			//SoundManager.Instance.PlaySoundEffect("Sound_UIEff_EctypeTabFallDown");
			if(TweenFloatObj!=null){DestroyImmediate(TweenFloatObj);}
			int posY = 126 * ((index+1 < 4 ? 4 : index+1) - 4);//126*((MyEctypeDiffItemList.Count<4?4:MyEctypeDiffItemList.Count)-4)
			TweenFloatObj = TweenFloat.Begin(animTime,startPosY,posY,SetDraggablePanelPosition,PlaySound);
		}

		void PlaySound(object obj)
		{
			//SoundManager.Instance.PlaySoundEffect("Sound_UIEff_EctypeTabFallDown");
		}
		private Vector3 originalPos ;
		void SetDraggablePanelPosition(float posY)
		{
			m_UIDraggablePanel.transform.localPosition = new Vector3(0,posY,0);
			var clip = m_UIDraggablePanel.clipRange;
			clip.y = -posY;
			m_UIDraggablePanel.clipRange = clip;
		}

		void SetPanelConstraint(Vector3 constraint)
		{
			ConstraintSpriteDefaultScale.y = constraint.y;
			ConstraintSprite_Up.transform.localScale =ConstraintSpriteDefaultScale;
			ConstraintSprite_Down.transform.localScale =ConstraintSpriteDefaultScale;
		}

		void ResetItemListPosition()
		{
			m_UIDraggablePanel.transform.localPosition = originalPos;//Vector3.zero;
			var clip = m_UIDraggablePanel.clipRange;
			clip.y = clip.x = 0;
			m_UIDraggablePanel.clipRange = clip;
		}
		private SingleEctypeDiffItem curSelectItem;
		public void OnMyChildItemClick(SingleEctypeDiffItem clickItem)
		{
			if (curSelectItem == clickItem) {
				return;			
			}
			curSelectItem = clickItem;
			MyEctypeDiffItemList.ApplyAllItem(C=>C.SetSelectStatus(clickItem));
			MyParent.OnEctypdDiffItemSelect(clickItem);
		}

	}
}