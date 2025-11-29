using UnityEngine;

public class RockMovement : MonoBehaviour
{
    public float rockSpeed = 0.1f;       // Speed at which the rock travels toward the player
    public float destroyZPosition = -50f; // Z-coordinate where the rock is destroyed

    void Update()
    {
        // Move the rock in the negative Z direction (towards the player/camera)
        transform.Translate(Vector3.back * rockSpeed * Time.deltaTime, Space.World);

        // Check if the rock has passed the designated destroy point
        if (transform.position.z < destroyZPosition)
        {
            Destroy(gameObject);
        }
    }
}
