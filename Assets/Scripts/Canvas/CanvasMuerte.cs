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

    public ControladorPato control;

    public CheckPoints check;

    [Header("Canvas")]
    public GameObject CanvIn;
    public GameObject CanvMuerte;

    [Header("Textos")]
    [SerializeField] private TMP_Text textoMonedas;
    [SerializeField] private TMP_Text textoDistancia;

    [SerializeField] private TMP_Text textoRecordDistancia;

    [Header("Audio")]
    public AudioSource audioClick;
    public AudioSource audioMenu;
    public AudioSource audioJuego;

    //Funcion que permite reintentar desde el punto de partida
    public void reintentar()
    {
        audioClick.Play();
        Time.timeScale = 1f;

        pato.transform.position = Spawn.transform.position;

        Detectar detectar = pato.GetComponent<Detectar>();
        ControladorPato cont = pato.GetComponent<ControladorPato>();

        control.entradaBloqueada = false;
        detectar.ResetearDistancia();
        CanvMuerte.gameObject.SetActive(false); //Apago el canvas

        cont.ReiniciarVelocidad();
        cont.saltoActual = 0;
        mapin.ReiniciarMapa();
    }

    //Funcion que vuelve al menu
    public void Menu()
    {
        audioClick.Play();
        audioJuego.Stop();
        audioMenu.Play();
        
        DistanciaMonedas dm = DM.GetComponent<DistanciaMonedas>();
        ControladorPato cont = pato.GetComponent<ControladorPato>();

        pato.transform.position = Spawn.transform.position; //Reseteo la posicion del pato
        cont.saltoActual = 0; //Reseteo los saltos del pato
        mapin.ReiniciarMapa(); //Reseteo el mapa

        CanvMuerte.gameObject.SetActive(false); //Apago el canvas
        CanvIn.gameObject.SetActive(true);
        textoMonedas.text = dm.monedas.ToString();
        textoDistancia.text = dm.distancia.ToString("F1") + " m";

        //Actualizo el record distancia
        if(dm.distancia > dm.recordDistancia)
        {
            dm.recordDistancia = dm.distancia;
            textoRecordDistancia.text = dm.recordDistancia.ToString("F1");
        }
    }

    //Funcion que permite revivir tras el ultimo checkpoint si tienes 10 monedas

    public void Revivir()
    {
        audioClick.Play();
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
        cont.saltoActual = 0;
        control.entradaBloqueada = false;

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
        if (dm.monedas >= 10)
        {
            dm.monedas -= 10;
            dm.ActualizarMonedas();
            Revivir();
        }
    }
}
