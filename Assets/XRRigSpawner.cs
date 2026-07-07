using UnityEngine;
using UnityEngine.SceneManagement;

public class XRRigSpawner : MonoBehaviour
{
    private void Start()
    {
        // Check if we're being spawned after a scene transition
        string rigsName = PlayerPrefs.GetString("XRRigName", "");

        if (!string.IsNullOrEmpty(rigsName) && gameObject.name == rigsName)
        {
            // This is the XR Rig that should be positioned for the new scene
            // The player will automatically spawn at this GameObject's position
            Debug.Log($"XR Rig '{rigsName}' is ready in scene '{SceneManager.GetActiveScene().name}'");
            PlayerPrefs.DeleteKey("XRRigName");
            PlayerPrefs.Save();
        }
    }
}
