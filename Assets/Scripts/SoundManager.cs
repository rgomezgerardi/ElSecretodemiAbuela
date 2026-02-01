using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Música Ambiental")]
    [SerializeField] private AudioSource musicaAmbiental;

    [Header("Efectos de Sonido")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip clickSound;

    [Header("Configuración")]
    [Range(0f, 1f)] public float musicaVolumen = 0.5f;
    [Range(0f, 1f)] public float sfxVolumen = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            if (musicaAmbiental != null)
            {
                musicaAmbiental.loop = true;
                musicaAmbiental.volume = musicaVolumen;
                musicaAmbiental.Play();
            }

            if (sfxSource != null)
                sfxSource.volume = sfxVolumen;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusicaAmbiental()
    {
        if (musicaAmbiental == null) return;
        if (!musicaAmbiental.isPlaying)
            musicaAmbiental.Play();
    }

    public void PausarMusicaAmbiental()
    {
        if (musicaAmbiental == null) return;
        musicaAmbiental.Pause();
    }

    public void PlayClickSound()
    {
        if (sfxSource == null || clickSound == null) return;

        sfxSource.PlayOneShot(clickSound, sfxVolumen);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;

        sfxSource.PlayOneShot(clip, sfxVolumen);
    }

    public void ToggleMute(bool estado)
    {
        if (musicaAmbiental != null)
            musicaAmbiental.mute = estado;
        if (sfxSource != null)
            sfxSource.mute = estado;
    }
}
