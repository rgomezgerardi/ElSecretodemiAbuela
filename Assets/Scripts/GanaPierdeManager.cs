using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GanaPierdeManager : MonoBehaviour
{
    [SerializeField] private Text tituloGanPierde;
    [SerializeField] private Button exitGame;

    void Start()
    {
        // Verificamos que GameManager exista
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager no encontrado");
            return;
        }

        // Configurar el texto según ganePartida
        // 0 = sobrevive, 1 = muerto
        int ganaPartida = GameManager.Instance.ganoPartida;

        if (ganaPartida == 1)
        {
            tituloGanPierde.text = "YOU SURVIVE";
        }
        else
        {
            tituloGanPierde.text = "YOU ARE DEAD";
        }

        // Configurar listener del botón
        exitGame.onClick.RemoveAllListeners(); // por si ya tenía
        exitGame.onClick.AddListener(IrAMainMenu);
    }

    private void IrAMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
