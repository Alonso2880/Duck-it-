using System.Collections;
using UnityEngine;

public class AutodestruccionBalas : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(DestruccionBalas());
    }

    IEnumerator DestruccionBalas()
    {
        yield return new WaitForSeconds(3f);

        Destroy(this.gameObject);
    }

}
