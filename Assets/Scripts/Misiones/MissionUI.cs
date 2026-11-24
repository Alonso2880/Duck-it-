using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionUI : MonoBehaviour
{
    [Header("Mission Manager")]
    [SerializeField] private MissionManager missionManager;

    [Header("Mission Slot")]
    [SerializeField] private int slotIndex;

    [Header("Mission Objects")]
    [SerializeField] private GameObject normalMission;
    [SerializeField] private GameObject completedMission;

    [Header("Normal Mission UI")]
    [SerializeField] private TMP_Text missionNameText;

    // Aquí puedes asignar los dos textos que muestran las monedas.
    [SerializeField] private TMP_Text[] normalRewardTexts;

    [SerializeField] private Image progressFill;

    [Header("Completed Mission UI")]
    [SerializeField] private TMP_Text completedRewardText;
    [SerializeField] private TMP_Text countdownText;

    [Header("Buttons")]
    [SerializeField] private Button changeMissionButton;
    [SerializeField] private Button claimRewardButton;

    private string currentMissionID;

    private float countdownRefreshTimer;

    private void OnEnable()
    {
        Subscribe();
        RefreshUI();
    }

    private void Start()
    {
        Subscribe();
        RefreshUI();
    }

    private void OnDisable()
    {
        if (missionManager != null)
            missionManager.OnMissionUpdated -= RefreshUI;
    }

    private void Subscribe()
    {
        if (missionManager == null)
            missionManager = MissionManager.Instance;

        if (missionManager != null)
        {
            missionManager.OnMissionUpdated -= RefreshUI;
            missionManager.OnMissionUpdated += RefreshUI;
        }
    }

    private void Update()
    {
        // Actualizar la cuenta atrás cada segundo.
        countdownRefreshTimer += Time.unscaledDeltaTime;

        if (countdownRefreshTimer >= 1f)
        {
            countdownRefreshTimer = 0f;
            RefreshCountdown();
        }
    }

    public void RefreshUI()
    {
        if (missionManager == null)
            return;

        var missions = missionManager.GetActiveMissions();

        if (slotIndex < 0 || slotIndex >= missions.Count)
        {
            normalMission.SetActive(false);
            completedMission.SetActive(false);
            currentMissionID = null;
            return;
        }

        MissionProgress progress = missions[slotIndex];

        currentMissionID = progress.missionID;

        MissionData data =
            missionManager.GetMissionData(currentMissionID);

        if (data == null)
            return;

        // Actualizar el nombre.
        missionNameText.text = data.description;

        // Actualizar los textos de las monedas.
        foreach (TMP_Text rewardText in normalRewardTexts)
        {
            if (rewardText != null)
                rewardText.text = data.rewardCoins.ToString();
        }

        completedRewardText.text = data.rewardCoins.ToString();

        // Actualizar la barra de progreso.
        if (progressFill != null)
        {
            float percentage = data.targetValue > 0
                ? progress.currentProgress / data.targetValue
                : 0f;

            progressFill.fillAmount =
                Mathf.Clamp01(percentage);
        }

        // Mostrar una vista u otra.
        normalMission.SetActive(!progress.completed);
        completedMission.SetActive(progress.completed);

        // El botón de cambiar solo funciona si no está completada.
        if (changeMissionButton != null)
            changeMissionButton.interactable = !progress.completed;

        // La recompensa solo se puede reclamar una vez.
        if (claimRewardButton != null)
        {
            claimRewardButton.gameObject.SetActive(
                progress.completed && !progress.claimed
            );
        }

        if (countdownText != null)
            countdownText.gameObject.SetActive(progress.claimed);

        RefreshCountdown();
    }

    private void RefreshCountdown()
    {
        if (missionManager == null ||
            string.IsNullOrEmpty(currentMissionID) ||
            countdownText == null)
            return;

        MissionProgress progress =
            missionManager.GetMissionProgress(currentMissionID);

        if (progress == null || !progress.completed)
            return;

        long remaining =
            missionManager.GetRemainingSeconds(currentMissionID);

        if (remaining > 0)
        {
            TimeSpan time = TimeSpan.FromSeconds(remaining);

            countdownText.text = time.ToString(@"mm\:ss");
        }
        else if (!progress.claimed)
        {

        }
    }

    // Asignar al botón de cambiar misión.
    public void ChangeMission()
    {
        if (missionManager == null ||
            string.IsNullOrEmpty(currentMissionID))
            return;

        missionManager.ChangeMission(currentMissionID);
    }

    // Asignar al botón de reclamar recompensa.
    public void ClaimReward()
    {
        if (missionManager == null ||
            string.IsNullOrEmpty(currentMissionID))
            return;

        missionManager.ClaimMission(currentMissionID);

        RefreshUI();
    }
}
