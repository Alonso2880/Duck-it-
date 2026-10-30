
using UnityEngine;

public class TransicionPaneles : MonoBehaviour
{
    [Header("Contenedores")]
    [SerializeField] private RectTransform contenedorPrincipal;
    [SerializeField] private RectTransform contenedorDestino;

    [Header("Configuración")]
    [SerializeField, Min(0.05f)] private float duracion = 0.45f;

    private RectTransform canvas;
    private CanvasGroup grupoPrincipal;
    private CanvasGroup grupoDestino;

    private bool preparado;
    private bool enTransicion;
    private bool destinoAbierto;

    private void Start()
    {
        if (contenedorPrincipal == null || contenedorDestino == null)
        {
            Debug.LogError("Asigna los dos Empty en el Inspector.", this);
            return;
        }

        Canvas canvasPrincipal = contenedorPrincipal.GetComponentInParent<Canvas>();
        Canvas canvasDestino = contenedorDestino.GetComponentInParent<Canvas>();

        if (canvasPrincipal == null || canvasPrincipal != canvasDestino)
        {
            Debug.LogError("Los dos contenedores deben pertenecer al mismo Canvas.", this);
            return;
        }

        canvas = canvasPrincipal.transform as RectTransform;

        grupoPrincipal = PrepararGrupo(contenedorPrincipal);
        grupoDestino = PrepararGrupo(contenedorDestino);

        // Estado inicial
        contenedorPrincipal.gameObject.SetActive(true);
        contenedorDestino.gameObject.SetActive(false);

        Canvas.ForceUpdateCanvases();

        contenedorPrincipal.anchoredPosition = Vector2.zero;
        contenedorDestino.anchoredPosition =
            Vector2.down * canvas.rect.height;

        grupoPrincipal.interactable = true;
        grupoPrincipal.blocksRaycasts = true;

        preparado = true;
    }

    public void AbrirPanel()
    {
        CambiarPanel(true);
    }

    public void Volver()
    {
        CambiarPanel(false);
    }

    private void CambiarPanel(bool abrir)
    {
        if (!preparado || enTransicion || abrir == destinoAbierto)
            return;

        enTransicion = true;

        float altura = canvas.rect.height;
        float direccion = abrir ? 1f : -1f;

        RectTransform salida = abrir
            ? contenedorPrincipal
            : contenedorDestino;

        RectTransform entrada = abrir
            ? contenedorDestino
            : contenedorPrincipal;

        CanvasGroup grupoSalida = abrir
            ? grupoPrincipal
            : grupoDestino;

        CanvasGroup grupoEntrada = abrir
            ? grupoDestino
            : grupoPrincipal;

        // Activar el contenedor de entrada antes de animarlo.
        entrada.gameObject.SetActive(true);

        salida.anchoredPosition = Vector2.zero;
        entrada.anchoredPosition =
            Vector2.down * altura * direccion;

        grupoSalida.interactable = false;
        grupoSalida.blocksRaycasts = false;

        grupoEntrada.interactable = false;
        grupoEntrada.blocksRaycasts = false;

        LeanTween.moveY(salida, altura * direccion, duracion)
            .setEase(LeanTweenType.easeInOutCubic)
            .setIgnoreTimeScale(true);

        LeanTween.moveY(entrada, 0f, duracion)
            .setEase(LeanTweenType.easeInOutCubic)
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                // Desactivar el contenedor anterior.
                salida.gameObject.SetActive(false);

                entrada.anchoredPosition = Vector2.zero;

                destinoAbierto = abrir;

                grupoPrincipal.interactable = !abrir;
                grupoPrincipal.blocksRaycasts = !abrir;

                grupoDestino.interactable = abrir;
                grupoDestino.blocksRaycasts = abrir;

                enTransicion = false;
            });
    }

    private CanvasGroup PrepararGrupo(RectTransform contenedor)
    {
        CanvasGroup grupo = contenedor.GetComponent<CanvasGroup>();

        if (grupo == null)
            grupo = contenedor.gameObject.AddComponent<CanvasGroup>();

        grupo.alpha = 1f;
        grupo.blocksRaycasts = true;

        return grupo;
    }
}

