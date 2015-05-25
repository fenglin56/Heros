using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class AnimationMapData
{
    public string GameState;
    public string PlayerStatus;
    public string Clip;
    public GameManager.GameState State
    {
        get
        {
            GameManager.GameState state;
            switch (GameState.ToLower())
            {
                case "town":
                    state = GameManager.GameState.GAME_STATE_TOWN;
                    break;
                case "battle":
                    state = GameManager.GameState.GAME_STATE_BATTLE;
                    break;
                default:
                    state = GameManager.GameState.INTRO;
                    break;
            }
            return state;
        }
    }
    public PlayerActionStatus ActionStatus
    {
        get
        {
            PlayerActionStatus actionStatus;
            switch (PlayerStatus.ToLower())
            {
                case "idle":
                    actionStatus = PlayerActionStatus.Idle;
                    break;
                case "running":
                    actionStatus = PlayerActionStatus.Running;
                    break;
                case "attacking":
                    actionStatus = PlayerActionStatus.Attacking;
                    break;
                default:
                    actionStatus = PlayerActionStatus.None;
                    break;
            }
            return actionStatus;
        }
    }
}
public class AnimationMapDataBase : ScriptableObject
{
    public AnimationMapData[] AnimationMapDatas;
}