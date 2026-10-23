
using UnityEngine;

public class ControladorPato : MonoBehaviour
{
    [Header("Velocidad avanzar")]
    public float velocidad = 5f;
    private Rigidbody rb;

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
        ultimaPosicionX = transform.position.x;
    }

    void Update()
    {
        // Movimiento hacia delante
        Vector3 Velocidad = rb.linearVelocity;
        Velocidad.x = velocidad;
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

