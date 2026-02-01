using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Cartas del nivel (20)")]
    [SerializeField] private List<CardManager> cartas;

    [Header("Estado")]
    [SerializeField] private int nivelActual;
    public int NivelActual => nivelActual;
    [SerializeField] private int topeCartas;
    [SerializeField] private int ultimoAcierto;

    [Header("Tiempo")]
    [SerializeField] private float tiempoNivel;
    public float TiempoNivel => tiempoNivel;
    public float TiempoRestante => tiempoRestante;
    [SerializeField] private float tiempoRestante;

    [SerializeField] private bool enModoError;

    [Header("Menu Pause")]
    [SerializeField] private GameObject panelMenuPause;

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

        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            ActivarPausa();
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
        EnemyMovement.Instance.ActivarPorNivelMask();
        LimpiarHighlights();
        IluminarFilaSiguiente();

        nivelActivo = true;
    }

    private void AsignarCartasRandom()
    {
        List<int> pool = new List<int>();
        for (int i = 1; i <= 20; i++)
            pool.Add(i);

        // Mezclar
        for (int i = 0; i < pool.Count; i++)
        {
            int r = Random.Range(i, pool.Count);
            (pool[i], pool[r]) = (pool[r], pool[i]);
        }

        for (int i = 0; i < cartas.Count; i++)
        {
            cartas[i].SetValorCarta(pool[i]);
        }
    }


    public bool EvaluarCarta(ObjetoCarta carta)
    {
        int valor = carta.ValorCarta;

        if (valor == ultimoAcierto + 1 && valor <= topeCartas)
        {
            // ACIERTO
            ultimoAcierto = valor;

            // Beneficio de tiempo según nivel
            if (NivelActual == 1 || NivelActual == 2)
                tiempoRestante += 2f;
            else if (NivelActual == 3 || NivelActual == 4)
                tiempoRestante += 1f;

            LimpiarHighlights();

            if (ultimoAcierto == topeCartas)
            {
                FinNivel(true);
            }
            else
            {
                enModoError = false;
                IluminarFilaSiguiente();
            }

            return true;
        }


        // ERROR
        LimpiarHighlights();
        enModoError = true;
        IluminarError();

        // Penalización de tiempo según nivel
        if (NivelActual >= 1 && NivelActual <= 4)
            tiempoRestante -= 2f;
        else if (NivelActual == 5)
            tiempoRestante -= 3f;

        // Asegurarnos que no se pase de 0
        tiempoRestante = Mathf.Max(tiempoRestante, 0f);

        return false;
    }

    private void IluminarFilaSiguiente()
    {
        int siguienteValor = ultimoAcierto + 1;

        if (siguienteValor > topeCartas)
            return;

        int indexCarta = -1;

        for (int i = 0; i < cartas.Count; i++)
        {
            if (cartas[i].Carta.ValorCarta == siguienteValor)
            {
                indexCarta = i;
                break;
            }
        }

        if (indexCarta == -1)
            return;

        int fila = indexCarta / 5;
        int inicio = fila * 5;
        int fin = inicio + 5;

        for (int i = inicio; i < fin && i < cartas.Count; i++)
            cartas[i].SetHighlight(true);

        Debug.Log($"Highlight fila {fila} (valor esperado {siguienteValor})");
    }

    private void IluminarError()
    {
        List<CardManager> candidatas = new List<CardManager>();
        CardManager correcta = null;

        int valorCorrecto = ultimoAcierto + 1;

        foreach (var carta in cartas)
        {
            if (!carta.EstaBocaAbajo)
                continue;

            if (carta.ValorCarta == valorCorrecto)
            {
                correcta = carta;
            }
            else
            {
                candidatas.Add(carta);
            }
        }

        if (correcta == null)
            return;

        // Mezclar trampas
        for (int i = 0; i < candidatas.Count; i++)
        {
            int r = Random.Range(i, candidatas.Count);
            (candidatas[i], candidatas[r]) = (candidatas[r], candidatas[i]);
        }

        correcta.SetHighlight(true);

        int trampas = Mathf.Min(2, candidatas.Count);
        for (int i = 0; i < trampas; i++)
            candidatas[i].SetHighlight(true);
    }

    private void LimpiarHighlights()
    {
        foreach (var carta in cartas)
            carta.SetHighlight(false);
        Debug.Log("Se limpio el Highlight");
    }

    private void FinNivelInterno(bool ganado)
    {
        nivelActivo = false;
        LimpiarHighlights();

        if (ganado)
        {
            GameManager.Instance.GuardarBonificacion(tiempoRestante);
            GameManager.Instance.AvanzarNivel();

            if (GameManager.Instance.NivelActual <= 5)
            {
                ReiniciarNivel();
            }
            else
            {
                GameManager.Instance.ganoPartida = 1;
                SceneManager.LoadScene("GanaPierde");
            }
        }
        else
        {
            GameManager.Instance.ganoPartida = 0;
            SceneManager.LoadScene("GanaPierde");
        }
    }

    public void FinNivel(bool ganado)
    {
        FinNivelInterno(ganado);
    }

    public void ReiniciarNivel()
    {
        StopAllCoroutines();

        nivelActivo = false;
        enModoError = false;
        ultimoAcierto = 0;

        LimpiarHighlights();

        // Resetear estado de las cartas
        foreach (var carta in cartas)
            carta.ResetCarta();

        InicializarNivel();
    }

    private void ActivarPausaInterno()
    {
        panelMenuPause.SetActive(true);
        Time.timeScale = 0f;
        SoundManager.Instance.PausarMusicaAmbiental();

        RectTransform rt = panelMenuPause.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchoredPosition = Vector2.zero;
            rt.localScale = Vector3.one;
        }

        panelMenuPause.transform.SetAsLastSibling();
    }

    private void DesactivarPausaInterno()
    {
        panelMenuPause.SetActive(false);
        Time.timeScale = 1f;
        SoundManager.Instance.PlayMusicaAmbiental();
    }

    public void ActivarPausa()
    {
        ActivarPausaInterno();
    }

    public void DesactivarPausa()
    {
        DesactivarPausaInterno();
    }

}
