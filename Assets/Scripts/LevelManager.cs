using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Objetos del nivel")]
    [SerializeField] private List<GameObject> objetosCartas;

    [Header("Estado del nivel")]
    [SerializeField] private int nivelActual;
    [SerializeField] private int topeCartas;
    [SerializeField] private int ultimoAcierto;

    [Header("Tiempo")]
    [SerializeField] private float tiempoNivel;
    [SerializeField] private float tiempoBonificacion;

    private bool nivelActivo = true;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
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

    void Update()
    {
        if (!nivelActivo)
            return;

        tiempoNivel -= Time.deltaTime;

        if (tiempoNivel <= 0f)
        {
            tiempoNivel = 0f;
            TerminarNivel(false);
        }
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
        ultimoAcierto = 0;

        tiempoBonificacion = GameManager.Instance.BonificacionTiempo;
        tiempoNivel = 60f + tiempoBonificacion;

        AsignarCartasRandom();

        Debug.Log($"Nivel {nivelActual} iniciado | Tiempo: {tiempoNivel}s");
    }

    public bool EvaluarCarta(ObjetoCarta carta)
    {
        int valor = carta.ValorCarta;

        if (valor > ultimoAcierto && valor <= topeCartas)
        {
            ultimoAcierto = valor;
            Debug.Log($"ACIERTO → Carta {valor}");

            if (ultimoAcierto == topeCartas)
                TerminarNivel(true);

            return true;
        }

        Debug.Log($"ERROR → Carta {valor}");
        return false;
    }

    private void TerminarNivel(bool completado)
    {
        nivelActivo = false;

        if (completado)
        {
            //GameManager.Instance.GuardarBonificacion(tiempoNivel);
            GameManager.Instance.AvanzarNivel();
            Debug.Log($"Nivel completado | Bonificación: {tiempoNivel}s");
        }
        else
        {
            //GameManager.Instance.GuardarBonificacion(0f);
            Debug.Log("Nivel fallado | Sin bonificación");
        }
    }

    private void AsignarCartasRandom()
    {
        List<int> pool = new List<int>();
        for (int i = 1; i <= 20; i++)
            pool.Add(i);

        for (int i = 0; i < pool.Count; i++)
        {
            int r = Random.Range(i, pool.Count);
            (pool[i], pool[r]) = (pool[r], pool[i]);
        }

        for (int i = 0; i < objetosCartas.Count; i++)
        {
            ObjetoCarta carta = objetosCartas[i].GetComponent<ObjetoCarta>();

            if (carta == null)
            {
                Debug.LogError($"El objeto {objetosCartas[i].name} no tiene ObjetoCarta");
                continue;
            }

            carta.SetValor(pool[i]);
        }
    }

}
