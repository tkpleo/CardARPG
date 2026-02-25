using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayButton : MonoBehaviour
{
    [SerializeField] public string sceneToLoad = "";

    public void StartGame()
    {
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError("Scene to load is not set. Please assign a scene name in the inspector.");
            return;
        }

        SceneManager.LoadScene(sceneToLoad);
    }
}
