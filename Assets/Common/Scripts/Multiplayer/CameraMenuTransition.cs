using UnityEngine;

public class CameraMenuTransition : MonoBehaviour
{
    private bool hasReachedPos = true;  // Whether the camera has reached the target position and rotation.
    private Vector3 targetPos;  // The target position to move to.
    private Quaternion targetRot;  // The target rotation to rotate towards.
    private float duration = 23.0f;  // Duration of the movement.
    private float startedMoving;  // Timestamp when the movement starts.

    [SerializeField]
    private Transform _cameraTransform;  // Reference to the camera's transform.

    void Update()
    {
        // If the camera has reached the target position and rotation, stop updating.
        if (hasReachedPos)
        {
            return;
        }

        // Calculate the fraction of completion of the transition.
        float fracCompleted = (Time.time - startedMoving) / duration;

        // If the movement is complete (position is close enough to the target)
        float distance = Vector3.Distance(_cameraTransform.position, targetPos);
        if (distance < .001f)
        {
            _cameraTransform.position = targetPos;  // Ensure the camera lands at the target position.
            _cameraTransform.rotation = targetRot;  // Ensure the camera rotates to the target rotation.
            hasReachedPos = true;
            return;
        }

        // Smoothly interpolate position using Slerp.
        _cameraTransform.position = Vector3.Slerp(_cameraTransform.position, targetPos, fracCompleted);
        
        // Smoothly interpolate rotation using Slerp (for rotation).
        _cameraTransform.rotation = Quaternion.Slerp(_cameraTransform.rotation, targetRot, fracCompleted);
    }

    // Public method to start the transition to a target position and rotation (takes a Transform as input).
    public void GoToPos(Transform target, float duration)
    {
        targetPos = target.position;  // Set the target position.
        targetRot = target.rotation;  // Set the target rotation.
        hasReachedPos = false;  // Start the movement.
        startedMoving = Time.time;  // Record the time when movement starts.
        this.duration = duration;  // Use the specified duration for the transition.
    }
}
