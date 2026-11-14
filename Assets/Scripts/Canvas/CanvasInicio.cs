using TMPro;
using UnityEngine;

public class CanvasInicio : MonoBehaviour
{
    [Header("Referencias a otros sistemas")]
    public GameObject DM;
    public GameObject CI;
    public ControladorPato control;

    public CanvasMuerte canvasMuerte;

    [Header("Audio")]
    public AudioSource audioClick;
    public AudioSource audioMenu;
    public AudioSource audioJuego;

    [Header("Distancia")]
    public DistanciaMonedas distancia;

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
        control.enabled = false;

        control.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        control.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        Time.timeScale = 1f;
        CI.SetActive(false);
        DM.SetActive(true);

        canvasMuerte.StartCoroutine(canvasMuerte.CountDown(control));

        distancia.distancia = 0;

        MissionManager.Instance?.StartNewRun();
    }
}
