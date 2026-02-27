using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 originalRotation;
    public Vector3 forwardRotation;
    public float rotateSpeed = 5f;

    public bool controlActivo = true;

    private bool mirarEnemy = false;

    void Start()
    {
        originalRotation = transform.eulerAngles;
        forwardRotation = new Vector3(5f, originalRotation.y, originalRotation.z);
    }

    void Update()
    {
        if (!controlActivo)
            return;

        Vector3 targetRotation = mirarEnemy ? forwardRotation : originalRotation;

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.Euler(targetRotation),
            Time.deltaTime * rotateSpeed
        );
    }

    // Llamado desde InputManager (PC / Gamepad)
    public void SetLook(bool state)
    {
        mirarEnemy = state;
    }

    // Llamado desde botón UI móvil
    public void BotonPresionado()
    {
        mirarEnemy = true;
    }

    public void BotonSoltado()
    {
        mirarEnemy = false;
    }
}