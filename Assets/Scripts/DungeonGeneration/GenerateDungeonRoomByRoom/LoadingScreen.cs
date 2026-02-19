using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreenPrefab; // Assign this in the inspector with your loading screen UI prefab

    public IEnumerator LoadingScreenCoroutine(){

        Time.timeScale = 0f; // Pause the game during loading
        StartCoroutine(TurnLoadingScreenOn(0.5f, loadingScreenPrefab));
        yield return new WaitForSecondsRealtime(2f); // Simulate loading time
        yield return StartCoroutine(TurnLoadingScreenOff(0.5f, loadingScreenPrefab));
        Time.timeScale = 1f; // Resume the game after loading
    }

    private IEnumerator TurnLoadingScreenOn(float fadeDuration, GameObject loadingScreen)
    {
        Color color = loadingScreen.GetComponent<Image>().material.color;
        color.a = 0f; // Start fully transparent

        loadingScreen.gameObject.SetActive(true); // Ensure the loading screen is active before fading in

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            color.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            loadingScreen.GetComponent<Image>().material.color = color;
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        loadingScreen.GetComponent<Image>().material.color = new Color(color.r, color.g, color.b, 1f); // Ensure it's fully visible at the end
    }

    private IEnumerator TurnLoadingScreenOff(float fadeDuration, GameObject loadingScreen)
    {
        Color color = loadingScreen.GetComponent<Image>().material.color;

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            loadingScreen.GetComponent<Image>().material.color = color;
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        loadingScreen.GetComponent<Image>().material.color = new Color(color.r, color.g, color.b, 0f); // Ensure it's fully invisible at the end
        loadingScreen.gameObject.SetActive(false); // Deactivate the loading screen after fading out
    }

}
