using TMPro;
using UnityEngine;

public class DistanciaMonedas : MonoBehaviour
{
    [Header("Metricas")]
    public int monedas;
    public float distancia;

    [Header("Referencias")]
    [SerializeField] private TMP_Text textoMonedas;
    [SerializeField] private TMP_Text textoDistancia;

    public void ActualizarMonedas()
    {
        if (textoMonedas != null)
            textoMonedas.text = "Coins: " + monedas;
    }

    public void ActualizarDistancia()
    {
        if (textoDistancia != null)
            textoDistancia.text = distancia.ToString("F1") + " m";
    }
}
