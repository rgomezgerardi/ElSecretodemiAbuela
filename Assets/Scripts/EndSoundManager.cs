using UnityEngine;

public class EndSoundManager : MonoBehaviour
{
    public static EndSoundManager Instance { get; private set; }

    [Header("Audio")]
    [SerializeField] private AudioSource winSource;
    [SerializeField] private AudioSource loseSource;

    private void Awake()
    {
        Instance = this;
    }

    public void PlayWin()
    {
        if (winSource != null) winSource.Stop();
        if (loseSource != null) loseSource.Stop();
        PlayWinInterno();
    }

    public void PlayLose()
    {
        if (winSource != null) winSource.Stop();
        if (loseSource != null) loseSource.Stop();
        PlayLoseInterno();
    }

    private void PlayWinInterno()
    {
        if (winSource != null)
        {
            winSource.Play();
        }
    }

    private void PlayLoseInterno()
    {
        if (loseSource != null)
        {
            loseSource.Play();
        }
    }
}
