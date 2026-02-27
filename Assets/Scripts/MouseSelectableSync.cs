using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseSelectableSync : MonoBehaviour, IPointerEnterHandler
{
    public MenuManager menuManager;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (menuManager == null) return;

        Selectable selectable = GetComponent<Selectable>();
        if (selectable != null)
            menuManager.OnPointerEnterButton(selectable);
    }
}