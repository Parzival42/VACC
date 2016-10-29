using UnityEngine;
using UnityEditor;
using System.Collections;

[CanEditMultipleObjects]
[CustomEditor(typeof(PlaneGenerator))]
public class PlaneGeneratorEditor : Editor
{
    // Reference to the PlaneGenerator
    private PlaneGenerator planeGenerator;

    // Serialized version of the plane generator
    private SerializedObject serializedPlaneGenerator;

    // Styles
    private GUIStyle titleStyle;

    /// <summary>
    /// Called everytime the inspected object is selected.
    /// </summary>
    private void OnEnable()
    {
        planeGenerator = (PlaneGenerator)target;
        serializedPlaneGenerator = new SerializedObject(planeGenerator);

        InitStyles();
    }

    public override void OnInspectorGUI()
    {
        DrawSettings();
        DrawDebugSettings();
        DrawButton();

        if (GUI.changed)
            EditorUtility.SetDirty(planeGenerator);

        serializedPlaneGenerator.ApplyModifiedProperties();
    }

    private void DrawButton()
    {
        if (GUILayout.Button("Generate Mesh!", GUILayout.Height(Screen.height * 0.03f)))
        {
            planeGenerator.GenerateMesh();
        }
    }

    private void DrawDebugSettings()
    {
        SerializedProperty debugDraw = serializedPlaneGenerator.FindProperty("debugDrawVertex");
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Debug", titleStyle);
        EditorGUILayout.PropertyField(debugDraw);
        EditorGUILayout.EndVertical();

    }

    private void DrawSettings()
    {
        // Title
        SerializedProperty serializedX = serializedPlaneGenerator.FindProperty("xSize");
        SerializedProperty serializedY = serializedPlaneGenerator.FindProperty("ySize");
        SerializedProperty serializedVertexOffset = serializedPlaneGenerator.FindProperty("vertexOffset");
        SerializedProperty serializedMeshMaterial = serializedPlaneGenerator.FindProperty("meshMaterial");


        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Mesh settings", titleStyle);
        EditorGUILayout.PropertyField(serializedX);
        EditorGUILayout.PropertyField(serializedY);
        EditorGUILayout.PropertyField(serializedVertexOffset);
        EditorGUILayout.PropertyField(serializedMeshMaterial);
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Inits the styles.
    /// </summary>
    private void InitStyles()
    {
        titleStyle = new GUIStyle();
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.fontSize = 15;
        titleStyle.fontStyle = FontStyle.Bold;
    }
}
