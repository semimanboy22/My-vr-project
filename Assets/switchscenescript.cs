using UnityEngine;

using UnityEngine;
using UnityEngine.SceneManagement;

public class switchscenescript : MonoBehaviour
{
    [SerializeField] private int targetSceneIndex;
    [SerializeField] private string xrRigName = "XR Origin";

    private object interactable;
    private System.Type interactableType;
    private string[] allScenePaths;

    private void Start()
    {
        // Cache all scene paths in the project
        CacheAllScenePaths();

        // Use reflection to get XRSimpleInteractable type to avoid assembly reference issues
        interactableType = System.Type.GetType("UnityEngine.XR.Interaction.Toolkit.XRSimpleInteractable, Unity.XR.Interaction.Toolkit");

        if (interactableType != null)
        {
            interactable = GetComponent(interactableType);

            if (interactable != null)
            {
                // Subscribe to activated event using reflection
                var activatedProperty = interactableType.GetProperty("activated");
                if (activatedProperty != null)
                {
                    var activatedEvent = activatedProperty.GetValue(interactable);
                    var addListenerMethod = activatedEvent.GetType().GetMethod("AddListener");
                    if (addListenerMethod != null)
                    {
                        addListenerMethod.Invoke(activatedEvent, new object[] { (System.Action)OnButtonPressed });
                        Debug.Log("Scene switcher initialized with XRSimpleInteractable!");
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("XRSimpleInteractable type not found. Make sure XR Interaction Toolkit is installed.", gameObject);
        }
    }

    private void CacheAllScenePaths()
    {
        #if UNITY_EDITOR
        // In editor, use AssetDatabase to find all scenes
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:Scene", new[] { "Assets" });
        allScenePaths = new string[guids.Length];

        for (int i = 0; i < guids.Length; i++)
        {
            allScenePaths[i] = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
        }
        #endif
    }

    private void OnButtonPressed()
    {
        SwitchScene();
    }

    public void SwitchScene()
    {
        #if UNITY_EDITOR
        if (allScenePaths == null || allScenePaths.Length == 0)
        {
            CacheAllScenePaths();
        }

        if (targetSceneIndex < 0 || targetSceneIndex >= allScenePaths.Length)
        {
            Debug.LogError("Target scene index is invalid!", gameObject);
            return;
        }

        string scenePath = allScenePaths[targetSceneIndex];
        #else
        // At runtime, fallback to Build Settings
        if (targetSceneIndex < 0 || targetSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError("Target scene index is invalid!", gameObject);
            return;
        }
        string scenePath = SceneUtility.GetScenePathByBuildIndex(targetSceneIndex);
        #endif

        // Store the XR rig name so we can find it in the new scene
        PlayerPrefs.SetString("XRRigName", xrRigName);
        PlayerPrefs.Save();

        // Load scene by path
        SceneManager.LoadScene(scenePath);
    }
}


