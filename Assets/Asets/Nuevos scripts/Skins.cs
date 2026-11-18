using UnityEngine;

public class Skins : MonoBehaviour
{
    public GameObject[] modelos;

    void Start()
    {
        Elegir(PlayerPrefs.GetInt("skin", 0));
    }

    public void Elegir(int i)
    {
        for (int j = 0; j < modelos.Length; j++)
            modelos[j].SetActive(j == i); 

        PlayerPrefs.SetInt("skin", i);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) Elegir(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) Elegir(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) Elegir(2);
    }
}