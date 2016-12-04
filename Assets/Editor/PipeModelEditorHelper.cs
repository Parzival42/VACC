using UnityEngine;
using UnityEditor;
using System.Collections;

public class PipeModelEditorHelper : EditorWindow
{
    // Styles
    private GUIStyle titleStyle;
    private GUIStyle centeredStyle;
    private GUIStyle marginStyle;

    [MenuItem("Window/Water Simulation/Pipe Model Water Helper")]
    public static void ShowWindow()
    {
        EditorWindow window = GetWindow(typeof(PipeModelEditorHelper), true);
        window.titleContent = new GUIContent("Pipe Water Simulation Debug View");
    }

    private void Update()
    {
        Repaint();
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
            DrawWaterTerrainVelocity(waterSimulation);

            if(!EditorApplication.isPlaying)
                GUILayout.Label("The render textures are only shown in Play-Mode!");
        }
        else
            GUILayout.Label("No Pipe model script found! You made everything wrong! :D", titleStyle);
    }

    private void DrawWaterTerrainVelocity(ComputeMeshPaintWaterPipe waterSimulation)
    {
        float size = EditorGUIUtility.currentViewWidth / 4.3f;
        EditorGUILayout.BeginHorizontal();
            ShowImage("Water:", waterSimulation.WaterHeight, size, size);
            ShowImage("Terrain:", waterSimulation.TerrainHeight, size, size);

            ShowImage("Velocity X:", waterSimulation.VelocityX, size, size);
            ShowImage("Velocity Y:", waterSimulation.VelocityY, size, size);
        EditorGUILayout.EndHorizontal();
    }

    private void DrawFlux(ComputeMeshPaintWaterPipe waterSimulation)
    {
        float size = EditorGUIUtility.currentViewWidth / 4.3f;
        EditorGUILayout.BeginHorizontal();
            ShowImage("Flux Left:", waterSimulation.FluxLeft, size, size);
            ShowImage("Flux Right:", waterSimulation.FluxRight, size, size);
            ShowImage("Flux Top:", waterSimulation.FluxTop, size, size);
            ShowImage("Flux Bottom:", waterSimulation.FluxBottom, size, size);
        EditorGUILayout.EndHorizontal();
    }

    private void ShowImage(string label, Texture img, float width, float height)
    {
        EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(label, titleStyle, GUILayout.Width(width));
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

        marginStyle = new GUIStyle();
        marginStyle.margin = new RectOffset(0, 0, 0, 0);
    }
}
