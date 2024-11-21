using UnityEngine;

public class ServerCommunication : MonoBehaviour
{
    // Player position (Quick update)
    public Transform playerTransform;
    public Vector3 lastPositionUpdate;

    public float distThreshold = 0.1f;
    public float quickUpdate = 0.05f;

    void Awake()
    {
        // Check every 0.05 seconds for new packets
        InvokeRepeating("QuickUpdate", 0f, quickUpdate);

        // Attempt to find the player on awake
        FindPlayerTransform();
    }

    private void QuickUpdate()
    {
        if (playerTransform == null)
        {
            Debug.LogWarning("Player Transform is not assigned!");
            FindPlayerTransform(); // Try to find it again
            return;
        }

        if (Vector3.Distance(playerTransform.position, lastPositionUpdate) < distThreshold)
        {
            return;
        }

        ClientSend.PlayerPosition(playerTransform.position);
        lastPositionUpdate = playerTransform.position;
    }

    private void FindPlayerTransform()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            Debug.Log("Player Transform found and assigned.");
        }
        else
        {
            Debug.LogWarning("Player object not found! Ensure the player has the 'Player' tag assigned.");
        }
    }
}
