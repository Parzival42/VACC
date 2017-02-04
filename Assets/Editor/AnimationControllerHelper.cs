using UnityEditor;
using UnityEngine;

public class AnimationControllerHelper : EditorWindow
{
    // Styles
    private GUIStyle titleStyle;
    private GUIStyle centeredStyle;

    [MenuItem("Window/Animation Helper/Animation Helper")]
    public static void ShowWindow()
    {
        EditorWindow window = GetWindow(typeof(AnimationControllerHelper), true);
        window.titleContent = new GUIContent("Animation Controller Helper");
    }

    private void OnGUI()
    {
        InitStyles();
        AnimationManager animManager = FindObjectOfType<AnimationManager>();

        if (animManager != null)
        {
            if (GUILayout.Button("Animate Car Light"))
                animManager.PlayCarLight();

            if (GUILayout.Button("Light Chromatic Aberration"))
                animManager.LightChromaticDispersion(1f);

            if (GUILayout.Button("Fancy Chromatic Aberration"))
                animManager.FancyChromaticDispersion(1f);
        }
        else
            GUILayout.Label("No Animation Manager :(", titleStyle);
    }

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
