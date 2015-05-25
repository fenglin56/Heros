using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Runtime.InteropServices;
using UI.MainUI;
using System.Reflection;
using System.Collections;

public static class CommonTools
{
    /// <summary>
    /// list中是否包含某元素（c#自带的在ios上会报错）
    /// </summary>
    /// <returns><c>true</c>, if contains was localed, <c>false</c> otherwise.</returns>
    /// <param name="list">List.</param>
    /// <param name="item">Item.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public static bool LocalContains<T>(this IList<T> list,T item)
    {
        bool contain=false;
        foreach(var listItem in list )
        {
            if(listItem.Equals(item))
            {
                contain=true;
                break;
            }
        }
        return contain;
    }
    public static bool RecursiveGetComponent<T>(this Transform sourceGameObject, string componentName, out List<PlayDataStruct<T>> components) where T : Component
    {
        components = new List<PlayDataStruct<T>>();
        GetComponent(componentName, sourceGameObject, ref components);

        return components.Count > 0;
    }
    public static bool RemoveComponent<T>(this GameObject sourceGameObject, string componentName) where T : Component
    {
        var com = sourceGameObject.GetComponent(componentName);
        if (com != null)
        {
            GameObject.Destroy(com);
            return true;
        }
        return false;
    }   
    private static void GetComponent<T>(string componentName, Transform parent, ref List<PlayDataStruct<T>> components) where T : Component
    {
        foreach (Transform child in parent)
        {
            //if (componentName == "ParticleSystem")
            //{
            //    TraceUtil.Log(child.name + "  componentName  " + parent.name);
            //}
            var com = child.GetComponent(componentName);
            if (com != null)
            {
                components.Add(new PlayDataStruct<T>() { AnimComponent = (T)com, LoopTimes = 1 });
            }
            GetComponent(componentName, child, ref components);
        }
    }
    public static bool RecursiveFindObject(this Transform sourceGameObject, string targetName,out Transform findResult)
    {
        findResult= FindObject(sourceGameObject, targetName);

        return findResult == null ? false : true;
    }
    public static void ClearChild(this Transform sourceGameObject)
    {
        var childCount = sourceGameObject.childCount;
        if (childCount > 0)
        {
            for (int i = childCount-1; i >=0; i--)
            {
                UnityEngine.Object.Destroy(sourceGameObject.GetChild(i).gameObject);
            }
        }
    }
	public static void ClearChildImmediate(this Transform sourceGameObject)
	{
		var childCount = sourceGameObject.childCount;
		if (childCount > 0)
		{
			for (int i = childCount-1; i >=0; i--)
			{
				UnityEngine.Object.DestroyImmediate(sourceGameObject.GetChild(i).gameObject);
			}
		}
	}
    public static void ClearChild(this Transform sourceGameObject, string childName)
    {
        //Debug.Log(sourceGameObject.name+":"+childName);
        var childCount = sourceGameObject.childCount;
        if (childCount > 0)
        {
            for (int i = childCount - 1; i >= 0; i--)
            {
                var child= sourceGameObject.GetChild(i);
                if(child==null)
                {
                    Debug.Log("child=null");
                }
                if (child.name == childName)
                {
                    UnityEngine.Object.Destroy(child.gameObject);
                }
            }
        }
    }
    public static string GetAttachment(this AttachPoint attachPoint)
    {
        string parentName = string.Empty;
        switch (attachPoint)
        {
            case AttachPoint.LBWeapon:
                parentName = ConstDefineManager.LBWeaponPos;
                break;
            case AttachPoint.LHWeapon:
                parentName = ConstDefineManager.LHWeaponPos;
                break;
            case AttachPoint.RBWeapon:
                parentName = ConstDefineManager.RBWeaponPos;
                break;
            case AttachPoint.RHWeapon:
                parentName = ConstDefineManager.RHWeaponPos;
                break;
        }

		return parentName;
    }
    public static bool RecursiveObjectExist(this Transform sourceGameObject, string targetName)
    {
        var findResult = FindObject(sourceGameObject, targetName);

        return findResult == null ? false : true;
    }
    public static void ApplyAllItem<T>(this IEnumerable<T> collection, Action<T> action)
    {
        foreach (var t in collection)
        {
            action(t);
        }
    }
//    public static void AddChatRecord<T>(this List<T> collection, T t)
//    {
//        if (collection.Count >= 100)
//        {
//            collection.RemoveAt(0);
//        }
//        collection.Add(t);
//    }
	public static void AddChatRecord<SMsgChat_SC>(this List<SMsgChat_SC> collection, SMsgChat_SC t)
	{
		if (collection.Count >= 100)
		{
			collection.RemoveAt(0);
		}
		collection.Add(t);
	}


    /// <summary>
    /// 此函数仅给NGUIButton使用
    /// </summary>
    /// <param name="?"></param>
    /// <param name="flag"></param>
    //public static void SetButtonStatus(this GameObject go, bool flag)
    //{
    //    if (go != null)
    //    {
    //        go.GetComponent<BoxCollider>().enabled = flag;
    //        GuideButtonEvent guideButtonEvent = go.GetComponent<GuideButtonEvent>();
    //        if (guideButtonEvent != null)
    //        {
    //            guideButtonEvent.SetActHandler(flag);
    //        }
    //    }

    //    //if (go.GetComponent<UIImageButton>())
    //    //    go.GetComponent<UIImageButton>().enabled = flag;
    //}
	public static int GetStructSize<T>() where T: struct
	{
		return Marshal.SizeOf(typeof(T));
	}
    private static Transform FindObject(Transform parent, string targetName)
    {
        Transform targetObject = null;
        foreach(Transform child in parent)
        {
            if(child.name==targetName)
            {
                targetObject= child;
                break;
            }
            else
            {
                targetObject= FindObject(child,targetName);
                if (targetObject != null)
                {
                    break;
                }
            }
        }
        return targetObject;
    }
    
	public static float AngleBetween2Vector(Vector3 from, Vector3 to)
    {
        from.y = 0;
        
		// Vector3.Angle 返回的角度值小于 180
		return Vector3.Angle(from, to);
    }
    /// <summary>
     /// 把服务器下发的坐标信息转成客户端向量
    /// </summary>
    /// <param name="V"></param>
    /// <param name="serverXValue"></param>
    /// <param name="xScale"></param>
    /// <param name="serverYValue"></param>
    /// <param name="yScale"></param>
    /// <returns></returns>
     public static Vector3 GetFromServer(this Vector3 V, float serverXValue, float xScale, float serverYValue, float yScale)
     {
         V.x = serverXValue * xScale;
         V.z = serverYValue * yScale;
         
         return V;
     }    
     public static Vector3 GetFromServer(this Vector3 V, float serverXValue, float serverYValue)
     {
         //xScale和yScale设为0.1 是因为服务器端单位为厘米，客户端为分米。yScale乘-1是因为服务器2D坐标的y轴对应客户端的-Z轴
         return GetFromServer(V, serverXValue, 0.1f, serverYValue, -0.1f);
     }
     public static void SetToServer(this Vector3 V, out float serverXValue, float xScale, out float serverYValue, float yScale)
     {
         serverXValue = V.x* xScale;
         serverYValue = V.z * yScale;
     }
     public static void SetToServer(this Vector3 V, out float serverXValue, out float serverYValue)
     {
         SetToServer(V, out serverXValue, 10f, out serverYValue, -10f);
     }


     /// <summary>
     /// 转换位置和角度
     /// </summary>
     /// <param name="heroPos">英雄位置</param>
     /// <param name="targetPos">目标位置</param>
     /// <param name="initPosX">配置位置x坐标点</param>
     /// <param name="initPosY">配置位置y坐标点</param>
     /// <param name="startPos">out初始位置</param>
     /// <param name="quaterionY">out角度</param>
     public static void TransformPosAndAngle(Vector3 heroPos, Vector3 targetPos, float initPosX, float initPosY, out Vector3 startPos, out float quaterionY)
     {

         float zVector = targetPos.z - heroPos.z;
         float xVector = targetPos.x - heroPos.x;
         float rad = Mathf.Atan2(zVector, xVector);
         Vector3 diretXVector = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)).normalized;
         Vector3 diretYVector = new Vector3(Mathf.Cos(rad - 90), 0, Mathf.Sin(rad - 90)).normalized;
         //发射起始点
         startPos = heroPos + diretXVector * initPosX + diretYVector * initPosY;
         //自身y轴角度
         quaterionY = 90 - rad * Mathf.Rad2Deg;
     }
     public static void TransformPosAndAngle(Transform heroTrans, float initPosX, float initPosY, out Vector3 startPos, out float quaterionY)
     {
         startPos = heroTrans.position + heroTrans.TransformDirection(initPosY, 0, initPosX);
         quaterionY = heroTrans.eulerAngles.y;
     }

     public static string SetColor(this string text, TextColorType colorLevel)
    {
        string returnTest = text;
        Color NameTextColor = Color.white;
        switch (colorLevel)//物品品质颜色
        {
            case TextColorType.ItemQuality0:
                returnTest = "[54e44f]" + text + "[-]";//绿307719
                break;
            case TextColorType.ItemQuality1:
                returnTest = "[67b9ff]" + text + "[-]"; //蓝00a2ff
                break;
            case TextColorType.ItemQuality2:
                returnTest = "[fc6cfa]" + text + "[-]";  //紫d64bfe
                break;
            case TextColorType.ItemQuality3:
                returnTest = "[ff9860]" + text + "[-]";  //金ffc600
                break;
            case TextColorType.Gray:
                returnTest = "[b7b7b7]" + text + "[-]";  //Gray
                break;
            case TextColorType.Pink:
                returnTest = "[84c7cf]" + text + "[-]";  //Pink
                break;
            case TextColorType.Yelow:
                returnTest = "[cfcc85]" + text + "[-]";  //Yelow
                break;
            case TextColorType.Red:
                returnTest = "[ff0000]" + text + "[-]";  //Red
                break;
			case TextColorType.EquipProperty:
				returnTest = "[fffa6f]" + text + "[-]";  //
				break;
            default:
                break;
        }

        return returnTest;
    }

	public static string GetGoodsSubClassName(this GoodsSubClass subClass)
	{
		string goodName=string.Empty;
		switch((GoodsSubClass)subClass)
		{
		case GoodsSubClass.Weapon:
			goodName="IDS_I3_12";
			break;
		case GoodsSubClass.Fashion:
			goodName="";
			break;
		case GoodsSubClass.Headwear:
			goodName="IDS_I3_14";
			break;
		case GoodsSubClass.Dress:
			goodName="IDS_I3_13";
			break;
		case GoodsSubClass.Boots:
			goodName="IDS_I3_15";
			break;
		case GoodsSubClass.Accessories:
			goodName="IDS_I3_16";
			break;
		case GoodsSubClass.Badge:
			goodName="";
			break;
		}
		return LanguageTextManager.GetString(goodName);
	}

     /// <summary>
     /// 返回 children transforms, 根据名称排列 .
     /// </summary>
     public static Transform[] GetChildTransforms( this GameObject parentGameObject)
     {
         if (parentGameObject != null)
         {
             List<Component> components = new List<Component>(parentGameObject.GetComponentsInChildren(typeof(Transform)));
             List<Transform> transforms = components.ConvertAll(c => (Transform)c);

             transforms.Remove(parentGameObject.transform);
             transforms.Sort(delegate(Transform a, Transform b)
             {
                 return a.name.CompareTo(b.name);
             });

             return transforms.ToArray();
         }
         return null;
     }
     //public static void RegisterBtnMappingId(this GameObject btnObj, UIType mainType, BtnMapId_Sub subType)
     //{
     //    RegisterBtnMappingId(btnObj,mainType,subType,true);
     //}
     public static void RegisterBtnMappingId(this GameObject btnObj, UIType mainType, BtnMapId_Sub subType)
     {
         if (btnObj != null)
         {
             var guideBtnBehaviour = btnObj.GetComponent<GuideBtnBehaviour>();
             if (guideBtnBehaviour == null)
             {
                 guideBtnBehaviour = btnObj.AddComponent<GuideBtnBehaviour>();
             }
             guideBtnBehaviour.RegisterBtnMappingId(mainType, subType);
         }

     }
	public static void RegisterBtnMappingId(this GameObject btnObj, UIType mainUiType , BtnMapId_Sub subBtnIdType, Action<float> noticeToDragSlerp, float itemAmount)
	{
		if (btnObj != null)
		{
			var guideBtnBehaviour = btnObj.GetComponent<GuideBtnBehaviour>();
			if (guideBtnBehaviour == null)
			{
				guideBtnBehaviour = btnObj.AddComponent<GuideBtnBehaviour>();
				
			}
			guideBtnBehaviour.RegisterBtnMappingId( mainUiType, subBtnIdType, noticeToDragSlerp, itemAmount);
		}
	}
     //public static void RegisterBtnMappingId(this GameObject btnObj, int mappingId, UIType mainUiType
     //    , BtnMapId_Sub subBtnIdType, Func<UIDraggablePanel, float, IEnumerator> dragAmountSlerpAct, UIDraggablePanel panel, float itemAmount)
     //{
     //    RegisterBtnMappingId(btnObj, mappingId, mainUiType, subBtnIdType, dragAmountSlerpAct, panel, itemAmount, true);
     //}
     public static void RegisterBtnMappingId(this GameObject btnObj, int mappingId, UIType mainUiType
         , BtnMapId_Sub subBtnIdType, Action<float> noticeToDragSlerp, float itemAmount)
     {
         if (btnObj != null)
         {
             var guideBtnBehaviour = btnObj.GetComponent<GuideBtnBehaviour>();
             if (guideBtnBehaviour == null)
             {
                 guideBtnBehaviour = btnObj.AddComponent<GuideBtnBehaviour>();
                 
             }
             guideBtnBehaviour.RegisterBtnMappingId(mappingId, mainUiType, subBtnIdType
                 , noticeToDragSlerp, itemAmount);
         }
     }
     //public static void RegisterBtnMappingId(this GameObject btnObj, int mappingId, UIType mainType, BtnMapId_Sub subType)
     //{
     //    RegisterBtnMappingId(btnObj, mappingId, mainType,subType,true);
     //}
     public static void RegisterBtnMappingId(this GameObject btnObj, int mappingId, UIType mainType, BtnMapId_Sub subType)
     {
         if (btnObj != null)
         {
             var guideBtnBehaviour = btnObj.GetComponent<GuideBtnBehaviour>();
             if (guideBtnBehaviour == null)
             {
                 guideBtnBehaviour = btnObj.AddComponent<GuideBtnBehaviour>();
             }
             guideBtnBehaviour.RegisterBtnMappingId(mappingId, mainType, subType);
         }
     }
	public static GameObject InstantiateNGUIObj(this Transform parent, GameObject prefab)
	{
		if(prefab==null||parent == null)
		{
			return null;
		}
		GameObject newObj = GameObject.Instantiate(prefab) as GameObject;
		newObj.transform.parent = parent;
		newObj.transform.localPosition = Vector3.zero;
		newObj.transform.localScale = prefab.transform.localScale;
		newObj.transform.localRotation = prefab.transform.localRotation;
		return newObj;
	}
    public static string ToDescription<T>(this T uiType)
    {
        string desc = string.Empty;
        Type type = uiType.GetType();
        FieldInfo info = type.GetField(uiType.ToString());
        if (Attribute.IsDefined(info, typeof(EnumDescAttribute)))
        {
            var cusAtts = Attribute.GetCustomAttribute(info, typeof(EnumDescAttribute)) as EnumDescAttribute;
            desc = cusAtts.Description;
        }
        return desc;
    }

}
public class PlayDataStruct<T>
{
    public T AnimComponent { get; set; }
    public float PlayTimeLength { get; set; }
    public float PlayingTime { get; set; }
    public int LoopTimes { get; set; }
    public int PlayedTimes { get; set; }
    public string ComponentName { get; set; }
}


