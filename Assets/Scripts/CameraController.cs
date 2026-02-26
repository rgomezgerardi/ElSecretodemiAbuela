using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Vector3 originalRotation;
    public Vector3 forwardRotation;
    public float rotateSpeed = 5f;

    public bool controlActivo = true;

    private bool mirarPorTeclado = false;
    private bool mirarPorBoton = false;

    void Start()
    {
        originalRotation = transform.eulerAngles;
        forwardRotation = new Vector3(5f, originalRotation.y, originalRotation.z);
    }

    void Update()
    {
        if (!controlActivo)
            return;

        // Leer teclado sin pisar el botón
        if (Keyboard.current != null)
            mirarPorTeclado = Keyboard.current.wKey.isPressed;

        bool mirarEnemy = mirarPorTeclado || mirarPorBoton;

        Vector3 targetRotation = mirarEnemy ? forwardRotation : originalRotation;

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.Euler(targetRotation),
            Time.deltaTime * rotateSpeed
        );
    }

    public void BotonPresionado()
    {
        mirarPorBoton = true;
    }

    public void BotonSoltado()
    {
        mirarPorBoton = false;
    }
}