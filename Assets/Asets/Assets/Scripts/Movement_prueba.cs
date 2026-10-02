using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering;

public class Movement_prueba : MonoBehaviour
{

    Rigidbody pes;

    public float jump = 0f;         //fuerza predeterminada del salto, está en 0, pero el valor mínimo que emite al final es 0.5
    public float maxJump = 5f;          // fuerza máxima permitida del salto
    public float j_acceleration = 0.5f;     //velocidad a la que se acumula la fuerza del salto
    public float f_movimiento = 0;    
    bool ground = false;


    void Start()
    {
        pes = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (jump > maxJump)     //esto hace que la fuerza del salto no exceda la fuerza máxima permitida
        {
            jump = maxJump;
            f_movimiento = maxJump;
        }

        if ((UnityEngine.InputSystem.Keyboard.current.spaceKey.isPressed) && (ground == true))//Entre más tiempo se presione la tecla ESPACIO, más fuerza tendrá
        {
            jump += j_acceleration;

        }


        if (Input.GetKeyUp(KeyCode.Space))  //El salto sucede cuando se suelta la telca ESPACIO
        {
            pes.AddForce(Vector3.up * jump, ForceMode.Impulse);
            
        }
        /*-------Movimiento en el aire------- */


       if      ((ground==false) &&
                
                (UnityEngine.InputSystem.Keyboard.current.wKey.isPressed) ||
                (UnityEngine.InputSystem.Keyboard.current.aKey.isPressed) ||
                (UnityEngine.InputSystem.Keyboard.current.sKey.isPressed) ||
                (UnityEngine.InputSystem.Keyboard.current.dKey.isPressed))
  {
    f_movimiento += jump/0.8f;
  }

        if ((ground == false) && (Input.GetKeyDown(KeyCode.W)))
        {
            pes.AddForce(transform.forward * f_movimiento, ForceMode.Impulse);

        } 

    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject)
        {
            ground = true;
            jump = 0;      //La fuerza del salto se resetea
            print("piso");
        }   
    }
    void OnCollisionExit(Collision collision)
    { 
                 ground = false;
            print("aire");
    }

}
