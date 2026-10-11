using UnityEngine;

public class CanvasInicio : MonoBehaviour
{
    public GameObject DM;
    public GameObject CI;

    void Start()
    {
        Time.timeScale = 0f; 
    }


    public void EmpezarJuego()
    {
        Time.timeScale = 1f;
        CI.SetActive(false);
        DM.SetActive(true);
    }
}
