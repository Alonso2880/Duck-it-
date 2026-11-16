
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class RankingUIManager : MonoBehaviour
{
    [Header("Ranking Manager")]
    public RankingManager rankingManager;

    [Header("PRIMER PUESTO")]
    public TMP_Text firstNameText;
    public TMP_Text firstScoreText;

    [Header("SEGUNDO PUESTO")]
    public TMP_Text secondNameText;
    public TMP_Text secondScoreText;

    [Header("TERCER PUESTO")]
    public TMP_Text thirdNameText;
    public TMP_Text thirdScoreText;

    [Header("JUGADOR ACTUAL")]
    public TMP_Text playerPositionText;
    public TMP_Text playerNameText;
    public TMP_Text playerScoreText;

    private void Start()
    {
        UpdateRanking();
    }

    public void UpdateRanking()
    {
        rankingManager.LoadRanking(DisplayRanking);
    }

    private void DisplayRanking(List<RankingManager.RankingEntry> ranking)
    {
        if (ranking == null || ranking.Count == 0)
        {
            Debug.LogError("No se pudo cargar el ranking.");

            return;
        }

        // PRIMER PUESTO
        SetPlayerData(
            ranking, 0,
            firstNameText,
            firstScoreText
        );

        // SEGUNDO PUESTO
        SetPlayerData(
            ranking, 1,
            secondNameText,
            secondScoreText
        );

        // TERCER PUESTO
        SetPlayerData(
            ranking, 2,
            thirdNameText,
            thirdScoreText
        );

        // BUSCAR AL JUGADOR ACTUAL
        string currentName = rankingManager.CurrentPlayerName;

        int playerIndex = -1;

        if (!string.IsNullOrWhiteSpace(currentName))
        {
            playerIndex = ranking.FindIndex(p =>
                p != null &&
                !string.IsNullOrWhiteSpace(p.name) &&
                p.name.Trim().Equals(
                    currentName.Trim(),
                    System.StringComparison.OrdinalIgnoreCase
                )
            );
        }
    }

    private void SetPlayerData(
        List<RankingManager.RankingEntry> ranking,
        int index,
        TMP_Text nameText,
        TMP_Text scoreText)
    {
        if (index < ranking.Count)
        {
            nameText.text = ranking[index].name;
            scoreText.text = ranking[index].score.ToString("0.##");
        }
        else
        {
            nameText.text = "---";
            scoreText.text = "0";
        }
    }
}

