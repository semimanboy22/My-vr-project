using UnityEngine;
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
