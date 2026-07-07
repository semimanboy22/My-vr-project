using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(switchscenescript))]
public class SwitchSceneScriptEditor : Editor
{
    private GUIContent[] sceneOptions;
    private string[] scenePaths;
    private int selectedSceneIndex;
    private GUIContent[] xrRigOptions;
    private int selectedXRRigIndex;
    private Scene previewScene;
    private bool sceneLoaded = false;

    private void OnEnable()
    {
        RefreshSceneList();
    }

    private void RefreshSceneList()
    {
        List<GUIContent> sceneOptionsList = new List<GUIContent>();
        List<string> scenePathsList = new List<string>();

        // Find all .unity files in the Assets folder
        string[] guids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" });

        foreach (string guid in guids)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);
            string sceneName = Path.GetFileNameWithoutExtension(scenePath);

            sceneOptionsList.Add(new GUIContent(sceneName));
            scenePathsList.Add(scenePath);
        }

        sceneOptions = sceneOptionsList.ToArray();
        scenePaths = scenePathsList.ToArray();

        // Get the current target scene index from the serialized property
        var targetSceneIndexProp = serializedObject.FindProperty("targetSceneIndex");
        selectedSceneIndex = targetSceneIndexProp.intValue;

        // Clamp the index to valid range
        if (selectedSceneIndex >= sceneOptions.Length)
        {
            selectedSceneIndex = 0;
            targetSceneIndexProp.intValue = 0;
        }

        // Refresh XR Rigs for the selected scene
        RefreshXRRigs();
    }

    private void RefreshXRRigs()
    {
        if (selectedSceneIndex < 0 || selectedSceneIndex >= scenePaths.Length)
        {
            xrRigOptions = new GUIContent[] { new GUIContent("No scene selected") };
            return;
        }

        List<GUIContent> xrRigOptionsList = new List<GUIContent>();

        // Load the scene additively to find XR Origins
        string scenePath = scenePaths[selectedSceneIndex];
        Scene loadedScene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);

        // Find all XR Origins in the scene
        GameObject[] allObjects = loadedScene.GetRootGameObjects();
        FindXROrigins(allObjects, xrRigOptionsList);

        // If no XR Origins found, show a message
        if (xrRigOptionsList.Count == 0)
        {
            xrRigOptionsList.Add(new GUIContent("No XR Origins found in scene"));
        }

        xrRigOptions = xrRigOptionsList.ToArray();

        // Get the current XR Rig name
        var xrRigNameProp = serializedObject.FindProperty("xrRigName");
        string currentXRRigName = xrRigNameProp.stringValue;

        // Find the index of the current XR Rig
        selectedXRRigIndex = 0;
        for (int i = 0; i < xrRigOptions.Length; i++)
        {
            if (xrRigOptions[i].text == currentXRRigName)
            {
                selectedXRRigIndex = i;
                break;
            }
        }

        // Unload the scene
        EditorSceneManager.CloseScene(loadedScene, true);
    }

    private void FindXROrigins(GameObject[] objects, List<GUIContent> xrRigList)
    {
        foreach (GameObject obj in objects)
        {
            // Check if this object or any child has "XR Origin" or "XR Rig" in the name
            if (obj.name.Contains("XR Origin") || obj.name.Contains("XR Rig") || obj.name.Contains("XROrigin") || obj.name.Contains("XRRig"))
            {
                xrRigList.Add(new GUIContent(obj.name));
            }

            // Check children
            if (obj.transform.childCount > 0)
            {
                List<GameObject> childList = new List<GameObject>();
                foreach (Transform child in obj.transform)
                {
                    childList.Add(child.gameObject);
                }
                FindXROrigins(childList.ToArray(), xrRigList);
            }
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Scene Switcher Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (sceneOptions.Length == 0)
        {
            EditorGUILayout.HelpBox("No scenes found in Assets folder!", MessageType.Warning);
            if (GUILayout.Button("Refresh Scene List", GUILayout.Height(30)))
            {
                RefreshSceneList();
            }
            return;
        }

        // Scene dropdown
        EditorGUILayout.LabelField("Target Scene", EditorStyles.label);
        var targetSceneIndexProp = serializedObject.FindProperty("targetSceneIndex");
        int newSceneIndex = EditorGUILayout.Popup(selectedSceneIndex, sceneOptions);

        // If scene selection changed, refresh XR Rigs
        if (newSceneIndex != selectedSceneIndex)
        {
            selectedSceneIndex = newSceneIndex;
            targetSceneIndexProp.intValue = selectedSceneIndex;
            RefreshXRRigs();
        }

        EditorGUILayout.Space();

        // XR Rig dropdown
        EditorGUILayout.LabelField("XR Rig in Target Scene", EditorStyles.label);
        var xrRigNameProp = serializedObject.FindProperty("xrRigName");

        if (xrRigOptions.Length > 0)
        {
            selectedXRRigIndex = EditorGUILayout.Popup(selectedXRRigIndex, xrRigOptions);

            // Update the XR Rig name if it's a valid selection
            if (selectedXRRigIndex >= 0 && selectedXRRigIndex < xrRigOptions.Length && 
                !xrRigOptions[selectedXRRigIndex].text.Contains("No XR Origins"))
            {
                xrRigNameProp.stringValue = xrRigOptions[selectedXRRigIndex].text;
            }
        }
        else
        {
            EditorGUILayout.HelpBox("No XR Origins found in the selected scene.", MessageType.Warning);
        }

        EditorGUILayout.Space();

        // Display current scene info
        if (selectedSceneIndex >= 0 && selectedSceneIndex < sceneOptions.Length)
        {
            string scenePath = scenePaths[selectedSceneIndex];
            string sceneName = sceneOptions[selectedSceneIndex].text;
            string xrRigName = xrRigNameProp.stringValue;
            EditorGUILayout.HelpBox($"Scene: {sceneName}\nPath: {scenePath}\nXR Rig: {xrRigName}", MessageType.Info);
        }

        EditorGUILayout.Space();

        // Refresh button
        if (GUILayout.Button("Refresh Scene & XR Rig List", GUILayout.Height(30)))
        {
            RefreshSceneList();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
