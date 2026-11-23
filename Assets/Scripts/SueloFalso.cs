using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class SueloFalso : MonoBehaviour
{

    private Animator animator;

    // Referencia al objeto que rota
    private Transform objetoRotacion;

    // Guardar rotación original
    private Quaternion rotacionOriginal;

    void Start()
    {
        animator = this.gameObject.GetComponent<Animator>();

        objetoRotacion = transform.GetChild(0).GetChild(0);

        // Guardar rotación inicial
        rotacionOriginal = objetoRotacion.localRotation;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            animator.SetBool("Abrir", true);
            //Debug.Log("Pato Detectado");
            StartCoroutine(Vuelta());
        }
    }

    private IEnumerator Vuelta()
    {
        yield return new WaitForSeconds(1.5f);
        animator.SetBool("Abrir", false);
        ReiniciarRotacion();
    }

    // Método independiente para reiniciar la rotación
    public void ReiniciarRotacion()
    {
        Debug.Log("coco rot");
        objetoRotacion.localRotation = rotacionOriginal;
    }
}
