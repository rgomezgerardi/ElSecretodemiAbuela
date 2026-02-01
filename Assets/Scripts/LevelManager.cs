using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Cartas del nivel")]
    [SerializeField] private List<CardManager> cartas;

    [Header("Estado")]
    [SerializeField] private int nivelActual;
    [SerializeField] private int topeCartas;
    [SerializeField] private int ultimoAcierto;

    [Header("Tiempo")]
    [SerializeField] private float tiempoNivel;
    public float TiempoNivel => tiempoNivel;
    public float TiempoRestante => tiempoRestante;
    [SerializeField] private float tiempoRestante;

    private bool nivelActivo;

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

        tiempoRestante -= Time.deltaTime;

        if (tiempoRestante <= 0f)
        {
            tiempoRestante = 0f;
            FinNivel(false);
        }
    }

    private void InicializarNivel()
    {
        nivelActual = GameManager.Instance.NivelActual;
        topeCartas = GameManager.Instance.TopeCartas;

        ultimoAcierto = 0;

        tiempoNivel = 60f + GameManager.Instance.BonificacionTiempo;
        tiempoRestante = tiempoNivel;

        AsignarCartasRandom();

        nivelActivo = true;
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

        for (int i = 0; i < cartas.Count; i++)
            cartas[i].SetValorCarta(pool[i]);
    }

    public bool EvaluarCarta(ObjetoCarta carta)
    {
        int valor = carta.ValorCarta;

        if (valor == ultimoAcierto + 1 && valor <= topeCartas)
        {
            ultimoAcierto = valor;

            if (ultimoAcierto == topeCartas)
                FinNivel(true);

            return true;
        }

        return false;
    }

    private void FinNivelInterno(bool ganado)
    {
        nivelActivo = false;

        if (ganado)
        {
            GameManager.Instance.GuardarBonificacion(tiempoRestante);
            GameManager.Instance.AvanzarNivel();
        }
        else
        {
            GameManager.Instance.GuardarBonificacion(0f);
        }
    }

    public void FinNivel(bool ganado)
    {
        FinNivelInterno(ganado);
    }
}
