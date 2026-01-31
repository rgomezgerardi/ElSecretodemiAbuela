using System.Collections;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [Header("Datos de la carta")]
    [SerializeField] private ObjetoCarta carta;

    [Header("Animación")]
    [SerializeField] private float duracionGiro = 0.3f;

    private bool estaGirando = false;
    private bool estaBloqueada = false;

    public ObjetoCarta Carta => carta;

    public void SetValorCarta(int valor)
    {
        carta.SetValor(valor);
    }

    public void OnClick()
    {
        if (estaGirando || estaBloqueada)
            return;

        if (LevelManager.Instance == null)
            return;

        StartCoroutine(GirarYEvaluar());
    }

    private IEnumerator GirarYEvaluar()
    {
        estaGirando = true;

        // Giro para mostrar carta
        yield return StartCoroutine(Girar(0f, 180f));

        bool acierto = LevelManager.Instance.EvaluarCarta(carta);

        if (acierto)
        {
            estaBloqueada = true;
        }
        else
        {
            yield return new WaitForSeconds(0.5f);

            // Volver a ocultar
            yield return StartCoroutine(Girar(180f, 0f));
        }

        estaGirando = false;
    }

    private IEnumerator Girar(float desde, float hasta)
    {
        float tiempo = 0f;

        while (tiempo < duracionGiro)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracionGiro;

            float anguloY = Mathf.Lerp(desde, hasta, t);
            transform.localRotation = Quaternion.Euler(0f, anguloY, 0f);

            yield return null;
        }

        transform.localRotation = Quaternion.Euler(0f, hasta, 0f);
    }
}
