using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public static bool IsPaused {get; private set;}

    [SerializeField] private PlayerInput pauseActionMap;

    [SerializeField] private InputActionReference pauseActionReference;

    [Space(10)]
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject overlayUI;
    private Vector3 originalPauseMenuPosition;

    private void OnEnable()
    {
        pauseActionReference.action.performed += TogglePause;
    }

    private void OnDisable()
    {
        pauseActionReference.action.performed -= TogglePause;
    }

    private void Start()
    {
        if(!IsPaused)
            pauseMenuUI.SetActive(false);

        originalPauseMenuPosition = pauseMenuUI.transform.position;
        EnsureOverlayStarting();
    }

    private void TogglePause(InputAction.CallbackContext context)
    {
        if (IsPaused)
            ResumeGame();
        else
            PauseGame();
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        IsPaused = true;
        SwapControls("UI");
        StartCoroutine(SlidePauseMenuIn());
        
    }

    private void SwapControls(string swapMap)
    {
        pauseActionMap.SwitchCurrentActionMap(swapMap);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        SwapControls("Player");
        StartCoroutine(SlidePauseMenuOut());
    }

    private void EnsureOverlayStarting()
    {
        Image overlayImage = overlayUI.GetComponent<Image>();
        if (overlayImage != null)
        {
            overlayImage.color = new Color(0, 0, 0, 0); // Start fully transparent
        }
    }

    private IEnumerator FadeInOverlay()
    {
        Image overlayImage = overlayUI.GetComponent<Image>();
        if (overlayImage == null) yield break;

        float elapsedTime = 0f;
        float duration = 0.5f; // Duration of the fade
        Color startColor = overlayImage.color; // Current color (should be fully transparent)
        Color targetColor = new Color(0, 0, 0, 0.5f); // Semi-transparent black

        while (elapsedTime < duration)
        {
            overlayImage.color = Color.Lerp(startColor, targetColor, elapsedTime / duration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        overlayImage.color = targetColor; // Ensure it ends at the exact target color
    }

    private IEnumerator FadeOutOverlay()
    {
        Image overlayImage = overlayUI.GetComponent<Image>();
        if (overlayImage == null) yield break;

        float elapsedTime = 0f;
        float duration = 0.5f; // Duration of the fade
        Color startColor = overlayImage.color; // Current color (should be semi-transparent black)
        Color targetColor = new Color(0, 0, 0, 0); // Fully transparent

        while (elapsedTime < duration)
        {
            overlayImage.color = Color.Lerp(startColor, targetColor, elapsedTime / duration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        overlayImage.color = targetColor; // Ensure it ends at the exact target color
    }

    private IEnumerator SlidePauseMenuIn()
    {
        float elapsedTime = 0f;
        float duration = 0.5f; // Duration of the animation
        pauseMenuUI.SetActive(true);
        Vector3 startPosition = originalPauseMenuPosition;
        Vector3 targetPosition = originalPauseMenuPosition + new Vector3(514, 0, 0); // Start above the original position;
        StartCoroutine(FadeInOverlay());

        while (elapsedTime < duration)
        {
            pauseMenuUI.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        pauseMenuUI.transform.position = targetPosition; // Ensure it ends at the exact target position
    }

    private IEnumerator SlidePauseMenuOut()
    {
        float elapsedTime = 0f;
        float duration = 0.5f; // Duration of the animation
        Vector3 startPosition = pauseMenuUI.transform.position; // Current position (should be the target position from SlidePauseMenuIn)
        Vector3 targetPosition = originalPauseMenuPosition; // Slide back to the original position

        while (elapsedTime < duration)
        {
            pauseMenuUI.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        StartCoroutine(FadeOutOverlay());
        pauseMenuUI.transform.position = targetPosition; // Ensure it ends at the exact target position
        pauseMenuUI.SetActive(false);
    }
}
