
using System.Collections.Generic;
using UnityEngine;

public class MapaInfinito : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;

    // El mapa que siempre permanece en la escena.
    public GameObject mapaInicial;

    public GameObject mapA;
    public GameObject mapB;

    [Header("Configuracion")]
    public float segmentLength = 100f;

    private GameObject currentSegment;
    private GameObject nextSegment;

    private bool nextIsB = true;

    private Vector3 posicionInicialMapa;
    private Quaternion rotacionInicialMapa;

    // Guardamos todos los tramos creados por este script.
    private List<GameObject> tramosGenerados =
        new List<GameObject>();

    void Start()
    {
        if (player == null || mapaInicial == null ||
            mapA == null || mapB == null)
        {
            Debug.LogError(
                "MapaInfinito: faltan referencias en el Inspector."
            );
            enabled = false;
            return;
        }

        if (segmentLength <= 0f)
        {
            Debug.LogError(
                "MapaInfinito: la longitud debe ser mayor que cero."
            );
            enabled = false;
            return;
        }

        // Guardamos el mapa permanente.
        posicionInicialMapa = mapaInicial.transform.position;
        rotacionInicialMapa = mapaInicial.transform.rotation;

        currentSegment = mapaInicial;

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
            // Nunca destruimos el mapa inicial.
            if (currentSegment != mapaInicial)
            {
                tramosGenerados.Remove(currentSegment);
                Destroy(currentSegment);
            }

            currentSegment = nextSegment;
            nextSegment = null;

            nextIsB = !nextIsB;

            CrearSiguienteTramo();
        }
    }

    void CrearSiguienteTramo()
    {
        GameObject prefab = nextIsB ? mapB : mapA;

        Vector3 posicion =
            currentSegment.transform.position
            + Vector3.right * segmentLength;

        nextSegment = Instantiate(
            prefab,
            posicion,
            currentSegment.transform.rotation
        );

        tramosGenerados.Add(nextSegment);
    }

    public void ReiniciarMapa()
    {
        // Destruir todos los tramos generados durante la partida.
        foreach (GameObject tramo in tramosGenerados)
        {
            if (tramo != null)
                Destroy(tramo);
        }

        tramosGenerados.Clear();

        // Restaurar el mapa inicial, que nunca se destruye.
        mapaInicial.transform.SetPositionAndRotation(
            posicionInicialMapa,
            rotacionInicialMapa
        );

        currentSegment = mapaInicial;
        nextSegment = null;

        // Reiniciar la secuencia A/B.
        nextIsB = true;

        // Crear únicamente el siguiente tramo.
        CrearSiguienteTramo();
    }
}

