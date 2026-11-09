using UnityEngine;

public class Skins : MonoBehaviour
{
    public GameObject[] modelos;   // Los 3 modelos del pez

    void Start()
    {
        Elegir(PlayerPrefs.GetInt("skin", 0));   // Carga la skin guardada
    }

    public void Elegir(int i)
    {
        for (int j = 0; j < modelos.Length; j++)
            modelos[j].SetActive(j == i);        // Solo el elegido queda visible

        PlayerPrefs.SetInt("skin", i);           // Lo guarda
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) Elegir(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) Elegir(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) Elegir(2);
    }
}