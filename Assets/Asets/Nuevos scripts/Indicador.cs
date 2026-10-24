using UnityEngine;

public class Indicador : MonoBehaviour
{
    public float altura = 0.05f;    
    public float velocidad = 2f;    
    public float giro = 90f;        

    Vector3 inicio;

    void Start() { inicio = transform.position; }

    void Update()
    {
        transform.position = inicio + Vector3.up * Mathf.Sin(Time.time * velocidad) * altura;
        transform.Rotate(0, giro * Time.deltaTime, 0, Space.World);
    }
}