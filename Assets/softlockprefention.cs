using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class FallObject
{
    public Transform obj;
    public Vector3 spawnPosition;
}

public class softlockprefention : MonoBehaviour
{
    [Header("Fall Prevention")]
    public List<FallObject> objectsToTrack = new List<FallObject>();

    void Start()
    {
        // Store initial spawn positions for all tracked objects
        foreach (FallObject fallObj in objectsToTrack)
        {
            if (fallObj.obj != null)
            {
                fallObj.spawnPosition = fallObj.obj.position;
            }
        }
    }

    void Update()
    {
        // Check if any tracked objects have fallen below this object's Y position
        foreach (FallObject fallObj in objectsToTrack)
        {
            if (fallObj.obj != null && fallObj.obj.position.y < transform.position.y)
            {
                // Return object to spawn position
                fallObj.obj.position = fallObj.spawnPosition;

                // Reset velocity if it has a Rigidbody
                Rigidbody rb = fallObj.obj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
        }
    }
}
