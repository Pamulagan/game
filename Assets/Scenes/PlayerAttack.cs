using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public KeyCode attackKey = KeyCode.Space; // Key to trigger the attack
    private Animator animator;
    private bool isAttacking = false; // Local tracking of attack state

    public bool IsAttacking => isAttacking; // Public getter for other scripts to check attack state

    void Start()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator component not found!");
        }
    }

    void Update()
    {
        // Check for attack input
        if (Input.GetKeyDown(attackKey) && !isAttacking)
        {
            StartAttack();
        }
    }

    private void StartAttack()
    {
        isAttacking = true;
        animator.SetBool("isAttacking", true); // Set the Animator parameter

        // Reset attack state after animation finishes
        float animationLength = GetAnimationLength("CloseAttack");
        if (animationLength > 0)
        {
            Invoke(nameof(EndAttack), animationLength);
        }
    }

    private void EndAttack()
    {
        isAttacking = false;
        animator.SetBool("isAttacking", false); // Reset the Animator parameter
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
