using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("Botones")]
    [SerializeField] private Button resumeBtn;
    [SerializeField] private Button restartBtn;
    [SerializeField] private Button menuBtn;

    private List<Button> botones = new List<Button>();
    private int indiceSeleccionado = 0;
    private bool activo = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        // 🔥 Construir lista AQUÍ (no en Start)
        botones.Clear();

        if (resumeBtn != null) botones.Add(resumeBtn);
        if (restartBtn != null) botones.Add(restartBtn);
        if (menuBtn != null) botones.Add(menuBtn);
    }

    void Start()
    {
        // Listeners
        if (resumeBtn != null)
            resumeBtn.onClick.AddListener(ResumeGame);

        if (restartBtn != null)
            restartBtn.onClick.AddListener(RestartLevel);

        if (menuBtn != null)
            menuBtn.onClick.AddListener(GoToMainMenu);
    }

    void OnEnable()
    {
        activo = true;
        indiceSeleccionado = 0;

        if (botones.Count > 0)
            ActualizarSeleccion();

        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnNavigate += HandleNavigate;
            InputManager.Instance.OnConfirm += HandleConfirm;
        }
    }

    void OnDisable()
    {
        activo = false;

        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnNavigate -= HandleNavigate;
            InputManager.Instance.OnConfirm -= HandleConfirm;
        }
    }

    private void HandleNavigate(Vector2 dir)
    {
        if (InputManager.Instance.CurrentState != GameInputState.Pausa)
            return;

        if (!activo || botones.Count == 0)
            return;

        if (dir.y > 0.5f)
            indiceSeleccionado--;
        else if (dir.y < -0.5f)
            indiceSeleccionado++;

        // Wrap
        if (indiceSeleccionado < 0)
            indiceSeleccionado = botones.Count - 1;

        if (indiceSeleccionado >= botones.Count)
            indiceSeleccionado = 0;

        ActualizarSeleccion();
    }

    private void HandleConfirm()
    {
        if (InputManager.Instance.CurrentState != GameInputState.Pausa)
            return;

        if (!activo) return;

        botones[indiceSeleccionado].onClick.Invoke();
    }

    private void ActualizarSeleccion()
    {
        if (botones.Count == 0)
            return;

        indiceSeleccionado = Mathf.Clamp(indiceSeleccionado, 0, botones.Count - 1);
        botones[indiceSeleccionado].Select();
    }

    // =====================
    // ACCIONES
    // =====================

    private void ResumeGame()
    {
        if (LevelManager.Instance != null)
            LevelManager.Instance.DesactivarPausa();
    }

    private void RestartLevel()
    {
        if (LevelManager.Instance != null)
            LevelManager.Instance.ReiniciarNivel();

        ResumeGame();
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}