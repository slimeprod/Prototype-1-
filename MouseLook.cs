using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;
public class MouseLook : MonoBehaviour
{
    private PlayerControls playerControls;
    Vector2 mouseInput;
    public Transform playerBody;
    float xRotation = 0f;
    public float sensitivity = 30f;

    void Awake()
    {
        playerControls = new PlayerControls();
        Cursor.lockState = CursorLockMode.Locked;
    }
    void OnEnable()
    {
        playerControls.Player.Look.performed += OnLook;
        playerControls.Player.Look.canceled += OnLook;
        playerControls.Enable();
    }
    void OnDisable()
    {
        playerControls.Player.Look.performed -= OnLook;
        playerControls.Player.Look.canceled -= OnLook;
        playerControls.Disable();
    }
    void OnLook(InputAction.CallbackContext context)
    {
        mouseInput = context.ReadValue<Vector2>();
    }
    void Update()
    {
        float mouseX = mouseInput.x * sensitivity * Time.deltaTime;
        float mouseY = mouseInput.y * sensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
