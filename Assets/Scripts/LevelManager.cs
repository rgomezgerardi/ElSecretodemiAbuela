using Unity.VisualScripting.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager: MonoBehaviour {
    [SerializeField] private Button startButton;

    private void Awake()
    {
        startButton.onClick.AddListener(LoadGameScene);
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene("#1"); // Reemplaza con el nombre de tu escena
    }
}
