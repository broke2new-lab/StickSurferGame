using UnityEngine;

public class SurferController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;           // Speed of horizontal (XZ) movement
    public float rotationSpeed = 10f;      // How fast the surfer turns
    public float minX = -40f;              // Left boundary 
    public float maxX = 40f;               // Right boundary 
    public float minZ = -40f;              // Backward boundary 
    public float maxZ = 40f;               // Forward boundary 
    public float hoverHeight = 0.2f;       // How high above the water's surface to float

    [Header("References")]
    public WaveGenerator waveGenerator;    // The script calculating the wave motion
    public Transform waterPlaneTransform;  // The transform of the WaterPlane object

    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Escape))
    {
        // This command closes the application when run as a standalone build.
        Application.Quit();

        // In the Unity Editor, Application.Quit() has no effect. 
        // We use the following line to stop Play Mode:
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
	
        // 1. READ PLAYER INPUT
        float horizontalInput = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrows
        float verticalInput = Input.GetAxis("Vertical");     // W/S or Up/Down Arrows

        // Create a movement vector based on input (normalized for consistent speed on diagonals)
        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

        Vector3 currentPos = transform.position;
        
        // 2. APPLY XZ MOVEMENT
        // Move the current position based on the calculated direction vector
        currentPos += moveDirection * moveSpeed * Time.deltaTime;

        // 3. CLAMP POSITION (Stay within the water boundaries)
        currentPos.x = Mathf.Clamp(currentPos.x, minX, maxX);
        currentPos.z = Mathf.Clamp(currentPos.z, minZ, maxZ); 

        // 4. VERTICAL WAVE FOLLOWING (Y-axis)
        float waveHeight = GetWaveHeightAtPosition(currentPos);
        currentPos.y = waveHeight + hoverHeight;

        // Apply the new position
        transform.position = currentPos;

        // 5. ROTATION (Yaw - Match the movement direction)
        HandleYawRotation(moveDirection);
    }
    
    // --- ROTATION HANDLER ---
    void HandleYawRotation(Vector3 moveDirection)
    {
        // Only rotate if the player is actively moving (magnitude > small threshold)
        if (moveDirection.magnitude >= 0.1f)
        {
            // Calculate the target rotation (Quaternion) that faces the direction vector.
            // Quaternion.LookRotation aligns the object's forward (Z-axis) with the moveDirection.
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            // Smoothly rotate the surfer to match the target rotation
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime
            );
        }
    }
		// In SurferController.cs

		// New function to handle collisions
	void OnCollisionEnter(Collision collision)
{
    // Check if the surfer hit an object tagged "Rock"
    if (collision.gameObject.CompareTag("Rock"))
    {
        Debug.Log("Game Over! Surfer hit a rock. Closing application.");
        
        // --- GAME END LOGIC: CLOSE APPLICATION ---
        
        // 1. Closes the application when run as a standalone build (Windows, Mac, etc.).
        Application.Quit();

        // 2. In the Unity Editor, Application.Quit() has no effect. 
        // We use the following line to stop Play Mode for testing:
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        
        // We no longer need to freeze time or disable the script, 
        // as the application is closing immediately.
    }
}
    /// <summary>
    /// Gets the calculated wave height from the water mesh at the given world position.
    /// </summary>
    float GetWaveHeightAtPosition(Vector3 worldPosition)
    {
        // Safety checks for references
        if (waveGenerator == null || waterPlaneTransform == null) return 0f;

        MeshFilter meshFilter = waterPlaneTransform.GetComponent<MeshFilter>();
        if (meshFilter == null || meshFilter.mesh == null) return 0f;

        Vector3[] vertices = meshFilter.mesh.vertices;
        int[] triangles = meshFilter.mesh.triangles;
        
        // Convert world position to local position of the water plane
        Vector3 localPos = waterPlaneTransform.InverseTransformPoint(worldPosition);

        // Iterate through triangles to find the one the surfer is above
        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 p1 = vertices[triangles[i]];
            Vector3 p2 = vertices[triangles[i + 1]];
            Vector3 p3 = vertices[triangles[i + 2]];

            // Simplified XZ boundary check for the triangle
            float minXTri = Mathf.Min(p1.x, p2.x, p3.x);
            float maxXTri = Mathf.Max(p1.x, p2.x, p3.x);
            float minZTri = Mathf.Min(p1.z, p2.z, p3.z);
            float maxZTri = Mathf.Max(p1.z, p2.z, p3.z);

            if (localPos.x >= minXTri && localPos.x <= maxXTri &&
                localPos.z >= minZTri && localPos.z <= maxZTri)
            {
                // Return the average Y-height of the triangle (approximation)
                float avgY = (p1.y + p2.y + p3.y) / 3f;
                // Convert the local Y position back to world Y position
                return waterPlaneTransform.TransformPoint(new Vector3(localPos.x, avgY, localPos.z)).y;
            }
        }
        // Fallback: If outside the mesh bounds, return the plane's base Y position
        return waterPlaneTransform.position.y;
    }
}