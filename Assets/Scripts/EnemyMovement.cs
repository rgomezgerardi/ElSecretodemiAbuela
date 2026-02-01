using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class EnemyMovement : MonoBehaviour
{
    public static EnemyMovement Instance { get; private set; }
    [Header("Prefabs por dificultad")]
    [SerializeField] private List<GameObject> enemigos;

    [Header("Objetivo")]
    [SerializeField] private Transform targetPosition;

    private Vector3 startPosition;
    private Vector3 endPosition;

    private GameObject enemigoActivo;
    private bool activo;

    [Header("Sonido del enemigo")]
    [SerializeField] private AudioClip sonidoEnemigo;
    [SerializeField] private float volumenBase = 1f;
    [SerializeField] private float radioAuditivo = 5f;
    private AudioSource audioSource;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        startPosition = transform.position;

        if (targetPosition != null)
            endPosition = targetPosition.position;
        else
            endPosition = startPosition + new Vector3(0f, 0f, -3f);

        DesactivarTodos();
        ActivarPorNivel();

        if (enemigoActivo != null)
        {
            audioSource = enemigoActivo.AddComponent<AudioSource>();
            audioSource.clip = sonidoEnemigo;
            audioSource.loop = true;
            audioSource.spatialBlend = 1f;
            audioSource.minDistance = 0.1f;
            audioSource.maxDistance = radioAuditivo;
            audioSource.volume = volumenBase;
            audioSource.Play();
        }

        activo = true;
    }

    void Update()
    {
        if (!activo || LevelManager.Instance == null || enemigoActivo == null)
            return;

        float tiempoNivel = LevelManager.Instance.TiempoNivel;
        float tiempoRestante = LevelManager.Instance.TiempoRestante;

        if (tiempoNivel <= 0f)
            return;

        float progreso = 1f - (tiempoRestante / tiempoNivel);
        progreso = Mathf.Clamp01(progreso);

        enemigoActivo.transform.position =
            Vector3.Lerp(startPosition, endPosition, progreso);

        if (progreso >= 1f)
        {
            activo = false;
            LevelManager.Instance.FinNivel(false);
        }
    }

    private void ActivarPorNivel()
    {
        int nivel = LevelManager.Instance.NivelActual;
        int index = ObtenerIndexPorNivel(nivel);
        foreach (var enemigo in enemigos)
        {
            if (enemigo != null)
                enemigo.SetActive(false);
        }

        if (index >= 0 && index <= enemigos.Count)
        {
            enemigoActivo = enemigos[index];
            enemigoActivo.SetActive(true);
            enemigoActivo.transform.position = startPosition;
        }
    }

    public void ActivarPorNivelMask()
    {
        ActivarPorNivel();
    }

    private int ObtenerIndexPorNivel(int nivel)
    {
        if (nivel <= 2)
            return 0;

        if (nivel <= 4)
            return 1;

        return 2;
    }

    private void DesactivarTodos()
    {
        foreach (var enemigo in enemigos)
            enemigo.SetActive(false);
    }

    public void ResetEnemy()
    {
        DesactivarTodos();
        ActivarPorNivel();

        if (enemigoActivo != null && sonidoEnemigo != null)
        {
            audioSource = enemigoActivo.AddComponent<AudioSource>();
            audioSource.clip = sonidoEnemigo;
            audioSource.loop = true;
            audioSource.spatialBlend = 1f;
            audioSource.minDistance = 0.1f;
            audioSource.maxDistance = radioAuditivo;
            audioSource.volume = volumenBase;
            audioSource.Play();
        }

        activo = true;
    }
}
