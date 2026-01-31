using UnityEngine;

public class RaycastMouseClick : MonoBehaviour
{
    [SerializeField] private LayerMask capaCartas;

    void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

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
