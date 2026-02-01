using UnityEngine;
using TMPro;

public class TutorialHint : MonoBehaviour
{
    [SerializeField] private float showTime = 20f;
    [SerializeField] private CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup.alpha = 1f;
        Invoke(nameof(Hide), showTime);
    }

    void Update()
    {
        // Sutil flotación
        transform.position += Vector3.up * Mathf.Sin(Time.time) * 0.0005f;
    }

    public void OnFirstAction()
    {
        Destroy(gameObject);
    }

    void Hide()
    {
        Destroy(gameObject);
    }
}
