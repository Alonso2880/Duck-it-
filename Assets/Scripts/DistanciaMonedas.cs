using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

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

    [SerializeField] private TMP_Text textoMonedasGameOver;


    [Header("Textos distancia")]
    [SerializeField] private TMP_Text textoDistancia;

    [Header("Texto nombre")]
    [SerializeField] private TMP_Text textonombreMenu;
    [SerializeField] private TMP_Text textoNombre;

    public void ActualizarMonedas()
    {
        if (textoMonedasMainMenu != null)
            textoMonedasMainMenu.text = monedas.ToString();

        if (textoMonedasShop != null)
            textoMonedasShop.text = monedas.ToString();

        if (textoMonedasMissions != null)
            textoMonedasMissions.text = monedas.ToString();

        if (textoMonedasGameOver != null)
            textoMonedasGameOver.text = monedas.ToString();
    }

    public void ActualizarDistancia()
    {
        if (textoDistancia != null)
            textoDistancia.text = distancia.ToString("F1");
    }

    public void ActualizarNombre()
    {
        string nombre = textoNombre.text;
        textonombreMenu.text = nombre;
    }

    private void Start()
    {
        ActualizarNombre();
    }
}
