using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Vector3 originalRotation;
    public Vector3 forwardRotation;
    public float rotateSpeed = 5f;

    public bool controlActivo = true; // ← NUEVO

    void Start()
    {
        originalRotation = transform.eulerAngles;
        forwardRotation = new Vector3(5f, originalRotation.y, originalRotation.z);
    }

    void Update()
    {
        if (!controlActivo) // ← NUEVO: No hacer nada si está desactivado
            return;

        Vector3 targetRotation = originalRotation;

        if (Keyboard.current != null && Keyboard.current.wKey.isPressed)
            targetRotation = forwardRotation;

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRotation), Time.deltaTime * rotateSpeed);
    }
}