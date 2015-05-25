using UnityEngine;
using System.Collections;
using System.Linq;

namespace UI.MainUI{

	public class SingleFashionBtn : MonoBehaviour {

		public GameObject NewIconPrefab;
		public GameObject EquiptIconPrefab;
		public Transform StatusPos;
		public Transform NamePos;
		public UISprite HeardIcon;
		public UILabel LevelLabel;
		public SpriteSwith BackgroundSpriteSwith;
		SpriteSwith NameSpriteSwith;

		public FashionListPanel_V3 MyParent{get;private set;}
		public ItemData MyItemData{get;private set;}
		
		Color enableColor = new Color(128/255f,40/255f,0);
		Color disableColor = new Color(236/255f,224/255f,158/255f);

		public void Show(ItemData itemData,FashionListPanel_V3 myParent)
		{
			MyItemData = itemData;
			MyParent = myParent;
			StatusPos.ClearChild();
			NamePos.ClearChild();
			string[] fashionstr = CommonDefineManager.Instance.CommonDefine.HeroIcon_Fashion.Split('|');
			var m_HeroDataModel = PlayerManager.Instance.FindHeroDataModel();
			int vocationID = m_HeroDataModel.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
			foreach(var child in fashionstr)
			{
				string[] childStr = child.Split('+');
				int fashionID = int.Parse(childStr[1]);
				if(int.Parse(childStr[0])== vocationID)
				{
					if(fashionID==itemData._goodID||(fashionID == 0&&itemData._goodID == myParent.MyParent.GetAllFashionDatas()[0]._goodID))
					{
						HeardIcon.spriteName = childStr[2];
					}
				}
			}
			if(myParent.MyParent.IsNewItem(itemData))//isNew
			{
				StatusPos.InstantiateNGUIObj(NewIconPrefab);
			}else if((EquipmentData)itemData == myParent.MyParent.CurrentFashiondata)//IsEquipt
			{
				StatusPos.InstantiateNGUIObj(EquiptIconPrefab);
			}
			NameSpriteSwith = NamePos.InstantiateNGUIObj(itemData.DisplayBig_prefab).GetComponent<SpriteSwith>();//NamePrefab
			NameSpriteSwith.target.depth = 3; 
			LevelLabel.SetText(string.Format("Lv.{0}",itemData._Level));

            //引导代码 
            gameObject.RegisterBtnMappingId(itemData._goodID, UIType.Package, BtnMapId_Sub.Package_FashionPanel_V3_SingleFashionBtn);
		}

		void OnClick()
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Avatar_Change");
			MyParent.OnMyBtnClick(MyItemData);
		}

		public void SetMySelectStatus(ItemData selectData)
		{
			bool flag = selectData!=null&&selectData == this.MyItemData;
			NameSpriteSwith.ChangeSprite(flag?2:1);
			BackgroundSpriteSwith.ChangeSprite(flag?2:1);
			LevelLabel.color = flag?enableColor:disableColor;
		}

	}
}