using UnityEngine;
using UnityEngine.EventSystems;

public class BotonMirarEnemy : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public CameraController cameraController;

    public void OnPointerDown(PointerEventData eventData)
    {
        cameraController.BotonPresionado();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        cameraController.BotonSoltado();
    }
}