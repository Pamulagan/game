using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f; // Movement speed
    private Animator animator;

    void Start()
    {
        // Get the Animator component
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found!");
        }
    }

    void Update()
    {
        // Get input from WASD keys (Horizontal and Vertical axes)
        float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float vertical = Input.GetAxis("Vertical");     // W/S or Up/Down

        // Create a movement vector for 2D (X and Y)
        Vector3 movement = new Vector3(horizontal, vertical, 0f) * moveSpeed * Time.deltaTime;

        // Apply movement to the Transform component
        transform.Translate(movement);

        // Calculate the movement speed (magnitude of the input)
        float speed = new Vector2(horizontal, vertical).magnitude;

        // Set Speed parameter in Animator
        animator.SetFloat("Speed", speed);

        // Debug to ensure Speed is updating correctly
        Debug.Log("Speed: " + speed);
    }
}
