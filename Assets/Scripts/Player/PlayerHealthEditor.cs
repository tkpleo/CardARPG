using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerHealth))]
public class PlayerHealthEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlayerHealth playerHealth = (PlayerHealth)target;

        GUILayout.Space(10);
        GUILayout.Label("Debug Actions", EditorStyles.boldLabel);

        if (GUILayout.Button("Heal (Static)"))
        {
            PlayerHealth.Heal_Static(1f); // You can change the heal amount as needed
        }
        if (GUILayout.Button("Take Damage (Static)"))
        {
            PlayerHealth.TakeDamage_Static(1f); // You can change the damage amount as needed
        }
    }
}
