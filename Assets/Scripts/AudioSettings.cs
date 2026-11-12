using UnityEngine;
using UnityEngine.Audio;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    public void SetMusicVolume(float volume)
    {
        // Convertir el valor del slider (0-1) a decibelios
        float volumeDB = volume <= 0.0001f
            ? -80f
            : Mathf.Log10(volume) * 20f;

        audioMixer.SetFloat("MusicVolume", volumeDB);
    }

    public void SetEffectsVolume(float volume)
    {
        // Convertir el valor del slider (0-1) a decibelios
        float volumeDB = volume <= 0.0001f
            ? -80f
            : Mathf.Log10(volume) * 20f;

        audioMixer.SetFloat("EffectsVolume", volumeDB);
    }
}
