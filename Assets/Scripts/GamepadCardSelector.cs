using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Maneja la navegación del grid de cartas (5x4) con mando.
/// Adjuntar al GameObject: LevelManager
/// Requiere: public List<CardManager> GetCartas() => cartas; en LevelManager.cs
///
/// Controles:
///   Stick izquierdo / D-Pad  → navegar cartas
///   Botón Sur (A/Cruz)       → confirmar / voltear carta
///   Start (Menú)             → pausa
/// </summary>
public class GamepadCardSelector : MonoBehaviour
{
    [Header("Configuración del grid")]
    [SerializeField] private int columnas = 5;
    [SerializeField] private int filas = 4;

    [Header("Input")]
    [SerializeField] private float retardoInput = 0.2f;

    // ── Estado interno ────────────────────────────────────────────
    private int indiceActual = 0;
    private int indiceAnterior = -1;
    private float proximoInputTime = 0f;
    private bool gamepadActivo = false;

    private CardManager[] cartasCache;

    // ── Unity ─────────────────────────────────────────────────────

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Al cargar cualquier escena, resetea el estado para evitar
    /// referencias obsoletas a cartas de la escena anterior.
    /// Espera un frame para que LevelManager termine su inicialización.
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        indiceActual = 0;
        indiceAnterior = -1;
        cartasCache = null;
        gamepadActivo = false;
        StartCoroutine(ReactivarSiguienteFrame());
    }

    /// <summary>
    /// Espera un frame antes de reactivar el gamepad para asegurar
    /// que LevelManager ya inicializó las cartas.
    /// </summary>
    private IEnumerator ReactivarSiguienteFrame()
    {
        yield return null;
        gamepadActivo = Gamepad.current != null;
        indiceAnterior = -1;
    }

    void Update()
    {
        if (LevelManager.Instance == null) return;

        // Mientras el juego esté pausado, limpiar el selector y no procesar input
        if (Time.timeScale == 0f)
        {
            LimpiarSelector();
            return;
        }

        DetectarGamepad();

        if (!gamepadActivo) return;

        RefrescarCacheCartas();

        if (cartasCache == null || cartasCache.Length == 0) return;

        ManejarNavegacion();

        // Actualizar selector visual solo cuando cambia la selección
        if (indiceActual != indiceAnterior)
        {
            if (indiceAnterior >= 0 && indiceAnterior < cartasCache.Length)
                cartasCache[indiceAnterior].MostrarSelector(false);

            if (indiceActual >= 0 && indiceActual < cartasCache.Length)
                cartasCache[indiceActual].MostrarSelector(true);

            indiceAnterior = indiceActual;
        }

        ManejarConfirmar();
        ManejarPausa();
    }

    void OnDisable()
    {
        LimpiarSelector();
    }

    // ── Detección de gamepad ──────────────────────────────────────

    private void DetectarGamepad()
    {
        bool hayGamepad = Gamepad.current != null;

        if (hayGamepad && !gamepadActivo)
        {
            // Gamepad conectado: resetear estado para empezar desde carta 0
            gamepadActivo = true;
            indiceActual = 0;
            indiceAnterior = -1;
            cartasCache = null;
        }
        else if (!hayGamepad && gamepadActivo)
        {
            // Gamepad desconectado: limpiar selector visual
            LimpiarSelector();
            gamepadActivo = false;
        }
    }

    // ── Cache de cartas ───────────────────────────────────────────

    /// <summary>
    /// Mantiene una copia local de las cartas del nivel.
    /// Se refresca cuando el número de cartas cambia (nuevo nivel o reinicio).
    /// </summary>
    private void RefrescarCacheCartas()
    {
        var cartas = LevelManager.Instance.GetCartas();
        if (cartas == null) return;

        if (cartasCache == null || cartasCache.Length != cartas.Count)
        {
            LimpiarSelector();
            cartasCache = cartas.ToArray();
            indiceActual = Mathf.Clamp(indiceActual, 0, cartasCache.Length - 1);
            indiceAnterior = -1;
        }
    }

    // ── Navegación ────────────────────────────────────────────────

    /// <summary>
    /// Mueve la selección en el grid según el input del D-Pad o stick izquierdo.
    /// D-Pad tiene prioridad sobre el stick.
    /// </summary>
    private void ManejarNavegacion()
    {
        if (Time.unscaledTime < proximoInputTime) return;

        Gamepad gp = Gamepad.current;
        Vector2 stick = gp.leftStick.ReadValue();
        Vector2 dpad  = gp.dpad.ReadValue();

        // D-Pad tiene prioridad sobre el stick
        Vector2 input = dpad.magnitude > 0.5f ? dpad : stick;

        int dx = 0;
        int dy = 0;

        if (input.x >  0.5f) dx =  1;  // derecha
        if (input.x < -0.5f) dx = -1;  // izquierda
        if (input.y >  0.5f) dy = -1;  // arriba (fila anterior)
        if (input.y < -0.5f) dy =  1;  // abajo (fila siguiente)

        if (dx == 0 && dy == 0) return;

        int fila    = indiceActual / columnas;
        int columna = indiceActual % columnas;

        columna = Mathf.Clamp(columna + dx, 0, columnas - 1);
        fila    = Mathf.Clamp(fila    + dy, 0, filas    - 1);

        indiceActual = fila * columnas + columna;
        proximoInputTime = Time.unscaledTime + retardoInput;
    }

    // ── Confirmar ─────────────────────────────────────────────────

    private void ManejarConfirmar()
    {
        if (!Gamepad.current.buttonSouth.wasPressedThisFrame) return;
        if (indiceActual < 0 || indiceActual >= cartasCache.Length) return;

        cartasCache[indiceActual].OnClick();
    }

    // ── Pausa ─────────────────────────────────────────────────────

    private void ManejarPausa()
    {
        if (Gamepad.current.startButton.wasPressedThisFrame)
            LevelManager.Instance.ActivarPausa();
    }

    // ── Sincronización con mouse ──────────────────────────────────

    /// <summary>
    /// Sincroniza la selección del mando cuando el jugador hace click con el mouse.
    /// Así ambos métodos de input comparten el mismo cursor de selección.
    /// </summary>
    public void SincronizarDesdeClick(CardManager carta)
    {
        if (cartasCache == null) return;

        for (int i = 0; i < cartasCache.Length; i++)
        {
            if (cartasCache[i] != carta) continue;

            if (indiceAnterior >= 0 && indiceAnterior < cartasCache.Length)
                cartasCache[indiceAnterior].MostrarSelector(false);

            indiceActual = i;
            indiceAnterior = i;
            cartasCache[i].MostrarSelector(true);
            return;
        }
    }

    // ── Helpers ───────────────────────────────────────────────────

    /// <summary>
    /// Oculta el selector visual en todas las cartas del cache.
    /// </summary>
    private void LimpiarSelector()
    {
        if (cartasCache == null) return;

        foreach (var carta in cartasCache)
            if (carta != null)
                carta.MostrarSelector(false);
    }
}
