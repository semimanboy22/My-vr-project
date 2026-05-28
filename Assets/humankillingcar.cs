using UnityEngine;
using UnityEngine.SceneManagement;

public class humankillingcar : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 10f;
    public Vector3 moveDirection = Vector3.forward;

    [Header("Kill Barrier")]
    public float killDistance = 2f;
    public Vector3 barrierSize = new Vector3(3f, 3f, 1f);

    void Update()
    {
        // Move the car in a straight line
        transform.Translate(moveDirection.normalized * speed * Time.deltaTime, Space.World);

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
                SendPlayerToSpawn(hit.transform);
            }
        }
    }

    void SendPlayerToSpawn(Transform player)
    {
        // Find spawn point object in scene
        GameObject spawn = GameObject.FindWithTag("SpawnPoint");

        if (spawn != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();

            // Disable CharacterController temporarily if needed
            if (cc != null)
                cc.enabled = false;

            player.position = spawn.transform.position;

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
    }
}