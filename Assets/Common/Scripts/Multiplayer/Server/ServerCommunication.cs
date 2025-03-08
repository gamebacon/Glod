using UnityEngine;

public class ServerCommunication : MonoBehaviour
{
    // Player position (Quick update)
    public Transform playerTransform;
    private Camera playerCamera; // Reference to the player's camera for rotation updates

    private Vector3 lastPositionUpdate;

    public float distThreshold = 0.075f; // Minimum distance change to trigger an update
    public float rotThreshold = 0.1f; // Minimum rotation change to trigger an update

    private float lastSentRotationX;
    private float lastSentRotationY;

    private static readonly float updatesPerSecond = 12f;
    private static readonly float slowUpdatesPerSecond = 8f;

    private float quickUpdateFrequency = 1f / updatesPerSecond;
    private float slowUpdateFrequency = 1f / slowUpdatesPerSecond;

    void Awake()
    {

        if (GameManager.GetInstance().isSinglePlayer) 
        {
            return;
        }

        // Set up quick and slow updates
        InvokeRepeating(nameof(QuickUpdate), quickUpdateFrequency, quickUpdateFrequency);
        InvokeRepeating(nameof(SlowUpdate), slowUpdateFrequency, slowUpdateFrequency);

        // Attempt to find the player and their camera
        FindPlayerAndCamera();
    }

    private void QuickUpdate()
    {
        if (playerTransform == null)
        {
            Debug.LogWarning("Player Transform is not assigned!");
            FindPlayerAndCamera(); // Try to find it again
            return;
        }

        if (Vector3.Distance(playerTransform.position, lastPositionUpdate) < distThreshold)
            return;

        ClientSend.PlayerPosition(playerTransform.position);
        lastPositionUpdate = playerTransform.position;
    }

    private void SlowUpdate()
    {
        if (playerCamera == null)
        {
            Debug.LogWarning("Player Camera is not assigned!");
            FindPlayerAndCamera(); // Attempt to find the camera again
            return;
        }

        // Get camera rotation
        float rotationY = playerCamera.transform.eulerAngles.y;
        float rotationX = playerCamera.transform.eulerAngles.x;

        // Normalize rotationX for proper comparison
        if (rotationX >= 270f)
            rotationX -= 360f;

        // Check if rotation change exceeds the threshold
        if (Mathf.Abs(lastSentRotationX - rotationX) <= rotThreshold &&
            Mathf.Abs(lastSentRotationY - rotationY) <= rotThreshold)
            return;

        ClientSend.PlayerRotation(rotationY, rotationX);
        lastSentRotationY = rotationY;
        lastSentRotationX = rotationX;
    }

    private void FindPlayerAndCamera()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            // Debug.Log("Player Transform found and assigned.");

            // Attempt to find the Camera component
            playerCamera = playerObject.GetComponentInChildren<Camera>();
            if (playerCamera != null)
            {
                // Debug.Log("Player Camera found and assigned.");
            }
            else
            {
                Debug.LogWarning("Player Camera not found! Ensure a Camera component exists on the player or its children.");
            }
        }
        else
        {
            Debug.LogWarning("Player object not found! Ensure the player has the 'Player' tag assigned.");
        }
    }
}
