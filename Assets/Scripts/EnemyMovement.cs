using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveTimer = 60f; // Tiempo total para llegar al jugador
    public Transform targetPosition; // Posición del jugador/objetivo
    public bool startMoving = true; // Para controlar cuándo empieza a moverse

    [Header("Position Settings")]
    public Vector3 startPosition; // Posición inicial del enemigo
    public Vector3 endPosition; // Posición final (cerca del jugador)

    private float currentTime = 0f;
    private float moveSpeed;

    void Start()
    {
        // Guardar posición inicial
        startPosition = transform.position;

        // Definir posición final (puedes ajustar esto)
        if (targetPosition != null)
        {
            endPosition = targetPosition.position;
        }
        else
        {
            // Si no hay target, moverlo hacia adelante
            endPosition = startPosition + new Vector3(0, 0, -3f);
        }

        // Calcular velocidad basada en el timer
        CalculateSpeed();
    }

    void Update()
    {
        if (startMoving)
        {
            MoveTowardsPlayer();
        }
    }

    void MoveTowardsPlayer()
    {
        if (currentTime < moveTimer)
        {
            currentTime += Time.deltaTime;

            // Calcular el progreso (0 a 1)
            float progress = currentTime / moveTimer;

            // Mover suavemente desde start a end
            transform.position = Vector3.Lerp(startPosition, endPosition, progress);
        }
        else
        {
            // Llegó al jugador
            transform.position = endPosition;
            OnReachPlayer();
        }
    }

    void CalculateSpeed()
    {
        // La velocidad se ajusta automáticamente al timer
        float distance = Vector3.Distance(startPosition, endPosition);
        moveSpeed = distance / moveTimer;
    }

    void OnReachPlayer()
    {
        Debug.Log("¡El enemigo llegó al jugador!");
        // Aquí puedes agregar lo que pase cuando llegue (Game Over, etc)
        startMoving = false;
    }

    // Métodos públicos para controlar desde otros scripts o UI
    public void StartMovement()
    {
        startMoving = true;
        currentTime = 0f;
    }

    public void StopMovement()
    {
        startMoving = false;
    }

    public void ResetPosition()
    {
        transform.position = startPosition;
        currentTime = 0f;
    }

    public void SetTimer(float newTimer)
    {
        moveTimer = newTimer;
        CalculateSpeed();
    }

    public float GetRemainingTime()
    {
        return Mathf.Max(0, moveTimer - currentTime);
    }

    public float GetProgress()
    {
        return currentTime / moveTimer;
    }
}