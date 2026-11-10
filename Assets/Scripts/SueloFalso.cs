using UnityEngine;

public class SueloFalso : MonoBehaviour
{

    private Animator animator;

    void Start()
    {
        animator = this.gameObject.GetComponent<Animator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            animator.SetBool("Abrir", true);
            Debug.Log("Pato Detectado");
        }
    }
}
