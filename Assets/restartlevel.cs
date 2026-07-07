using UnityEngine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class restartlevel : MonoBehaviour
{
    private float pressedCooldown = 0.5f;
    private float lastPressTime = 0f;

    void Start()
    {
        // Try to find an XRSimpleInteractable using reflection to avoid namespace issues
        System.Type interactableType = System.Type.GetType("UnityEngine.XR.Interaction.Toolkit.XRSimpleInteractable, Unity.XR.Interaction.Toolkit");

        if (interactableType != null)
        {
            // Get the component using the type we found
            var interactable = GetComponent(interactableType);

            if (interactable != null)
            {
                // Subscribe to selectEntered event using reflection
                var selectEnteredProperty = interactableType.GetProperty("selectEntered");
                if (selectEnteredProperty != null)
                {
                    var selectEnteredEvent = selectEnteredProperty.GetValue(interactable);
                    var addListenerMethod = selectEnteredEvent.GetType().GetMethod("AddListener");
                    addListenerMethod.Invoke(selectEnteredEvent, new object[] { (System.Action)OnButtonPressed });
                    Debug.Log("Restart button initialized with XRSimpleInteractable!");
                }
            }
        }
        else
        {
            Debug.LogWarning("XRSimpleInteractable not found. Make sure XR Interaction Toolkit is imported.");
        }

        // Also check for standard collider as backup
        Collider collider = GetComponent<Collider>();
        if (collider != null && collider.isTrigger)
        {
            Debug.Log("Restart button also has a trigger collider as fallback.");
        }
    }

    private void OnButtonPressed()
    {
        RestartScene();
    }

    private void OnTriggerEnter(Collider collision)
    {
        // Fallback trigger detection if XRSimpleInteractable is not available
        if ((collision.CompareTag("Player") || collision.gameObject.name.ToLower().Contains("player")) && 
            Time.time - lastPressTime > pressedCooldown)
        {
            lastPressTime = Time.time;
            RestartScene();
        }
    }

    private void RestartScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
        Debug.Log("Scene restarted: " + currentSceneName);
    }
}
