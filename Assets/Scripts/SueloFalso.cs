using UnityEngine;

public class SueloFalso : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("Pato Detectado");
        }
    }
}
