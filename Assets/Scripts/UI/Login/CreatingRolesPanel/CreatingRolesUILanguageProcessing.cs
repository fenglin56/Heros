using UnityEngine;
using System.Collections;

namespace UI.Login
{

    public class CreatingRolesUILanguageProcessing : MonoBehaviour
    {

        public UILabel JoinGameButton;
        public UILabel TitleLable;
        public UILabel NameEditorLable;

        void Start()
        {
            JoinGameButton.text = LoginDataManager.Instance.GetLanguageTextData("IDS_H2_21");
            TitleLable.text = LoginDataManager.Instance.GetLanguageTextData("IDS_H2_21");
            NameEditorLable.text = LoginDataManager.Instance.GetLanguageTextData("IDS_H1_41");
        }

    }

}