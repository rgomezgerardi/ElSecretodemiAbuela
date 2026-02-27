using UnityEngine;
using UnityEngine.InputSystem;

public class RaycastClick : MonoBehaviour
{
    [SerializeField] private LayerMask capaCartas;

    void Update()
    {
        Vector2 inputPosition;

        // ===== MOUSE (PC / Editor) =====
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            inputPosition = Mouse.current.position.ReadValue();
        }
        // ===== TOUCH (Mobile) =====
        else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            inputPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        }
        else
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(inputPosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, capaCartas))
        {
            if (!hit.collider.CompareTag("Carta")) return;

            CardManager card = hit.collider.GetComponent<CardManager>();
            if (card != null)
            {
                card.OnClick();

                GamepadCardSelector selector = FindFirstObjectByType<GamepadCardSelector>();
                if (selector != null)
                    selector.SincronizarDesdeClick(card);
            }
        }
    }
}