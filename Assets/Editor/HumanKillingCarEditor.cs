using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(humankillingcar))]
public class HumanKillingCarEditor : Editor
{
    private SerializedProperty speedProp;
    private SerializedProperty moveDirectionProp;
    private SerializedProperty maxDistanceProp;
    private SerializedProperty killDistanceProp;
    private SerializedProperty barrierSizeProp;
    private SerializedProperty playerOriginalSpawnPointProp;
    private SerializedProperty respawnableObjectsProp;
    private SerializedProperty checkpointSettingsProp;

    private SerializedObject serializedObject_;
    private humankillingcar targetScript;

    void OnEnable()
    {
        serializedObject_ = serializedObject;
        targetScript = target as humankillingcar;

        speedProp = serializedObject_.FindProperty("speed");
        moveDirectionProp = serializedObject_.FindProperty("moveDirection");
        maxDistanceProp = serializedObject_.FindProperty("maxDistance");
        killDistanceProp = serializedObject_.FindProperty("killDistance");
        barrierSizeProp = serializedObject_.FindProperty("barrierSize");
        playerOriginalSpawnPointProp = serializedObject_.FindProperty("playerOriginalSpawnPoint");
        respawnableObjectsProp = serializedObject_.FindProperty("respawnableObjects");
        checkpointSettingsProp = serializedObject_.FindProperty("checkpointSettings");
    }

    public override void OnInspectorGUI()
    {
        serializedObject_.Update();

        // Movement Section
        EditorGUILayout.LabelField("Movement", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(speedProp);
        EditorGUILayout.PropertyField(moveDirectionProp);
        EditorGUILayout.PropertyField(maxDistanceProp);
        EditorGUILayout.Space();

        // Kill Barrier Section
        EditorGUILayout.LabelField("Kill Barrier", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(killDistanceProp);
        EditorGUILayout.PropertyField(barrierSizeProp);
        EditorGUILayout.Space();

        // Player Respawn Section
        EditorGUILayout.LabelField("Player Respawn", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(playerOriginalSpawnPointProp, new GUIContent("Player Original Spawn Point"));
        EditorGUILayout.Space();

        // Respawnable Objects Section
        EditorGUILayout.LabelField("Respawnable Objects", EditorStyles.boldLabel);
        DrawRespawnableObjectsList();
        EditorGUILayout.Space();

        // Checkpoint Respawn Settings Section
        EditorGUILayout.LabelField("Checkpoint Respawn Settings", EditorStyles.boldLabel);
        DrawCheckpointSettingsList();

        serializedObject_.ApplyModifiedProperties();
    }

    private void DrawRespawnableObjectsList()
    {
        EditorGUILayout.HelpBox("Add objects that should be respawned when hit by the car.", MessageType.Info);

        int listSize = respawnableObjectsProp.arraySize;

        // Add button
        if (GUILayout.Button("Add Respawnable Object", GUILayout.Height(30)))
        {
            respawnableObjectsProp.arraySize++;
        }

        EditorGUILayout.Space();

        // Display list
        List<int> toRemove = new List<int>();

        for (int i = 0; i < listSize; i++)
        {
            var element = respawnableObjectsProp.GetArrayElementAtIndex(i);
            var objProp = element.FindPropertyRelative("obj");
            var defaultRespawnProp = element.FindPropertyRelative("defaultRespawnPoint");
            var checkpointRespawnProp = element.FindPropertyRelative("checkpointRespawnSettings");

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(objProp, new GUIContent($"Object {i + 1}"));

            if (GUILayout.Button("Remove", GUILayout.Width(70)))
            {
                toRemove.Add(i);
            }
            EditorGUILayout.EndHorizontal();

            // Default respawn point
            EditorGUILayout.PropertyField(defaultRespawnProp, new GUIContent("Default Respawn Point"));

            // Checkpoint respawn settings
            EditorGUILayout.LabelField("Checkpoint Respawn Points", EditorStyles.boldLabel);
            int checkpointListSize = checkpointRespawnProp.arraySize;

            if (GUILayout.Button("Add Checkpoint Respawn", GUILayout.Height(25)))
            {
                checkpointRespawnProp.arraySize++;
            }

            List<int> checkpointToRemove = new List<int>();

            for (int j = 0; j < checkpointListSize; j++)
            {
                var checkpointElement = checkpointRespawnProp.GetArrayElementAtIndex(j);
                var orderProp = checkpointElement.FindPropertyRelative("checkpointOrder");
                var respawnPointProp = checkpointElement.FindPropertyRelative("respawnPoint");

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(orderProp, new GUIContent("Checkpoint Order"), GUILayout.Width(200));
                EditorGUILayout.PropertyField(respawnPointProp, new GUIContent("Respawn Point"));

                if (GUILayout.Button("X", GUILayout.Width(25)))
                {
                    checkpointToRemove.Add(j);
                }
                EditorGUILayout.EndHorizontal();
            }

            // Remove checkpoint settings in reverse order
            for (int j = checkpointToRemove.Count - 1; j >= 0; j--)
            {
                checkpointRespawnProp.DeleteArrayElementAtIndex(checkpointToRemove[j]);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        // Remove marked indices in reverse order to maintain correct indices
        for (int i = toRemove.Count - 1; i >= 0; i--)
        {
            respawnableObjectsProp.DeleteArrayElementAtIndex(toRemove[i]);
        }
    }

    private void DrawCheckpointSettingsList()
    {
        EditorGUILayout.HelpBox("Checkpoints are sorted by order. Highest order has priority for player respawn.", MessageType.Info);

        int listSize = checkpointSettingsProp.arraySize;

        // Add button
        if (GUILayout.Button("Add Checkpoint", GUILayout.Height(30)))
        {
            checkpointSettingsProp.arraySize++;
            var newElement = checkpointSettingsProp.GetArrayElementAtIndex(checkpointSettingsProp.arraySize - 1);
            newElement.FindPropertyRelative("checkpointOrder").intValue = listSize;
            newElement.FindPropertyRelative("playerSpawnOffset").vector3Value = new Vector3(1f, 0f, 0f);
            newElement.FindPropertyRelative("triggerBoxSize").vector3Value = new Vector3(5f, 5f, 5f);
        }

        EditorGUILayout.Space();

        // Display list
        List<int> toRemove = new List<int>();

        for (int i = 0; i < listSize; i++)
        {
            var element = checkpointSettingsProp.GetArrayElementAtIndex(i);
            var orderProp = element.FindPropertyRelative("checkpointOrder");
            var checkpointObjProp = element.FindPropertyRelative("checkpointObject");
            var offsetProp = element.FindPropertyRelative("playerSpawnOffset");
            var boxSizeProp = element.FindPropertyRelative("triggerBoxSize");

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Checkpoint {orderProp.intValue}", EditorStyles.boldLabel, GUILayout.Width(150));

            if (GUILayout.Button("Remove", GUILayout.Width(70)))
            {
                toRemove.Add(i);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(orderProp, new GUIContent("Checkpoint Order"));
            EditorGUILayout.PropertyField(checkpointObjProp, new GUIContent("Checkpoint Object"));
            EditorGUILayout.PropertyField(offsetProp, new GUIContent("Spawn Offset"));
            EditorGUILayout.PropertyField(boxSizeProp, new GUIContent("Trigger Box Size"));

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        // Remove marked indices in reverse order to maintain correct indices
        for (int i = toRemove.Count - 1; i >= 0; i--)
        {
            checkpointSettingsProp.DeleteArrayElementAtIndex(toRemove[i]);
        }

        // Sort checkpoints by order
        if (listSize > 0)
        {
            SortCheckpointsByOrder();
        }
    }

    private void SortCheckpointsByOrder()
    {
        int listSize = checkpointSettingsProp.arraySize;

        // Simple bubble sort
        for (int i = 0; i < listSize - 1; i++)
        {
            for (int j = 0; j < listSize - i - 1; j++)
            {
                var element1 = checkpointSettingsProp.GetArrayElementAtIndex(j);
                var element2 = checkpointSettingsProp.GetArrayElementAtIndex(j + 1);

                int order1 = element1.FindPropertyRelative("checkpointOrder").intValue;
                int order2 = element2.FindPropertyRelative("checkpointOrder").intValue;

                if (order1 > order2)
                {
                    // Swap
                    SwapArrayElements(j, j + 1);
                }
            }
        }
    }

    private void SwapArrayElements(int index1, int index2)
    {
        var element1 = checkpointSettingsProp.GetArrayElementAtIndex(index1);
        var element2 = checkpointSettingsProp.GetArrayElementAtIndex(index2);

        // Store values
        int order1 = element1.FindPropertyRelative("checkpointOrder").intValue;
        Transform checkpoint1 = element1.FindPropertyRelative("checkpointObject").objectReferenceValue as Transform;
        Vector3 offset1 = element1.FindPropertyRelative("playerSpawnOffset").vector3Value;
        Vector3 boxSize1 = element1.FindPropertyRelative("triggerBoxSize").vector3Value;

        int order2 = element2.FindPropertyRelative("checkpointOrder").intValue;
        Transform checkpoint2 = element2.FindPropertyRelative("checkpointObject").objectReferenceValue as Transform;
        Vector3 offset2 = element2.FindPropertyRelative("playerSpawnOffset").vector3Value;
        Vector3 boxSize2 = element2.FindPropertyRelative("triggerBoxSize").vector3Value;

        // Swap
        element1.FindPropertyRelative("checkpointOrder").intValue = order2;
        element1.FindPropertyRelative("checkpointObject").objectReferenceValue = checkpoint2;
        element1.FindPropertyRelative("playerSpawnOffset").vector3Value = offset2;
        element1.FindPropertyRelative("triggerBoxSize").vector3Value = boxSize2;

        element2.FindPropertyRelative("checkpointOrder").intValue = order1;
        element2.FindPropertyRelative("checkpointObject").objectReferenceValue = checkpoint1;
        element2.FindPropertyRelative("playerSpawnOffset").vector3Value = offset1;
        element2.FindPropertyRelative("triggerBoxSize").vector3Value = boxSize1;
    }
}

