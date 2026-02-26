using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Gestiona la navegación de menús con mando, alimentando el input
/// directamente al EventSystem de Unity.
/// Adjuntar al GameObject: EventSystem en cada escena con menús UI.
///
/// Controles:
///   Stick izquierdo / D-Pad  → navegar botones
///   Botón Sur (A/Cruz)       → confirmar / activar botón o toggle
/// </summary>
public class GamepadMenuNavigator : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private float inputDelay = 0.2f;

    [Header("Cooldown tras cambio de panel")]
    [Tooltip("Tiempo en segundos que se ignoran confirmaciones al cambiar de panel, evita activar botones accidentalmente.")]
    [SerializeField] private float confirmCooldown = 0.3f;

    private float nextInputTime = 0f;
    private float nextConfirmTime = 0f;
    private bool gamepadActivo = false;

    private List<Selectable> selectablesActivos = new List<Selectable>();
    private int indiceActual = 0;

    // Referencia al primer selectable activo del frame anterior,
    // usada para detectar cambios de panel
    private Selectable primerSelectableAnterior = null;

    // ── Unity ─────────────────────────────────────────────────────

    void Update()
    {
        DetectarGamepad();

        if (!gamepadActivo) return;

        RefrescarSiCambioPanel();

        if (selectablesActivos.Count == 0) return;

        // Si el objeto seleccionado desapareció, volver al primero
        if (EventSystem.current.currentSelectedGameObject == null ||
            !EventSystem.current.currentSelectedGameObject.activeInHierarchy)
        {
            indiceActual = 0;
            Seleccionar(indiceActual);
        }

        ManejarNavegacion();
        ManejarConfirmar();
    }

    // ── Detección de gamepad ──────────────────────────────────────

    private void DetectarGamepad()
    {
        bool hayGamepad = Gamepad.current != null;

        if (hayGamepad && !gamepadActivo)
        {
            gamepadActivo = true;
            ForzarRefresh();
        }
        else if (!hayGamepad && gamepadActivo)
        {
            gamepadActivo = false;
        }
    }

    // ── Detección de cambio de panel ──────────────────────────────

    /// <summary>
    /// Detecta si el panel activo cambió comparando cuál es el selectable
    /// más alto en pantalla. Si cambia, refresca la lista de selectables.
    /// </summary>
    private void RefrescarSiCambioPanel()
    {
        Selectable primerActual = ObtenerPrimerSelectableActivo();

        if (primerActual != primerSelectableAnterior)
        {
            primerSelectableAnterior = primerActual;
            ForzarRefresh();
        }
    }

    /// <summary>
    /// Devuelve el selectable activo con la posición Y más alta en pantalla.
    /// Se usa como proxy para detectar qué panel está visible.
    /// </summary>
    private Selectable ObtenerPrimerSelectableActivo()
    {
        Selectable[] todos = FindObjectsByType<Selectable>(FindObjectsSortMode.None);
        Selectable primero = null;
        float maxY = float.MinValue;

        foreach (var s in todos)
        {
            if (!s.gameObject.activeInHierarchy || !s.interactable) continue;
            if (s.transform.position.y > maxY)
            {
                maxY = s.transform.position.y;
                primero = s;
            }
        }

        return primero;
    }

    // ── Refresh ───────────────────────────────────────────────────

    /// <summary>
    /// Reconstruye la lista de selectables activos ordenados por posición Y
    /// (de arriba a abajo) y selecciona el primero.
    /// Aplica un cooldown de confirmación para evitar activar botones
    /// accidentalmente al cambiar de panel.
    /// </summary>
    public void ForzarRefresh()
    {
        Selectable[] todos = FindObjectsByType<Selectable>(FindObjectsSortMode.None);

        List<Selectable> activos = new List<Selectable>();
        foreach (var s in todos)
        {
            if (s.gameObject.activeInHierarchy && s.interactable)
                activos.Add(s);
        }

        activos.Sort((a, b) => b.transform.position.y.CompareTo(a.transform.position.y));

        selectablesActivos = activos;
        indiceActual = 0;
        Seleccionar(indiceActual);

        nextConfirmTime = Time.unscaledTime + confirmCooldown;
    }

    // ── Seleccionar ───────────────────────────────────────────────

    private void Seleccionar(int indice)
    {
        if (selectablesActivos.Count == 0) return;
        indiceActual = Mathf.Clamp(indice, 0, selectablesActivos.Count - 1);
        EventSystem.current.SetSelectedGameObject(selectablesActivos[indiceActual].gameObject);
    }

    // ── Navegación ────────────────────────────────────────────────

    private void ManejarNavegacion()
    {
        if (Time.unscaledTime < nextInputTime) return;

        Gamepad gp = Gamepad.current;
        Vector2 stick = gp.leftStick.ReadValue();
        Vector2 dpad  = gp.dpad.ReadValue();

        // D-Pad tiene prioridad sobre el stick
        Vector2 input = dpad.magnitude > 0.5f ? dpad : stick;

        if (input.y > 0.5f)
        {
            Seleccionar(indiceActual - 1);
            nextInputTime = Time.unscaledTime + inputDelay;
        }
        else if (input.y < -0.5f)
        {
            Seleccionar(indiceActual + 1);
            nextInputTime = Time.unscaledTime + inputDelay;
        }
    }

    // ── Confirmar ─────────────────────────────────────────────────

    private void ManejarConfirmar()
    {
        if (Time.unscaledTime < nextConfirmTime) return;
        if (!Gamepad.current.buttonSouth.wasPressedThisFrame) return;

        GameObject actual = EventSystem.current.currentSelectedGameObject;
        if (actual == null) return;

        // Deseleccionar antes de invocar para evitar doble procesamiento
        // por parte del Input System UI Input Module
        EventSystem.current.SetSelectedGameObject(null);

        Button boton = actual.GetComponent<Button>();
        if (boton != null && boton.interactable)
        {
            nextConfirmTime = Time.unscaledTime + confirmCooldown;
            boton.onClick.Invoke();
            return;
        }

        Toggle toggle = actual.GetComponent<Toggle>();
        if (toggle != null && toggle.interactable)
        {
            toggle.isOn = !toggle.isOn;
        }
    }
}
