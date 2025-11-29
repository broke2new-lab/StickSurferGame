using UnityEngine;

public class WaterFlow : MonoBehaviour
{
    // Public variables to control speed and direction in the Inspector
    public float flowSpeedX = 0.05f;
    public float flowSpeedY = 0.05f;
    public string textureName = "_MainTex"; // Name of the main texture property
    public string normalMapName = "_BumpMap"; // Name of the normal map property

    private Renderer rend;
    private Material waterMaterial;
    private float offsetX;
    private float offsetY;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            // Get the instantiated material copy
            waterMaterial = rend.material;
        }
    }

    void Update()
    {
        if (waterMaterial == null) return;

        // Calculate the new offset based on time and speed
        offsetX += Time.deltaTime * flowSpeedX;
        offsetY += Time.deltaTime * flowSpeedY;

        // Apply the offset to the texture(s)
        waterMaterial.SetTextureOffset(textureName, new Vector2(offsetX, offsetY));
        waterMaterial.SetTextureOffset(normalMapName, new Vector2(offsetX, offsetY));

        // Optional: Keep the offsets within a range (e.g., 0 to 1) to prevent large floating point numbers
        if (offsetX > 1f) offsetX -= 1f;
        if (offsetY > 1f) offsetY -= 1f;
    }
}