using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Detectar : MonoBehaviour
{
    [Header("Distancia & Monedas")]
    public GameObject DM;
    [SerializeField] private Transform jugador;

    public int boostAct = 0; //0-ningun0, 1-boost doble puntuacion, 2-boost obstaculos
    private Vector3 posicionAnterior;

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

        // Contamos solo el movimiento horizontal (X y Z)
        if(boostAct != 1)
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
            Destroy(other.gameObject);
            dm.monedas++;
            dm.ActualizarMonedas();

        }

        //Si colisiona con un obtaculo
        if (other.gameObject.tag == "Obstaculo" && boostAct != 2)
        {
            Muerte muerte = GetComponent<Muerte>();
            muerte.Death();
        }

        if(other.gameObject.tag == "Boost1")
        {
            boostAct = 1;
            Destroy(other.gameObject);
            StartCoroutine(boostTiempo());
        }

        if (other.gameObject.tag == "Boost2")
        {
            boostAct = 2;
            Destroy(other.gameObject);
            StartCoroutine(boostTiempo());
        }

        //Si colisiona con un obtaculo
        if (other.gameObject.tag == "Border")
        {
            Muerte muerte = GetComponent<Muerte>();
            muerte.Death();
        }
    }


    public void ResetearDistancia()
    {
        DistanciaMonedas dm = DM.GetComponent<DistanciaMonedas>();

        dm.distancia = 0f;
        posicionAnterior = jugador.position;

        dm.ActualizarDistancia();
    }

    IEnumerator boostTiempo()
    {
        yield return new WaitForSeconds(3f);
        boostAct = 0;
        yield return null;
    }


}
