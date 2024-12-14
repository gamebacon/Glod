using UnityEngine;

public class FacePlayer : MonoBehaviour
{

    private Transform playerTransform;


    // Update is called once per frame
    void Update()
    {

        // set on start
        if (!playerTransform) {
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        } else {

            float distance = Vector3.Distance(playerTransform.position, transform.position);

            if (distance > 20) 
            {
            } else {
                transform.LookAt(playerTransform);
            }
        }

    }
}
