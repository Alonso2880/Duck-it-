using System.Collections;
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

    public CheckPoints check;

    [Header("Canvas")]
    public GameObject CanvIn;
    public GameObject CanvMuerte;

    [Header("Textos")]
    [SerializeField] private TMP_Text textoMonedas;
    [SerializeField] private TMP_Text textoDistancia;

    //Funcion que permite reintentar desde el punto de partida
    public void reintentar()
    {
        Time.timeScale = 1f;

        pato.transform.position = Spawn.transform.position;

        Detectar detectar = pato.GetComponent<Detectar>();
        ControladorPato cont = pato.GetComponent<ControladorPato>();

        detectar.ResetearDistancia();
        CanvMuerte.gameObject.SetActive(false); //Apago el canvas

        cont.ReiniciarVelocidad();
        cont.saltoActual = 0;
        mapin.ReiniciarMapa();
    }

    //Funcion que vuelve al menu
    public void Menu()
    {
        DistanciaMonedas dm = DM.GetComponent<DistanciaMonedas>();
        ControladorPato cont = pato.GetComponent<ControladorPato>();

        pato.transform.position = Spawn.transform.position; //Reseteo la posicion del pato
        cont.saltoActual = 0; //Reseteo los saltos del pato
        mapin.ReiniciarMapa(); //Reseteo el mapa

        CanvMuerte.gameObject.SetActive(false); //Apago el canvas
        CanvIn.gameObject.SetActive(true);
        textoMonedas.text = dm.monedas.ToString();
        textoDistancia.text = dm.distancia.ToString();
    }

    //Funcion que permite revivir tras el ultimo checkpoint si tienes 10 monedas

    public void Revivir()
    {
        //Referencias 
        Detectar detectar = pato.GetComponent<Detectar>();
        ControladorPato cont = pato.GetComponent<ControladorPato>();
        Rigidbody rb = pato.GetComponent<Rigidbody>();

        GameObject UltimoCheckPoint = check.UltimoCheck; //Obtenemos la referencia del ultimo checkpoint

        //Detener completamente el movimiento
        cont.BloquearEntrada();
        cont.enabled = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        //Teletransportar al ultimo checkpoint
        pato.transform.position = UltimoCheckPoint.transform.position; //Cambiamos la posicion del pato al del checkpoint

        // Reiniciar la física después del teletransporte
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        //Reseteos de la distancia y de los saltos
        detectar.ResetearDistancia();
        cont.saltoActual = 0;

        //Ocultar canvas muerte
        CanvMuerte.gameObject.SetActive(false);

        Time.timeScale = 1f;
        StartCoroutine(CountDown(cont));
    }

    IEnumerator CountDown(ControladorPato cont)
    {
        cont.enabled = false;
   
        yield return new WaitForSeconds(1f);
        Debug.Log("1");

        yield return new WaitForSeconds(1f);
        Debug.Log("2");
        yield return new WaitForSeconds(1f);
        Debug.Log("3");
        yield return new WaitForSeconds(1f);
        cont.enabled = true;
        yield return null;
    }

    public void monedas()
    {
        DistanciaMonedas dm = DM.GetComponent<DistanciaMonedas>();
        if (dm.monedas >= 0)
        {
            dm.monedas -= 0;
            Revivir();
        }
    }
}
