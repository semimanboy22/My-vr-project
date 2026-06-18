using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private int checkpointOrder = 0;

    private humankillingcar respawnManager;

    void Start()
    {
        // Find the respawn manager in the scene
        respawnManager = FindObjectOfType<humankillingcar>();
        
        if (respawnManager == null)
        {
            Debug.LogError("Checkpoint could not find humankillingcar in scene!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && respawnManager != null)
        {
            // Register this checkpoint as the latest touched
            respawnManager.RegisterCheckpoint(checkpointOrder, transform.position);
        }
    }

    void OnDrawGizmos()
    {
        // Draw checkpoint position as a green sphere in the editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
