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

    [Header("Animación")]
    [SerializeField] private float duracionGiro = 0.3f;

    [SerializeField] private bool estaGirando;
    [SerializeField] private bool estaBloqueada;

    [Header("Highlight")]
    [SerializeField] private Renderer rend;
    [SerializeField] private Color colorNormal = Color.white;
    [SerializeField] private Color colorHighlight = Color.yellow;


    public void SetValorCarta(int valor)
    {
        carta.SetValor(valor);
    }

    public void SetHighlight(bool estado)
    {
        if (estaBloqueada)
            return;

        if (rend == null)
            return;

        rend.material.color = estado ? colorHighlight : colorNormal;
        Debug.Log("HighLight realizado");
    }


    public void OnClick()
    {
        if (estaGirando || estaBloqueada || LevelManager.Instance == null)
            return;

        StartCoroutine(GirarYEvaluar());
    }

    private IEnumerator GirarYEvaluar()
    {
        estaGirando = true;

        yield return StartCoroutine(Girar(0f, 180f));

        bool acierto = LevelManager.Instance.EvaluarCarta(carta);

        if (!acierto)
        {
            yield return new WaitForSeconds(0.5f);
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
        float alturaSalto = 0.2f; // ajustable

        while (tiempo < duracionGiro)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracionGiro;

            // Rotación
            float anguloZ = Mathf.Lerp(desde, hasta, t);
            transform.localRotation = Quaternion.Euler(0f, 0f, anguloZ);

            // Movimiento en Y (sube y baja)
            float desplazamientoY;

            if (t <= 0.5f)
            {
                desplazamientoY = Mathf.Lerp(0f, alturaSalto, t / 0.5f);
            }
            else
            {
                desplazamientoY = Mathf.Lerp(alturaSalto, 0f, (t - 0.5f) / 0.5f);
            }

            transform.localPosition = posicionInicial + Vector3.up * desplazamientoY;

            yield return null;
        }

        transform.localRotation = Quaternion.Euler(0f, 0f, hasta);
        transform.localPosition = posicionInicial;
    }


}
