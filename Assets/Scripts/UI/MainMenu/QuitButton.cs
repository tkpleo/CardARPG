using UnityEngine;

public class QuitButton : MonoBehaviour
{
    public void QuitGame()
    {
        Debug.Log("Quit button pressed. Exiting game...");
        Application.Quit();
    }
}
