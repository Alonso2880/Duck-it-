using UnityEngine;

public class Muerte : MonoBehaviour
{
    [Header("Canvas")]
    public GameObject canvasMuerte;


    public void Death()
    {
        Time.timeScale = 0f;
        canvasMuerte.SetActive(true);

    }
}
