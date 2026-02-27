using UnityEngine;
using UnityEngine.EventSystems;

public class LookEnemyButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Pointer Down detectado");
        InputManager.Instance?.TriggerLookEnemy(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        InputManager.Instance?.TriggerLookEnemy(false);
    }
}