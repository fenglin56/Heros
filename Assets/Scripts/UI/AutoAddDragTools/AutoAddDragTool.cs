using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AutoAddDragTool : MonoBehaviour {

	public UIDraggablePanel m_uiDraggablePanel;


	public GameObject prefab01;
	public GameObject prefab02;


	public float Spacing;

	private int m_totalObjCounts = 0;

	private float m_totalLength = 0;

    private List<GameObject> m_objectList = new List<GameObject>();




	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Add(GameObject obj)
	{

		float height = NGUIMath.CalculateAbsoluteWidgetBounds(obj.transform).size.y;

		Transform objTrans = obj.transform;
		Vector3 itemScale = objTrans.localScale;
		objTrans.parent = transform;

        obj.RemoveComponent<UIPanel>("UIPanel");
        if(obj.GetComponent<UIDragPanelContents>() == null)
        {
            obj.AddComponent<UIDragPanelContents>();
        }

		float localY = 0;
		if(m_totalObjCounts == 0)
		{
			localY = -height/2;
		}
		else
		{
			localY = -(m_totalLength + Spacing + height/2);
		}

		objTrans.localPosition = new Vector3(0, localY, -0.1f);
		objTrans.localScale = itemScale;


		

		m_totalObjCounts++;
        m_objectList.Add(obj);


		if(m_totalObjCounts == 1)
		{
			m_totalLength += height;
		}
		else 
		{
			m_totalLength += (height + Spacing);
		}

	}

    public void ClearAll()
    {
        foreach(GameObject obj in m_objectList)
        {
            Destroy(obj);
        }
        m_objectList.Clear();
        m_totalLength = 0;
        m_totalObjCounts = 0;
    }


}
