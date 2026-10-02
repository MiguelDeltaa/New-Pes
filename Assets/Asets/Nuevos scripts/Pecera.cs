using UnityEngine;
using UnityEngine.SceneManagement;

public class Pecera : MonoBehaviour
{
    public GameObject pantallaWin;
    public MonoBehaviour[] desactivar;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        pantallaWin.SetActive(true);
        foreach (var s in desactivar) s.enabled = false;

        Cursor.lockState = CursorLockMode.None;
    }

    public void Reiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}