using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

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

    Rigidbody rb;
    Vector3 input;
    float flopTimer;
    bool jumpPressed;
    bool grounded;   

    public float o2 = 100f;
    public float o2Bleed = 2f;
    public float o2Recov = 5f;
    public TMP_Text o2Texto;          
    public GameObject pantallaGameOver; 
    public Camara camara;               
    bool wet = false;
    bool muerto = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    // Suelo = cualquier cosa que toque al pez por debajo (la normal del contacto apunta hacia arriba)
    void OnCollisionStay(Collision c)
    {
        foreach (var p in c.contacts)
            if (p.normal.y > 0.5f) grounded = true;
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
        if (muerto) return;   // no se mueve ni gasta oxigeno

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

        // oxigeno
        if (wet) o2 += o2Recov * Time.deltaTime;
        else o2 -= o2Bleed * Time.deltaTime;

        o2 = Mathf.Clamp(o2, 0f, 100f);

        if (o2Texto != null) o2Texto.text = "O2: " + Mathf.CeilToInt(o2);

        if (o2 <= 0) GameOver();

        grounded = false;   
    }

    void GameOver()
    {
        if (muerto) return;  
        muerto = true;
        pantallaGameOver.SetActive(true);
        if (camara != null) camara.enabled = false;
        Cursor.lockState = CursorLockMode.None;  
    }

    // Reintentar
    public void Reiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //peligros
    private void OnTriggerEnter(Collider collision)
    {

        if (collision.gameObject.CompareTag("peligro"))
        {

            GameOver();

        }
        if (collision.gameObject.CompareTag("awa"))     //O2
        {
            wet = true;
            print("ahua");
            //o2 += o2Recov*Time.deltaTime;
        }

    }

    void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("awa"))
        {
            print("aire");
            wet = false;
            //o2 -= o2Bleed * Time.deltaTime;
        }
    }
}