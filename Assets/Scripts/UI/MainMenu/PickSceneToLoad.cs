using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PickSceneToLoad : MonoBehaviour
{
    protected string[] allSceneNames;
    [SerializeField] private TMP_Dropdown sceneDropdown;

    private void Start()
    {
        LoadBuildScene();
        LoadSceneNamesIntoDropdown();

        if(sceneDropdown != null)
            sceneDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        else
            Debug.LogError("Scene Dropdown reference is not set. Please assign it in the inspector.");
    }

    private void LoadSceneNamesIntoDropdown()
    {
        if (sceneDropdown == null)
        {
            Debug.LogError("Scene Dropdown reference is not set. Please assign it in the inspector.");
            return;
        }

        sceneDropdown.ClearOptions();
        List<string> options = new List<string>(allSceneNames);
        sceneDropdown.AddOptions(options);
    }

    private void LoadBuildScene()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        List<string> sceneNames = new List<string>();

        for (int i = 0; i < sceneCount; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
            sceneNames.Add(sceneName);
        }

        allSceneNames = sceneNames.ToArray();
    }

    public void OnDropdownValueChanged(int index)
    {
        if (index < 0 || index >= allSceneNames.Length)
        {
            Debug.LogError("Selected index is out of range. Please check the dropdown options.");
            return;
        }

        string selectedScene = allSceneNames[index];

        var parent = sceneDropdown.transform.parent;

        PlayButton playButton = parent.GetComponentInChildren<PlayButton>();


        if(playButton != null)
            playButton.sceneToLoad = selectedScene;
        else
            Debug.LogError("PlayButton component not found in parent. Please ensure it is correctly set up.");
    }
}
