using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Progresión")]
    [SerializeField] private int nivelActual = 1;
    [SerializeField] private int[] topesPorNivel = { 5, 8, 12, 16, 20 };

    private float bonificacionTiempo = 0f;

    public int NivelActual => nivelActual;
    public float BonificacionTiempo => bonificacionTiempo;

    public int ganoPartida;

    public int TopeCartas
    {
        get
        {
            int index = nivelActual - 1;
            if (index < 0 || index >= topesPorNivel.Length)
                return topesPorNivel[topesPorNivel.Length - 1];

            return topesPorNivel[index];
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AvanzarNivel()
    {
        nivelActual = Mathf.Min(nivelActual + 1, topesPorNivel.Length);
    }

    public void GuardarBonificacion(float tiempoSobrante)
    {
        bonificacionTiempo = Mathf.Max(0f, tiempoSobrante);
    }
}
