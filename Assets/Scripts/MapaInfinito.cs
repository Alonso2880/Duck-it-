
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

    [Header("Mapas fase 2 y 3")]
    public GameObject mapaFase2;
    public GameObject mapaFase3;

    private int fase = 1;

    [Header("Distancia")]
    [SerializeField] private DistanciaMonedas dist;
    public float DistanciaFase2 = 400f;
    public float DistnaciaFase3 = 800f;

    [Header("Configuracion")]
    public float segmentLength = 100f;

    private GameObject currentSegment;
    private GameObject nextSegment;

    private bool nextIsB = true;

    private Vector3 posicionInicialMapa;
    private Quaternion rotacionInicialMapa;

    // Guardamos todos los tramos creados por este script.
    private List<GameObject> tramosGenerados =new List<GameObject>();

    public ObjetosReiniciables[] objetosIniciales;
    private Transform grupoMonedas;
    private Transform grupoBoosts;

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

        objetosIniciales = mapaInicial.GetComponentsInChildren<ObjetosReiniciables>(true);
        grupoMonedas = mapaInicial.transform.Find("Monedas&Boost");
        grupoBoosts = mapaInicial.transform.Find("Boost");

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

        //Comprobamos la distancia para cambiar de fase
        if (dist.distancia >= DistanciaFase2 && dist.distancia < DistnaciaFase3)
            fase = 2;
        if (dist.distancia >= DistnaciaFase3)
            fase = 3;

    }

    void CrearSiguienteTramo()
    {
        GameObject prefab = null;

        switch (fase)
        {
            case 1:
                prefab = nextIsB ? mapB : mapA;
                break;

            case 2:
                prefab = mapaFase2;
                break;

            case 3:
                prefab = mapaFase3;
                break;
        }

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
        // Destruir todos los tramos generados.
        foreach (GameObject tramo in tramosGenerados)
        {
            if (tramo != null)
                Destroy(tramo);
        }

        fase = 1; //volvemos a fase 1

        tramosGenerados.Clear();

        // Restaurar el mapa inicial.
        mapaInicial.transform.SetPositionAndRotation(
            posicionInicialMapa,
            rotacionInicialMapa
        );

        // Activar los grupos completos.
        if (grupoMonedas != null)
            grupoMonedas.gameObject.SetActive(true);

        if (grupoBoosts != null)
            grupoBoosts.gameObject.SetActive(true);

        // Reactivar todas las monedas y boosts.
        foreach (ObjetosReiniciables objeto in objetosIniciales)
        {
                objeto.gameObject.SetActive(true);
        }

        currentSegment = mapaInicial;
        nextSegment = null;

        nextIsB = true;

        CrearSiguienteTramo();
    }

}

