
using UnityEngine;

public class ObjetosReiniciables : MonoBehaviour
{
    public void Recoger()
    {
        gameObject.SetActive(false);
    }

    public void Reiniciar()
    {
        gameObject.SetActive(true);
    }
}

