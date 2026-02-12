using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoomCreator))]
public class RoomCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        RoomCreator roomCreator = (RoomCreator)target;
        if (GUILayout.Button("Create New Room"))
        {
            roomCreator.InitializeLevel();
        }
    }
}