using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Vector3 originalRotation; // rotación inicial (Inspector o Start)
    public Vector3 forwardRotation;  // rotación al mirar hacia adelante

    public float rotateSpeed = 5f;   // velocidad de rotación

    void Start()
    {
        // Guardar la rotación inicial
        originalRotation = transform.eulerAngles;

        // Configurar rotación hacia adelante (mirando al enemigo)
        forwardRotation = new Vector3(30f, originalRotation.y, originalRotation.z);
    }

    void Update()
    {
        // Determinar rotación objetivo según W
        Vector3 targetRotation = originalRotation;

        if (Keyboard.current != null && Keyboard.current.wKey.isPressed)
            targetRotation = forwardRotation;

        // Suavizar rotación
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRotation), Time.deltaTime * rotateSpeed);
    }
}
