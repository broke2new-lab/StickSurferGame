using UnityEngine;
using System.Collections.Generic;

public class WaveGenerator : MonoBehaviour
{
    
  
    
   

    [Header("Base Wave Properties")]
    public float waveSpeed = 0.4f;        // How fast the waves travel
    public float waveScale = 0.05f;      // Overall height multiplier (Amplitude)
    public float waveFrequency = 0.5f;    // How closely packed the waves are (Wavelength inverse)
    public float detailMultiplier = 0.5f; // For Choppy/Calm texture

    // --- Private Variables ---
    private MeshFilter meshFilter;
    private Vector3[] baseVertices;
    private Vector3[] workingVertices;

    void Awake()
    {
        // 1. Get the MeshFilter component
        meshFilter = GetComponent<MeshFilter>();
        
        // Ensure the mesh exists
        if (meshFilter == null || meshFilter.mesh == null)
        {
            Debug.LogError("WaveGenerator requires a MeshFilter component with a mesh.");
            enabled = false; // Disable the script if setup is wrong
            return;
        }

        // 2. Cache the original vertices (x and z positions)
        baseVertices = meshFilter.mesh.vertices;
        
        // 3. Create a copy for the working vertices (y will be changed)
        workingVertices = new Vector3[baseVertices.Length];
        baseVertices.CopyTo(workingVertices, 0);
    }

    void Update()
    {
        // Check if we have vertices to work with
        if (workingVertices == null || workingVertices.Length == 0) return;

      

        // Calculate and apply the wave height to all vertices
        GenerateWaves();

        // Re-assign the modified vertices back to the mesh
        meshFilter.mesh.vertices = workingVertices;
        
        // Recalculate normals to ensure lighting looks correct after deformation
        meshFilter.mesh.RecalculateNormals();
    }
	
	/// <summary>
/// Public function called by the UI buttons to change the wave state.
/// </summary>
/// <param name="typeIndex">The integer index corresponding to the WaveType enum.</param>


    
    
  

    /// <summary>
    /// Core function to calculate the vertical displacement for each vertex.
    /// </summary>
    void GenerateWaves()
    {
        // Use Time.time to make the waves move over time
        float timeOffset = Time.time * waveSpeed;

        for (int i = 0; i < workingVertices.Length; i++)
        {
            Vector3 vertex = baseVertices[i];
            float xPos = vertex.x;
            float zPos = vertex.z;
            
            // --- Wave Formula ---
            // The height (y) is the sum of two sine waves to make the motion less uniform and more natural.

            float height1 = Mathf.Sin(xPos * waveFrequency + timeOffset) * waveScale;
            float height2 = Mathf.Cos(zPos * waveFrequency * detailMultiplier + timeOffset * detailMultiplier) * (waveScale / detailMultiplier);

            float totalHeight = height1 + height2;

            // Set the new Y position for the vertex
            workingVertices[i].y = totalHeight;
        }
    }
}
