using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Transform targetPosition;

    private Vector3 startPosition;
    private Vector3 endPosition;

    private bool activo;

    void Start()
    {
        startPosition = transform.position;

        if (targetPosition != null)
            endPosition = targetPosition.position;
        else
            endPosition = startPosition + new Vector3(0f, 0f, -3f);

        activo = true;
    }

    void Update()
    {
        if (!activo || LevelManager.Instance == null)
            return;

        float tiempoNivel = LevelManager.Instance.TiempoNivel;
        float tiempoRestante = LevelManager.Instance.TiempoRestante;

        if (tiempoNivel <= 0f)
            return;

        float progreso = 1f - (tiempoRestante / tiempoNivel);
        progreso = Mathf.Clamp01(progreso);

        transform.position = Vector3.Lerp(startPosition, endPosition, progreso);

        if (progreso >= 1f)
        {
            activo = false;
            LevelManager.Instance.FinNivel(false);
        }
    }

    public void ResetEnemy()
    {
        transform.position = startPosition;
        activo = true;
    }
}
