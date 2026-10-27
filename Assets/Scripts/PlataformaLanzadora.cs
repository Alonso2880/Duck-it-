using System.Collections;
using UnityEngine;

public class PlataformaLanzadora : MonoBehaviour
{
    public enum ModoLanzamiento
    {
        HaciaArriba,
        HaciaAbajo
    }

    [Header("Referencias")]
    public Transform puntoLanzamiento;
    public GameObject prefabProyectil;

    [Header("Configuración")]
    public ModoLanzamiento modo = ModoLanzamiento.HaciaArriba;
    public float alturaObjetivo = 3f;
    public float velocidadSubida = 8f;

    private Coroutine rutinaLanzamiento;

    public void LanzarProyectil()
    {
        if (puntoLanzamiento == null || prefabProyectil == null)
            return;

        GameObject proyectil = Instantiate(prefabProyectil,puntoLanzamiento.position, puntoLanzamiento.rotation
        );

        Rigidbody rb = proyectil.GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("El prefab del proyectil necesita un Rigidbody.");
            Destroy(proyectil);
            return;
        }

        rb.useGravity = true;

        // Modo de lanzamiento hacia arriba.
        if (modo == ModoLanzamiento.HaciaArriba)
        {
            float alturaRestante =
                alturaObjetivo - proyectil.transform.position.y;

            if (alturaRestante <= 0f)
            {
                Debug.LogWarning(
                    "El proyectil ya está en la altura objetivo o por encima."
                );
                Destroy(proyectil);
                return;
            }

            float gravedad = Physics.gravity.y;
            float velocidadNecesaria =
                Mathf.Sqrt(-2f * gravedad * alturaRestante);

            rb.linearVelocity = new Vector3( 0f,velocidadNecesaria,0f);
        }
        // Modo de lanzamiento hacia abajo.
        else if (modo == ModoLanzamiento.HaciaAbajo)
        {
            rb.linearVelocity = new Vector3(0f, -velocidadSubida,0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (rutinaLanzamiento == null)
            {
                rutinaLanzamiento = StartCoroutine(
                    LanzarPeriodicamente()
                );
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (rutinaLanzamiento != null)
            {
                StopCoroutine(rutinaLanzamiento);
                rutinaLanzamiento = null;
            }
        }
    }

    private IEnumerator LanzarPeriodicamente()
    {
        while (true)
        {
            LanzarProyectil();
            yield return new WaitForSeconds(3f);
        }
    }
}
