// Only include this in the Editor
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHealthBar : MonoBehaviour
{
    private static PlayerHealthBar instance;
    [SerializeField] private GameObject healthBarUI;
    [SerializeField] private Image healthSegmentPrefab;

    [SerializeField] internal List<Image> healthSegments = new List<Image>();
    private int currentHealth;
    private string lostHealthHexDecimal = "#FF0000"; 
    private string fullHealthHexDecimal = "#00FF00"; 

    private int startingMaxHealth = PlayerHealth.startingMaxHealth;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (healthBarUI == null)
            Debug.LogWarning("PlayerHealthBar is missing a reference to the health bar UI GameObject.");
 
        PopulateHealthBarWithStartingSegments();
        AddEachHealthSegment();
        SetColorToAllSegments(fullHealthHexDecimal);
        currentHealth = startingMaxHealth;
    }
    
    private void PopulateHealthBarWithStartingSegments()
    {
        for (int i = 0; i < startingMaxHealth; i++)
        {
            InitializeStartingHealthSegments();
        }
    }

    private void SetColorToAllSegments(string hexColor)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(hexColor, out color))
        {
            foreach (Image segment in healthSegments)
            {
                segment.color = color;
            }
        }
        else
        {
            Debug.LogWarning("Invalid color hex string: " + hexColor);
        }
    }

    private void AddEachHealthSegment()
    {
        healthSegments.Clear();
        foreach (Transform child in healthBarUI.transform)
        {
            Image segmentImage = child.GetComponent<Image>();
            if (segmentImage != null)
                healthSegments.Add(segmentImage);
            else
                Debug.LogWarning($"Child GameObject '{child.name}' of health bar UI is missing an Image component.");
        }
    }

    public static PlayerHealthBar AddNewSegment_Static()
    {
        instance.AddNewSegment();
        return instance;
    }

    public static PlayerHealthBar SetSegmentToLoseHealth_Static()
    {
        instance.SetSegmentToLoseHealth();
        return instance;
    }

    public static PlayerHealthBar ResetAllSegmentsToFullHealth_Static()
    {
        instance.ResetAllSegmentsToFullHealth();
        return instance;
    }

    public static PlayerHealthBar SetSegmentToGainHealth_Static()
    {
        instance.SetSegmentToGainHealth();
        return instance;
    }

    private void InitializeStartingHealthSegments()
    {
        if (healthSegmentPrefab == null)
        {
            Debug.LogError("healthSegmentPrefab is null! Please assign a valid prefab in the Inspector.");
            return;
        }
        Image newSegment = Instantiate(healthSegmentPrefab, healthBarUI.transform);
        healthSegments.Add(newSegment);
    }

    private void SetColor(Image segment, string hexColor)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(hexColor, out color))
        {
            segment.color = color;
        }
        else
        {
            Debug.LogWarning("Invalid color hex string: " + hexColor);
        }
    }

    internal void AddNewSegment()
    {
        Debug.Log("healthSegmentPrefab is " + (healthSegmentPrefab == null ? "NULL" : "NOT NULL"));
        if (healthSegmentPrefab == null)
        {
            Debug.LogError("healthSegmentPrefab is null! Please assign a valid prefab in the Inspector.");
            return;
        }
        Image newSegment = Instantiate(healthSegmentPrefab, healthBarUI.transform);
        healthSegments.Add(newSegment);

        SetColor(newSegment, fullHealthHexDecimal);
        
        PlayerHealth.IncreaseMaxHealth_Static(1);
        PlayerHealth.FullHeal_Static();
        currentHealth++;
    }

    private void SetSegmentToLoseHealth()
    {
        if (currentHealth > 0)
        {
            currentHealth--;
            SetColor(healthSegments[currentHealth], lostHealthHexDecimal);
        }
        else
        {
            Debug.LogWarning("PlayerHealthBar: Attempted to remove a health segment when current health is already at zero.");
        }
    }

    private void ResetAllSegmentsToFullHealth()
    {
        SetColorToAllSegments(fullHealthHexDecimal);
    }

    private void SetSegmentToGainHealth()
    {
        if (currentHealth < healthSegments.Count)
        {
            SetColor(healthSegments[currentHealth], fullHealthHexDecimal);
            currentHealth++;
        }
        else
        {
            Debug.LogWarning("Invalid color hex string: " + fullHealthHexDecimal);
        }
    }

}


#if UNITY_EDITOR
    [CustomEditor(typeof(PlayerHealthBar))]
    public class PlayerHealthBarEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            PlayerHealthBar bar = (PlayerHealthBar)target;
            if (GUILayout.Button("Add Health Segment (Runtime)"))
            {
                if (Application.isPlaying)
                {
                    bar.AddNewSegment();
                }
                else
                {
                    Debug.LogWarning("Can only add health segments at runtime.");
                }
            }
        }
    }
#endif