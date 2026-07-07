using UnityEngine;
using System.Collections.Generic;
using UnityEngine;

public class honkingcar : MonoBehaviour
{
    [SerializeField] private List<AudioClip> audioClips = new List<AudioClip>();
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Vector3 carHitboxSize = new Vector3(2f, 2f, 2f);
    [SerializeField] private Vector3 carHitboxOffset = Vector3.zero;
    [SerializeField] private Vector3 playerHitboxSize = new Vector3(1f, 2f, 1f);
    [SerializeField] private Vector3 playerHitboxOffset = Vector3.zero;
    [SerializeField] private float soundVolume = 1f;

    private AudioSource audioSource;
    private BoxCollider carHitboxCollider;
    private Rigidbody carRigidbody;
    private GameObject playerHitboxObject;
    private BoxCollider playerHitboxCollider;
    private Rigidbody playerRigidbody;
    private const string PLAYER_HITBOX_NAME = "PlayerHitbox";
    private bool isPlayerInside = false;

    void Start()
    {
        InitializeAudioSource();
        SetupCarHitbox();
        SetupPlayerHitbox();
    }

    void OnDrawGizmos()
    {
        // Draw yellow hitbox for the car
        Gizmos.color = new Color(1f, 1f, 0f, 0.7f);
        Gizmos.DrawWireCube(transform.position + carHitboxOffset, carHitboxSize);

        // Draw blue hitbox for the player (when playerTransform is assigned)
        if (playerTransform != null)
        {
            Gizmos.color = new Color(0f, 0f, 1f, 0.7f);
            Gizmos.DrawWireCube(playerTransform.position + playerHitboxOffset, playerHitboxSize);
        }
    }

    private void InitializeAudioSource()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
        audioSource.volume = soundVolume;
    }

    private void SetupCarHitbox()
    {
        carHitboxCollider = GetComponent<BoxCollider>();
        if (carHitboxCollider == null)
        {
            carHitboxCollider = gameObject.AddComponent<BoxCollider>();
        }
        carHitboxCollider.isTrigger = true;
        carHitboxCollider.size = carHitboxSize;
        carHitboxCollider.center = carHitboxOffset;

        // Ensure we have a Rigidbody for collision detection
        carRigidbody = GetComponent<Rigidbody>();
        if (carRigidbody == null)
        {
            carRigidbody = gameObject.AddComponent<Rigidbody>();
        }
        carRigidbody.isKinematic = true;
        carRigidbody.useGravity = false;
        carRigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    private void SetupPlayerHitbox()
    {
        if (playerTransform == null)
        {
            Debug.LogError("Player Transform not assigned to honkingcar! Please assign it in the Inspector.");
            return;
        }

        // Look for existing player hitbox child object
        Transform existingHitbox = playerTransform.Find(PLAYER_HITBOX_NAME);
        if (existingHitbox != null)
        {
            playerHitboxObject = existingHitbox.gameObject;
        }
        else
        {
            // Create new player hitbox object as child of player
            playerHitboxObject = new GameObject(PLAYER_HITBOX_NAME);
            playerHitboxObject.transform.SetParent(playerTransform);
            playerHitboxObject.transform.localPosition = Vector3.zero;
            playerHitboxObject.transform.localRotation = Quaternion.identity;
        }

        // Setup the collider
        playerHitboxCollider = playerHitboxObject.GetComponent<BoxCollider>();
        if (playerHitboxCollider == null)
        {
            playerHitboxCollider = playerHitboxObject.AddComponent<BoxCollider>();
        }

        playerHitboxCollider.isTrigger = true;
        playerHitboxCollider.size = playerHitboxSize;
        playerHitboxCollider.center = playerHitboxOffset;

        // Add rigidbody to player hitbox if it doesn't have one
        playerRigidbody = playerHitboxObject.GetComponent<Rigidbody>();
        if (playerRigidbody == null)
        {
            playerRigidbody = playerHitboxObject.AddComponent<Rigidbody>();
        }
        playerRigidbody.isKinematic = true;
        playerRigidbody.useGravity = false;
        playerRigidbody.constraints = RigidbodyConstraints.FreezeAll;

        Debug.Log("Player hitbox setup complete!");
    }

    void Update()
    {
        // Update car hitbox collider size and position
        if (carHitboxCollider != null)
        {
            carHitboxCollider.size = carHitboxSize;
            carHitboxCollider.center = carHitboxOffset;
            audioSource.volume = soundVolume;
        }

        // Update player hitbox collider size and position
        if (playerHitboxCollider != null && playerHitboxObject != null)
        {
            playerHitboxCollider.size = playerHitboxSize;
            playerHitboxCollider.center = playerHitboxOffset;
        }

        // Manual AABB collision detection - checks if the two boxes overlap
        if (playerTransform != null)
        {
            bool isColliding = CheckAABBCollision();

            if (isColliding && !isPlayerInside)
            {
                isPlayerInside = true;
                PlayRandomSound();
                Debug.Log("Player entered car hitbox! Sound playing...");
            }
            else if (!isColliding && isPlayerInside)
            {
                isPlayerInside = false;
                Debug.Log("Player exited car hitbox.");
            }
        }
    }

    private bool CheckAABBCollision()
    {
        // Car hitbox bounds
        Vector3 carCenter = transform.position + carHitboxOffset;
        Vector3 carMin = carCenter - carHitboxSize * 0.5f;
        Vector3 carMax = carCenter + carHitboxSize * 0.5f;

        // Player hitbox bounds
        Vector3 playerCenter = playerTransform.position + playerHitboxOffset;
        Vector3 playerMin = playerCenter - playerHitboxSize * 0.5f;
        Vector3 playerMax = playerCenter + playerHitboxSize * 0.5f;

        // Check if boxes overlap on all axes
        bool overlapX = carMin.x <= playerMax.x && carMax.x >= playerMin.x;
        bool overlapY = carMin.y <= playerMax.y && carMax.y >= playerMin.y;
        bool overlapZ = carMin.z <= playerMax.z && carMax.z >= playerMin.z;

        return overlapX && overlapY && overlapZ;
    }

    private void OnTriggerEnter(Collider collision)
    {
        // Backup trigger detection
        if (collision.gameObject == playerHitboxObject && !isPlayerInside)
        {
            isPlayerInside = true;
            PlayRandomSound();
            Debug.Log("Player entered car hitbox (OnTriggerEnter)! Sound playing...");
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        // Check if the player is leaving
        if (collision.gameObject == playerHitboxObject)
        {
            isPlayerInside = false;
            Debug.Log("Player exited car hitbox.");
        }
    }

    private void PlayRandomSound()
    {
        if (audioClips.Count == 0)
        {
            Debug.LogError("No audio clips assigned to " + gameObject.name);
            return;
        }

        if (audioSource == null)
        {
            Debug.LogError("AudioSource is null!");
            return;
        }

        int randomIndex = Random.Range(0, audioClips.Count);
        AudioClip clip = audioClips[randomIndex];

        if (clip != null)
        {
            audioSource.volume = soundVolume;
            audioSource.PlayOneShot(clip);
            Debug.Log("Playing sound: " + clip.name + " at volume: " + soundVolume);
        }
        else
        {
            Debug.LogWarning("Audio clip at index " + randomIndex + " is null!");
        }
    }
}
