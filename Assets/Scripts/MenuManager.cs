using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Botones")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button backOptionsButton;
    [SerializeField] private Button backCreditsButton;

    [Header("Options UI")]
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Slider volumeSlider;

    [Header("Panels")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject creditsPanel;

    [Header("Historia")]
    [SerializeField] private GameObject panelHistoria;
    [SerializeField] private RawImage historiaImage;
    [SerializeField] private Button btnSiguiente;
    [SerializeField] private List<Texture2D> listaHistoria;

    private int indiceHistoria = 0;

    private List<Selectable> elementosActuales = new List<Selectable>();
    private int indiceSeleccionado = 0;

    private void Awake()
    {
        startButton.onClick.AddListener(IniciarHistoria);
        optionsButton.onClick.AddListener(OpenOptions);
        creditsButton.onClick.AddListener(OpenCredits);
        backOptionsButton.onClick.AddListener(CloseOptions);
        backCreditsButton.onClick.AddListener(CloseCredits);
        quitButton.onClick.AddListener(QuitGame);
        btnSiguiente.onClick.AddListener(SiguienteHistoria);

        panelHistoria.SetActive(false);
    }

    private void OnEnable()
    {
        StartCoroutine(EsperarInputManager());
    }

    private IEnumerator EsperarInputManager()
    {
        // Espera hasta que exista el Instance
        while (InputManager.Instance == null)
            yield return null;

        InputManager.Instance.OnNavigate += HandleNavigate;
        InputManager.Instance.OnConfirm += HandleConfirm;

        ConfigurarMenuPrincipal();
    }

    private void OnDisable()
    {
        if (InputManager.Instance == null) return;

        InputManager.Instance.OnNavigate -= HandleNavigate;
        InputManager.Instance.OnConfirm -= HandleConfirm;
    }

    public void OnPointerEnterButton(Selectable selectable)
    {
        int index = elementosActuales.IndexOf(selectable);
        if (index >= 0)
        {
            indiceSeleccionado = index;
            selectable.Select();
        }
    }

    // =========================
    // NAVEGACIÓN
    // =========================

    private void HandleNavigate(Vector2 direction)
    {
        if (elementosActuales.Count == 0) return;

        Selectable actual = elementosActuales[indiceSeleccionado];

        // ── Si es slider ─────────────────────────
        if (actual is Slider slider)
        {
            // Modificar valor con izquierda/derecha
            if (direction.x > 0)
                slider.value += 0.1f;

            if (direction.x < 0)
                slider.value -= 0.1f;

            // SOLO salimos si fue movimiento horizontal
            if (direction.x != 0)
                return;
        }

        // ── Navegación vertical normal ───────────
        if (direction.y > 0)
            indiceSeleccionado--;
        else if (direction.y < 0)
            indiceSeleccionado++;

        if (indiceSeleccionado < 0)
            indiceSeleccionado = elementosActuales.Count - 1;

        if (indiceSeleccionado >= elementosActuales.Count)
            indiceSeleccionado = 0;

        elementosActuales[indiceSeleccionado].Select();
    }

    private void HandleConfirm()
    {
        if (elementosActuales.Count == 0) return;

        Selectable actual = elementosActuales[indiceSeleccionado];

        if (actual is Button boton)
        {
            boton.onClick.Invoke();
        }
        else if (actual is Toggle toggle)
        {
            toggle.isOn = !toggle.isOn;
        }
    }

    // =========================
    // CONFIGURACIONES
    // =========================

    private void ConfigurarMenuPrincipal()
    {
        elementosActuales.Clear();
        elementosActuales.Add(startButton);
        elementosActuales.Add(optionsButton);
        elementosActuales.Add(creditsButton);
        elementosActuales.Add(quitButton);

        indiceSeleccionado = 0;
        elementosActuales[indiceSeleccionado].Select();
    }

    private void ConfigurarOptions()
    {
        elementosActuales.Clear();
        elementosActuales.Add(fullscreenToggle);
        elementosActuales.Add(volumeSlider);
        elementosActuales.Add(backOptionsButton);

        indiceSeleccionado = 0;
        elementosActuales[indiceSeleccionado].Select();
    }

    private void ConfigurarCredits()
    {
        elementosActuales.Clear();
        elementosActuales.Add(backCreditsButton);

        indiceSeleccionado = 0;
        elementosActuales[indiceSeleccionado].Select();
    }

    private void ConfigurarHistoria()
    {
        elementosActuales.Clear();
        elementosActuales.Add(btnSiguiente);

        indiceSeleccionado = 0;
        elementosActuales[indiceSeleccionado].Select();
    }

    // =========================
    // HISTORIA
    // =========================

    private void IniciarHistoria()
    {
        menuPanel.SetActive(false);
        panelHistoria.SetActive(true);

        indiceHistoria = 0;
        MostrarHistoria();
        ConfigurarHistoria();
    }

    private void SiguienteHistoria()
    {
        indiceHistoria++;

        if (indiceHistoria >= listaHistoria.Count)
        {
            SceneManager.LoadScene("#1");
            return;
        }

        MostrarHistoria();
    }

    private void MostrarHistoria()
    {
        if (listaHistoria.Count == 0) return;
        historiaImage.texture = listaHistoria[indiceHistoria];
    }

    // =========================
    // OPTIONS
    // =========================

    private void OpenOptions()
    {
        menuPanel.SetActive(false);
        optionsPanel.SetActive(true);
        ConfigurarOptions();
    }

    private void CloseOptions()
    {
        optionsPanel.SetActive(false);
        menuPanel.SetActive(true);
        ConfigurarMenuPrincipal();
    }

    // =========================
    // CREDITS
    // =========================

    private void OpenCredits()
    {
        menuPanel.SetActive(false);
        creditsPanel.SetActive(true);
        ConfigurarCredits();
    }

    private void CloseCredits()
    {
        creditsPanel.SetActive(false);
        menuPanel.SetActive(true);
        ConfigurarMenuPrincipal();
    }

    // =========================
    // QUIT
    // =========================

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
        Debug.Log("No se puede cerrar WebGL.");
#else
        Application.Quit();
#endif
    }
}