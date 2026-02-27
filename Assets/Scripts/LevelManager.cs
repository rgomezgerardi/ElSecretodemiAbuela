using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    private int indiceSeleccionado = 0;

    [Header("Tiempo")]
    [SerializeField] private float tiempoNivel;
    public float TiempoNivel => tiempoNivel;
    public float TiempoRestante => tiempoRestante;
    [SerializeField] private float tiempoRestante;

    [Header("Menu Pause")]
    [SerializeField] private GameObject panelMenuPause;

    [Header("Sistema de Errores")]
    [SerializeField] private int erroresConsecutivos = 0;
    [SerializeField] private int maxErroresParaReaccion = 3;

    [Header("UI Fin de Nivel")]
    [SerializeField] private GameObject panelNivelSuperado;
    [SerializeField] private Text ptextNivelSuperado;

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

    void OnEnable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnNavigate += HandleNavigate;
            InputManager.Instance.OnConfirm += HandleConfirm;
            InputManager.Instance.OnLookEnemy += HandleLook;
            InputManager.Instance.OnPause += HandlePause;
        }
    }

    void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnNavigate -= HandleNavigate;
            InputManager.Instance.OnConfirm -= HandleConfirm;
            InputManager.Instance.OnLookEnemy -= HandleLook;
            InputManager.Instance.OnPause -= HandlePause;
        }
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

    // =========================
    // INPUT HANDLERS
    // =========================

    private void HandleNavigate(Vector2 dir)
    {
        if (!nivelActivo || cartas.Count == 0)
            return;

        int columnas = 5;
        int filas = cartas.Count / columnas;

        int filaActual = indiceSeleccionado / columnas;
        int colActual = indiceSeleccionado % columnas;

        if (dir.y > 0.5f)       // ARRIBA
            filaActual--;
        else if (dir.y < -0.5f) // ABAJO
            filaActual++;
        else if (dir.x > 0.5f)  // DERECHA
            colActual++;
        else if (dir.x < -0.5f) // IZQUIERDA
            colActual--;

        // Wrap vertical
        if (filaActual < 0) filaActual = filas - 1;
        if (filaActual >= filas) filaActual = 0;

        // Wrap horizontal
        if (colActual < 0) colActual = columnas - 1;
        if (colActual >= columnas) colActual = 0;

        indiceSeleccionado = filaActual * columnas + colActual;

        SeleccionarCartaActual();
    }

    private void HandleConfirm()
    {
        if (!nivelActivo || cartas.Count == 0)
            return;

        cartas[indiceSeleccionado].OnClick();
    }

    private void HandleLook(bool mirar)
    {
        if (!nivelActivo)
            return;

        CameraController cam = Camera.main.GetComponent<CameraController>();
        if (cam != null)
            cam.SetLook(mirar);
    }

    private IEnumerator ResetLook(CameraController cam)
    {
        yield return new WaitForSeconds(0.3f);

        if (cam != null)
            cam.controlActivo = true;
    }

    private void HandlePause()
    {
        if (!nivelActivo)
            return;

        if (panelMenuPause.activeSelf)
            DesactivarPausa();
        else
            ActivarPausa();
    }

    private void SeleccionarCartaActual()
    {
        for (int i = 0; i < cartas.Count; i++)
        {
            cartas[i].MostrarSelector(i == indiceSeleccionado);
        }
    }

    private void InicializarNivel()
    {
        nivelActivo = false;

        InputManager.Instance.SetState(GameInputState.Gameplay);

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


    private IEnumerator ReaccionEnemigo()
    {
        CameraController camController = Camera.main.GetComponent<CameraController>();
        if (camController != null)
            yield return StartCoroutine(LevantarCabeza(camController));

        yield return StartCoroutine(ScreenShake());
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator LevantarCabeza(CameraController camController)
    {
        camController.controlActivo = false;

        float duration = 0.3f;
        float elapsed = 0f;

        Quaternion rotacionInicial = camController.transform.rotation;
        Quaternion rotacionHaciaEnemigo = Quaternion.Euler(camController.forwardRotation);

        // Levantar cabeza rápidamente
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            camController.transform.rotation = Quaternion.Lerp(rotacionInicial, rotacionHaciaEnemigo, t);
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        // Volver a posición original
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            camController.transform.rotation = Quaternion.Lerp(rotacionHaciaEnemigo, Quaternion.Euler(camController.originalRotation), t);
            yield return null;
        }

        camController.controlActivo = true;
    }

    private IEnumerator ScreenShake()
    {
        Camera cam = Camera.main;
        Vector3 posicionOriginal = cam.transform.localPosition;

        float duracion = 0.5f;
        float magnitud = 0.1f;
        float elapsed = 0f;

        // [gamepad-support] Rumble sincronizado con el screen shake
        if (Gamepad.current != null)
            Gamepad.current.SetMotorSpeeds(0.6f, 0.8f);

        while (elapsed < duracion)
        {
            float x = Random.Range(-1f, 1f) * magnitud;
            float y = Random.Range(-1f, 1f) * magnitud;
            cam.transform.localPosition = posicionOriginal + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.transform.localPosition = posicionOriginal;

        // [gamepad-support] Detener rumble al terminar el shake
        if (Gamepad.current != null)
            Gamepad.current.SetMotorSpeeds(0f, 0f);
    }

    public bool EvaluarCarta(ObjetoCarta carta)
    {
        int valor = carta.ValorCarta;

        if (valor == ultimoAcierto + 1 && valor <= topeCartas)
        {
            // ACIERTO
            ultimoAcierto = valor;
            erroresConsecutivos = 0;

            if (NivelActual == 1 || NivelActual == 2)
                tiempoRestante += 2f;
            else if (NivelActual == 3 || NivelActual == 4)
                tiempoRestante += 1f;

            LimpiarHighlights();

            if (ultimoAcierto == topeCartas)
                FinNivel(true);
            else
            {
                IluminarFilaSiguiente();
            }

            return true;
        }

        // ERROR
        erroresConsecutivos++;

        LimpiarHighlights();
        IluminarError();

        if (NivelActual >= 1 && NivelActual <= 4)
            tiempoRestante -= 2f;
        else if (NivelActual == 5)
            tiempoRestante -= 3f;

        tiempoRestante = Mathf.Max(tiempoRestante, 0f);

        if (erroresConsecutivos >= maxErroresParaReaccion)
        {
            StartCoroutine(ReaccionEnemigo());
            erroresConsecutivos = 0;
        }

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
                correcta = carta;
            else
                candidatas.Add(carta);
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
    }

    private void FinNivelInterno(bool ganado)
    {
        nivelActivo = false;
        LimpiarHighlights();

        if (ganado)
        {
            GameManager.Instance.GuardarBonificacion(tiempoRestante);

            // 🔥 Guardamos el nivel que acaba de terminar
            int nivelQueSeAcabaDeGanar = GameManager.Instance.NivelActual;

            if (nivelQueSeAcabaDeGanar >= GameManager.Instance.MaxNivel)
            if (GameManager.Instance.NivelActual < 5)
                ReiniciarNivel();
            else
            {
                // Es el último nivel → terminar partida
                GameManager.Instance.ganoPartida = 1;
                SceneManager.LoadScene("GanaPierde");
            }
            else
            {
                // No es el último → avanzar
                GameManager.Instance.AvanzarNivel();
                StartCoroutine(TransicionNivelSuperado());
            }
        }
        else
        {
            GameManager.Instance.ganoPartida = 0;
            GameManager.Instance.ResetearInfo();
            SceneManager.LoadScene("GanaPierde");
        }
    }

    private IEnumerator TransicionNivelSuperado()
    {
        nivelActivo = false;

        ptextNivelSuperado.text = $"SOBREVIVISTE AL NIVEL {nivelActual}... PREPARATE PARA EL SIGUIENTE!";
        panelNivelSuperado.SetActive(true);

        yield return new WaitForSeconds(5f);

        panelNivelSuperado.SetActive(false);

        ReiniciarNivel();
    }

    public void FinNivel(bool ganado)
    {
        FinNivelInterno(ganado);
    }

    public void ReiniciarNivel()
    {
        StopAllCoroutines();

        nivelActivo = false;
        ultimoAcierto = 0;
        erroresConsecutivos = 0;

        LimpiarHighlights();

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
        InputManager.Instance.SetState(GameInputState.Pausa);
    }

    public void DesactivarPausa()
    {
        DesactivarPausaInterno();
        InputManager.Instance.SetState(GameInputState.Gameplay);
    }

    // ── Cambios feature/gamepad-support ──────────────────────────

    /// <summary>
    /// Expone la lista de cartas del nivel para que GamepadCardSelector
    /// pueda navegarlas sin romper el encapsulamiento del LevelManager.
    /// </summary>
    public List<CardManager> GetCartas() => cartas;
}
