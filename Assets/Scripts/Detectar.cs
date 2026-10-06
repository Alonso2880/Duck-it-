using Unity.VisualScripting;
using UnityEngine;

public class Detectar : MonoBehaviour
{
    [Header("Distancia & Monedas")]
    public GameObject DM;

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

        }
    }

}
