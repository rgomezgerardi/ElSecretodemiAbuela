using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    [Header("Botones")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button backOptionsButton;
    [SerializeField] private Button backCreditsButton;

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

    // =========================
    // HISTORIA
    // =========================
    private void IniciarHistoria()
    {
        menuPanel.SetActive(false);
        panelHistoria.SetActive(true);

        indiceHistoria = 0;
        MostrarHistoria();
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
        if (listaHistoria.Count == 0)
            return;

        historiaImage.texture = listaHistoria[indiceHistoria];
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
