using UnityEngine;
using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>
/// 城镇剧情圣诞框，
/// </summary>
public class StoryDialogBehaviour : MonoBehaviour {

    public UILabel NpcName;
    public UILabel DialogContent;
    public UILabel ContinueTips;
	public GameObject ContinueIcon;
    public GameObject NpcIconPoint;
    public UIEventListener ClientEvent;
    //public TweenScale TopScaleTween;
    //public TweenScale DownScaleTween;

    public Action StoryGuideFinishAct;
    private TalkIdConfigData m_talkIdConfigData;
    private string[] talkTexts;   //对白段
    private int m_talkTextIndex;  //对白段索引
    private bool m_typingFinish;  //是否本段对白呈现完毕
      

    void Awake()
    {        
        ContinueTips.text = LanguageTextManager.GetString("IDS_I16_1");
    }

	public void Init(TalkIdConfigData talkIdConfigData)
	{
		Init (talkIdConfigData,true);
	}
    public void Init(TalkIdConfigData talkIdConfigData,bool hasTips)
    {
		ContinueIcon.SetActive (hasTips);
		ContinueTips.gameObject.SetActive (hasTips);
        m_talkIdConfigData = talkIdConfigData;
		if (talkIdConfigData.isTaskTalkMark) {
			string[] talkStrArry = m_talkIdConfigData.TalkText.Split ('|');
			int vocattion = PlayerManager.Instance.FindHeroDataModel ().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
			foreach (string str in talkStrArry) {
				string[] talkStr = str.Split ('+');
				if (vocattion == int.Parse (talkStr [0])) {
					talkTexts = new string[talkStr.Length-1];
					for(int i = 1 ; i < talkStr.Length; i++)
					{
						talkTexts[i-1] = talkStr[i];
					}
					break;
				}
			}
		} else {
			talkTexts = m_talkIdConfigData.TalkText.Split('+');
		}
		m_talkTextIndex = 0;
        FillTackContent(m_talkIdConfigData .TalkType);

		AddClientEvent ();
    }
	private void AddClientEvent()
	{
		ClientEvent.onClick = (obj) =>
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Story_Click");
			if (m_typingFinish)
			{
				m_talkTextIndex++;
				if (m_talkTextIndex < talkTexts.Length)
				{
					TalkContentData content = new TalkContentData();
//					string[] talkStrArr = talkTexts[m_talkTextIndex].Split('+');
//					int playerProfession = int.Parse(talkStrArr[0]);
//					switch(playerProfession)
//					{
//
//					}
					content.Init(LanguageTextManager.GetString(talkTexts[m_talkTextIndex]));
					StartCoroutine(TypingTalkContent(content));
				}
				else
				{
					if(StoryGuideFinishAct!=null)
					{
						StoryGuideFinishAct();
					}
				}
			}
			else
			{
				m_typingFinish = true;
			}
		};
	}
    /// <summary>
    /// Fill dialog box conetent
    /// </summary>
    /// <param name="storyTallType"></param>
    private void FillTackContent(StoryTallType storyTallType)
    {
        string titleName = string.Empty, dialogContent = string.Empty;
        GameObject icon=null;
        switch (storyTallType)
        {
            case StoryTallType.Player:  //玩家,看对话框类型               
                var playerDataStruct = (SMsgPropCreateEntity_SC_MainPlayer)PlayerManager.Instance.FindHeroEntityModel().EntityDataStruct;
                titleName = playerDataStruct.Name;
                dialogContent = LanguageTextManager.GetString(m_talkIdConfigData.TalkText);
                var fashionId=playerDataStruct.GetCommonValue().PLAYER_FIELD_VISIBLE_FASHION;
                var vocationId=playerDataStruct.GetPlayerKind();
                var npcTackData=CommonDefineManager.Instance.CommonDefine.HeroIcon_NPCTalk.SingleOrDefault(P=>P.FashionID==fashionId&&P.VocationID==vocationId);
                if(npcTackData!=null)
                {
                    icon = npcTackData.TalkHead;
                }
                break;
            case StoryTallType.NPC:  //NPC
                titleName = LanguageTextManager.GetString(m_talkIdConfigData.NPCName);
                dialogContent = LanguageTextManager.GetString(m_talkIdConfigData.TalkText);
                icon = m_talkIdConfigData.TalkHead;
                break;
        }
        switch (m_talkIdConfigData.DialogPrefab)
        {
            case DialogBoxType.Right:
            case DialogBoxType.Left:
                NpcName.text = titleName;
                break;
            case DialogBoxType.RightWithIcon:
            case DialogBoxType.LeftWithIcon:
                NpcName.text = titleName;
                if (icon != null)
                {
                    var pic=NGUITools.AddChild(NpcIconPoint, icon);
                    pic.transform.localScale = icon.transform.localScale;
                }
                break;
        }
        if (talkTexts.Length > 0)
        {
            TalkContentData content = new TalkContentData();
            content.Init(LanguageTextManager.GetString(talkTexts[m_talkTextIndex]));
            StartCoroutine(TypingTalkContent(content));
        }
    }
    /// <summary>
    /// NPC内容打字呈现
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    private IEnumerator TypingTalkContent(TalkContentData content)
    {
        m_typingFinish = false;
        var waitingTime=new WaitForSeconds(1f/CommonDefineManager.Instance.CommonDefine.DialogSpeed);
        int length = content.FormatString.Length;
        int subLength=0;
        while (!m_typingFinish)
        {
            DialogContent.text = content.GetTalContent(++subLength);
            yield return waitingTime;
            if (subLength >= length)
            {
                m_typingFinish = true;
            }
        }
		DialogContent.text = content.GetTalContent (length);
    }
}

public struct TalkContentData
{
    public Dictionary<int, Match[]> MatchsDic;
    public string[] FormatString;

    public void Init(string sourceStr)
    {
        string regEx = @"\[[0-9a-f]{6}\]|(\[-\])";
        MatchsDic = new Dictionary<int, Match[]>();
        var cols = Regex.Matches(sourceStr, regEx);
        if (cols != null && cols.Count > 0)
        {
            for (int i = 0; i < cols.Count; i = i + 2)
            {
                MatchsDic.Add(i, new Match[] { cols[i], cols[i + 1] });
            }
        }
        var tempStr = Regex.Replace(sourceStr, regEx, "&");
        FormatString = new string[tempStr.Length];
        int j = 0, m = 0;

        bool isClose = true;
        for (int i = 0; i < tempStr.Length; i++)
        {
            string singleStr = tempStr.Substring(i, 1);
            if (singleStr == "&")
            {
                isClose = !isClose;
                if (isClose)
                {
                    j += 2;
                }
                continue;
            }
            if (!isClose)
            {
                FormatString[m++] = MatchsDic[j][0].Value + singleStr + MatchsDic[j][1].Value;
            }
            else
            {
                FormatString[m++] = singleStr;
            }
        }
    }
    public string GetTalContent(int subLength)
    {
        string backValue = string.Empty;
        for (int i = 0; i < subLength; i++)
        {
            if (!string.IsNullOrEmpty(FormatString[i]))
            {
                backValue += FormatString[i];
            }
        }
        return backValue;
    }
}