using UnityEngine;

public class Camara : MonoBehaviour
{
    public Transform target;       
    public float distance = 4f;
    public float height = 0.5f;    
    public float sensitivity = 3f;

    float yaw;
    float pitch = 20f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch = Mathf.Clamp(pitch - Input.GetAxis("Mouse Y") * sensitivity, -10f, 70f);

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0);
        Vector3 focus = target.position + Vector3.up * height;
        Vector3 desired = focus - rot * Vector3.forward * distance;

        // si hay Pared acerca cam
        if (Physics.Linecast(focus, desired, out RaycastHit hit))
            desired = hit.point + hit.normal * 0.2f;

        transform.position = desired;
        transform.rotation = rot;
    }
}