using UnityEngine;

public class Muerte : MonoBehaviour
{
    [Header("Canvas")]
    public GameObject canvasMuerte;
    public GameObject canvasMuerteAnuncio;

    public CanvasMuerte can;


    public void Death()
    {
        Time.timeScale = 0f;

        float probabilidad = Random.value;

        if(probabilidad <= 0.25f)
        {
            canvasMuerteAnuncio.SetActive(true);
            can.IniciarCountdown();
        }
        else
        {
            canvasMuerte.SetActive(true);
        }

    }
}
