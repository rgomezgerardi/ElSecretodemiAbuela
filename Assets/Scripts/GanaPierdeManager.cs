using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GanaPierdeManager : MonoBehaviour
{
    [SerializeField] private Text tituloGanPierde;
    [SerializeField] private Button exitGame;

    void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager no encontrado");
            return;
        }

        // Aseguramos estado de menú
        if (InputManager.Instance != null)
            InputManager.Instance.SetState(GameInputState.Menu);

        int ganaPartida = GameManager.Instance.ganoPartida;

        if (ganaPartida == 1)
        {
            EndSoundManager.Instance.PlayWin();
            tituloGanPierde.text = "HAS SOBREVIVIDO!";
        }
        else
        {
            EndSoundManager.Instance.PlayLose();
            tituloGanPierde.text = "HAS MUERTO...";
        }

        // Limpiar listeners previos
        exitGame.onClick.RemoveAllListeners();
        exitGame.onClick.AddListener(IrAMainMenu);

        // Selección automática para teclado y mando
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(exitGame.gameObject);
        }
    }

    void OnEnable()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.OnConfirm += HandleConfirm;
    }

    void OnDisable()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.OnConfirm -= HandleConfirm;
    }

    private void HandleConfirm()
    {
        exitGame.onClick.Invoke();
    }

    private void IrAMainMenu()
    {
        GameManager.Instance.ResetearInfo();
        SceneManager.LoadScene("MainMenu");
    }
}