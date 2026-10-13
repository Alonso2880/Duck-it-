using Unity.VisualScripting;
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

    public bool IsGrounded { get; private set; }
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ultimaPosicionX = transform.position.x;
    }

    void Update()
    {
        //Movimiento hacia delante
        Vector3 Velocidad = rb.linearVelocity;
        Velocidad.x = velocidad;
        rb.linearVelocity = Velocidad;

        //Salto
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Salto();
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Salto();
        }

        //Detectar el suelo
        IsGrounded = Physics.Raycast(transform.position, Vector3.down, radioSuelo, layerSuelo);
   
        if (IsGrounded)
        {
            saltoActual = 0;
        }
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
        if (saltoActual >= MaxSaltos)
            return;

        saltoActual++;

        Vector3 Velocidad = rb.linearVelocity;
        Velocidad.y = 0f;
        rb.linearVelocity = Velocidad;

        rb.AddForce(Vector3.up * FuerzaSalto, ForceMode.Impulse);
    }
}
