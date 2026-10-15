using UnityEngine;
public class PesMovimiento : MonoBehaviour
{
    public Transform cam;
    public Vector3 rotacionModelo;  
    public float moveForce = 25f;    
    public float maxSpeed = 4f;
    public float jumpForce = 7f;
    public float flopForce = 3f; 
    public float flopInterval = 0.4f; 
    public float turnSpeed = 5f;
    public float groundCheckDistance = 0.6f;

    Rigidbody rb;
    Vector3 input;
    float flopTimer;
    bool jumpPressed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    bool Grounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
    }

    void Update()
    {
        //direcion en la q se mueve la camara
        Vector3 fwd = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(cam.right, Vector3.up).normalized;
        input = fwd * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal");
        input = Vector3.ClampMagnitude(input, 1f);

        if (Input.GetKeyDown(KeyCode.Space)) jumpPressed = true;
    }

    void FixedUpdate()
    {
        bool grounded = Grounded();

        // movimientooo
        rb.AddForce(input * moveForce, ForceMode.Acceleration);

        Vector3 flat = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (flat.magnitude > maxSpeed)
        {
            flat = flat.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(flat.x, rb.linearVelocity.y, flat.z);
        }

        // rebotes
        if (grounded && input.sqrMagnitude > 0.01f)
        {
            flopTimer += Time.fixedDeltaTime;
            if (flopTimer >= flopInterval)
            {
                rb.AddForce(Vector3.up * flopForce, ForceMode.Impulse);
                flopTimer = 0f;
            }
        }

        // salta
        if (jumpPressed && grounded)
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        jumpPressed = false;

        // gira
        if (input.sqrMagnitude > 0.01f)
        {
            Quaternion target = Quaternion.LookRotation(input) * Quaternion.Euler(rotacionModelo);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, turnSpeed * Time.fixedDeltaTime);
        }
    }
}