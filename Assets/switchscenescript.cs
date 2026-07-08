using UnityEngine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class switchscenescript : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string sceneName;
    [SerializeField] private string xrRigName = "XR Origin";

    private XRSimpleInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        Debug.Log($"[SceneSwitcher] Initialized on {gameObject.name}");
    }

    private void OnEnable()
    {
        if (interactable != null)
        {
            interactable.selectEntered.AddListener(OnSelected);
            Debug.Log($"[SceneSwitcher] Subscribed to selectEntered");
        }
    }

    private void OnDisable()
    {
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnSelected);
            Debug.Log($"[SceneSwitcher] Unsubscribed from selectEntered");
        }
    }

    private void OnSelected(SelectEnterEventArgs args)
    {
        Debug.Log($"[SceneSwitcher] Button selected!");
        changeScene();
    }

    // Public method so other scripts can call it
    public void changeScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            Debug.Log($"[SceneSwitcher] Loading scene: {sceneName}, Target XR Rig: {xrRigName}");

            // Store the XR rig name so XRRigSpawner can find it in the new scene
            PlayerPrefs.SetString("TargetXRRigName", xrRigName);
            PlayerPrefs.SetString("SwitchingScene", "true");
            PlayerPrefs.Save();

            Debug.Log($"[SceneSwitcher] PlayerPrefs set - TargetXRRigName: {xrRigName}");

            SceneManager.LoadScene(sceneName);
            Debug.Log($"[SceneSwitcher] Scene load initiated");
        }
        else
        {
            Debug.LogWarning("Scene name is not assigned!");
        }
    }
}
