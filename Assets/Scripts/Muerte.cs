using UnityEngine;

public class Muerte : MonoBehaviour
{
    [Header("Spawn")]
    public GameObject Spawn;
    public void Death()
    {
        transform.position = Spawn.transform.position;
    }
}
