using UnityEngine;

public class MoverAgua : MonoBehaviour
{
    public Material miMaterial;

    public float offsetX;
    public float offsetY;

    public Vector2 offset;

    public float velocidad = 0.05f;

    void Update()
    {
        offset.x += velocidad * Time.deltaTime;

        if (offset.x > 1f) offset.x -= 1f;
        else if (offset.x < 0f) offset.x += 1f;

        miMaterial.mainTextureOffset = offset;
    }
}
