using TMPro;
using UnityEngine;

public class CanvasMuerte : MonoBehaviour
{
    [Header("Spawn")]
    public GameObject Spawn;

    [Header("Distancia & Monedas")]
    public GameObject DM;

    [Header("CanvasInicio")]
    public GameObject CanvIn;

    [Header("Textos")]
    [SerializeField] private TMP_Text textoMonedas;
    [SerializeField] private TMP_Text textoDistancia;
    public void reintentar()
    {
        Time.timeScale = 1f;

        transform.position = Spawn.transform.position;

        Detectar detectar = GetComponent<Detectar>();
        detectar.ResetearDistancia();
        this.gameObject.SetActive(false); //Apago el canvas
    }

    public void Menu()
    {
        DistanciaMonedas dm = DM.GetComponent<DistanciaMonedas>();

        this.gameObject.SetActive(false); //Apago el canvas
        CanvIn.SetActive(true);
        textoMonedas.text = dm.monedas.ToString();
        textoDistancia.text = dm.distancia.ToString();
    }

    public void monedas()
    {
        DistanciaMonedas dm = DM.GetComponent<DistanciaMonedas>();
        if (dm.monedas >= 10)
        {
            dm.monedas -= 10;
            reintentar();
        }
    }
}
