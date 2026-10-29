using UnityEngine;

public class TransicionPaneles : MonoBehaviour
{
    [SerializeField] private RectTransform panelPrincipal;
    [SerializeField] private RectTransform panelDestino;
    [SerializeField, Min(0.05f)] private float duracion = 0.45f;

    private RectTransform contenedor;
    private CanvasGroup grupoPrincipal;
    private CanvasGroup grupoDestino;

    private bool preparado;
    private bool enTransicion;
    private bool destinoAbierto;

    private void Start()
    {
        if (panelPrincipal == null || panelDestino == null)
        {
            Debug.LogError(
                "TransicionPaneles: arrastra los dos paneles al Inspector.",
                this
            );
            return;
        }

        if (panelPrincipal == panelDestino)
        {
            Debug.LogError(
                "TransicionPaneles: debes asignar dos paneles diferentes.",
                this
            );
            return;
        }

        contenedor = panelPrincipal.parent as RectTransform;

        if (contenedor == null || panelDestino.parent != contenedor)
        {
            Debug.LogError(
                "TransicionPaneles: los dos paneles deben tener el mismo padre.",
                this
            );
            return;
        }

        grupoPrincipal = PrepararGrupo(panelPrincipal);
        grupoDestino = PrepararGrupo(panelDestino);

        panelPrincipal.gameObject.SetActive(true);
        panelDestino.gameObject.SetActive(false);

        Canvas.ForceUpdateCanvases();

        panelPrincipal.anchoredPosition = Vector2.zero;
        panelDestino.anchoredPosition =
            Vector2.down * contenedor.rect.height;

        grupoPrincipal.interactable = true;
        grupoDestino.interactable = false;

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
        if (!preparado || !isActiveAndEnabled || enTransicion)
            return;

        if (abrir == destinoAbierto)
            return;

        enTransicion = true;

        Canvas.ForceUpdateCanvases();

        float altura = contenedor.rect.height;
        float direccion = abrir ? 1f : -1f;

        RectTransform salida = abrir ? panelPrincipal : panelDestino;
        RectTransform entrada = abrir ? panelDestino : panelPrincipal;

        grupoPrincipal.interactable = false;
        grupoDestino.interactable = false;

        salida.anchoredPosition = Vector2.zero;
        entrada.anchoredPosition = Vector2.down * altura * direccion;

        entrada.gameObject.SetActive(true);

        LeanTween.moveY(salida, altura * direccion, duracion)
            .setEase(LeanTweenType.easeInOutCubic)
            .setIgnoreTimeScale(true);

        LeanTween.moveY(entrada, 0f, duracion)
            .setEase(LeanTweenType.easeInOutCubic)
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                salida.gameObject.SetActive(false);
                entrada.anchoredPosition = Vector2.zero;

                destinoAbierto = abrir;

                grupoPrincipal.interactable = !abrir;
                grupoDestino.interactable = abrir;

                enTransicion = false;
            });
    }

    private CanvasGroup PrepararGrupo(RectTransform panel)
    {
        CanvasGroup grupo = panel.GetComponent<CanvasGroup>();

        if (grupo == null)
            grupo = panel.gameObject.AddComponent<CanvasGroup>();

        grupo.alpha = 1f;
        grupo.blocksRaycasts = true;

        return grupo;
    }
}
