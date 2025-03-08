using NUnit.Framework.Constraints;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody _rb;
    private Transform _cameraTransform;

    public float speed = 10f;
    public float jumpForce = 5f;
    public float cameraDistance = 5f;
    public float cameraHeight = 3f;
    public float rotationSpeed = 2f;

    private bool isGrounded;
    private bool isSprinting;
    private bool cameraActive = true;

    [SerializeField]
    private Camera _camera;

    private float verticalClampAngle = 80f;
    private float currentVerticalRotation = 0f;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();

        if (_camera)
        {
            _cameraTransform = _camera.transform;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (GameManager.GetInstance().gameState != GameState.Game) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            cameraActive = !cameraActive;

            Cursor.lockState = cameraActive ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !cameraActive;
        }

        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        MovePlayer(movement);

        if (movement.magnitude > 0.1f && isGrounded)
        {
            if (!AudioManager.GetInstance().IsPlaying(SoundType.PLAYER_FOOTSTEP))
                AudioManager.GetInstance().Play(SoundType.PLAYER_FOOTSTEP);
        }
        else
        {
            AudioManager.GetInstance().Stop(SoundType.PLAYER_FOOTSTEP);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        if (cameraActive)
        {
            RotateCamera();
        }

        if (Input.GetMouseButtonDown(0) && !cameraActive)
        {
            cameraActive = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        isSprinting = Input.GetKey(KeyCode.LeftShift);
    }

    void MovePlayer(Vector3 movement)
    {

        float calculatedSpeed = speed;

        if (isSprinting) {
            calculatedSpeed *= 10;
        }

        Vector3 moveDirection = transform.TransformDirection(movement) * calculatedSpeed;
        _rb.linearVelocity = new Vector3(moveDirection.x, _rb.linearVelocity.y, moveDirection.z);
    }

    void Jump()
    {
        _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void RotateCamera()
    {
        if (_cameraTransform == null) return;

        float horizontalRotation = Input.GetAxis("Mouse X") * rotationSpeed;
        float verticalRotation = -Input.GetAxis("Mouse Y") * rotationSpeed;

        transform.Rotate(0, horizontalRotation, 0);

        currentVerticalRotation = Mathf.Clamp(currentVerticalRotation + verticalRotation, -verticalClampAngle, verticalClampAngle);
        _cameraTransform.localEulerAngles = new Vector3(currentVerticalRotation, 0, 0);

        Vector3 offset = transform.position - _cameraTransform.forward * cameraDistance + Vector3.up * cameraHeight;
        _cameraTransform.position = offset;
    }

    void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.9f)
            {
                isGrounded = true;
                return;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}
