using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[System.Serializable]
public class RespawnableObjectCheckpointSettings
{
    public int checkpointOrder = 0;
    public Vector3 respawnPoint = Vector3.zero;
}

[System.Serializable]
public class RespawnableObject
{
    public Transform obj;
    [SerializeField]
    private Vector3 defaultRespawnPoint = Vector3.zero;
    [SerializeField]
    private List<RespawnableObjectCheckpointSettings> checkpointRespawnSettings = new List<RespawnableObjectCheckpointSettings>();

    public Vector3 GetDefaultRespawnPoint() => defaultRespawnPoint;
    public void SetDefaultRespawnPoint(Vector3 point) => defaultRespawnPoint = point;
    
    public List<RespawnableObjectCheckpointSettings> GetCheckpointRespawnSettings() => checkpointRespawnSettings;
}

[System.Serializable]
public class CheckpointRespawnSettings
{
    public int checkpointOrder = 0;
    public Transform checkpointObject;
    public Vector3 spawnOffset = new Vector3(1f, 0f, 0f);
    public Vector3 triggerBoxSize = new Vector3(5f, 5f, 5f);
}

public class humankillingcar : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 10f;
    public Vector3 moveDirection = Vector3.forward;
    public float maxDistance = 50f;

    [Header("Kill Barrier")]
    public float killDistance = 2f;
    public Vector3 barrierSize = new Vector3(3f, 3f, 1f);

    [Header("Player Respawn")]
    public Vector3 playerOriginalSpawnPoint = Vector3.zero;

    [Header("Respawnable Objects")]
    public List<RespawnableObject> respawnableObjects = new List<RespawnableObject>();

    [Header("Checkpoint Respawn Settings")]
    public List<CheckpointRespawnSettings> checkpointSettings = new List<CheckpointRespawnSettings>();

    private Vector3 startPosition;
    private float distanceTraveled = 0f;
    
    // Checkpoint system
    private int lastCheckpointOrderReached = -1;

    public void SetPlayerSpawnPoint(Vector3 newSpawnPoint)
    {
        playerOriginalSpawnPoint = newSpawnPoint;
    }

    public void SetRespawnableObjectDefaultSpawn(RespawnableObject respawnObj, Vector3 spawnPoint)
    {
        respawnObj.SetDefaultRespawnPoint(spawnPoint);
    }

    public void SetRespawnableObjectCheckpointSpawn(RespawnableObject respawnObj, int checkpointOrder, Vector3 spawnPoint)
    {
        var checkpointSettings = respawnObj.GetCheckpointRespawnSettings();
        
        // Find if checkpoint setting already exists
        for (int i = 0; i < checkpointSettings.Count; i++)
        {
            if (checkpointSettings[i].checkpointOrder == checkpointOrder)
            {
                checkpointSettings[i].respawnPoint = spawnPoint;
                return;
            }
        }
        
        // Create new checkpoint setting if it doesn't exist
        RespawnableObjectCheckpointSettings newSetting = new RespawnableObjectCheckpointSettings
        {
            checkpointOrder = checkpointOrder,
            respawnPoint = spawnPoint
        };
        checkpointSettings.Add(newSetting);
    }

    void Start()
    {
        startPosition = transform.position;
    }

    public void RegisterCheckpoint(int checkpointOrder, Vector3 checkpointPosition)
    {
        // Update last checkpoint order if this is newer
        if (checkpointOrder > lastCheckpointOrderReached)
        {
            lastCheckpointOrderReached = checkpointOrder;
        }
    }

    private Vector3 GetRespawnPositionForCheckpoint()
    {
        // If no checkpoint reached, respawn at player's original spawn point
        if (lastCheckpointOrderReached == -1)
        {
            return playerOriginalSpawnPoint;
        }

        // Find the checkpoint with the highest order reached
        for (int i = 0; i < checkpointSettings.Count; i++)
        {
            if (checkpointSettings[i].checkpointOrder == lastCheckpointOrderReached && checkpointSettings[i].checkpointObject != null)
            {
                // Return the spawn offset position directly
                return checkpointSettings[i].checkpointObject.position + checkpointSettings[i].spawnOffset;
            }
        }

        // Fallback to start position if checkpoint not found
        return startPosition;
    }

    private Vector3 GetRespawnPositionForRespawnableObject(RespawnableObject respawnableObj)
    {
        // If no checkpoint reached, use default respawn point
        if (lastCheckpointOrderReached == -1)
        {
            return respawnableObj.GetDefaultRespawnPoint();
        }

        // Find the checkpoint-specific respawn point for this object
        var checkpointSettings = respawnableObj.GetCheckpointRespawnSettings();
        for (int i = 0; i < checkpointSettings.Count; i++)
        {
            if (checkpointSettings[i].checkpointOrder == lastCheckpointOrderReached)
            {
                return checkpointSettings[i].respawnPoint;
            }
        }

        // If no checkpoint-specific respawn point found, use default
        return respawnableObj.GetDefaultRespawnPoint();
    }

    void Update()
    {
        // Move the car in a straight line
        transform.Translate(moveDirection.normalized * speed * Time.deltaTime, Space.World);
        distanceTraveled += speed * Time.deltaTime;

        // Reset position if max distance reached
        if (distanceTraveled >= maxDistance)
        {
            transform.position = startPosition;
            distanceTraveled = 0f;
        }

        // Position of invisible barrier in front of the car
        Vector3 barrierPosition = transform.position + moveDirection.normalized * killDistance;

        // Check if player touches the invisible barrier
        Collider[] hits = Physics.OverlapBox(
            barrierPosition,
            barrierSize / 2,
            transform.rotation
        );

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                RespawnObject(hit.transform);
            }
            else
            {
                // Check if hit object is in the respawnable list
                for (int i = 0; i < respawnableObjects.Count; i++)
                {
                    if (hit.transform == respawnableObjects[i].obj)
                    {
                        // Respawn object at its configured respawn point based on checkpoint
                        Vector3 respawnPos = GetRespawnPositionForRespawnableObject(respawnableObjects[i]);
                        RespawnObjectAtPosition(hit.transform, respawnPos);
                        break;
                    }
                }
            }
        }
    }

    void RespawnObjectAtPosition(Transform obj, Vector3 respawnPosition)
    {
        CharacterController cc = obj.GetComponent<CharacterController>();

        if (cc != null)
            cc.enabled = false;

        obj.position = respawnPosition;

        if (cc != null)
            cc.enabled = true;
    }

    void RespawnObject(Transform obj)
    {
        // Respawn player at the last checkpoint reached, or start position if none reached
        Vector3 respawnPos = GetRespawnPositionForCheckpoint();
        
        CharacterController cc = obj.GetComponent<CharacterController>();

        if (cc != null)
            cc.enabled = false;

        obj.position = respawnPos;

        if (cc != null)
            cc.enabled = true;
    }

    // Draw invisible barrier in editor
    void OnDrawGizmos()
    {
        // Reset matrix
        Gizmos.matrix = Matrix4x4.identity;

        // Draw all checkpoint trigger boxes and spawn offsets
        foreach (CheckpointRespawnSettings setting in checkpointSettings)
        {
            if (setting.checkpointObject != null)
            {
                Vector3 checkpointPos = setting.checkpointObject.position;
                
                // Draw trigger box as blue outline
                Gizmos.color = Color.blue;
                Gizmos.matrix = Matrix4x4.TRS(checkpointPos, Quaternion.identity, Vector3.one);
                Gizmos.DrawWireCube(Vector3.zero, setting.triggerBoxSize);
                Gizmos.matrix = Matrix4x4.identity;
                
                // Draw checkpoint position as white sphere
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(checkpointPos, 0.5f);
                
                // Draw spawn offset position as green sphere (where player will respawn)
                Vector3 spawnPos = checkpointPos + setting.spawnOffset;
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(spawnPos, 0.3f);
                
                // Draw line from checkpoint to spawn position
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(checkpointPos, spawnPos);
            }
        }

        // Draw respawnable objects' positions and respawn points
        Gizmos.color = Color.cyan;
        foreach (RespawnableObject respObj in respawnableObjects)
        {
            if (respObj.obj != null)
            {
                // Draw current object position
                Gizmos.DrawSphere(respObj.obj.position, 0.3f);
                
                // Draw default respawn point
                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(respObj.GetDefaultRespawnPoint(), 0.25f);
                
                // Draw line from object to default respawn point
                Gizmos.color = new Color(1, 0, 1, 0.5f);
                Gizmos.DrawLine(respObj.obj.position, respObj.GetDefaultRespawnPoint());
                
                // Draw checkpoint-specific respawn points
                foreach (RespawnableObjectCheckpointSettings checkpointSetting in respObj.GetCheckpointRespawnSettings())
                {
                    Gizmos.color = new Color(0, 1, 1, 0.7f);
                    Gizmos.DrawSphere(checkpointSetting.respawnPoint, 0.2f);
                }
            }
        }

        // Draw current player respawn position as large blue sphere preview
        if (playerOriginalSpawnPoint != Vector3.zero)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(playerOriginalSpawnPoint, 0.5f);
            
            // Draw a semi-transparent outer sphere for better visibility
            Gizmos.color = new Color(0, 0, 1, 0.3f);
            Gizmos.DrawWireSphere(playerOriginalSpawnPoint, 0.7f);
        }
    }
}
