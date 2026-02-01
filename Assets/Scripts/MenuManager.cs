using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button backOptionsButton;
    [SerializeField] private Button backCreditsButton;

    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject creditsPanel;

    private void Awake()
    {
        startButton.onClick.AddListener(StartGame);
        optionsButton.onClick.AddListener(OpenOptions);
        creditsButton.onClick.AddListener(OpenCredits);
        backOptionsButton.onClick.AddListener(CloseOptions);
        backCreditsButton.onClick.AddListener(CloseCredits);
        quitButton.onClick.AddListener(QuitGame);
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

    private void OpenCredits()
    {
        menuPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }

    private void CloseCredits()
    {
        creditsPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    private void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}