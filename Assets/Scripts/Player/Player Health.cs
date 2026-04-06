using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = 100f;
    private float oldHealth;
    private static PlayerHealth instance;
    public static float CurrentHealth { get; private set; }
    public static event Action OnHealthChanged;

    public void Awake()
    {
        instance = this;
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

    private void Heal(float healAmount)
    {
        oldHealth = currentHealth;
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        CurrentHealth = currentHealth;
        OnHealthChanged?.Invoke();
    }

    private void TakeDamage(float damageAmount)
    {
        oldHealth = currentHealth;
        currentHealth -= damageAmount;
        CurrentHealth = currentHealth;
        OnHealthChanged?.Invoke();
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
