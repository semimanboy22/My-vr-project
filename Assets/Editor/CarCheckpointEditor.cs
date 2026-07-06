using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Car))]
public class CarCheckpointEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
}
