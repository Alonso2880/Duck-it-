using UnityEngine;

public class MapaInfinito : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    public GameObject currentSegment;
    public GameObject mapA;
    public GameObject mapB;

    [Header("Configuracion")]
    public float segmentLength = 100f;

    private GameObject nextSegment;
    private bool nextIsB = true;

    private Vector3 posicionInicialMapa;
    private Quaternion rotacionInicialMapa;

    void Start()
    {
        if (player == null || currentSegment == null ||
            mapA == null || mapB == null)
        {
            Debug.LogError(
                "EndlessMap: faltan referencias en el Inspector."
            );
            enabled = false;
            return;
        }

        if (segmentLength <= 0f)
        {
            Debug.LogError(
                "EndlessMap: la longitud debe ser mayor que cero."
            );
            enabled = false;
            return;
        }

        posicionInicialMapa = currentSegment.transform.position;
        rotacionInicialMapa = currentSegment.transform.rotation;

        CrearSiguienteTramo();
    }

    void Update()
    {
        if (currentSegment == null || nextSegment == null)
            return;

        float finalActual =
            currentSegment.transform.position.x + segmentLength;

        if (player.position.x >= finalActual)
        {
            Destroy(currentSegment);
            currentSegment = nextSegment;

            nextIsB = !nextIsB;

            CrearSiguienteTramo();
        }
    }

    void CrearSiguienteTramo()
    {
        GameObject prefab = nextIsB ? mapB : mapA;

        Vector3 posicion = currentSegment.transform.position
                         + Vector3.right * segmentLength;

        nextSegment = Instantiate(
            prefab,
            posicion,
            currentSegment.transform.rotation
        );
    }

    public void ReiniciarMapa()
    {
        // Eliminar el tramo siguiente, si existe.
        if (nextSegment != null)
            Destroy(nextSegment);

        // Recuperar el tramo inicial si fue destruido.
        if (currentSegment == null)
        {
            currentSegment = Instantiate(
                mapA,
                posicionInicialMapa,
                rotacionInicialMapa
            );
        }
        else
        {
            // Restaurar su posición original.
            currentSegment.transform.SetPositionAndRotation(
                posicionInicialMapa,
                rotacionInicialMapa
            );
        }

        // Volver a empezar con el tramo B.
        nextIsB = true;

        // Crear el siguiente tramo junto al inicial.
        CrearSiguienteTramo();
    }
}
