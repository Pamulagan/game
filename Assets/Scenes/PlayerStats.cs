using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    // Player's HP (Health Points)
    public int maxHP = 5;
    private int currentHP;

    // Player's Mana
    public int maxMana = 10;
    private int currentMana;

    // Events to trigger when stats change
    public delegate void OnHPChanged(int currentHP);
    public event OnHPChanged HPChanged;

    public delegate void OnManaChanged(int currentMana);
    public event OnManaChanged ManaChanged;

    void Start()
    {
        // Initialize HP and Mana at the start of the game
        currentHP = maxHP;
        currentMana = maxMana;

        // Print initial values to the Console
        PrintStats();
    }

    void Update()
    {
        // Example for testing: Decrease HP and Mana when pressing specific keys
        if (Input.GetKeyDown(KeyCode.H)) // Decrease HP by 1 when "H" is pressed
        {
            TakeDamage(1);
        }

        if (Input.GetKeyDown(KeyCode.M)) // Decrease Mana by 1 when "M" is pressed
        {
            UseMana(1);
        }
    }

    // Function to deal damage to the player (decrease HP)
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP); // Prevent HP from going below 0

        // Trigger HP changed event
        HPChanged?.Invoke(currentHP);

        // Print updated HP to the Console
        PrintStats();
    }

    // Function to heal the player (increase HP)
    public void Heal(int amount)
    {
        currentHP += amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP); // Prevent HP from going above maxHP

        // Trigger HP changed event
        HPChanged?.Invoke(currentHP);

        // Print updated HP to the Console
        PrintStats();
    }

    // Function to use mana (decrease mana)
    public void UseMana(int amount)
    {
        currentMana -= amount;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana); // Prevent Mana from going below 0

        // Trigger Mana changed event
        ManaChanged?.Invoke(currentMana);

        // Print updated Mana to the Console
        PrintStats();
    }

    // Function to restore mana
    public void RestoreMana(int amount)
    {
        currentMana += amount;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana); // Prevent Mana from going above maxMana

        // Trigger Mana changed event
        ManaChanged?.Invoke(currentMana);

        // Print updated Mana to the Console
        PrintStats();
    }

    // Function to get current HP
    public int GetCurrentHP()
    {
        return currentHP;
    }

    // Function to get current Mana
    public int GetCurrentMana()
    {
        return currentMana;
    }

    // Function to print the current HP and Mana to the Console
    private void PrintStats()
    {
        Debug.Log($"Current HP: {currentHP}/{maxHP}");
        Debug.Log($"Current Mana: {currentMana}/{maxMana}");
    }
}
