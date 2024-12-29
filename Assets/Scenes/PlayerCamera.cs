using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform player; // Reference to the player object
    public Vector3 offset;   // Offset to keep the camera slightly behind the player

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player Transform is not assigned!");
        }
    }

    void LateUpdate()
    {
        if (player != null)
        {
            // Update the camera's position relative to the player with an offset
            transform.position = player.position + offset;
        }
    }
}