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
    }



    private void QuickUpdate()
    {
        if (Vector3.Distance(playerTransform.position, lastPositionUpdate) < distThreshold)
        {
            return;
        }

        ClientSend.PlayerPosition(playerTransform.position);
        lastPositionUpdate = playerTransform.position;
    }

    
}
