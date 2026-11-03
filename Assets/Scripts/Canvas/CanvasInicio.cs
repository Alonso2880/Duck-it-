using UnityEngine;

public class CanvasInicio : MonoBehaviour
{
    public GameObject DM;
    public GameObject CI;

    [Header("Audio")]
    public AudioSource audioClick;
    public AudioSource audioMenu;
    public AudioSource audioJuego;

    void Start()
    {
        Time.timeScale = 0f; 
    }


    public void EmpezarJuego()
    {
        audioClick.Play();
        audioMenu.Stop();
        audioJuego.Play();

        Time.timeScale = 1f;
        CI.SetActive(false);
        DM.SetActive(true);
    }
}
