using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(VacuumSound))]
class VacuumSoundEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        VacuumSound script = (VacuumSound)target;
        if (GUILayout.Button("Start Engine"))
        {
            script.StartEngine();
        }

        if (GUILayout.Button("Stop Engine"))
        {
            script.StopEngine();
        }

    }
}

