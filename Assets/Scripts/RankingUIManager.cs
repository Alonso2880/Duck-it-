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
    public TMP_Text playerPositionTextMainMenu;
    public TMP_Text playerNameText;
    public TMP_Text playerScoreText;

    [Header("RANKING POPUP DE MUERTE")]
    public TMP_Text deathFirstNameText;
    public TMP_Text deathFirstScoreText;

    public TMP_Text deathSecondNameText;
    public TMP_Text deathSecondScoreText;

    public TMP_Text deathThirdNameText;
    public TMP_Text deathThirdScoreText;

    public TMP_Text deathPlayerPositionText;
    public TMP_Text deathPlayerNameText;
    public TMP_Text deathPlayerScoreText;

    private void OnEnable()
    {
        if (rankingManager != null)
            rankingManager.OnScoreSaved += UpdateRanking;
    }

    private void OnDisable()
    {
        if (rankingManager != null)
            rankingManager.OnScoreSaved -= UpdateRanking;
    }

    public void UpdateRanking()
    {
        rankingManager.LoadRanking(DisplayRanking);
    }

    private void DisplayRanking(List<RankingManager.RankingEntry> ranking)
    {
        if (ranking == null)
        {
            Debug.LogError("No se pudo cargar el ranking.");
            return;
        }

        Debug.Log("Ranking recibido: " + ranking.Count + " jugadores.");
        Debug.Log("Jugador actual: " + rankingManager.CurrentPlayerName);

        // PRIMER PUESTO
        SetPlayerData(ranking, 0, firstNameText, firstScoreText);

        // SEGUNDO PUESTO
        SetPlayerData(ranking, 1, secondNameText, secondScoreText);

        // TERCER PUESTO
        SetPlayerData(ranking, 2, thirdNameText, thirdScoreText);

        // BUSCAR JUGADOR ACTUAL
        string currentName = rankingManager.CurrentPlayerName;

        int playerIndex = -1;

        // RANKING POPUP DE MUERTE
        SetPlayerData(ranking, 0, deathFirstNameText, deathFirstScoreText);

        SetPlayerData(ranking, 1, deathSecondNameText, deathSecondScoreText);

        SetPlayerData(ranking, 2, deathThirdNameText, deathThirdScoreText);

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

        Debug.Log("Índice del jugador: " + playerIndex);

        // MOSTRAR DATOS DEL JUGADOR ACTUAL
        if (playerIndex != -1)
        {
            var player = ranking[playerIndex];

            playerPositionText.text = (playerIndex + 1) + "º";
            playerNameText.text = player.name;
            playerScoreText.text = player.score.ToString("0.##");

            deathPlayerPositionText.text = (playerIndex + 1) + "º";
            deathPlayerNameText.text = player.name;
            deathPlayerScoreText.text = player.score.ToString("0.##");

            playerPositionTextMainMenu.text = (playerIndex + 1) + "º";

            Debug.Log("Jugador mostrado: " + player.name);
        }
        else
        {
            playerPositionText.text = "-";
            playerNameText.text = currentName;
            playerScoreText.text = "0";

            deathPlayerPositionText.text = "-";
            deathPlayerNameText.text = currentName;
            deathPlayerScoreText.text = "0";

            playerPositionTextMainMenu.text = "-";

        }
    }

    private void SetPlayerData(
        List<RankingManager.RankingEntry> ranking,
        int index,
        TMP_Text nameText,
        TMP_Text scoreText)
    {
        if (index < ranking.Count && ranking[index] != null)
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

