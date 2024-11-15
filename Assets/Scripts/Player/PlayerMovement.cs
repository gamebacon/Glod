using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody _rb;
    private Transform _cameraTransform;

    public float speed = 10f;
    public float jumpForce = 5f;
    public float cameraDistance = 5f;
    public float cameraHeight = 3f;
    public float rotationSpeed = 3f;

    private bool isGrounded;
    private bool cameraActive = true;  // Toggle for enabling/disabling camera follow

    void Start()
    {
        return;
        _rb = GetComponent<Rigidbody>();

        // Initialize camera if main camera exists
        if (Camera.main != null)
        {
            _cameraTransform = Camera.main.transform;
        }

        // Hide the cursor initially (before Escape is pressed)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        return;
        // Toggle camera movement with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            cameraActive = !cameraActive;

/*
            if (cameraActive)
            {
                // Re-enable camera follow and show the cursor
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                // Disable camera follow and show the cursor
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            */
        }

        // Update player movement direction
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        MovePlayer(movement);

        // Play footstep sound if moving and grounded
        if (movement.magnitude > 0.1f && isGrounded)
        {
            if (!AudioManager.Instance.IsPlaying("Footsteps"))
            {
                AudioManager.Instance.Play("Footsteps");
            }
        }
        else
        {
            AudioManager.Instance.Stop("Footsteps");
        }

        // Handle jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        // Handle camera rotation if active
        if (cameraActive)
        {
            RotateCamera();
        }

        // Reactivate camera follow and hide cursor on left-click
        if (Input.GetMouseButtonDown(0) && !cameraActive) // Left-click
        {
            cameraActive = true;

            // Hide the cursor and lock it back into the screen
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void MovePlayer(Vector3 movement)
    {
        // Calculate movement relative to player direction
        Vector3 moveDirection = transform.TransformDirection(movement) * speed;
        _rb.linearVelocity = new Vector3(moveDirection.x, _rb.linearVelocity.y, moveDirection.z);
    }

    void Jump()
    {
        _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void RotateCamera()
    {
        if (_cameraTransform == null) return;

        // Get mouse inputs
        float horizontalRotation = Input.GetAxis("Mouse X") * rotationSpeed;
        float verticalRotation = Input.GetAxis("Mouse Y") * rotationSpeed;

        // Rotate the player around the Y-axis
        transform.Rotate(0, horizontalRotation, 0);

        // Calculate and clamp the vertical rotation for the camera
        _cameraTransform.RotateAround(transform.position, transform.right, -verticalRotation);

        /*
        // Set camera position and offset
        Vector3 offset = transform.position - _cameraTransform.forward * cameraDistance + Vector3.up * cameraHeight;
        _cameraTransform.position = offset;
        */
    }

    void OnCollisionStay(Collision collision)
    {
        // Check if the player is on the ground
        isGrounded = true;
    }

    void OnCollisionExit(Collision collision)
    {
        // Player is no longer grounded
        isGrounded = false;
    }
}
