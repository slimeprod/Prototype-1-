using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.ProBuilder;
using Unity.VisualScripting;
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
    public float knockbackFov = 90f;
    [SerializeField] private float targetFov;
    Vector3 slideVel;
    bool isSliding = false;
    [Header("Sliding")]
    public float slideSize = 0.5f;
    private float startYSize;
    void Awake()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody>();
        normalFov = Camera.main.fieldOfView;
        startYSize = transform.localScale.y;
        Camera.main.GetComponent<Animator>().Play("Idle");
    }
    void OnEnable()
    {
        playerControls.Player.Move.performed += OnMove;
        playerControls.Player.Move.canceled += OnMove;
        playerControls.Player.Jumping.performed += OnJump;
        playerControls.Player.Jumping.canceled += OnJump;
        playerControls.Player.Slide.performed += OnSlide;
        playerControls.Player.Slide.canceled += OnSlide;
        playerControls.Enable();
    }
    void OnDisable()
    {
        playerControls.Player.Move.performed -= OnMove;
        playerControls.Player.Move.canceled -= OnMove;
        playerControls.Player.Jumping.performed -= OnJump;
        playerControls.Player.Jumping.canceled -= OnJump;
        playerControls.Player.Slide.performed -= OnSlide;
        playerControls.Player.Slide.canceled -= OnSlide;
        playerControls.Disable();
    }
    void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded())
        {
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
    }
    void OnSlide(InputAction.CallbackContext context)
    {
        isSliding = context.performed;
        if (context.performed)
        {
            Vector3 vel = rb.linearVelocity;
            slideVel = new Vector3(vel.x, 0f, vel.z);
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
    IEnumerator InAirRoutine()
    {
        yield return new WaitWhile(() => !isGrounded());
        Camera.main.GetComponent<Animator>().Play("Ground Thud");
        yield return new WaitForSeconds(0.25f);
        Camera.main.GetComponent<Animator>().Play("Idle");
    }
    void Update()
    {
        if (!isGrounded())
        {
            StartCoroutine(InAirRoutine());
        }
        transform.localScale = new Vector3(transform.localScale.x, Mathf.Lerp(transform.localScale.y, isSliding ? slideSize : startYSize, 10f * Time.deltaTime), transform.localScale.z);
        if (moveInput.magnitude > 0.1f)
        {
            targetFov = isSafe() ? safeWalkFov : walkFOV;
        }   
        else
        {
            targetFov = normalFov;
        }
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetFov, 10f * Time.deltaTime);

        currentSpeed = Mathf.Lerp(currentSpeed, moveInput.magnitude > 0.2f ? walkspeed : moveInput.magnitude > 0.2f && isSafe() ? safeSpeed : 0f, 5f * Time.deltaTime);
        if (isSliding && !isKnockedBack)
        {
            //rb.linearVelocity = new Vector3(Mathf.Lerp(slideVel.x, 0f, 100f * Time.deltaTime), rb.linearVelocity.y, Mathf.Lerp(slideVel.z, 0f, 100f * Time.deltaTime));
            slideVel = Vector3.Lerp(slideVel, Vector3.zero, 0.5f * Time.deltaTime); //interpolates every value of slideVel to zero by 0.5f 
            rb.linearVelocity = new Vector3(slideVel.x, rb.linearVelocity.y, slideVel.z); //applies the interpolating values to the rigidbodies linearVelocity
        }
        else if (!isKnockedBack)
        {
            Vector3 moveDir = (transform.right * moveInput.x + transform.forward * moveInput.y) * currentSpeed;
            rb.linearVelocity = new Vector3(moveDir.x, rb.linearVelocity.y, moveDir.z);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !isSafe())
        {
            targetFov = knockbackFov;
            Vector3 dir = (transform.position - collision.gameObject.transform.position).normalized;
            StartCoroutine(KnockbackRoutine(dir));
            GetComponent<PlayerSettings>().DamagePlayer(20f);
        }
    }
}