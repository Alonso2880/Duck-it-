using UnityEngine;

public class CanvasInicio : MonoBehaviour
{
    public GameObject DM;
    public GameObject CI;

    [Header("Audio")]
    public AudioSource audioClick;

    void Start()
    {
        Time.timeScale = 0f; 
    }


    public void EmpezarJuego()
    {
        audioClick.Play();
        Time.timeScale = 1f;
        CI.SetActive(false);
        DM.SetActive(true);
    }
}
