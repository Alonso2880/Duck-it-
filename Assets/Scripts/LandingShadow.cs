
using UnityEngine;

public class LandingShadow : MonoBehaviour
{
    public Transform player;
    public Transform shadow;

    public LayerMask groundMask;
    public float offset = 0.03f;
    public float maxDistance = 100f;

    void LateUpdate()
    {
        if (player == null || shadow == null)
            return;

        // Buscar el suelo debajo del personaje
        Vector3 origin = player.position + Vector3.up * 0.2f;

        if (Physics.Raycast(
            origin,
            Vector3.down,
            out RaycastHit hit,
            maxDistance,
            groundMask))
        {
            shadow.gameObject.SetActive(true);

            // Colocar la sombra justo sobre el suelo detectado
            shadow.position = hit.point + hit.normal * offset;

            // Adaptar la sombra a la inclinación del suelo
            shadow.rotation = Quaternion.FromToRotation(
                Vector3.forward,
                hit.normal
            );
        }
        else
        {
            shadow.gameObject.SetActive(false);
        }
    }
}

