using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Detectar : MonoBehaviour
{
    [Header("Distancia & Monedas")]
    public GameObject DM;
    [SerializeField] private Transform jugador;

    [Header("Controlador pato")]
    public ControladorPato control;

    [Header("Efectos pato")]
    public GameObject Splash;

    [Header("Checkpoints")]

    public CheckPoints check;

    public int boostAct = 0; //0-ningun0, 1-boost doble puntuacion, 2-boost obstaculos
    private Vector3 posicionAnterior;

    [Header("Audio")]
    public AudioSource audioChocarte, audioCaerte;

    void Start()
    {
        if (jugador != null)
        {
            posicionAnterior = jugador.position;
        }
    }

    void Update()
    {
        DistanciaMonedas dm = DM.GetComponent<DistanciaMonedas>();
        if (jugador == null)
            return;

        // Calculamos el movimiento desde el ultimo fotograma
        Vector3 desplazamiento = jugador.position - posicionAnterior;

        // Contabilizar tiempo de Slowmo.
        if (control != null && control.BoostLentoActivo)
        {
            MissionManager.Instance?.AddSlowmoTime(Time.deltaTime);
        }

        // Contamos solo el movimiento horizontal (X y Z)
        if (boostAct != 1)
        {
            desplazamiento.y = 0f;
            dm.distancia += desplazamiento.magnitude;

            posicionAnterior = jugador.position;
            dm.ActualizarDistancia();
        }
        else
        {
            desplazamiento.y = 0f;
            dm.distancia += desplazamiento.magnitude * 2;
            posicionAnterior = jugador.position;
            dm.ActualizarDistancia();
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        //Si colisiona con una moneda
        if (other.gameObject.tag == "Coin")
        {
            DistanciaMonedas dm = DM.GetComponent<DistanciaMonedas>();
            
            dm.monedas++;
            dm.MonedasTotalPartida++;
            dm.ActualizarMonedas();

            StartCoroutine(ActivarAnimaciones(other.gameObject));

            MissionManager.Instance?.SetGeneralCoins(dm.MonedasTotalPartida);

        }

        //Si colisiona con un obtaculo
        if (other.gameObject.tag == "Obstaculo" && boostAct != 2)
        {
            audioChocarte.Play();
            Muerte muerte = GetComponent<Muerte>();
            control.entradaBloqueada = true;
            muerte.Death();
        }

        if(other.gameObject.tag == "Boost1")
        {
            boostAct = 1;
            MissionManager.Instance?.NotifyBoosterCollected();
            StartCoroutine(ActivarAnimaciones(other.gameObject));
            StartCoroutine(boostTiempo());
        }

        if (other.gameObject.tag == "Boost2")
        {
            boostAct = 2;
            MissionManager.Instance?.NotifyBoosterCollected();
            StartCoroutine(ActivarAnimaciones(other.gameObject));
            StartCoroutine(boostTiempo());
        }

        if (other.gameObject.CompareTag("Boost3"))
        {
            MissionManager.Instance?.NotifyBoosterCollected();

            boostAct = 3;
            ControladorPato controlador = GetComponent<ControladorPato>();

            controlador.ActivarBoostLento();

            StartCoroutine(ActivarAnimaciones(other.gameObject));
        }

        //Si colisiona con el borde
        if (other.gameObject.tag == "Border")
        {
            Debug.Log("Borde detectado");
            audioCaerte.Play();
            Muerte muerte = GetComponent<Muerte>();
            control.entradaBloqueada = true;

            muerte.Death();
        }

        //Si colisiona con splash
        if(other.gameObject.tag == "Splash")
        {
            //Activo el efecto splash
            Splash.SetActive(true);
            Animator animatorSplash = Splash.GetComponent<Animator>();
            animatorSplash.SetBool("Death", true);
        }

        if(other.gameObject.tag == "CheckPoint")
        {
            check.RegistrarCheckPoint(other.gameObject);
        }
    }


    public void ResetearDistancia()
    {
        DistanciaMonedas dm = DM.GetComponent<DistanciaMonedas>();

        dm.distancia = 0f;
        posicionAnterior = jugador.position;

        dm.ActualizarDistancia();

        MissionManager.Instance?.StartNewRun();
    }

    IEnumerator boostTiempo()
    {
        yield return new WaitForSeconds(3f);
        boostAct = 0;
        yield return null;
    }

    public bool HayPotenciadorActivo
    {
        get
        {
            return boostAct == 1 ||
                   boostAct == 2 ||
                   (control != null && control.BoostLentoActivo);
        }
    }

    IEnumerator ActivarAnimaciones(GameObject other)
    {
        GameObject objetoAnimado = other.transform.GetChild(0).gameObject; //Accedo al hijo 1
        GameObject particulas = other.transform.GetChild(1).gameObject; //Accedo al objeto particulas
        Animator animatorObjeto = other.GetComponent<Animator>(); //Animator del objeto principal
        Animator animatorParticula = particulas.GetComponent<Animator>(); //Animator particulas

        // Guardar escala original
        Vector3 escalaOriginal = objetoAnimado.transform.localScale;

        particulas.SetActive(true);

        animatorObjeto.SetBool("Cogido", true);
        animatorParticula.SetBool("Cogido", true);

        yield return new WaitForSeconds(1.5f);

        // Restaurar escala del objeto animado
        objetoAnimado.transform.localScale = escalaOriginal;

        other.gameObject.GetComponent<ObjetosReiniciables>().Recoger();

        particulas.SetActive(false);
        animatorObjeto.SetBool("Cogido", false);
        animatorParticula.SetBool("Cogido", false);

        particulas = null;
        animatorObjeto = null;
        animatorParticula = null;
    }
}
