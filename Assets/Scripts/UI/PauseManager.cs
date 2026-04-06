using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public static bool IsPaused {get; private set;}

    [SerializeField] private InputActionReference pauseActionReference;

    [Space(10)]
    [SerializeField] private GameObject pauseMenuUI;

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
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
    }
}
