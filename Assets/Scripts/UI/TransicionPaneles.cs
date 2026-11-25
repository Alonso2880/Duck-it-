using UnityEngine;

public class TransicionPaneles : MonoBehaviour
{
    public RankingUIManager rankingUI;

    // =========================================================
    // CONFIGURACIÓN DEL MENÚ
    // =========================================================

    [System.Serializable]
    private class ElementoMenu
    {
        [Header("Panel al que lleva este botón")]
        public RectTransform panel;

        [Header("Iconos del botón")]
        public GameObject iconoNoSeleccionado;
        public RectTransform iconoSeleccionado;
    }

    [Header("Paneles e iconos del menú")]
    [Tooltip("Orden: 0 Skins, 1 Misiones, 2 MenuPrincipal, 3 AddFriends, 4 Shop")]
    [SerializeField] private ElementoMenu[] elementosMenu = new ElementoMenu[5];

    [Header("Configuración de las transiciones")]
    [SerializeField, Min(0.05f)] private float duracionTransicion = 0.45f;

    [Header("Animación del botón seleccionado")]
    [SerializeField, Min(0f)] private float desplazamientoSeleccionado = 15f;
    [SerializeField, Min(0.05f)] private float duracionSeleccion = 0.2f;

    // =========================================================
    // POP-UPS
    // =========================================================

    [Header("Ventanas emergentes")]
    [SerializeField] private GameObject popupPerfil;
    [SerializeField] private GameObject popupRanking;
    [SerializeField] private GameObject popupAjustes;
    [SerializeField] private GameObject popupRankingGameover;

    [Header("Animación de los pop-ups")]
    [SerializeField, Min(0.05f)] private float duracionPopup = 0.2f;

    // =========================================================
    // VARIABLES INTERNAS
    // =========================================================

    private const int INDICE_MENU_PRINCIPAL = 2;

    private RectTransform canvas;

    private CanvasGroup[] gruposPaneles;
    private Vector2[] posicionesBaseIconos;

    private CanvasGroup grupoPopupPerfil;
    private CanvasGroup grupoPopupRanking;
    private CanvasGroup grupoPopupAjustes;

    private int indicePanelActual = INDICE_MENU_PRINCIPAL;

    private bool preparado;
    private bool enTransicion;

    private CanvasGroup grupoPopupRankingGameover;

    [Header("Escala de los pop-ups")]
    [SerializeField, Range(0.1f, 1f)]
    private float escalaInicialPopup = 0.75f;

    [SerializeField]
    private LeanTweenType animacionPopup = LeanTweenType.easeOutBack;

    [Header("Audio")]
    public AudioSource audioClick;

    // =========================================================
    // INICIALIZACIÓN
    // =========================================================

    private void Start()
    {
        if (!ValidarConfiguracion())
            return;

        Canvas.ForceUpdateCanvases();

        gruposPaneles = new CanvasGroup[elementosMenu.Length];
        posicionesBaseIconos = new Vector2[elementosMenu.Length];

        // Preparar todos los paneles.
        for (int i = 0; i < elementosMenu.Length; i++)
        {
            ElementoMenu elemento = elementosMenu[i];

            gruposPaneles[i] = PrepararGrupo(elemento.panel);

            posicionesBaseIconos[i] =
                elemento.iconoSeleccionado.anchoredPosition;

            elemento.panel.anchoredPosition = Vector2.zero;

            bool esPanelInicial = i == INDICE_MENU_PRINCIPAL;

            elemento.panel.gameObject.SetActive(esPanelInicial);

            gruposPaneles[i].alpha = 1f;
            gruposPaneles[i].interactable = esPanelInicial;
            gruposPaneles[i].blocksRaycasts = esPanelInicial;
        }

        // Preparar los iconos del menú.
        ActualizarIconosMenu(INDICE_MENU_PRINCIPAL, false);

        // Preparar las ventanas emergentes.
        grupoPopupPerfil = PrepararPopup(popupPerfil);
        grupoPopupRanking = PrepararPopup(popupRanking);
        grupoPopupAjustes = PrepararPopup(popupAjustes);
        grupoPopupRankingGameover = PrepararPopup(popupRankingGameover);

        indicePanelActual = INDICE_MENU_PRINCIPAL;

        preparado = true;
    }

    // =========================================================
    // VALIDAR CONFIGURACIÓN
    // =========================================================

    private bool ValidarConfiguracion()
    {
        if (elementosMenu == null || elementosMenu.Length != 5)
        {
            Debug.LogError(
                "Debes configurar exactamente 5 elementos en el menú.",
                this
            );

            return false;
        }

        for (int i = 0; i < elementosMenu.Length; i++)
        {
            if (elementosMenu[i] == null ||
                elementosMenu[i].panel == null ||
                elementosMenu[i].iconoNoSeleccionado == null ||
                elementosMenu[i].iconoSeleccionado == null)
            {
                Debug.LogError(
                    "Falta asignar algún panel o icono del elemento " + i +
                    " en el Inspector.",
                    this
                );

                return false;
            }
        }

        Canvas canvasPrincipal =
            elementosMenu[0].panel.GetComponentInParent<Canvas>();

        if (canvasPrincipal == null)
        {
            Debug.LogError(
                "Los paneles deben pertenecer a un Canvas.",
                this
            );

            return false;
        }

        canvas = canvasPrincipal.transform as RectTransform;

        if (canvas == null)
        {
            Debug.LogError("No se ha podido obtener el RectTransform del Canvas.", this);
            return false;
        }

        for (int i = 1; i < elementosMenu.Length; i++)
        {
            Canvas canvasPanel =
                elementosMenu[i].panel.GetComponentInParent<Canvas>();

            if (canvasPanel != canvasPrincipal)
            {
                Debug.LogError(
                    "Todos los paneles deben pertenecer al mismo Canvas.",
                    this
                );

                return false;
            }
        }

        return true;
    }

    // =========================================================
    // BOTONES DEL MENÚ
    // =========================================================

    public void AbrirSkins()
    {
        audioClick.Play();
        CambiarPanel(0);
    }

    public void AbrirMisiones()
    {
        audioClick.Play();
        CambiarPanel(1);
    }

    public void AbrirMenuPrincipal()
    {
        audioClick.Play();
        CambiarPanel(2);
    }

    public void AbrirAddFriends()
    {
        audioClick.Play();
        CambiarPanel(3);
    }

    public void AbrirShop()
    {
        audioClick.Play();
        CambiarPanel(4);
    }

    // Alias por compatibilidad con el botón anterior.
    public void VolverPrincipal()
    {
        audioClick.Play();
        AbrirMenuPrincipal();
    }

    public void AbrirCompras()
    {
        audioClick.Play();
        AbrirShop();
    }

    // =========================================================
    // TRANSICIÓN ENTRE PANELES
    // =========================================================

    private void CambiarPanel(int nuevoIndice)
    {
        if (!preparado ||
            enTransicion ||
            nuevoIndice < 0 ||
            nuevoIndice >= elementosMenu.Length ||
            nuevoIndice == indicePanelActual)
        {
            return;
        }

        enTransicion = true;

        // Actualizar los iconos del menú.
        ActualizarIconosMenu(nuevoIndice, true);

        RectTransform panelSalida =
            elementosMenu[indicePanelActual].panel;

        RectTransform panelEntrada =
            elementosMenu[nuevoIndice].panel;

        CanvasGroup grupoSalida =
            gruposPaneles[indicePanelActual];

        CanvasGroup grupoEntrada =
            gruposPaneles[nuevoIndice];

        float altura = canvas.rect.height;

        // Si el destino está por encima, el panel sale hacia arriba.
        // Si el destino está por debajo, el panel sale hacia abajo.
        float desplazamientoY =
            nuevoIndice < indicePanelActual ? -altura : altura;

        // Activar el panel de entrada.
        panelEntrada.gameObject.SetActive(true);

        // Evitar que queden animaciones antiguas.
        LeanTween.cancel(panelSalida.gameObject);
        LeanTween.cancel(panelEntrada.gameObject);

        // Posiciones iniciales.
        panelSalida.anchoredPosition = Vector2.zero;

        panelEntrada.anchoredPosition =
            Vector2.up * -desplazamientoY;

        // Bloquear interacciones durante la transición.
        grupoSalida.interactable = false;
        grupoSalida.blocksRaycasts = false;

        grupoEntrada.interactable = false;
        grupoEntrada.blocksRaycasts = false;

        // Animar la salida del panel actual.
        LeanTween.moveY(
            panelSalida,
            desplazamientoY,
            duracionTransicion
        )
        .setEase(LeanTweenType.easeInOutCubic)
        .setIgnoreTimeScale(true);

        // Animar la entrada del nuevo panel.
        LeanTween.moveY(
            panelEntrada,
            0f,
            duracionTransicion
        )
        .setEase(LeanTweenType.easeInOutCubic)
        .setIgnoreTimeScale(true)
        .setOnComplete(() =>
        {
            // Desactivar el panel anterior.
            panelSalida.gameObject.SetActive(false);
            panelSalida.anchoredPosition = Vector2.zero;

            // Colocar el nuevo panel correctamente.
            panelEntrada.anchoredPosition = Vector2.zero;

            // Actualizar el panel actual.
            indicePanelActual = nuevoIndice;

            // Reactivar las interacciones.
            grupoEntrada.interactable = true;
            grupoEntrada.blocksRaycasts = true;

            enTransicion = false;
        });
    }

    // =========================================================
    // ANIMACIÓN DE LOS ICONOS SELECCIONADOS
    // =========================================================

    private void ActualizarIconosMenu(int indiceSeleccionado, bool animar)
    {
        for (int i = 0; i < elementosMenu.Length; i++)
        {
            ElementoMenu elemento = elementosMenu[i];

            RectTransform iconoSeleccionado =
                elemento.iconoSeleccionado;

            GameObject iconoNoSeleccionado =
                elemento.iconoNoSeleccionado;

            // Cancelar cualquier animación anterior.
            LeanTween.cancel(iconoSeleccionado.gameObject);

            // Recuperar la posición original.
            iconoSeleccionado.anchoredPosition =
                posicionesBaseIconos[i];

            if (i == indiceSeleccionado)
            {
                // Mostrar el icono seleccionado.
                iconoSeleccionado.gameObject.SetActive(true);
                iconoNoSeleccionado.SetActive(false);

                if (animar)
                {
                    // Desplazamiento hacia la derecha.
                    float posicionFinal =
                        posicionesBaseIconos[i].x +
                        desplazamientoSeleccionado;

                    LeanTween.moveX(
                        iconoSeleccionado,
                        posicionFinal,
                        duracionSeleccion
                    )
                    .setEase(LeanTweenType.easeOutCubic)
                    .setIgnoreTimeScale(true);
                }
                else
                {
                    // Posición inicial del menú.
                    iconoSeleccionado.anchoredPosition +=
                        Vector2.right * desplazamientoSeleccionado;
                }
            }
            else
            {
                // Mostrar el icono no seleccionado.
                iconoSeleccionado.gameObject.SetActive(false);
                iconoNoSeleccionado.SetActive(true);
            }
        }
    }

    // =========================================================
    // PREPARAR CANVAS GROUP DE LOS PANELES
    // =========================================================

    private CanvasGroup PrepararGrupo(RectTransform panel)
    {
        CanvasGroup grupo = panel.GetComponent<CanvasGroup>();

        if (grupo == null)
        {
            grupo = panel.gameObject.AddComponent<CanvasGroup>();
        }

        grupo.alpha = 1f;
        grupo.interactable = false;
        grupo.blocksRaycasts = false;

        return grupo;
    }

    // =========================================================
    // PREPARAR POP-UPS
    // =========================================================

    private CanvasGroup PrepararPopup(GameObject popup)
    {
        if (popup == null)
            return null;

        CanvasGroup grupo = popup.GetComponent<CanvasGroup>();

        if (grupo == null)
        {
            grupo = popup.AddComponent<CanvasGroup>();
        }

        popup.SetActive(false);

        grupo.alpha = 0f;
        grupo.interactable = false;
        grupo.blocksRaycasts = false;

        return grupo;
    }

    // =========================================================
    // ABRIR POP-UPS
    // =========================================================

    public void AbrirRankingGameover()
    {
        audioClick.Play();
        MostrarPopup(popupRankingGameover, grupoPopupRankingGameover);
    }

    private void MostrarPopup(GameObject popup, CanvasGroup grupo)
    {
        if (popup == null || grupo == null)
        {
            Debug.LogWarning("No se ha asignado correctamente el pop-up.", this);
            return;
        }

        LeanTween.cancel(popup);

        popup.SetActive(true);

        RectTransform rect = popup.GetComponent<RectTransform>();

        // Estado inicial.
        grupo.alpha = 0f;
        grupo.interactable = false;
        grupo.blocksRaycasts = true;

        rect.localScale = Vector3.one * escalaInicialPopup;

        // Animación de aparición.
        LeanTween.alphaCanvas(grupo, 1f, duracionPopup)
            .setEase(LeanTweenType.easeOutCubic)
            .setIgnoreTimeScale(true);

        LeanTween.scale(rect, Vector3.one, duracionPopup)
            .setEase(animacionPopup)
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                grupo.interactable = true;
                grupo.blocksRaycasts = true;
            });
    }

    public void AbrirPerfil()
    {
        audioClick.Play();
        MostrarPopup(popupPerfil, grupoPopupPerfil);
    }

    public void AbrirRanking()
    {
        audioClick.Play();
        MostrarPopup(popupRanking, grupoPopupRanking);
        rankingUI.UpdateRanking();
    }

    public void AbrirAjustes()
    {
        audioClick.Play();
        MostrarPopup(popupAjustes, grupoPopupAjustes);
    }

    // =========================================================
    // CERRAR POP-UPS
    // =========================================================

    public void CerrarRankingGameover()
    {
        audioClick.Play();
        OcultarPopup(popupRankingGameover, grupoPopupRankingGameover);
    }
    private void OcultarPopup(GameObject popup, CanvasGroup grupo)
    {
        if (popup == null || grupo == null)
            return;

        if (!popup.activeSelf)
            return;

        LeanTween.cancel(popup);

        RectTransform rect = popup.GetComponent<RectTransform>();

        grupo.interactable = false;
        grupo.blocksRaycasts = true;

        // Reducir y desvanecer.
        LeanTween.alphaCanvas(grupo, 0f, duracionPopup)
            .setEase(LeanTweenType.easeInOutCubic)
            .setIgnoreTimeScale(true);

        LeanTween.scale(
            rect,
            Vector3.one * escalaInicialPopup,
            duracionPopup
        )
        .setEase(LeanTweenType.easeInBack)
        .setIgnoreTimeScale(true)
        .setOnComplete(() =>
        {
            popup.SetActive(false);

            grupo.alpha = 0f;
            grupo.interactable = false;
            grupo.blocksRaycasts = false;

            // Recuperar la escala original.
            rect.localScale = Vector3.one;
        });
    }

    public void CerrarPerfil()
    {
        audioClick.Play();
        OcultarPopup(popupPerfil, grupoPopupPerfil);
    }

    public void CerrarRanking()
    {
        audioClick.Play();
        OcultarPopup(popupRanking, grupoPopupRanking);
    }

    public void CerrarAjustes()
    {
        audioClick.Play();
        OcultarPopup(popupAjustes, grupoPopupAjustes);
    }
}



