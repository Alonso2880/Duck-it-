using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

public class RankingManager : MonoBehaviour
{
    private const string URL =
        "https://script.google.com/macros/s/AKfycbxsoL9FaTBBnKsS8M8jPsjcIQcL24s5miAjnZ6Z4zzmFZ-I1V09RhC1a0ePLNC5Z7_f1A/exec";

    [Serializable]
    public class RankingEntry
    {
        public string name;
        public float score;
    }

    [Serializable]
    public class RankingResponse
    {
        public bool success;
        public List<RankingEntry> ranking;
        public string message;
    }

    // ENVIAR PUNTUACIÓN
    public void SaveScore(string playerName, float score)
    {
        StartCoroutine(SaveScoreCoroutine(playerName, score));
    }

    private IEnumerator SaveScoreCoroutine(
        string playerName, float score)
    {
        WWWForm form = new WWWForm();

        form.AddField("name", playerName);
        form.AddField("score",score.ToString(System.Globalization.CultureInfo.InvariantCulture)
);

        using (UnityWebRequest request =
            UnityWebRequest.Post(URL, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Respuesta: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error al guardar: " + request.error);
            }
        }
    }

    // DESCARGAR RANKING
    public void LoadRanking(
        Action<List<RankingEntry>> onComplete)
    {
        StartCoroutine(LoadRankingCoroutine(onComplete));
    }

    private IEnumerator LoadRankingCoroutine(
        Action<List<RankingEntry>> onComplete)
    {
        using (UnityWebRequest request =
            UnityWebRequest.Get(URL + "?action=ranking"))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                RankingResponse response =
                    JsonUtility.FromJson<RankingResponse>(
                        request.downloadHandler.text
                    );

                if (response != null && response.success)
                {
                    onComplete?.Invoke(response.ranking);
                }
                else
                {
                    Debug.LogError("Error al obtener el ranking");
                    onComplete?.Invoke(null);
                }
            }
            else
            {
                Debug.LogError("Error de conexión: " + request.error);
                onComplete?.Invoke(null);
            }
        }
    }
}
