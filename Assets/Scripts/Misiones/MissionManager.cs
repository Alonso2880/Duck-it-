
using System;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;

    [Header("Mission Configuration")]
    [SerializeField] private List<MissionData> allMissions;
    [SerializeField] private int maxActiveMissions = 3;

    [Header("Referencias a otros sistemas")]
    public DistanciaMonedas distanciaMonedas;
    public Detectar detectar;
    public ControladorPato controladorPato;

    [Header("Active Missions")]
    [SerializeField] private List<string> activeMissionIDs = new List<string>();
    [SerializeField]
    private List<MissionProgress> missionProgress =
        new List<MissionProgress>();

    private List<string> claimedMissionIDs = new List<string>();

    // Últimos valores registrados para detectar incrementos.
    private float lastDistance;
    private int lastCoins;

    // Control de boosters durante la partida.
    private bool boosterUsedOrCollected = false;

    public event Action<int> OnRewardClaimed;
    public event Action OnMissionUpdated;

    #region INITIALIZATION

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        InitializeMissions();

        ResetMetricTracking();
    }

    private void Update()
    {
        UpdateDistance();
        UpdateCoins();

        // Referencia al tiempo de Slowmo aquí.
    }

    #endregion

    #region MISSION MANAGEMENT

    private void InitializeMissions()
    {
        activeMissionIDs.Clear();
        missionProgress.Clear();
        claimedMissionIDs.Clear();

        for (int i = 0; i < maxActiveMissions; i++)
        {
            AddRandomMission();
        }
    }

    private void AddRandomMission()
    {
        List<MissionData> availableMissions =
            new List<MissionData>();

        foreach (MissionData mission in allMissions)
        {
            if (mission == null)
                continue;

            if (activeMissionIDs.Contains(mission.missionID))
                continue;

            if (claimedMissionIDs.Contains(mission.missionID))
                continue;

            availableMissions.Add(mission);
        }

        // Si se han completado todas las misiones,
        // comienza un nuevo ciclo de selección.
        if (availableMissions.Count == 0)
        {
            claimedMissionIDs.Clear();

            foreach (MissionData mission in allMissions)
            {
                if (mission == null)
                    continue;

                if (!activeMissionIDs.Contains(mission.missionID))
                    availableMissions.Add(mission);
            }
        }

        if (availableMissions.Count == 0)
        {
            Debug.LogWarning("No hay misiones disponibles.");
            return;
        }

        int randomIndex = UnityEngine.Random.Range(
            0, availableMissions.Count);

        MissionData selectedMission =
            availableMissions[randomIndex];

        activeMissionIDs.Add(selectedMission.missionID);

        missionProgress.Add(
            new MissionProgress(selectedMission.missionID)
        );

        OnMissionUpdated?.Invoke();
    }

    public MissionData GetMissionData(string missionID)
    {
        return allMissions.Find(
            mission => mission != null &&
            mission.missionID == missionID
        );
    }

    public MissionProgress GetMissionProgress(string missionID)
    {
        return missionProgress.Find(
            progress => progress.missionID == missionID
        );
    }

    public List<MissionProgress> GetActiveMissions()
    {
        return missionProgress;
    }

    #endregion

    #region PROGRESS MANAGEMENT

    private void AddProgress(MissionProgress progress, float amount)
    {
        if (progress == null || progress.completed)
            return;

        MissionData data = GetMissionData(progress.missionID);

        if (data == null)
            return;

        progress.currentProgress += amount;

        CheckMissionCompletion(progress, data);
    }

    private void SetProgress(MissionProgress progress, float amount)
    {
        if (progress == null || progress.completed)
            return;

        MissionData data = GetMissionData(progress.missionID);

        if (data == null)
            return;

        progress.currentProgress = amount;

        CheckMissionCompletion(progress, data);
    }

    private void CheckMissionCompletion(
        MissionProgress progress,
        MissionData data)
    {
        if (progress.currentProgress >= data.targetValue)
        {
            progress.currentProgress = data.targetValue;
            progress.completed = true;

            Debug.Log("Misión completada: " + data.description);
        }

        OnMissionUpdated?.Invoke();
    }

    #endregion

    #region DISTANCE

    private void UpdateDistance()
    {
        if (distanciaMonedas == null)
            return;

        float currentDistance = distanciaMonedas.distancia;

        float difference = currentDistance - lastDistance;

        // Detectar si la distancia se ha reiniciado.
        if (difference < 0)
        {
            lastDistance = currentDistance;
            return;
        }

        if (difference > 0)
        {
            AddDistance(difference);
        }

        lastDistance = currentDistance;
    }

    private void AddDistance(float meters)
    {
        foreach (MissionProgress progress in missionProgress)
        {
            MissionData data = GetMissionData(progress.missionID);

            if (data == null || progress.completed)
                continue;

            switch (data.missionType)
            {
                case MissionType.DistanceSingle:
                case MissionType.DistanceTotal:

                    AddProgress(progress, meters);
                    break;

                case MissionType.DistanceWithoutBoosters:

                    if (!boosterUsedOrCollected)
                        AddProgress(progress, meters);

                    break;
            }
        }
    }

    #endregion

    #region COINS

    private void UpdateCoins()
    {
        if (distanciaMonedas == null)
            return;

        int currentCoins = distanciaMonedas.monedas;

        int difference = currentCoins - lastCoins;

        // Detectar si las monedas se han reiniciado.
        if (difference < 0)
        {
            lastCoins = currentCoins;
            return;
        }

        if (difference > 0)
        {
            bool boosterActive = detectar != null &&
                detectar.HayPotenciadorActivo;

            NotifyCoinCollected(boosterActive, difference);
        }

        lastCoins = currentCoins;
    }

    private void NotifyCoinCollected(bool boosterActive, int amount)
    {
        foreach (MissionProgress progress in missionProgress)
        {
            MissionData data = GetMissionData(progress.missionID);

            if (data == null || progress.completed)
                continue;

            switch (data.missionType)
            {
                case MissionType.CoinsTotal:
                case MissionType.CoinsSingle:

                    AddProgress(progress, amount);
                    break;

                case MissionType.CoinsWithBooster:

                    if (boosterActive)
                        AddProgress(progress, amount);

                    break;
            }
        }
    }

    public void SetGeneralCoins(int totalCoins)
    {
        foreach (MissionProgress progress in missionProgress)
        {
            MissionData data = GetMissionData(progress.missionID);

            if (data == null || progress.completed)
                continue;

            if (data.missionType == MissionType.GeneralCoins)
            {
                if (totalCoins > progress.currentProgress)
                {
                    SetProgress(progress, totalCoins);
                }
            }
        }
    }

    #endregion

    #region JUMPS

    public void RegisterJump(int jumpType)
    {
        foreach (MissionProgress progress in missionProgress)
        {
            MissionData data = GetMissionData(progress.missionID);

            if (data == null || progress.completed)
                continue;

            switch (data.missionType)
            {
                case MissionType.SimpleJumpsTotal:

                    if (jumpType == 1)
                        AddProgress(progress, 1);

                    break;

                case MissionType.MediumJumpsSingle:

                    if (jumpType == 2)
                        AddProgress(progress, 1);

                    break;

                case MissionType.MaxJumpsSingle:

                    if (jumpType == 3)
                        AddProgress(progress, 1);

                    break;

                case MissionType.AllJumpTypesSingle:

                    if (jumpType == 1)
                        progress.simpleJumpDone = true;

                    if (jumpType == 2)
                        progress.mediumJumpDone = true;

                    if (jumpType == 3)
                        progress.maxJumpDone = true;

                    int jumpTypes = 0;

                    if (progress.simpleJumpDone)
                        jumpTypes++;

                    if (progress.mediumJumpDone)
                        jumpTypes++;

                    if (progress.maxJumpDone)
                        jumpTypes++;

                    SetProgress(progress, jumpTypes);

                    break;
            }
        }
    }

    #endregion

    #region BOOSTERS

    public void NotifyBoosterCollected()
    {
        boosterUsedOrCollected = true;

        foreach (MissionProgress progress in missionProgress)
        {
            MissionData data = GetMissionData(progress.missionID);

            if (data == null || progress.completed)
                continue;

            switch (data.missionType)
            {
                case MissionType.BoostersSingle:
                case MissionType.BoostersTotal:

                    AddProgress(progress, 1);
                    break;

                case MissionType.DistanceWithoutBoosters:

                    SetProgress(progress, 0);
                    break;
            }
        }
    }

    public void NotifyBoosterUsed()
    {
        boosterUsedOrCollected = true;

        foreach (MissionProgress progress in missionProgress)
        {
            MissionData data = GetMissionData(progress.missionID);

            if (data == null || progress.completed)
                continue;

            if (data.missionType == MissionType.DistanceWithoutBoosters)
            {
                SetProgress(progress, 0);
            }
        }
    }

    #endregion

    #region SLOWMO

    public void AddSlowmoTime(float deltaTime)
    {
        foreach (MissionProgress progress in missionProgress)
        {
            MissionData data = GetMissionData(progress.missionID);

            if (data == null || progress.completed)
                continue;

            if (data.missionType == MissionType.SlowmoTimeSingle)
            {
                AddProgress(progress, deltaTime);
            }
        }
    }

    #endregion

    #region CLAIM REWARD

    public bool ClaimMission(string missionID) //LLamar a este metodo así MissionManager.Instance?.SetGeneralCoins(dm.MonedasTotalPartida);
    {
        MissionProgress progress = GetMissionProgress(missionID);

        if (progress == null || !progress.completed || progress.claimed)
            return false;

        MissionData data = GetMissionData(missionID);

        if (data == null)
            return false;

        progress.claimed = true;

        claimedMissionIDs.Add(missionID);

        // Referencia a monedas aquí.
        OnRewardClaimed?.Invoke(data.rewardCoins);

        activeMissionIDs.Remove(missionID);
        missionProgress.Remove(progress);

        AddRandomMission();

        OnMissionUpdated?.Invoke();

        Debug.Log("Recompensa reclamada: " + data.rewardCoins);

        return true;
    }

    #endregion

    #region NEW RUN

    public void StartNewRun()
    {
        boosterUsedOrCollected = false;

        foreach (MissionProgress progress in missionProgress)
        {
            if (progress.completed)
                continue;

            MissionData data = GetMissionData(progress.missionID);

            if (data == null)
                continue;

            switch (data.missionType)
            {
                case MissionType.DistanceSingle:
                case MissionType.CoinsSingle:
                case MissionType.MediumJumpsSingle:
                case MissionType.MaxJumpsSingle:
                case MissionType.AllJumpTypesSingle:
                case MissionType.SlowmoTimeSingle:
                case MissionType.BoostersSingle:
                case MissionType.DistanceWithoutBoosters:
                case MissionType.CoinsWithBooster:

                    progress.currentProgress = 0;

                    progress.simpleJumpDone = false;
                    progress.mediumJumpDone = false;
                    progress.maxJumpDone = false;

                    break;
            }
        }

        ResetMetricTracking();

        OnMissionUpdated?.Invoke();
    }

    private void ResetMetricTracking()
    {
        if (distanciaMonedas != null)
        {
            lastDistance = distanciaMonedas.distancia;
            lastCoins = distanciaMonedas.monedas;
        }
        else
        {
            lastDistance = 0;
            lastCoins = 0;
        }
    }

    #endregion
}
