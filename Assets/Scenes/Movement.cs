using System.Collections;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float dashDistance = 4f;
    public float dashDuration = 0.2f;
    public KeyCode closeAttackKey = KeyCode.Space; // Key to trigger the close attack
    public KeyCode longAttackKey = KeyCode.E; // Key to trigger the long attack
    public GameObject weaponPrefab; // Weapon prefab for long attack
    public float weaponSpeed = 10f; // Speed of the weapon

    private static readonly string StateIdle = "Idle";
    private static readonly string StateRunRight = "RunRight";
    private static readonly string StateRunLeft = "RunLeft";
    private static readonly string StateRunBack = "RunBack";
    private static readonly string StateRunFront = "RunFront";
    private static readonly string StateCloseAttack = "CloseAttack";
    private static readonly string StateLongAttack = "LongAttack";

    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 movementInput;
    private bool isDashing = false;
    private Vector2 dashDirection;
    private float dashEndTime;

    private bool isAttacking = false;

    public bool IsAttacking => isAttacking; // Public getter for other logic

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
        {
            Debug.LogError("Animator component not found!");
        }

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found!");
        }

        if (weaponPrefab == null)
        {
            Debug.LogError("Weapon prefab not assigned!");
        }
    }

    void Update()
    {
        // Handle close attack logic
        if (Input.GetKeyDown(closeAttackKey) && !isAttacking)
        {
            StartCloseAttack();
            return;
        }

        // Handle long attack logic
        if (Input.GetKeyDown(longAttackKey) && !isAttacking)
        {
            StartLongAttack();
            return;
        }

        // Get movement input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        movementInput = new Vector2(horizontal, vertical);

        if (!isDashing)
        {
            if (isAttacking)
            {
                // Skip movement animations while attacking
                return;
            }

            // Handle movement animations
            HandleMovementAnimations();

            // Start dash if Shift key is pressed
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                StartDash();
            }
        }
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            // Perform dashing movement
            rb.MovePosition(rb.position + dashDirection * (dashDistance / dashDuration) * Time.fixedDeltaTime);

            // End dash if duration has passed
            if (Time.time >= dashEndTime)
            {
                isDashing = false;
            }
        }
        else
        {
            // Perform normal movement
            if (movementInput.magnitude > 0)
            {
                Vector2 newPosition = rb.position + movementInput.normalized * moveSpeed * Time.fixedDeltaTime;
                rb.MovePosition(newPosition);
            }
        }
    }

    private void StartCloseAttack()
    {
        isAttacking = true;
        animator.Play(StateCloseAttack); // Play the close attack animation

        // Reset attack state after animation finishes
        float animationLength = GetAnimationLength(StateCloseAttack);
        if (animationLength > 0)
        {
            Invoke(nameof(EndAttack), animationLength);
        }
        else
        {
            Debug.LogWarning("CloseAttack animation length not found, defaulting to 1 second.");
            Invoke(nameof(EndAttack), 1.0f); // Default to 1 second as a fallback
        }
    }

    private void StartLongAttack()
    {
        isAttacking = true;
        animator.Play(StateLongAttack); // Play the long attack animation

        // Launch weapon after the animation finishes
        float animationLength = GetAnimationLength(StateLongAttack);
        if (animationLength > 0)
        {
            Invoke(nameof(LaunchWeapon), animationLength); // Launch weapon after animation ends
            Invoke(nameof(EndAttack), animationLength);
        }
        else
        {
            Debug.LogWarning("LongAttack animation length not found, defaulting to 1 second.");
            Invoke(nameof(LaunchWeapon), 1.0f); // Default to 1 second as a fallback
            Invoke(nameof(EndAttack), 1.0f);
        }
    }

    private void LaunchWeapon()
    {
        // Instantiate the weapon prefab
        GameObject weapon = Instantiate(weaponPrefab, transform.position, Quaternion.identity);

        // Determine weapon direction
        Vector2 weaponDirection = movementInput.normalized;
        if (weaponDirection == Vector2.zero)
        {
            weaponDirection = Vector2.up; // Default direction if no input
        }

        // Set weapon velocity
        Rigidbody2D weaponRb = weapon.GetComponent<Rigidbody2D>();
        if (weaponRb != null)
        {
            weaponRb.bodyType = RigidbodyType2D.Dynamic; // Ensure it's dynamic
            weaponRb.linearVelocity = weaponDirection * weaponSpeed;

            // Add no-bounce Physics Material
            PhysicsMaterial2D noBounceMaterial = new PhysicsMaterial2D("NoBounce");
            noBounceMaterial.bounciness = 0f; // Prevent bouncing
            noBounceMaterial.friction = 0f; // Optional: Make it slide without friction
            Collider2D weaponCollider = weapon.GetComponent<Collider2D>();
            if (weaponCollider != null)
            {
                weaponCollider.sharedMaterial = noBounceMaterial;
            }
        }

        // Disable collision between the player and the weapon
        Collider2D playerCollider = GetComponent<Collider2D>();
        if (playerCollider != null)
        {
            Collider2D weaponCollider = weapon.GetComponent<Collider2D>();
            if (weaponCollider != null)
            {
                Physics2D.IgnoreCollision(weaponCollider, playerCollider);
            }
        }

        // Add collision handler for stopping the weapon on impact
        weapon.AddComponent<WeaponStopper>();

        // Track weapon distance
        StartCoroutine(TrackWeaponDistance(weapon, transform.position, dashDistance));
    }

    private IEnumerator TrackWeaponDistance(GameObject weapon, Vector2 startPosition, float maxDistance)
    {
        Rigidbody2D weaponRb = weapon.GetComponent<Rigidbody2D>();

        while (weapon != null && Vector2.Distance(weapon.transform.position, startPosition) < maxDistance)
        {
            yield return null; // Wait until the next frame
        }

        if (weapon != null)
        {
            // Stop the weapon's movement
            if (weaponRb != null)
            {
                weaponRb.linearVelocity = Vector2.zero;
                weaponRb.angularVelocity = 0;
                weaponRb.freezeRotation = true;
            }

            // Start fade-out and destroy the weapon
            StartCoroutine(FadeOutAndDestroy(weapon));
        }
    }

    public IEnumerator FadeOutAndDestroy(GameObject weapon)
    {
        if (weapon == null)
        {
            yield break; // Exit if the weapon is already destroyed
        }

        SpriteRenderer spriteRenderer = weapon.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("Weapon prefab is missing a SpriteRenderer component.");
            Destroy(weapon); // Destroy immediately if no SpriteRenderer
            yield break;
        }

        float fadeDuration = 3.0f; // Duration of the fade-out
        Color originalColor = spriteRenderer.color;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            if (weapon == null || spriteRenderer == null)
            {
                yield break; // Exit if the weapon or SpriteRenderer is destroyed during the fade-out
            }

            float alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Ensure alpha is fully 0 and destroy the object
        if (weapon != null)
        {
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
            Destroy(weapon);
        }
    }

    private void StopWeapon(GameObject weapon)
    {
        // Stop rotation
        Rigidbody2D weaponRb = weapon.GetComponent<Rigidbody2D>();
        if (weaponRb != null)
        {
            weaponRb.angularVelocity = 0;
            weaponRb.freezeRotation = true;
        }

        // Start fade-out coroutine
        StartCoroutine(FadeOutAndDestroy(weapon));
    }

    private void EndAttack()
    {
        isAttacking = false;
    }

    private void HandleMovementAnimations()
    {
        if (movementInput != Vector2.zero)
        {
            if (movementInput.x > 0)
            {
                animator.Play(StateRunRight);
            }
            else if (movementInput.x < 0)
            {
                animator.Play(StateRunLeft);
            }
            else if (movementInput.y > 0)
            {
                animator.Play(StateRunBack);
            }
            else if (movementInput.y < 0)
            {
                animator.Play(StateRunFront);
            }
        }
        else
        {
            animator.Play(StateIdle);
        }
    }

    private void StartDash()
    {
        if (movementInput.magnitude > 0)
        {
            isDashing = true;
            dashDirection = movementInput.normalized;
            dashEndTime = Time.time + dashDuration;
        }
    }

    private float GetAnimationLength(string animationName)
    {
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;

        foreach (var clip in ac.animationClips)
        {
            if (clip.name == animationName)
            {
                return clip.length;
            }
        }

        Debug.LogWarning($"Animation '{animationName}' not found!");
        return 0f;
    }
}
