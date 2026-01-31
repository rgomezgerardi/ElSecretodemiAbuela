using UnityEngine;
using UnityEngine.InputSystem;

public class RaycastMouseClick : MonoBehaviour
{
    [SerializeField] private LayerMask capaCartas;

    void Update()
    {
        if (Mouse.current == null)
            return;

        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, capaCartas))
        {
            if (!hit.collider.CompareTag("Carta"))
                return;

            CardManager card = hit.collider.GetComponent<CardManager>();
            if (card != null)
                card.OnClick();
        }
    }
}
