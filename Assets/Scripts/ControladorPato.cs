using Unity.VisualScripting;
using UnityEngine;

public class ControladorPato : MonoBehaviour
{
    [Header("Velocidad avanzar")]
    private float velocidad = 5f;
    private Rigidbody rb;

    [Header("Salto")]
    private float FuerzaSalto = 5f;
    private int MaxSaltos = 3;
    public int saltoActual = 0;

    [Header("Suelo")]
    public float radioSuelo = 1f;
    public LayerMask layerSuelo;
    public bool IsGrounded { get; private set; }
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
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

        //Detectar el suelo
        IsGrounded = Physics.Raycast(transform.position, Vector3.down, radioSuelo, layerSuelo);
   
        if (IsGrounded)
        {
            saltoActual = 0;
        }
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
