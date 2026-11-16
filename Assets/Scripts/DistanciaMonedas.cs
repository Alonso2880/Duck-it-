using TMPro;
using UnityEngine;

public class DistanciaMonedas : MonoBehaviour
{
    [Header("Metricas")]
    public int monedas;
    public float distancia;

    public int MonedasTotalPartida = 0;

    [HideInInspector] public float recordDistancia = 0;

    [Header("Textos Monedas")]
    [SerializeField] private TMP_Text textoMonedasMainMenu;
    [SerializeField] private TMP_Text textoMonedasShop;
    [SerializeField] private TMP_Text textoMonedasMissions;


    [Header("Textos distancia")]
    [SerializeField] private TMP_Text textoDistancia;

    public void ActualizarMonedas()
    {
        if (textoMonedasMainMenu != null)
            textoMonedasMainMenu.text = "Coins: " + monedas;

        if (textoMonedasShop != null)
            textoMonedasShop.text = "Coins: " + monedas;

        if (textoMonedasMissions != null)
            textoMonedasMissions.text = "Coins: " + monedas;
    }

    public void ActualizarDistancia()
    {
        if (textoDistancia != null)
            textoDistancia.text = distancia.ToString("F1") + " m";
    }
}
