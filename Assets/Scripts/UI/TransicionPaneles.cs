
using UnityEngine;

public class TransicionPaneles : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private RectTransform panelPrincipal;
    [SerializeField] private RectTransform panelMisiones;
    [SerializeField] private RectTransform panelPerfil;
    [SerializeField] private RectTransform panelCompras;

    [Header("Configuración")]
    [SerializeField, Min(0.05f)] private float duracion = 0.45f;

    private RectTransform canvas;
    private CanvasGroup grupoPrincipal;
    private CanvasGroup grupoMisiones;
    private CanvasGroup grupoPerfil;
    private CanvasGroup grupoCompras;

    private RectTransform panelActual;
    private CanvasGroup grupoActual;

    private bool preparado;
    private bool enTransicion;

    public GameObject ajustes;

    private void Start()
    {
        // Comprobar que todos los paneles están asignados.
        if (panelPrincipal == null ||
            panelMisiones == null ||
            panelPerfil == null ||
            panelCompras == null)
        {
            Debug.LogError("Asigna todos los paneles en el Inspector.", this);
            return;
        }

        Canvas canvasPrincipal = panelPrincipal.GetComponentInParent<Canvas>();

        if (canvasPrincipal == null ||
            panelMisiones.GetComponentInParent<Canvas>() != canvasPrincipal ||
            panelPerfil.GetComponentInParent<Canvas>() != canvasPrincipal ||
            panelCompras.GetComponentInParent<Canvas>() != canvasPrincipal)
        {
            Debug.LogError("Todos los paneles deben pertenecer al mismo Canvas.", this);
            return;
        }

        canvas = canvasPrincipal.transform as RectTransform;

        // Preparar CanvasGroups.
        grupoPrincipal = PrepararGrupo(panelPrincipal);
        grupoMisiones = PrepararGrupo(panelMisiones);
        grupoPerfil = PrepararGrupo(panelPerfil);
        grupoCompras = PrepararGrupo(panelCompras);

        // Estado inicial.
        panelPrincipal.gameObject.SetActive(true);

        panelMisiones.gameObject.SetActive(false);
        panelPerfil.gameObject.SetActive(false);
        panelCompras.gameObject.SetActive(false);

        Canvas.ForceUpdateCanvases();

        panelPrincipal.anchoredPosition = Vector2.zero;

        panelMisiones.anchoredPosition = Vector2.down * canvas.rect.height;
        panelPerfil.anchoredPosition = Vector2.down * canvas.rect.height;
        panelCompras.anchoredPosition = Vector2.down * canvas.rect.height;

        grupoPrincipal.interactable = true;
        grupoPrincipal.blocksRaycasts = true;

        grupoMisiones.interactable = false;
        grupoMisiones.blocksRaycasts = false;

        grupoPerfil.interactable = false;
        grupoPerfil.blocksRaycasts = false;

        grupoCompras.interactable = false;
        grupoCompras.blocksRaycasts = false;

        panelActual = panelPrincipal;
        grupoActual = grupoPrincipal;

        preparado = true;
    }

    // BOTONES PARA ABRIR PANELES

    public void AbrirMisiones()
    {
        CambiarPanel(panelMisiones, grupoMisiones);
    }

    public void AbrirPerfil()
    {
        CambiarPanel(panelPerfil, grupoPerfil);
    }

    public void AbrirCompras()
    {
        CambiarPanel(panelCompras, grupoCompras);
    }

    // BOTÓN PARA VOLVER AL PANEL PRINCIPAL

    public void VolverPrincipal()
    {
        CambiarPanel(panelPrincipal, grupoPrincipal);
    }

    // TRANSICIÓN GENERAL

    private void CambiarPanel(RectTransform nuevoPanel, CanvasGroup nuevoGrupo)
    {
        if (!preparado || enTransicion || panelActual == nuevoPanel)
            return;

        enTransicion = true;

        float altura = canvas.rect.height;

        RectTransform salida = panelActual;
        RectTransform entrada = nuevoPanel;

        CanvasGroup grupoSalida = grupoActual;
        CanvasGroup grupoEntrada = nuevoGrupo;

        // Activar el panel de entrada.
        entrada.gameObject.SetActive(true);

        // Posiciones iniciales.
        salida.anchoredPosition = Vector2.zero;

        entrada.anchoredPosition = Vector2.down * altura;

        // Bloquear interacciones durante la animación.
        grupoSalida.interactable = false;
        grupoSalida.blocksRaycasts = false;

        grupoEntrada.interactable = false;
        grupoEntrada.blocksRaycasts = false;

        // Animar salida.
        LeanTween.moveY(salida, altura, duracion)
            .setEase(LeanTweenType.easeInOutCubic)
            .setIgnoreTimeScale(true);

        // Animar entrada.
        LeanTween.moveY(entrada, 0f, duracion)
            .setEase(LeanTweenType.easeInOutCubic)
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                // Desactivar el panel anterior.
                salida.gameObject.SetActive(false);

                entrada.anchoredPosition = Vector2.zero;

                // Actualizar panel actual.
                panelActual = entrada;
                grupoActual = grupoEntrada;

                // Activar interacciones del nuevo panel.
                grupoEntrada.interactable = true;
                grupoEntrada.blocksRaycasts = true;

                enTransicion = false;
            });
    }

    // PREPARAR CANVAS GROUP

    private CanvasGroup PrepararGrupo(RectTransform contenedor)
    {
        CanvasGroup grupo = contenedor.GetComponent<CanvasGroup>();

        if (grupo == null)
            grupo = contenedor.gameObject.AddComponent<CanvasGroup>();

        grupo.alpha = 1f;
        grupo.interactable = false;
        grupo.blocksRaycasts = false;

        return grupo;
    }

    public void AbrirAjustes()
    {
        ajustes.SetActive(true);
    }

    public void CerrarAjustes()
    {
        ajustes.SetActive(false);
    }
}


