using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class CarScript2 : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 10f;
    public Vector3 moveDirection = Vector3.forward;
    public float maxDistance = 50f;

    [Header("Kill Barrier (size & layers it can kill)")]
    [Tooltip("Layer mask for objects that can be killed by the barrier")]
    public LayerMask killLayerMask = ~0;
    public float killDistance = 2f;
    public Vector3 barrierSize = new Vector3(3f, 3f, 1f);

    [Header("Player Respawn (default)")]
    [Tooltip("Optional: assign a Transform to use as the default player spawn. If empty, defaultPlayerSpawnPoint is used.")]
    public Transform defaultPlayerSpawnTransform;
    public Vector3 defaultPlayerSpawnPoint = Vector3.zero;

    [Header("Respawnable Objects")]
    public List<RespawnableObject> respawnableObjects = new List<RespawnableObject>();

    [Header("Checkpoint Respawn Settings")]
    public List<CheckpointRespawnSettings> checkpointSettings = new List<CheckpointRespawnSettings>();

    Vector3 startPosition;
    float distanceTraveled = 0f;

    // Checkpoint system: -1 == none reached
    int lastCheckpointOrderReached = -1;

    void Start()
    {
        startPosition = transform.position;
        if (defaultPlayerSpawnTransform != null)
            defaultPlayerSpawnPoint = defaultPlayerSpawnTransform.position;
    }

    public void SetPlayerSpawnPoint(Vector3 newSpawnPoint)
    {
        defaultPlayerSpawnPoint = newSpawnPoint;
    }

    public void SetRespawnableObjectDefaultSpawn(RespawnableObject respawnObj, Vector3 spawnPoint)
    {
        respawnObj.SetDefaultRespawnPoint(spawnPoint);
    }

    public void SetRespawnableObjectCheckpointSpawn(RespawnableObject respawnObj, int checkpointOrder, Vector3 spawnPoint)
    {
        var settings = respawnObj.GetCheckpointRespawnSettings();
        for (int i = 0; i < settings.Count; i++)
        {
            if (settings[i].checkpointOrder == checkpointOrder)
            {
                settings[i].respawnPoint = spawnPoint;
                return;
            }
        }

        RespawnableObjectCheckpointSettings newS = new RespawnableObjectCheckpointSettings
        {
            checkpointOrder = checkpointOrder,
            respawnPoint = spawnPoint
        };
        settings.Add(newS);
    }

    // Register checkpoint: only increases order (priority) and cannot be decreased
    public void RegisterCheckpoint(int checkpointOrder)
    {
        if (checkpointOrder > lastCheckpointOrderReached)
            lastCheckpointOrderReached = checkpointOrder;
    }

    Vector3 GetRespawnPositionForCheckpoint()
    {
        if (lastCheckpointOrderReached == -1)
            return defaultPlayerSpawnPoint;

        for (int i = 0; i < checkpointSettings.Count; i++)
        {
            if (checkpointSettings[i].checkpointOrder == lastCheckpointOrderReached && checkpointSettings[i].checkpointObject != null)
            {
                return checkpointSettings[i].checkpointObject.position + checkpointSettings[i].spawnOffset;
            }
        }

        return defaultPlayerSpawnPoint;
    }

    Vector3 GetRespawnPositionForRespawnableObject(RespawnableObject respawnableObj)
    {
        if (lastCheckpointOrderReached == -1)
            return respawnableObj.GetDefaultRespawnPoint();

        var settings = respawnableObj.GetCheckpointRespawnSettings();
        for (int i = 0; i < settings.Count; i++)
        {
            if (settings[i].checkpointOrder == lastCheckpointOrderReached)
                return settings[i].respawnPoint;
        }

        return respawnableObj.GetDefaultRespawnPoint();
    }

    void Update()
    {
        // Check checkpoint trigger boxes first. If player enters a checkpoint, register it (order only increases).
        for (int i = 0; i < checkpointSettings.Count; i++)
        {
            var s = checkpointSettings[i];
            if (s.checkpointObject == null) continue;

            Vector3 center = s.checkpointObject.position;
            Quaternion rot = s.checkpointObject.rotation;
            Vector3 half = s.triggerBoxSize * 0.5f;

            Collider[] cols = Physics.OverlapBox(center, half, rot);
            foreach (var c in cols)
            {
                if (c.CompareTag("Player"))
                {
                    RegisterCheckpoint(s.checkpointOrder);
                    break;
                }
            }
        }

        // Move the object
        transform.Translate(moveDirection.normalized * speed * Time.deltaTime, Space.World);
        distanceTraveled += speed * Time.deltaTime;

        if (distanceTraveled >= maxDistance)
        {
            transform.position = startPosition;
            distanceTraveled = 0f;
        }

        // Kill barrier check
        Vector3 barrierPos = transform.position + moveDirection.normalized * killDistance;
        Collider[] hits = Physics.OverlapBox(barrierPos, barrierSize * 0.5f, transform.rotation);

        foreach (var hit in hits)
        {
            if (!IsInLayerMask(hit.gameObject.layer, killLayerMask))
                continue;

            if (hit.CompareTag("Player"))
            {
                RespawnObject(hit.transform);
            }
            else
            {
                for (int i = 0; i < respawnableObjects.Count; i++)
                {
                    if (respawnableObjects[i].obj == hit.transform)
                    {
                        Vector3 resp = GetRespawnPositionForRespawnableObject(respawnableObjects[i]);
                        RespawnObjectAtPosition(hit.transform, resp);
                        break;
                    }
                }
            }
        }
    }

    void RespawnObjectAtPosition(Transform obj, Vector3 respawnPosition)
    {
        CharacterController cc = obj.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        obj.position = respawnPosition;
        if (cc != null) cc.enabled = true;
    }

    void RespawnObject(Transform obj)
    {
        Vector3 respawnPos = GetRespawnPositionForCheckpoint();
        CharacterController cc = obj.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        obj.position = respawnPos;
        if (cc != null) cc.enabled = true;
    }

    bool IsInLayerMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }

    void OnDrawGizmos()
    {
        // Draw kill barrier
        Gizmos.color = Color.red;
        Vector3 barrierPos = transform.position + moveDirection.normalized * killDistance;
        Gizmos.matrix = Matrix4x4.TRS(barrierPos, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, barrierSize);
        Gizmos.matrix = Matrix4x4.identity;

        // Draw checkpoints
        foreach (var s in checkpointSettings)
        {
            if (s.checkpointObject == null) continue;
            Gizmos.color = Color.blue;
            Vector3 cp = s.checkpointObject.position;
            Gizmos.matrix = Matrix4x4.TRS(cp, s.checkpointObject.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, s.triggerBoxSize);
            Gizmos.matrix = Matrix4x4.identity;

            Gizmos.color = Color.white;
            Gizmos.DrawSphere(cp, 0.15f);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(cp + s.spawnOffset, 0.12f);
        }

        // Draw default player spawn
        if (defaultPlayerSpawnTransform != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(defaultPlayerSpawnTransform.position, 0.2f);
        }
        else if (defaultPlayerSpawnPoint != Vector3.zero)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(defaultPlayerSpawnPoint, 0.2f);
        }
    }
}
