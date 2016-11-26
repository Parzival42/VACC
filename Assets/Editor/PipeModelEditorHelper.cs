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
        EditorWindow window = GetWindow(typeof(PipeModelEditorHelper), true);
        window.titleContent = new GUIContent("Pipe Water Simulation Debug View");
    }

    private void OnGUI()
    {
        InitStyles();
        ComputeMeshPaintWaterPipe waterSimulation = FindObjectOfType<ComputeMeshPaintWaterPipe>();

        if (waterSimulation != null)
        {
            GUILayout.Label("Pipe water simulation script found! (" + waterSimulation.name + ")", titleStyle);

            // Draw Flux
            DrawFlux(waterSimulation);

            // Draw water and terrain texture
            DrawWaterAndTerrain(waterSimulation);

            if(!EditorApplication.isPlaying)
                GUILayout.Label("The render textures are only shown in Play-Mode!");
        }
        else
            GUILayout.Label("No Pipe model script found! You made everything wrong! :D", titleStyle);
    }

    private void DrawWaterAndTerrain(ComputeMeshPaintWaterPipe waterSimulation)
    {
        EditorGUILayout.BeginHorizontal();
            ShowImage("Water:", waterSimulation.WaterHeight, 320, 320);
            ShowImage("Terrain:", waterSimulation.TerrainHeight, 320, 320);
        EditorGUILayout.EndHorizontal();
    }

    private void DrawFlux(ComputeMeshPaintWaterPipe waterSimulation)
    {
        EditorGUILayout.BeginHorizontal();
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
