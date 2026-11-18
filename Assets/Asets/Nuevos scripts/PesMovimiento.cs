using UnityEngine;
using UnityEngine.SceneManagement;

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

    // Salto mas responsivo
    public float tiempoSuelo = 0.15f;   // Tiempo extra para saltar despues de dejar el suelo
    public float tiempoBuffer = 0.15f;  // Tiempo que se "recuerda" el espacio pulsado antes de tocar suelo
    float sueloTimer;
    float saltoTimer;

    Rigidbody rb;
    Vector3 input;
    float flopTimer;
    bool grounded;   // Se activa al tocar un suelo, sin importar como este girado el pez

    // Oxigeno en burbujas
    public GameObject[] burbujas;           // Burbujas de la UI, de izquierda a derecha
    public float tiempoPorBurbuja = 10f;    // Segundos que tarda en reventar una burbuja (fuera del agua)
    public float tiempoRecuperar = 2f;      // Segundos que tarda en reaparecer una burbuja (dentro del agua)
    float o2;                               // Se mide en burbujas: 5 burbujas = 5
    int aguas = 0;                          // Cuantos colliders de agua toca el pez (>0 = esta en el agua)

    public GameObject pantallaGameOver; // Panel de game over (desactivado al inicio)
    public Camara camara;               // Arrastra aqui la Main Camera (para que deje de girar)
    bool muerto = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        o2 = burbujas.Length;   // Empieza con todas las burbujas
        //rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    // Suelo = cualquier cosa que toque al pez por debajo (la normal del contacto apunta hacia arriba)
    void OnCollisionStay(Collision c)
    {
        foreach (var p in c.contacts)
            if (p.normal.y > 0.5f)
            {
                grounded = true;
                sueloTimer = tiempoSuelo;   // Reinicia el margen para saltar
            }
    }

    void Update()
    {
        //direcion en la q se mueve la camara
        Vector3 fwd = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(cam.right, Vector3.up).normalized;
        input = fwd * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal");
        input = Vector3.ClampMagnitude(input, 1f);

        if (Input.GetKeyDown(KeyCode.Space)) saltoTimer = tiempoBuffer;   // Recuerda la pulsacion
    }

    void FixedUpdate()
    {
        if (muerto) return;   // Ya perdio: no se mueve ni gasta oxigeno

        sueloTimer -= Time.fixedDeltaTime;
        saltoTimer -= Time.fixedDeltaTime;

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

        // salta (si se pulso hace poco Y estuvo en el suelo hace poco)
        if (saltoTimer > 0 && sueloTimer > 0)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);   // Salto siempre igual de alto
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            saltoTimer = 0;
            sueloTimer = 0;   // Evita saltar dos veces seguidas
        }

        // gira
        if (input.sqrMagnitude > 0.01f)
        {
            Quaternion target = Quaternion.LookRotation(input) * Quaternion.Euler(rotacionModelo);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, turnSpeed * Time.fixedDeltaTime);
        }

        // oxigeno: fuera del agua revienta una burbuja cada tiempoPorBurbuja, dentro reaparecen
        if (aguas > 0) o2 += Time.fixedDeltaTime / tiempoRecuperar;
        else o2 -= Time.fixedDeltaTime / tiempoPorBurbuja;

        o2 = Mathf.Clamp(o2, 0f, burbujas.Length);

        // Cada burbuja se ve solo si el oxigeno es mayor que su posicion (0, 1, 2...)
        for (int i = 0; i < burbujas.Length; i++)
            burbujas[i].SetActive(o2 > i);

        if (o2 <= 0) GameOver();

        grounded = false;   // Se vuelve a calcular en el siguiente paso de fisica
    }

    void GameOver()
    {
        if (muerto) return;   // Evita activarlo dos veces
        muerto = true;
        pantallaGameOver.SetActive(true);
        if (camara != null) camara.enabled = false;
        Cursor.lockState = CursorLockMode.None;   // Liberar el mouse para el boton
    }

    // Para el boton "Reintentar"
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
            aguas++;   // Se cuenta cada collider de agua, asi no se pierde el estado si hay varios
            print("ahua " + aguas);
        }
    }

    void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("awa"))
        {
            aguas = Mathf.Max(0, aguas - 1);
            print("aire " + aguas);
        }
    }
}