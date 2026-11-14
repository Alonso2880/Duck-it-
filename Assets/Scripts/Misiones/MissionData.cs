using UnityEngine;

[CreateAssetMenu(fileName = "NewMission", menuName = "Missions/Mission")]
public class MissionData : ScriptableObject
{
    [Header("Mission Information")]
    public string missionID;
    [TextArea(2, 4)]
    public string description;

    [Header("Mission Settings")]
    public MissionType missionType;

    public float targetValue;

    [Header("Reward")]
    public int rewardCoins;
}
