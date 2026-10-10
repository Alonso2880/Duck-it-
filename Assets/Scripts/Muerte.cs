using UnityEngine;

public class Muerte : MonoBehaviour
{
    [Header("Spawn")]
    public GameObject Spawn;

    [Header("Distancia & Monedas")]
    public GameObject DM;
    public void Death()
    {
        transform.position = Spawn.transform.position;

        Detectar detectar = GetComponent<Detectar>();
        detectar.ResetearDistancia();
    }
}
