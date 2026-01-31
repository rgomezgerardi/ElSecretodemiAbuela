using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Objetos del nivel")]
    [SerializeField] private List<ObjetoCarta> objetosCartas;

    private int nivelActual;
    private int topeCartas;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        InicializarNivel();
    }

    private void InicializarNivel()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager no encontrado.");
            return;
        }

        nivelActual = GameManager.Instance.NivelActual;
        topeCartas = GameManager.Instance.TopeCartas;

        Debug.Log($"LevelManager inicializado → Nivel {nivelActual} | Cartas: 1 a {topeCartas}");
    }
}
