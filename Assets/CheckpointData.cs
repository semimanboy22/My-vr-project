using UnityEngine;
using UnityEngine;

/// <summary>
/// Serializable data structure for checkpoint information.
/// </summary>
[System.Serializable]
public class CheckpointData
{
    public Vector3 checkpointPosition;
    public Vector3 respawnPoint;
    public int order;
    [TextArea(2, 3)]
    public string checkpointName = "Checkpoint";

    [Header("Trigger Axes")]
    [SerializeField]
    public bool triggerOnXAxis = true;
    [SerializeField]
    public bool triggerOnYAxis = true;
    [SerializeField]
    public bool triggerOnZAxis = true;
}
