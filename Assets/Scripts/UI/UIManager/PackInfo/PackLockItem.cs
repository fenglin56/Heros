using UnityEngine;
using System.Collections;


namespace UI.MainUI{

	public class PackLockItem : MonoBehaviour {

		public GameObject UnLockEffectPrefab;

		public SingleItemLineArea MyParent{get;private set;}

		private GameObject UnlockEffectObj;

		public void Init(SingleItemLineArea myParent)
		{
			this.MyParent = myParent;
		}


		void OnClick()
		{

		}

	}
}