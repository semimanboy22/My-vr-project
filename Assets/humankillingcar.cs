using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[System.Serializable]
public class RespawnObject
{
    public Transform obj;
    public Vector3 respawnPosition;
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

    [Header("Respawn Settings")]
    public List<RespawnObject> objectsToRespawn = new List<RespawnObject>();

    private Vector3 startPosition;
    private float distanceTraveled = 0f;

    void Start()
    {
        startPosition = transform.position;
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
                // Check if hit object is in the respawn list
                for (int i = 0; i < objectsToRespawn.Count; i++)
                {
                    if (hit.transform == objectsToRespawn[i].obj)
                    {
                        RespawnObjectAtPosition(hit.transform, objectsToRespawn[i].respawnPosition);
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
        // Find spawn point object in scene
        GameObject spawn = GameObject.FindWithTag("SpawnPoint");

        if (spawn != null)
        {
            CharacterController cc = obj.GetComponent<CharacterController>();

            // Disable CharacterController temporarily if needed
            if (cc != null)
                cc.enabled = false;

            obj.position = spawn.transform.position;

            if (cc != null)
                cc.enabled = true;
        }
    }

    // Draw invisible barrier in editor
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 barrierPosition = transform.position + moveDirection.normalized * killDistance;

        Gizmos.matrix = Matrix4x4.TRS(
            barrierPosition,
            transform.rotation,
            Vector3.one
        );

        Gizmos.DrawWireCube(Vector3.zero, barrierSize);

        // Reset matrix before drawing respawn spheres
        Gizmos.matrix = Matrix4x4.identity;

        // Draw respawn positions as blue spheres
        Gizmos.color = Color.blue;
        foreach (RespawnObject respawnObj in objectsToRespawn)
        {
            if (respawnObj.obj != null)
            {
                Gizmos.DrawSphere(respawnObj.respawnPosition, 0.5f);
            }
        }
    }
}