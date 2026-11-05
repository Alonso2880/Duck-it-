using UnityEngine;

public class CanvasInicio : MonoBehaviour
{
    [Header("Referencias a otros sistemas")]
    public GameObject DM;
    public GameObject CI;
    public ControladorPato control;

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

        control.entradaBloqueada = false;

        Time.timeScale = 1f;
        CI.SetActive(false);
        DM.SetActive(true);
    }
}
