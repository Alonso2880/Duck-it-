using UnityEngine;
using TMPro;

public class EnviarRanking : MonoBehaviour
{
    public TMP_Text textoNombre; //Referencia al nombre
    public RankingManager rankingManager; //Referencia al ranking manager


    public void SubmitScore(float score)
    {
        if (textoNombre == null)
        {
            return;
        }
        else
        {
            Debug.Log("hola");
;            string playerName = textoNombre.text;
            rankingManager.SaveScore(playerName, score);
        }
    }

    public void ShowRanking()
    {
        rankingManager.LoadRanking(entries =>
        {
            if (entries == null)
                return;

            foreach (var player in entries)
            {
                Debug.Log(
                    player.name + " - " + player.score
                );
            }
        });
    }
}
