using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class EntityModel
{
    public GameObject GO;
    public View Behaviour;
    public IEntityDataStruct EntityDataStruct;
	public string tipIDS;
    public void DestroyEntity()
    {
        UnityEngine.Object.Destroy(Behaviour);
        UnityEngine.Object.Destroy(GO);
    }
}
