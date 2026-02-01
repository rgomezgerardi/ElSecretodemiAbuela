using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("Botones")]
    [SerializeField] private Button resumeBtn;
    [SerializeField] private Button restartBtn;
    [SerializeField] private Button menuBtn;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Configurar listeners
        if (resumeBtn != null)
            resumeBtn.onClick.AddListener(ResumeGame);

        if (restartBtn != null)
            restartBtn.onClick.AddListener(RestartLevel);

        if (menuBtn != null)
            menuBtn.onClick.AddListener(GoToMainMenu);
    }

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
        SceneManager.LoadScene("MainMenu");
    }
}
