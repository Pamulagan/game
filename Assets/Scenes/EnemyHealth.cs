using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;  // The maximum health the enemy can have
    public int currentHealth;    // The current health of the enemy

    // Start is called before the first frame update
    private void Start()
    {
        // Initialize current health to max health at the start
        currentHealth = maxHealth;
    }

    // Method to take damage
    public void TakeDamage(int damage)
    {
        // Subtract damage from current health
        currentHealth -= damage;

        // Make sure current health doesn't go below 0
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        // Check if the enemy has died
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Method to handle enemy death
    private void Die()
    {
        // You can play a death animation, destroy the enemy, or trigger other behaviors
        Debug.Log("Enemy Died!");
        Destroy(gameObject); // Destroy the enemy object
    }
}
