using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button backButton;

    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject optionsPanel;

    private void Awake()
    {
        startButton.onClick.AddListener(StartGame);
        optionsButton.onClick.AddListener(OpenOptions);
        backButton.onClick.AddListener(CloseOptions);

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
    }

    private void StartGame()
    {
        SceneManager.LoadScene("#1");
    }

    private void OpenOptions()
    {
        menuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    private void CloseOptions()
    {
        optionsPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    private void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}