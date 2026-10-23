using UnityEngine;

public class CheckPoints : MonoBehaviour
{
    public GameObject UltimoCheck;
    public void RegistrarCheckPoint(GameObject check)
    {
        UltimoCheck = check;
    }
}
