using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.ProBuilder;
public class MoveHandler : MonoBehaviour
{
    //public ParticleSystem bloodSpatter;
    bool isKnockedBack = false;
    public LayerMask whatIsSafe;
    public LayerMask whatIsGround;
    public Transform groundCheck;
    
    private Rigidbody rb;
    
    [SerializeField] private float currentSpeed = 0f;
    private PlayerControls playerControls;
    Vector2 moveInput;
    [Header("Speeds")]
    public float walkspeed = 6f;
    public float jumpPower = 4f;
    public float safeSpeed = 5f;
    public float knockbackStrength = 20f;
    public float knockbackDuration = 0.25f;


    [Header("Dynamic Cam")]
    private float normalFov = 0f;
    public float safeWalkFov = 75f;
    public float walkFOV = 80f;


    void Awake()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody>();
        normalFov = Camera.main.fieldOfView;
    }
    void OnEnable()
    {
        playerControls.Player.Move.performed += OnMove;
        playerControls.Player.Move.canceled += OnMove;
        playerControls.Player.Jumping.performed += OnJump;
        playerControls.Player.Jumping.canceled += OnJump;
        playerControls.Enable();
    }
    void OnDisable()
    {
        playerControls.Player.Move.performed -= OnMove;
        playerControls.Player.Move.canceled -= OnMove;
        playerControls.Player.Jumping.performed -= OnJump;
        playerControls.Player.Jumping.canceled -= OnJump;
        playerControls.Disable();
    }
    void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded())
        {
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
    }
    void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    bool isGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, 0.25f, whatIsGround);
    }
    public bool isSafe()
    {
        return Physics.CheckSphere(groundCheck.position, 0.6f, whatIsSafe);
    }
    IEnumerator KnockbackRoutine(Vector3 forceDir)
    {
        isKnockedBack = true;
        rb.linearVelocity = forceDir * knockbackStrength + Vector3.up * 2f;
        yield return new WaitForSeconds(knockbackDuration); // how long knockback lasts
        isKnockedBack = false;
    }
    void Update()
    {
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, moveInput.magnitude > 0.1f ? walkFOV : isSafe() ? safeWalkFov : normalFov, 10f * Time.deltaTime);
        currentSpeed = Mathf.Lerp(currentSpeed, moveInput.magnitude > 0.2f ? walkspeed : isSafe() ? safeSpeed : 0f, 5f * Time.deltaTime);
        if (!isKnockedBack)
        {
            Vector3 moveDir = (transform.right * moveInput.x + transform.forward * moveInput.y) * currentSpeed;
            rb.linearVelocity = new Vector3(moveDir.x, rb.linearVelocity.y, moveDir.z);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !isSafe())
        {
            Vector3 dir = (transform.position - collision.gameObject.transform.position).normalized;
            StartCoroutine(KnockbackRoutine(dir));
            GetComponent<PlayerSettings>().DamagePlayer(20f);
        }
    }
}