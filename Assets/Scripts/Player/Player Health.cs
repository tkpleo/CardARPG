using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class PlayerHealth : MonoBehaviour
{
    public const int startingMaxHealth = 3;
    [SerializeField] private int maxHealth;
    [SerializeField] private float currentHealth;
    private float oldHealth;
    private static PlayerHealth instance;
    public static float CurrentHealth { get; private set; }
    public static event Action OnHealthChanged;

    public void Awake()
    {
        instance = this;
        maxHealth = startingMaxHealth;
        currentHealth = maxHealth;
        oldHealth = currentHealth;
        CurrentHealth = currentHealth;
    }

    public static PlayerHealth Heal_Static(float healAmount)
    {
        instance.Heal(healAmount);
        return instance;
    }
    public static PlayerHealth TakeDamage_Static(float damageAmount)
    {
        instance.TakeDamage(damageAmount);
        return instance;
    }

    public static PlayerHealth TryKillPlayer_Static()
    {
        instance.TryKillPlayer();
        return instance;
    }

    public static PlayerHealth IncreaseMaxHealth_Static(int amount)
    {
        instance.IncreaseMaxHealth(amount);
        return instance;
    }

    private void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
    }

    public static PlayerHealth FullHeal_Static()
    {
        instance.FullHeal();
        return instance;
    }

    private void FullHeal()
    {
        oldHealth = currentHealth;
        currentHealth = maxHealth;
        CurrentHealth = currentHealth;
        OnHealthChanged?.Invoke();
    }

    private void Heal(float healAmount)
    {
        oldHealth = currentHealth;
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        CurrentHealth = currentHealth;
        PlayerHealthBar.SetSegmentToGainHealth_Static();
        OnHealthChanged?.Invoke();
    }

    private void TakeDamage(float damageAmount)
    {
        oldHealth = currentHealth;
        currentHealth -= damageAmount;
        CurrentHealth = currentHealth;
        OnHealthChanged?.Invoke();
        PlayerHealthBar.SetSegmentToLoseHealth_Static();
        if (currentHealth <= 0f)
        {
            PlayerHealth.TryKillPlayer_Static();
        }
    }

    private void TryKillPlayer()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        currentHealth = maxHealth;
        CurrentHealth = currentHealth;
        OnHealthChanged?.Invoke();
    }
}
