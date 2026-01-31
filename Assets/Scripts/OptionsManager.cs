using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TextMeshProUGUI volumeValueText; // Opcional, para mostrar el valor

    private void Start()
    {
        // Cargar configuraciones guardadas
        LoadSettings();

        // Agregar listeners
        volumeSlider.onValueChanged.AddListener(SetVolume);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }

    private void SetVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("Volume", value);

        if (volumeValueText != null)
        {
            volumeValueText.text = Mathf.RoundToInt(value * 100) + "%";
        }
    }

    private void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    private void LoadSettings()
    {
        // Cargar volumen
        float savedVolume = PlayerPrefs.GetFloat("Volume", 0.5f);
        volumeSlider.value = savedVolume;
        AudioListener.volume = savedVolume;

        // Cargar fullscreen
        int fullscreen = PlayerPrefs.GetInt("Fullscreen", 1);
        fullscreenToggle.isOn = fullscreen == 1;
        Screen.fullScreen = fullscreen == 1;
    }
}