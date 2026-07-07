using UnityEngine;

using UnityEngine;

public class speedboost : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 boxSize = new Vector3(5f, 3f, 5f);
    [SerializeField] private Vector3 playerBoxSize = new Vector3(1f, 2f, 1f);
    [SerializeField] private float speedBoostAmount = 20f;

    private BoxCollider boostZoneCollider;
    private BoxCollider playerCollider;
    private Rigidbody playerRigidbody;
    private bool isPlayerInZone = false;

    void Start()
    {
        // Create the boost zone collider (trigger)
        boostZoneCollider = gameObject.GetComponent<BoxCollider>();
        if (boostZoneCollider == null)
        {
            boostZoneCollider = gameObject.AddComponent<BoxCollider>();
        }
        boostZoneCollider.isTrigger = true;
        boostZoneCollider.size = boxSize;

        // Setup player if assigned
        if (player != null)
        {
            SetupPlayer();
        }
    }

    void SetupPlayer()
    {
        // Get or create player collider
        playerCollider = player.GetComponent<BoxCollider>();
        if (playerCollider == null)
        {
            playerCollider = player.gameObject.AddComponent<BoxCollider>();
        }
        playerCollider.size = playerBoxSize;

        // Get or create Rigidbody
        playerRigidbody = player.GetComponent<Rigidbody>();
        if (playerRigidbody == null)
        {
            playerRigidbody = player.gameObject.AddComponent<Rigidbody>();
            playerRigidbody.useGravity = true;
            playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    void Update()
    {
        // Apply speed boost when player is in the zone
        if (isPlayerInZone && player != null && playerRigidbody != null)
        {
            Vector3 boostDirection = player.forward;
            playerRigidbody.linearVelocity = new Vector3(boostDirection.x * speedBoostAmount, playerRigidbody.linearVelocity.y, boostDirection.z * speedBoostAmount);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (player != null && other.gameObject == player.gameObject)
        {
            isPlayerInZone = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (player != null && other.gameObject == player.gameObject)
        {
            isPlayerInZone = false;
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        // Draw boost zone wireframe (light blue)
        Gizmos.color = new Color(0f, 0.7f, 1f, 1f);
        Gizmos.DrawWireCube(transform.position, boxSize);

        // Draw player hitbox wireframe (yellow)
        if (player != null)
        {
            Gizmos.color = new Color(1f, 1f, 0f, 1f);
            Gizmos.DrawWireCube(player.position, playerBoxSize);
        }
    }
#endif
}
