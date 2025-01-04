using UnityEngine;
using UnityEngine.Tilemaps;

public class WeaponStopper : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the weapon hits a tilemap (walls)
        if (collision.collider is TilemapCollider2D)
        {
            // Stop the weapon
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0;
                rb.freezeRotation = true;
            }

            // Fade out and destroy the weapon
            Movement movementScript = Object.FindFirstObjectByType<Movement>();
            if (movementScript != null)
            {
                movementScript.StartCoroutine(movementScript.FadeOutAndDestroy(gameObject));
            }
        }
    }
}
