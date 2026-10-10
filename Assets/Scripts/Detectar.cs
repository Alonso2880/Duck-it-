using Unity.VisualScripting;
using UnityEngine;

public class Detectar : MonoBehaviour
{
    [Header("Distancia & Monedas")]
    public GameObject DM;
    [SerializeField] private Transform jugador;

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
        desplazamiento.y = 0f;
        dm.distancia += desplazamiento.magnitude;

        posicionAnterior = jugador.position;
        dm.ActualizarDistancia();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Si colisiona con un obtaculo
        if (collision.gameObject.tag == "Obstaculo")
        {
            Muerte muerte = GetComponent<Muerte>();
            muerte.Death();
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
    }


    public void ResetearDistancia()
    {
        DistanciaMonedas dm = DM.GetComponent<DistanciaMonedas>();

        dm.distancia = 0f;
        posicionAnterior = jugador.position;

        dm.ActualizarDistancia();
    }


}
