using UnityEngine;
using UnityEngine;
using System.Collections.Generic;

public class Car : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Vector3 moveDirection = Vector3.right;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private bool isMoving = false;
    [SerializeField] private bool loopMovement = true;

    [Header("Random Start Delay")]
    [SerializeField] private bool useRandomDelay = true;
    [SerializeField] private float minRandomDelay = 0f;
    [SerializeField] private float maxRandomDelay = 1.5f;
    private float randomDelayTimer = 0f;
    private bool delayActive = false;

    [Header("Hitbox Settings")]
    [SerializeField] private bool useHitbox = true;
    [SerializeField] private Vector3 hitboxSize = Vector3.one;
    [SerializeField] private Vector3 hitboxOffset = Vector3.zero;
    private BoxCollider hitbox;

    [Header("Player Reference")]
    [SerializeField] private GameObject player;

    [Header("Respawn Settings")]
    [SerializeField] private bool useRespawn = true;
    [SerializeField] private Vector3 defaultRespawnPoint = Vector3.zero;

    [Header("Checkpoint Settings")]
    [SerializeField] private bool useCheckpoints = true;
    [SerializeField] private List<CheckpointData> checkpointsList = new List<CheckpointData>();
    private Vector3 currentRespawnPoint;
    private int currentCheckpointOrder = -1;

    // Checkpoint data structure
    private class Checkpoint
    {
        public Vector3 position;
        public Vector3 respawnPoint;
        public int order;
        public bool triggerOnX;
        public bool triggerOnY;
        public bool triggerOnZ;
    }

    private List<Checkpoint> checkpoints = new List<Checkpoint>();

    private Vector3 startPosition;
    private float distanceTraveled = 0f;

    void Start()
    {
        startPosition = transform.position;
        currentRespawnPoint = defaultRespawnPoint;
        if (moveDirection != Vector3.zero)
        {
            moveDirection.Normalize();
        }

        SetupHitbox();
        InitializeRandomDelay();
        LoadCheckpointsFromList();
    }

    void Update()
    {
        if (useRandomDelay && delayActive)
        {
            randomDelayTimer -= Time.deltaTime;
            if (randomDelayTimer <= 0f)
            {
                delayActive = false;
                isMoving = true;
            }
        }

        if (isMoving && !delayActive)
        {
            MoveObject();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!useHitbox)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + hitboxOffset, hitboxSize);
    }

    void OnDrawGizmos()
    {
        // Draw hitbox
        if (useHitbox)
        {
            Gizmos.color = new Color(0, 1, 0, 0.2f);
            Gizmos.DrawCube(transform.position + hitboxOffset, hitboxSize);
        }

        // Draw default respawn point
        if (useRespawn)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(defaultRespawnPoint, 0.5f);
        }

        // Draw checkpoints and their respawn points
        if (useCheckpoints && checkpointsList.Count > 0)
        {
            foreach (CheckpointData checkpoint in checkpointsList)
            {
                if (checkpoint == null)
                    continue;

                // Use checkpoint position directly
                Vector3 checkpointPos = checkpoint.checkpointPosition;

                // Draw checkpoint detection box (green)
                Gizmos.color = new Color(0, 1, 0, 0.3f);
                Gizmos.DrawCube(checkpointPos, new Vector3(1f, 1f, 1f));

                // Draw checkpoint detection box outline (bright green)
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(checkpointPos, new Vector3(1f, 1f, 1f));

                // Draw respawn point for this checkpoint (red sphere)
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(checkpoint.respawnPoint, 0.5f);

                // Draw line from checkpoint to respawn point (red dashed line effect)
                Gizmos.color = new Color(1, 0, 0, 0.5f);
                Gizmos.DrawLine(checkpointPos, checkpoint.respawnPoint);

                // Draw order label position indicator (small yellow cube)
                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(checkpointPos + Vector3.up * 0.75f, new Vector3(0.3f, 0.3f, 0.3f));
            }
        }
    }

    private void SetupHitbox()
    {
        if (!useHitbox)
            return;

        hitbox = GetComponent<BoxCollider>();

        if (hitbox == null)
        {
            hitbox = gameObject.AddComponent<BoxCollider>();
        }

        hitbox.size = hitboxSize;
        hitbox.center = hitboxOffset;
        hitbox.isTrigger = true;
    }

    void OnTriggerEnter(Collider collision)
    {
        if (!useRespawn || !player)
            return;

        if (collision.gameObject == player)
        {
            RespawnPlayer();
        }
    }

    private void RespawnPlayer()
    {
        if (player != null)
        {
            player.transform.position = currentRespawnPoint;
        }
    }

    private void InitializeRandomDelay()
    {
        if (useRandomDelay)
        {
            randomDelayTimer = Random.Range(minRandomDelay, maxRandomDelay);
            delayActive = true;
            isMoving = false;
        }
    }

    private void MoveObject()
    {
        if (distanceTraveled < maxDistance)
        {
            float step = moveSpeed * Time.deltaTime;
            transform.Translate(moveDirection * step, Space.World);
            distanceTraveled += step;

            // Check if player has reached any checkpoints
            if (useCheckpoints && useRespawn && player != null)
            {
                CheckForCheckpointCollision();
            }
        }
        else
        {
            if (loopMovement)
            {
                ResetMovement();
            }
            else
            {
                isMoving = false;
            }
        }
    }

    private void ResetMovement()
    {
        transform.position = startPosition;
        distanceTraveled = 0f;
        InitializeRandomDelay();
    }

    public void StartMoving()
    {
        isMoving = true;
        distanceTraveled = 0f;
        startPosition = transform.position;
        delayActive = false;
    }

    public void StopMoving()
    {
        isMoving = false;
        delayActive = false;
    }

    public void ResetPosition()
    {
        transform.position = startPosition;
        distanceTraveled = 0f;
        isMoving = false;
        delayActive = false;
    }

    public void SetMoveDirection(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            moveDirection = direction.normalized;
        }
    }

    public void SetMoveDirection(float x, float y, float z)
    {
        Vector3 newDirection = new Vector3(x, y, z);
        if (newDirection != Vector3.zero)
        {
            moveDirection = newDirection.normalized;
        }
    }

    public void SetMaxDistance(float distance)
    {
        if (distance > 0)
        {
            maxDistance = distance;
        }
    }

    public void SetMoveSpeed(float speed)
    {
        if (speed > 0)
        {
            moveSpeed = speed;
        }
    }

    public void SetLoopMovement(bool loop)
    {
        loopMovement = loop;
    }

    public void SetRandomDelay(bool useRandom, float minDelay = 0f, float maxDelay = 1.5f)
    {
        useRandomDelay = useRandom;
        minRandomDelay = minDelay;
        maxRandomDelay = maxDelay;
        if (useRandom)
        {
            InitializeRandomDelay();
        }
    }

    public void SetHitboxSize(Vector3 size)
    {
        hitboxSize = size;
        if (hitbox != null)
        {
            hitbox.size = hitboxSize;
        }
    }

    public void SetHitboxSize(float x, float y, float z)
    {
        SetHitboxSize(new Vector3(x, y, z));
    }

    public void SetHitboxOffset(Vector3 offset)
    {
        hitboxOffset = offset;
        if (hitbox != null)
        {
            hitbox.center = hitboxOffset;
        }
    }

    public void SetHitboxOffset(float x, float y, float z)
    {
        SetHitboxOffset(new Vector3(x, y, z));
    }

    public void EnableHitbox(bool enable)
    {
        useHitbox = enable;
        if (hitbox != null)
        {
            hitbox.enabled = enable;
        }
    }

    public float GetDistanceTraveled()
    {
        return distanceTraveled;
    }

    public float GetMaxDistance()
    {
        return maxDistance;
    }

    public Vector3 GetMoveDirection()
    {
        return moveDirection;
    }

    public bool IsMoving()
    {
        return isMoving;
    }

    public bool IsLooping()
    {
        return loopMovement;
    }

    public float GetRandomDelayRemaining()
    {
        return delayActive ? randomDelayTimer : 0f;
    }

    public void SetPlayer(GameObject newPlayer)
    {
        player = newPlayer;
    }

    public GameObject GetPlayer()
    {
        return player;
    }

    public bool HasPlayer()
    {
        return player != null;
    }

    public void SetDefaultRespawnPoint(Vector3 respawnPoint)
    {
        defaultRespawnPoint = respawnPoint;
    }

    public Vector3 GetDefaultRespawnPoint()
    {
        return defaultRespawnPoint;
    }

    public void SetUseRespawn(bool useResp)
    {
        useRespawn = useResp;
    }

    /// <summary>
    /// Loads checkpoints from the Inspector list into the runtime checkpoint system.
    /// Called automatically in Start().
    /// </summary>
    private void LoadCheckpointsFromList()
    {
        checkpoints.Clear();
        foreach (CheckpointData cpData in checkpointsList)
        {
            if (cpData != null)
            {
                Checkpoint cp = new Checkpoint
                {
                    position = cpData.checkpointPosition,
                    respawnPoint = cpData.respawnPoint,
                    order = cpData.order,
                    triggerOnX = cpData.triggerOnXAxis,
                    triggerOnY = cpData.triggerOnYAxis,
                    triggerOnZ = cpData.triggerOnZAxis
                };
                checkpoints.Add(cp);
            }
        }
        // Sort by order
        checkpoints.Sort((a, b) => a.order.CompareTo(b.order));
    }

    /// <summary>
    /// Adds a checkpoint at a specific position with a respawn point.
    /// Higher order checkpoints take priority over lower order ones.
    /// </summary>
    /// <param name="checkpointPosition">The Vector3 position where the checkpoint is located</param>
    /// <param name="respawnPoint">The position where the player will respawn after hitting this checkpoint</param>
    /// <param name="order">The checkpoint order (higher numbers = higher priority for respawning)</param>
    /// <param name="triggerX">Should this checkpoint trigger on X axis proximity</param>
    /// <param name="triggerY">Should this checkpoint trigger on Y axis proximity</param>
    /// <param name="triggerZ">Should this checkpoint trigger on Z axis proximity</param>
    public void AddCheckpoint(Vector3 checkpointPosition, Vector3 respawnPoint, int order, bool triggerX = true, bool triggerY = true, bool triggerZ = true)
    {
        // Add only to serializable list
        CheckpointData newCheckpointData = new CheckpointData
        {
            checkpointPosition = checkpointPosition,
            respawnPoint = respawnPoint,
            order = order,
            checkpointName = $"Checkpoint {order}",
            triggerOnXAxis = triggerX,
            triggerOnYAxis = triggerY,
            triggerOnZAxis = triggerZ
        };
        checkpointsList.Add(newCheckpointData);

        // Reload checkpoints from the list
        LoadCheckpointsFromList();
    }

    /// <summary>
    /// Clears all checkpoints and resets respawn to default.
    /// </summary>
    public void ClearCheckpoints()
    {
        checkpointsList.Clear();
        checkpoints.Clear();
        currentRespawnPoint = defaultRespawnPoint;
        currentCheckpointOrder = -1;
    }

    /// <summary>
    /// Removes a checkpoint at a specific order.
    /// </summary>
    public void RemoveCheckpointByOrder(int order)
    {
        checkpointsList.RemoveAll(cp => cp.order == order);
        LoadCheckpointsFromList();
    }

    /// <summary>
    /// Gets the current checkpoint order the player has reached.
    /// </summary>
    public int GetCurrentCheckpointOrder()
    {
        return currentCheckpointOrder;
    }

    /// <summary>
    /// Gets the current respawn point (which may be a checkpoint or the default).
    /// </summary>
    public Vector3 GetCurrentRespawnPoint()
    {
        return currentRespawnPoint;
    }

    /// <summary>
    /// Enables or disables the checkpoint system.
    /// </summary>
    public void SetUseCheckpoints(bool use)
    {
        useCheckpoints = use;
    }

    /// <summary>
    /// Internal method to check if the player is at a checkpoint position.
    /// Updates respawn point to the highest order checkpoint reached.
    /// </summary>
    private void CheckForCheckpointCollision()
    {
        foreach (Checkpoint checkpoint in checkpoints)
        {
            if (player == null)
                return;

            Vector3 playerPos = player.transform.position;
            Vector3 checkpointPos = checkpoint.position;

            // Check each axis individually based on trigger settings
            bool xAxisMatch = !checkpoint.triggerOnX || Mathf.Abs(playerPos.x - checkpointPos.x) < 1.5f;
            bool yAxisMatch = !checkpoint.triggerOnY || Mathf.Abs(playerPos.y - checkpointPos.y) < 1.5f;
            bool zAxisMatch = !checkpoint.triggerOnZ || Mathf.Abs(playerPos.z - checkpointPos.z) < 1.5f;

            // Checkpoint is triggered if all enabled axes match
            if (xAxisMatch && yAxisMatch && zAxisMatch)
            {
                // Update respawn point if this checkpoint has a higher order
                if (checkpoint.order > currentCheckpointOrder)
                {
                    currentCheckpointOrder = checkpoint.order;
                    currentRespawnPoint = checkpoint.respawnPoint;
                }
            }
        }
    }

    public bool IsRespawnEnabled()
    {
        return useRespawn;
    }
}
