using UnityEngine;

using UnityEngine;

public class speedboostcheckpoint : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 wallPosition = Vector3.zero;
    [SerializeField] private Vector3 wallSize = new Vector3(5f, 3f, 0.5f);
    [SerializeField] private float speedBoostAmount = 25f;

    [Header("Boost Trigger Axis")]
    [SerializeField] private bool triggerOnX = false;
    [SerializeField] private bool triggerOnY = false;
    [SerializeField] private bool triggerOnZ = true;

    private Rigidbody playerRigidbody;
    private bool hasBeenBoosted = false;
    private Vector3 lastPlayerPosition;

    void Start()
    {
        if (player != null)
        {
            playerRigidbody = player.GetComponent<Rigidbody>();
            lastPlayerPosition = player.position;
        }

        // Set this game object's position to the wall position for reference
        transform.position = wallPosition;
    }

    void Update()
    {
        if (player == null || playerRigidbody == null)
            return;

        // Check if player has crossed the wall
        if (HasCrossedWall())
        {
            if (!hasBeenBoosted)
            {
                ApplySpeedBoost();
                hasBeenBoosted = true;
            }
        }
        else
        {
            hasBeenBoosted = false;
        }

        lastPlayerPosition = player.position;
    }

    bool HasCrossedWall()
    {
        Vector3 playerPos = player.position;
        Vector3 wallMin = wallPosition - (wallSize * 0.5f);
        Vector3 wallMax = wallPosition + (wallSize * 0.5f);

        // Check if player is within the wall bounds
        bool withinX = playerPos.x >= wallMin.x && playerPos.x <= wallMax.x;
        bool withinY = playerPos.y >= wallMin.y && playerPos.y <= wallMax.y;
        bool withinZ = playerPos.z >= wallMin.z && playerPos.z <= wallMax.z;

        // Check which axis to trigger on
        if (triggerOnX && withinY && withinZ)
        {
            // Crossed on X axis
            if (lastPlayerPosition.x < wallMin.x && playerPos.x >= wallMin.x ||
                lastPlayerPosition.x > wallMax.x && playerPos.x <= wallMax.x)
            {
                return true;
            }
        }

        if (triggerOnY && withinX && withinZ)
        {
            // Crossed on Y axis
            if (lastPlayerPosition.y < wallMin.y && playerPos.y >= wallMin.y ||
                lastPlayerPosition.y > wallMax.y && playerPos.y <= wallMax.y)
            {
                return true;
            }
        }

        if (triggerOnZ && withinX && withinY)
        {
            // Crossed on Z axis
            if (lastPlayerPosition.z < wallMin.z && playerPos.z >= wallMin.z ||
                lastPlayerPosition.z > wallMax.z && playerPos.z <= wallMax.z)
            {
                return true;
            }
        }

        return false;
    }

    void ApplySpeedBoost()
    {
        // Get player's forward direction
        Vector3 boostDirection = player.forward;

        // Apply boost - preserve gravity on Y axis
        playerRigidbody.linearVelocity = new Vector3(
            boostDirection.x * speedBoostAmount,
            playerRigidbody.linearVelocity.y,
            boostDirection.z * speedBoostAmount
        );
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        // Draw wall outline (light blue)
        Gizmos.color = new Color(0f, 0.7f, 1f, 1f);
        Gizmos.DrawWireCube(wallPosition, wallSize);

        // Draw wall faces with semi-transparent color
        DrawWallFaces();

        // Draw player if assigned
        if (player != null)
        {
            Gizmos.color = new Color(1f, 1f, 0f, 1f);
            Gizmos.DrawWireCube(player.position, Vector3.one * 0.5f);
        }

        // Draw trigger axis indicators
        DrawAxisIndicators();
    }

    void DrawWallFaces()
    {
        Vector3 wallMin = wallPosition - (wallSize * 0.5f);
        Vector3 wallMax = wallPosition + (wallSize * 0.5f);

        // Draw semi-transparent faces with debug lines
        Gizmos.color = new Color(0f, 0.7f, 1f, 0.3f);

        // Front face
        DrawQuad(
            new Vector3(wallMin.x, wallMin.y, wallMax.z),
            new Vector3(wallMax.x, wallMin.y, wallMax.z),
            new Vector3(wallMax.x, wallMax.y, wallMax.z),
            new Vector3(wallMin.x, wallMax.y, wallMax.z)
        );

        // Back face
        DrawQuad(
            new Vector3(wallMin.x, wallMin.y, wallMin.z),
            new Vector3(wallMin.x, wallMax.y, wallMin.z),
            new Vector3(wallMax.x, wallMax.y, wallMin.z),
            new Vector3(wallMax.x, wallMin.y, wallMin.z)
        );
    }

    void DrawQuad(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p4, p1);
    }

    void DrawAxisIndicators()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 1f);
        if (triggerOnX)
        {
            Gizmos.DrawLine(wallPosition + Vector3.left * 1.5f, wallPosition + Vector3.right * 1.5f);
        }

        Gizmos.color = new Color(0f, 1f, 0f, 1f);
        if (triggerOnY)
        {
            Gizmos.DrawLine(wallPosition + Vector3.down * 1.5f, wallPosition + Vector3.up * 1.5f);
        }

        Gizmos.color = new Color(0f, 0f, 1f, 1f);
        if (triggerOnZ)
        {
            Gizmos.DrawLine(wallPosition + Vector3.back * 1.5f, wallPosition + Vector3.forward * 1.5f);
        }
    }
#endif
}
