using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

        exitGame.onClick.RemoveAllListeners();
        exitGame.onClick.AddListener(IrAMainMenu);
    }

    private void IrAMainMenu()
    {
        GameManager.Instance.ResetearInfo();
        SceneManager.LoadScene("MainMenu");
    }
}
