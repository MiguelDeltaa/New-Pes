using UnityEngine;

public class Pulso : MonoBehaviour
{
    public Color color = Color.green;
    public float min = 1f;
    public float max = 3f;
    public float velocidad = 2f;

    Material mat;

    void Start() { mat = GetComponent<Renderer>().material; }

    void Update()
    {
        float k = Mathf.Lerp(min, max, (Mathf.Sin(Time.time * velocidad) + 1f) / 2f);
        mat.SetColor("_BaseColor", color * k);
    }
}