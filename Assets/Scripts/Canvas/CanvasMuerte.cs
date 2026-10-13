using TMPro;
using UnityEngine;

public class CanvasMuerte : MonoBehaviour
{
    [Header("Spawn")]
    public GameObject Spawn;

    [Header("Referencias")]
    public GameObject DM;
    public MapaInfinito mapin;
    public GameObject pato;

    [Header("Canvas")]
    public GameObject CanvIn;
    public GameObject CanvMuerte;

    [Header("Textos")]
    [SerializeField] private TMP_Text textoMonedas;
    [SerializeField] private TMP_Text textoDistancia;
    public void reintentar()
    {
        Time.timeScale = 1f;

        pato.transform.position = Spawn.transform.position;

        Detectar detectar = pato.GetComponent<Detectar>();
        ControladorPato cont = pato.GetComponent<ControladorPato>();

        detectar.ResetearDistancia();
        CanvMuerte.gameObject.SetActive(false); //Apago el canvas

        cont.saltoActual = 0;
        mapin.ReiniciarMapa();
    }

    public void Menu()
    {
        DistanciaMonedas dm = DM.GetComponent<DistanciaMonedas>();

        CanvMuerte.gameObject.SetActive(false); //Apago el canvas
        CanvIn.gameObject.SetActive(true);
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
