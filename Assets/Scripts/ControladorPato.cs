
using System.Collections;
using UnityEngine;

public class ControladorPato : MonoBehaviour
{
    [Header("Velocidad y aceleracion")]
    public float velocidad = 5f;
    public float aceleracion = 0.5f;
    public float velocidadMaxima = 15f;

    private float velocidadActual;
    private Rigidbody rb;

    [Header("Boost de velocidad inicial")]
    public float duracionBoostLento = 3f;
    private bool boostLentoActivo = false;
    private Coroutine rutinaBoostLento;

    [Header("Salto")]
    public float FuerzaSalto = 5f;
    private int MaxSaltos = 3;
    public int saltoActual = 0;

    [Header("Suelo")]
    public float radioSuelo = 1f;
    public LayerMask layerSuelo;

    [Header("Detección de atasco")]
    public float tiempoMaximoAtascado = 1f;
    private float tiempoSinAvanzar = 0f;
    private float ultimaPosicionX;

    // Evita saltos mientras se mantiene pulsado
    // el dedo o la tecla al comenzar la partida.
    private bool entradaBloqueada = true;

    public bool IsGrounded { get; private set; }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        velocidadActual = velocidad;
        ultimaPosicionX = transform.position.x;
    }

    void Update()
    {
        //Aceleración progresiva hasta alcanzar el máximo.
        velocidadActual = Mathf.Min( velocidadActual + aceleracion * Time.deltaTime,velocidadMaxima);

        //El boost modifica la velocidad aplicada, pero no reinicia la aceleración acumulada.
        float velocidadAplicada = boostLentoActivo? velocidad : velocidadActual;

        // Movimiento hacia delante
        Vector3 Velocidad = rb.linearVelocity;
        Velocidad.x = velocidadAplicada;
        rb.linearVelocity = Velocidad;

        // Detectar cuándo se sueltan las entradas.
        if (entradaBloqueada)
        {
            bool dedoPulsado = Input.touchCount > 0;
            bool espacioPulsado = Input.GetKey(KeyCode.Space);

            if (!dedoPulsado && !espacioPulsado)
            {
                entradaBloqueada = false;
            }
        }
        else
        {
            // Salto con teclado
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Salto();
            }

            // Salto táctil
            if (Input.touchCount > 0 &&
                Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Salto();
            }
        }

        // Detectar el suelo
        IsGrounded = Physics.Raycast(
            transform.position,
            Vector3.down,
            radioSuelo,
            layerSuelo
        );

        if (IsGrounded)
        {
            saltoActual = 0;
        }
    }

    public void BloquearEntrada()
    {
        entradaBloqueada = true;
    }

    public void ActivarBoostLento()
    {
        if (rutinaBoostLento != null)
        {
            StopCoroutine(rutinaBoostLento);
        }

        rutinaBoostLento = StartCoroutine(BoostLento());
    }

    private IEnumerator BoostLento()
    {
        boostLentoActivo = true;

        yield return new WaitForSeconds(duracionBoostLento);

        boostLentoActivo = false;
        rutinaBoostLento = null;
    }

    public void ReiniciarVelocidad()
    {
        velocidadActual = velocidad;
        boostLentoActivo = false;

        if (rutinaBoostLento != null)
        {
            StopCoroutine(rutinaBoostLento);
            rutinaBoostLento = null;
        }

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void FixedUpdate()
    {
        Muerte muerte = GetComponent<Muerte>();
        float avance = transform.position.x - ultimaPosicionX;

        if (avance < 0.01f)
        {
            tiempoSinAvanzar += Time.fixedDeltaTime;

            if (tiempoSinAvanzar >= tiempoMaximoAtascado)
            {
                Debug.Log("Muerte por no avanzar");
                tiempoSinAvanzar = 0f;
                ultimaPosicionX = transform.position.x;
                muerte.Death();
            }
        }
        else
        {
            tiempoSinAvanzar = 0f;
        }

        ultimaPosicionX = transform.position.x;
    }

    public void Salto()
    {
        if (entradaBloqueada)
            return;

        if (saltoActual >= MaxSaltos)
            return;

        saltoActual++;

        Vector3 Velocidad = rb.linearVelocity;
        Velocidad.y = 0f;
        rb.linearVelocity = Velocidad;

        rb.AddForce(
            Vector3.up * FuerzaSalto,
            ForceMode.Impulse
        );
    }
}

