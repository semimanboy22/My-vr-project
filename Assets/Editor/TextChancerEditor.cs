using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(textchancer))]
public class TextChancerEditor : Editor
{
    private SerializedProperty pagesProperty;
    private SerializedProperty startPageIndexProperty;
    private SerializedProperty nextButtonProperty;
    private SerializedProperty previousButtonProperty;
    private SerializedProperty nextInteractableProperty;
    private SerializedProperty previousInteractableProperty;

    private void OnEnable()
    {
        pagesProperty = serializedObject.FindProperty("pages");
        startPageIndexProperty = serializedObject.FindProperty("startPageIndex");
        nextButtonProperty = serializedObject.FindProperty("nextButton");
        previousButtonProperty = serializedObject.FindProperty("previousButton");
        nextInteractableProperty = serializedObject.FindProperty("nextInteractable");
        previousInteractableProperty = serializedObject.FindProperty("previousInteractable");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw pages list
        EditorGUILayout.PropertyField(pagesProperty, new GUIContent("Pages"), true);

        EditorGUILayout.Space();

        // Draw start page dropdown
        DrawStartPageDropdown();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("UI Buttons (Optional)", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(nextButtonProperty);
        EditorGUILayout.PropertyField(previousButtonProperty);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("XR Interactables (Optional)", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(nextInteractableProperty, new GUIContent("Next Interactable"));
        EditorGUILayout.PropertyField(previousInteractableProperty, new GUIContent("Previous Interactable"));

        EditorGUILayout.HelpBox("Assign either UI Buttons OR XR Interactables (or both). Both will work together.", MessageType.Info);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawStartPageDropdown()
    {
        List<string> pageNames = new List<string>();

        // Get all page names
        for (int i = 0; i < pagesProperty.arraySize; i++)
        {
            SerializedProperty pageElement = pagesProperty.GetArrayElementAtIndex(i);
            SerializedProperty pageNameProperty = pageElement.FindPropertyRelative("pageName");

            string pageName = pageNameProperty.stringValue;
            if (string.IsNullOrEmpty(pageName))
            {
                pageName = "Page " + i;
            }

            pageNames.Add(pageName);
        }

        // If no pages, show a message
        if (pageNames.Count == 0)
        {
            EditorGUILayout.HelpBox("Create at least one page first", MessageType.Info);
            return;
        }

        // Draw the dropdown
        int currentIndex = startPageIndexProperty.intValue;
        if (currentIndex >= pageNames.Count)
        {
            currentIndex = 0;
            startPageIndexProperty.intValue = 0;
        }

        int newIndex = EditorGUILayout.Popup("Start Page", currentIndex, pageNames.ToArray());

        if (newIndex != currentIndex)
        {
            startPageIndexProperty.intValue = newIndex;
        }
    }
}
