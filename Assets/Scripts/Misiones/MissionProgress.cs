using UnityEngine;
using System;

[Serializable]
public class MissionProgress
{
    public string missionID;

    public float currentProgress;

    public bool completed;

    public bool claimed;

    // Para la misión de realizar los tres tipos de salto
    public bool simpleJumpDone;
    public bool mediumJumpDone;
    public bool maxJumpDone;

    public MissionProgress(string id)
    {
        missionID = id;
        currentProgress = 0;
        completed = false;
        claimed = false;
    }
}
