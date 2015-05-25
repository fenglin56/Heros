using UnityEngine;
using System.Collections;

namespace UI
{

    public class CreatObjectToNGUI : MonoBehaviour
    {

        public static GameObject InstantiateObj(GameObject instantiateObj, Transform ParentTransform)
        {
            if (instantiateObj == null)
                return null;
            GameObject CreatObject = (GameObject)Instantiate(instantiateObj);
            CreatObject.transform.parent = ParentTransform;
            CreatObject.transform.localPosition = instantiateObj.transform.localPosition;
            CreatObject.transform.localRotation = instantiateObj.transform.localRotation;
            CreatObject.transform.localScale = instantiateObj.transform.localScale;
            return CreatObject;  
        }

        public static GameObject InstantiateObjInSameParent(GameObject instantiateObj, Transform Objtransform)
        {
            return InstantiateObj(instantiateObj, Objtransform.parent);
        }

    }
}