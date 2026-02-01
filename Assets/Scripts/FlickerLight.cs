using UnityEngine;

public class FlickerLight : MonoBehaviour
{
    private Light lightSource;
    public float minIntensity = 1.5f;
    public float maxIntensity = 2.5f;
    public float flickerSpeed = 0.1f;

    void Start()
    {
        lightSource = GetComponent<Light>();
    }

    void Update()
    {
        lightSource.intensity = Mathf.Lerp(minIntensity, maxIntensity,
            Mathf.PerlinNoise(Time.time * flickerSpeed, 0));
    }
}