using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(VacuumSound))]
class VacuumSoundEditor : Editor
{
    // Reference to VacuumSound
    private VacuumSound vacuumSound;
    private SerializedObject serializedVacuumSound;

    /// <summary>
    /// Called everytime the inspected object is selected.
    /// </summary>
    private void OnEnable()
    {
        vacuumSound = (VacuumSound)target;
        serializedVacuumSound = new SerializedObject(vacuumSound);
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        VacuumSound script = (VacuumSound)target;
        if (GUILayout.Button("Start Engine"))
        {
            if (Application.isPlaying)
            {
                script.StartEngine();
            }
        }

        if (GUILayout.Button("Stop Engine"))
        {
            if (Application.isPlaying)
            {
                script.StopEngine();
            }
        }

        if (GUI.changed)
            EditorUtility.SetDirty(vacuumSound);

        serializedVacuumSound.ApplyModifiedProperties();
    }
}

