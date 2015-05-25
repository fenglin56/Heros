using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JHGridExt : UIGrid {
	public bool Revert;

	/// <summary>
	/// 控制从左到右还是从右到左
	/// 	从上到下还是从下到上
	/// </summary>
	/// <param name="revertFlag">If set to <c>true</c> revert flag.</param>
	public override void Reposition ()
	{
		if (!mStarted)
		{
			repositionNow = true;
			return;
		}
		
		Transform myTrans = transform;
		
		int x = 0;
		int y = 0;
		int factor=Revert?-1:1;
		if (sorted)
		{
			List<Transform> list = new List<Transform>();
			
			for (int i = 0; i < myTrans.childCount; ++i)
			{
				Transform t = myTrans.GetChild(i);
				if (t && (!hideInactive || NGUITools.GetActive(t.gameObject))) list.Add(t);
			}
			//list.Sort(SortByName);

			for (int i = 0, imax = list.Count; i < imax; ++i)
			{
				Transform t = list[i];
				
				if (!NGUITools.GetActive(t.gameObject) && hideInactive) continue;
				
				float depth = t.localPosition.z;
				t.localPosition = (arrangement == Arrangement.Horizontal) ?
					new Vector3(cellWidth * x*factor, -cellHeight * y, depth) :
						new Vector3(cellWidth * y, -cellHeight * x*factor, depth);
				
				if (++x >= maxPerLine && maxPerLine > 0)
				{
					x = 0;
					++y;
				}
			}
		}
		else
		{
			for (int i = 0; i < myTrans.childCount; ++i)
			{
				Transform t = myTrans.GetChild(i);
				
				if (!NGUITools.GetActive(t.gameObject) && hideInactive) continue;
				
				float depth = t.localPosition.z;
				t.localPosition = (arrangement == Arrangement.Horizontal) ?
					new Vector3(cellWidth * x*factor, -cellHeight * y, depth) :
						new Vector3(cellWidth * y, -cellHeight * x*factor, depth);
				
				if (++x >= maxPerLine && maxPerLine > 0)
				{
					x = 0;
					++y;
				}
			}
		}
		
		UIDraggablePanel drag = NGUITools.FindInParents<UIDraggablePanel>(gameObject);
		if (drag != null) drag.UpdateScrollbars(true);
	}
}
