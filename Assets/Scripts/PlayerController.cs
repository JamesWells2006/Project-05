using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;
    public float jumpForce = 5f;
    public Transform cameraTransform;
    public float interactDistance = 3f;
    public LayerMask interactLayer;

    private CharacterController controller;
    private float verticalRotation = 0f;
    private float verticalVelocity = 0f;
    private float gravity = 9.8f;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Debug: check grounded state
        Debug.Log("Grounded: " + controller.isGrounded);

        // Safety fallback: respawn if falling
        if (transform.position.y < -10f)
        {
            controller.enabled = false;
            transform.position = new Vector3(0, 2, 0);
            controller.enabled = true;
        }

        // Mouse look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(0, mouseX, 0);

        verticalRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);

        // Movement
        isGrounded = controller.isGrounded;
        if (isGrounded)
        {
            verticalVelocity = -0.5f;
            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = jumpForce;
            }
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= moveSpeed;
        moveDirection.y = verticalVelocity;

        controller.Move(moveDirection * Time.deltaTime);

        // Interaction with objects
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    void Interact()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, interactDistance, interactLayer))
        {
            if (hit.collider.CompareTag("Collectible"))
            {
                GameManager.Instance.CollectItem(hit.collider.gameObject);
            }
            else if (hit.collider.CompareTag("Door"))
            {
                Door door = hit.collider.GetComponent<Door>();
                if (door != null)
                {
                    door.ToggleDoor();
                }
            }
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Enemy"))
        {
            GameManager.Instance.PlayerHit();
        }
        else if (hit.gameObject.CompareTag("Exit"))
        {
            GameManager.Instance.CompleteLevel();
        }
    }
}
