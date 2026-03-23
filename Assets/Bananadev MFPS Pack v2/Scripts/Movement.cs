using Photon.Pun;
using UnityEngine;

public class Movement : MonoBehaviourPun
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float maxVelocityChange = 10f;

    [Header("Jumping")]
    public float jumpForce = 5f;
    public float extraGravity = 10f;

    private Vector2 input;
    private bool isSprinting;
    private Rigidbody rb;
    private bool isGrounded;
    private AnimationSyncer _animationSyncer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        _animationSyncer = GetComponent<AnimationSyncer>();
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        if (_animationSyncer != null)
        {
            _animationSyncer.horizontal = input.x;
            _animationSyncer.vertical = input.y;
        }

        isSprinting = Input.GetKey(KeyCode.LeftShift);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            photonView.RPC("RPC_PlayJump", RpcTarget.Others);
        }
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        // Karakter forgatás MouseLook-ból
        if (MouseLook.instance != null)
        {
            Quaternion targetRotation = Quaternion.Euler(0f, MouseLook.instance.GetTargetYaw(), 0f);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * 30f));
        }

        rb.AddForce(CalculateMovement(), ForceMode.VelocityChange);

        if (!isGrounded)
            rb.AddForce(Vector3.down * extraGravity, ForceMode.Acceleration);

        isGrounded = false;
    }

    Vector3 CalculateMovement()
    {
        float speedToUse = isSprinting ? sprintSpeed : walkSpeed;
        Vector3 targetVelocity = transform.TransformDirection(new Vector3(input.x, 0f, input.y)) * speedToUse;
        Vector3 velocityChange = targetVelocity - rb.linearVelocity;
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0f;
        return velocityChange;
    }

    void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                break;
            }
        }
    }

    public bool IsMoving()
    {
        return input.magnitude > 0.1f;
    }
}