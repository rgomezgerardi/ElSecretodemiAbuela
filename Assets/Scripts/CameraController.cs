using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Vector3 originalRotation;
    public Vector3 forwardRotation;
    public float rotateSpeed = 5f;

    public bool controlActivo = true;

    void Start()
    {
        originalRotation = transform.eulerAngles;
        forwardRotation = new Vector3(5f, originalRotation.y, originalRotation.z);
    }

    void Update()
    {
        if (!controlActivo) return;

        Vector3 targetRotation = originalRotation;

        bool wPressed = Keyboard.current != null && Keyboard.current.wKey.isPressed;

        // ── Cambios feature/gamepad-support ──────────────────────────
        // Stick izquierdo hacia arriba o D-Pad arriba replican la funcionalidad de W
        bool gamepadUp = Gamepad.current != null &&
                         (Gamepad.current.leftStick.ReadValue().y > 0.5f ||
                          Gamepad.current.dpad.up.isPressed);

        if (wPressed || gamepadUp)
            targetRotation = forwardRotation;

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRotation), Time.deltaTime * rotateSpeed);
    }
}
