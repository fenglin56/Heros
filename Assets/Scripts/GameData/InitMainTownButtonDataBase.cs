using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UI.MainUI;

    [Serializable]
    public class InitMainTownButtonData
    {
        public int ButtonProgress;
        public string AppearButton;
        [HideInDataReaderAttribute]
        public UIType[] MainButtonList;

        public string AppearFunction;
        [HideInDataReaderAttribute]
        public PackBtnType[] PackBtnTypeList;
    }

    public class InitMainTownButtonDataBase : ConfigBase
    {
        public InitMainTownButtonData[] Datas;

        public override void Init(int length, object dataList)
        {
            Datas = new InitMainTownButtonData[length];

            var realData = dataList as List<InitMainTownButtonData>;
            for (int i = 0; i < realData.Count; i++)
            {
                Datas[i] = (InitMainTownButtonData)realData[i];
                var appearButton = Datas[i].AppearButton;
                if (appearButton != "0")
                {
                    var btnIDs = appearButton.Split('+');
                    Datas[i].MainButtonList = new UIType[btnIDs.Length];
                    for(int j=0;j<btnIDs.Length;j++)
                    {
                        Datas[i].MainButtonList [j]=(UIType)int.Parse(btnIDs[j]);
                    }
                }
            var appearFunction=Datas[i].AppearFunction;
            if(appearFunction!="0")
            {
                var btnIDs = appearFunction.Split('+');
                Datas[i].PackBtnTypeList = new PackBtnType[btnIDs.Length];
                for(int j=0;j<btnIDs.Length;j++)
                {
                    Datas[i].PackBtnTypeList [j]=(PackBtnType)int.Parse(btnIDs[j]);
                }
            }
            }
        }
    }
