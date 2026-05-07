using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public class MoveHandler : MonoBehaviour
{
    //public ParticleSystem bloodSpatter;
    bool isKnockedBack = false;
    public LayerMask whatIsSafe;
    public LayerMask whatIsGround;
    public Transform groundCheck;
    public float jumpPower = 4f;
    private Rigidbody rb;
    public float walkspeed = 6f;
    [SerializeField] private float currentSpeed = 0f;
    private PlayerControls playerControls;
    Vector2 moveInput;

    void Awake()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody>();
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
    IEnumerator KnockbackRoutine()
    {
        isKnockedBack = true;
        yield return new WaitForSeconds(0.2f); // how long knockback lasts
        isKnockedBack = false;
    }
    void Update()
    {
        currentSpeed = Mathf.Lerp(currentSpeed, moveInput.magnitude > 0.2f ? walkspeed : 0f, 5f * Time.deltaTime);
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
            rb.AddForce ((transform.position - collision.gameObject.transform.position).normalized * 30f, ForceMode.Impulse);
            StartCoroutine(KnockbackRoutine());
            GetComponent<PlayerSettings>().DamagePlayer(20f);
            //bloodSpatter.Play();
        }
    }
}