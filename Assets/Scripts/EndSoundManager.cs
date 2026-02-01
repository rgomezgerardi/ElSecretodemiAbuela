using UnityEngine;

public class EndSoundManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource winSource;
    [SerializeField] private AudioSource loseSource;

    void Start()
    {
        ReproducirResultado();
    }

    private void ReproducirResultado()
    {
        if (GameManager.Instance == null)
            return;

        int resultado = GameManager.Instance.ganoPartida;

        // Seguridad: apagar ambos
        if (winSource != null) winSource.Stop();
        if (loseSource != null) loseSource.Stop();

        if (resultado == 1)
        {
            if (winSource != null)
                winSource.Play();
        }
        else
        {
            if (loseSource != null)
                loseSource.Play();
        }
    }
}
