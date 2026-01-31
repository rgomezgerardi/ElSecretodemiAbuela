using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Vector3 originalPosition;
    public Vector3 originalRotation;

    public Vector3 forwardPosition;
    public Vector3 forwardRotation;

    public float moveSpeed = 5f;

    private bool movingForward = false;

    void Start()
    {
        // Guardar la posición y rotación inicial
        originalPosition = transform.position;
        originalRotation = transform.eulerAngles;

        // Configurar posición hacia adelante (hacia el enemigo)
        forwardPosition = originalPosition + new Vector3(0, 0.5f, 3f);
        forwardRotation = new Vector3(30f, 0f, 0f);
    }

    void Update()
    {
        // Detectar si se presiona W
        if (Keyboard.current != null && Keyboard.current.wKey.isPressed)
        {
            movingForward = true;
        }
        else
        {
            movingForward = false;
        }

        // Mover la cámara suavemente
        if (movingForward)
        {
            // Mover hacia adelante
            transform.position = Vector3.Lerp(transform.position, forwardPosition, Time.deltaTime * moveSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(forwardRotation), Time.deltaTime * moveSpeed);
        }
        else
        {
            // Volver a la posición original
            transform.position = Vector3.Lerp(transform.position, originalPosition, Time.deltaTime * moveSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(originalRotation), Time.deltaTime * moveSpeed);
        }
    }
}
