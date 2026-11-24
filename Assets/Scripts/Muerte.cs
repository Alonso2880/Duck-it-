using UnityEngine;
using TMPro;
public class Muerte : MonoBehaviour
{
    [Header("Canvas")]
    public GameObject canvasMuerte;
    public GameObject canvasMuerteNormal;
    public GameObject CanvasMuerteRecord;
    public GameObject canvasMuerteAnuncio;

    public CanvasMuerte can;

    [Header("Textos distancia")]
    public TMP_Text textoDistanciaConseguida;
    public TMP_Text textoDistanciaRecord;
    public TMP_Text textoDistanciaRecordNormal;

    public DistanciaMonedas DM;

    [Header("Ranking")]
    public EnviarRanking enviar;

    public RankingUIManager rankingUI;

    [Header("Control del pato")]
    public ControladorPato control;


    public void Death()
    {
        // Bloquear inmediatamente cualquier entrada
        control.BloquearEntrada();

        // Detener completamente el movimiento
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Desactivar el controlador
        control.enabled = false;

        Time.timeScale = 0f;

        float probabilidad = Random.value;

        DM.ActualizarMonedas();
        enviar.SubmitScore(DM.distancia);
        rankingUI.UpdateRanking();

        if (HaHechoRecord())
        {
            textoDistanciaRecord.text = DM.distancia.ToString();
            CanvasMuerteRecord.SetActive(true);

        }
        else
        {
            textoDistanciaConseguida.text = DM.distancia.ToString();
            textoDistanciaRecordNormal.text = DM.recordDistancia.ToString();
            canvasMuerteNormal.SetActive(true);
        }

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

    public bool HaHechoRecord()
    {
        if(DM.distancia >= DM.recordDistancia)
        {
            return true;
        }
        return false;
    }
}
