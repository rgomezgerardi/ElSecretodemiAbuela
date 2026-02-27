using System.Collections;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [Header("Datos de la carta")]
    [SerializeField] private ObjetoCarta carta;
    public ObjetoCarta Carta => carta;
    public int ValorCarta => carta != null ? carta.ValorCarta : -1;
    public bool EstaBocaAbajo => !estaBloqueada && !estaGirando;
    public bool EstaBloqueada => estaBloqueada;

    [Header("Frontal")]
    [SerializeField] private Transform cuboFrontal;
    private Renderer rendFrontal;

    [Header("Animación")]
    [SerializeField] private float duracionGiro = 0.3f;

    [SerializeField] private bool estaGirando;
    [SerializeField] private bool estaBloqueada;

    [Header("Highlight")]
    [SerializeField] private Renderer rend;
    [SerializeField] private Color colorNormal = Color.white;
    [SerializeField] private Color colorHighlight = Color.yellow;

    private bool cancelarCoroutine = false;

    void Awake()
    {
        if (cuboFrontal != null)
            rendFrontal = cuboFrontal.GetComponent<Renderer>();
    }

    public void AsignarTextura(Texture2D textura)
    {
        if (rendFrontal == null || textura == null)
            return;

        rendFrontal.material = new Material(rendFrontal.material);
        rendFrontal.material.mainTexture = textura;
    }

    public void SetValorCarta(int valor)
    {
        carta.SetValor(valor);

        int index = valor - 1; // -1 porque el array es 0-based

        if (GameManager.Instance.cartasFrente != null &&
            index >= 0 && index < GameManager.Instance.cartasFrente.Count)
        {
            AsignarTextura(GameManager.Instance.cartasFrente[index]);
        }
    }

    public void SetHighlight(bool estado)
    {
        if (estaBloqueada) return;
        if (rend == null) return;

        // [gamepad-support] Trackear si la carta tiene highlight de pista activo
        // para que MostrarSelector no lo pise al restaurar el color
        tieneHighlightPista = estado;

        Material mat = rend.material;
        mat.color = estado ? colorHighlight : colorNormal;

        if (estado)
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", colorHighlight);
        }
        else
        {
            mat.SetColor("_EmissionColor", Color.black);
            mat.DisableKeyword("_EMISSION");
        }
    }

    public void OnClick()
    {
        VoltearCarta();
    }

    private void VoltearCarta()
    {
        if (estaGirando || estaBloqueada || LevelManager.Instance == null)
            return;

        SoundManager.Instance.PlayClickSound();
        StartCoroutine(GirarYEvaluar());
    }

    private IEnumerator GirarYEvaluar()
    {
        estaGirando = true;
        cancelarCoroutine = false;

        yield return StartCoroutine(Girar(0f, 180f));

        if (cancelarCoroutine) { estaGirando = false; yield break; }

        bool acierto = LevelManager.Instance.EvaluarCarta(carta);

        if (cancelarCoroutine) { estaGirando = false; yield break; }

        if (!acierto)
        {
            yield return new WaitForSeconds(0.5f);

            if (cancelarCoroutine) { estaGirando = false; yield break; }

            yield return StartCoroutine(Girar(180f, 0f));
        }
        else
        {
            estaBloqueada = true;
        }

        estaGirando = false;
    }

    private IEnumerator Girar(float desde, float hasta)
    {
        float tiempo = 0f;
        Vector3 posicionInicial = transform.localPosition;
        float alturaSalto = 0.2f;

        while (tiempo < duracionGiro)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracionGiro;

            float anguloZ = Mathf.Lerp(desde, hasta, t);
            transform.localRotation = Quaternion.Euler(0f, 0f, anguloZ);

            // Movimiento en Y (sube y baja)
            float desplazamientoY = t <= 0.5f
                ? Mathf.Lerp(0f, alturaSalto, t / 0.5f)
                : Mathf.Lerp(alturaSalto, 0f, (t - 0.5f) / 0.5f);

            transform.localPosition = posicionInicial + Vector3.up * desplazamientoY;

            yield return null;
        }

        transform.localRotation = Quaternion.Euler(0f, 0f, hasta);
        transform.localPosition = posicionInicial;
    }

    public void ResetCarta()
    {
        cancelarCoroutine = true;
        StopAllCoroutines();

        estaGirando = false;
        estaBloqueada = false;

        transform.localRotation = Quaternion.identity;
        SetHighlight(false);

        // [gamepad-support] Limpiar estado del selector al resetear la carta
        tieneHighlightPista = false;
        if (quadSelector != null)
            quadSelector.SetActive(false);
    }

    // ── Cambios feature/gamepad-support ──────────────────────────

    // Indica si la carta tiene activo el highlight de pista (amarillo),
    // para que MostrarSelector no lo sobreescriba al restaurar el color base.
    private bool tieneHighlightPista = false;

    // Quad semitransparente que actúa como indicador de selección del mando.
    // Se crea una sola vez y se reutiliza activando/desactivando.
    private GameObject quadSelector;

    /// <summary>
    /// Muestra u oculta el quad selector encima de la carta.
    /// El quad se crea en runtime la primera vez que se necesita.
    /// No interfiere con el sistema de highlights de pistas.
    /// </summary>
    public void MostrarSelector(bool estado)
    {
        if (estado)
        {
            if (quadSelector == null)
            {
                quadSelector = GameObject.CreatePrimitive(PrimitiveType.Quad);
                quadSelector.name = "SelectorGamepad";

                // Sin collider para no interferir con el raycast del mouse
                Destroy(quadSelector.GetComponent<Collider>());

                Material mat = new Material(Shader.Find("Sprites/Default"));
                mat.color = new Color(1f, 1f, 1f, 0.35f);
                quadSelector.GetComponent<Renderer>().material = mat;

                quadSelector.transform.SetParent(transform);
                quadSelector.transform.localPosition = new Vector3(0f, 0.01f, 0f);
                quadSelector.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                quadSelector.transform.localScale = new Vector3(1.1f, 1.1f, 1f);
            }

            quadSelector.SetActive(true);
        }
        else
        {
            if (quadSelector != null)
                quadSelector.SetActive(false);
        }
    }
}
