using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("Object Detected: " + other.name + " At frame:" + Time.frameCount);

    }
    private void OnCollisionEnter(Collision other)
    {

        Debug.Log("Collision Detected bumb: " + other.gameObject.name + " At frame:" + Time.frameCount);

    }
}
