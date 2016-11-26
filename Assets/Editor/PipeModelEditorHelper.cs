using UnityEngine;
using UnityEditor;
using System.Collections;

public class PipeModelEditorHelper : EditorWindow
{
    // Styles
    private GUIStyle titleStyle;
    private GUIStyle centeredStyle;

    [MenuItem("Window/WaterSimulation/PipeModelWaterHelper")]
    public static void ShowWindow()
    {
        GetWindow(typeof(PipeModelEditorHelper));
    }

    private void OnGUI()
    {
        InitStyles();
        ComputeMeshPaintWaterPipe waterSimulation = FindObjectOfType<ComputeMeshPaintWaterPipe>();

        if (waterSimulation != null)
        {
            GUILayout.Label("Pipe model script found! (" + waterSimulation.name + ")", EditorStyles.label);

            // Draw Flux
            DrawFlux(waterSimulation);

            // Draw water and terrain texture
            DrawWaterAndTerrain(waterSimulation);

        }
        else
            GUILayout.Label("No Pipe model script found! You made everything wrong! :D", EditorStyles.label);
    }

    private void DrawWaterAndTerrain(ComputeMeshPaintWaterPipe waterSimulation)
    {
        EditorGUILayout.BeginHorizontal("box");
            ShowImage("Water:", waterSimulation.WaterHeight, 320, 320);
            ShowImage("Terrain:", waterSimulation.TerrainHeight, 320, 320);
        EditorGUILayout.EndHorizontal();
    }

    private void DrawFlux(ComputeMeshPaintWaterPipe waterSimulation)
    {
        EditorGUILayout.BeginHorizontal("box");
            ShowImage("Flux Left:", waterSimulation.FluxLeft, 256, 256);
            ShowImage("Flux Right:", waterSimulation.FluxRight, 256, 256);
            ShowImage("Flux Top:", waterSimulation.FluxTop, 256, 256);
            ShowImage("Flux Bottom:", waterSimulation.FluxBottom, 256, 256);
        EditorGUILayout.EndHorizontal();
    }

    private void ShowImage(string label, Texture img, float width, float height)
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField(label, titleStyle);
        GUILayout.Label(img, GUILayout.Width(width), GUILayout.Height(height));
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

        centeredStyle = new GUIStyle();
        centeredStyle.alignment = TextAnchor.MiddleCenter;
    }
}
