using UnityEngine;
using UnityEngine.UI;
public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private GameObject healthBarUI;
    [SerializeField] private Slider healthSlider;
    private float currentSliderHealth;

    private void Start()
    {
        if (healthBarUI == null)
            Debug.LogWarning("PlayerHealthBar is missing a reference to the health bar UI GameObject.");
        if (healthSlider == null)
            Debug.LogWarning("PlayerHealthBar is missing a reference to the health slider component.");

        healthSlider.maxValue = PlayerHealth.CurrentHealth;
        healthSlider.value = PlayerHealth.CurrentHealth;
    }

    private void OnEnable()
    {
        PlayerHealth.OnHealthChanged += UpdateHealthBar;
    }

    private void OnDisable()
    {
        PlayerHealth.OnHealthChanged -= UpdateHealthBar;
    }

    private void UpdateHealthBar()
    {
        if (healthSlider != null && PlayerHealth.CurrentHealth != currentSliderHealth)
        {
            healthSlider.value = PlayerHealth.CurrentHealth;
            currentSliderHealth = PlayerHealth.CurrentHealth;
        }
    }
}
