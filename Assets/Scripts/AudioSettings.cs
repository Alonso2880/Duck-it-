using UnityEngine;
using UnityEngine.Audio;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    [Header("Iconos")]

    public GameObject IconoMusica;
    public GameObject IconoSonido;
    public GameObject IconoNoMusica;
    public GameObject IconoNoSonido;

    public void SetMusicVolume(float volume)
    {
        // Convertir el valor del slider (0-1) a decibelios
        float volumeDB = volume <= 0.0001f
            ? -80f
            : Mathf.Log10(volume) * 20f;

        if(volumeDB <= -80)
        {
            IconoMusica.SetActive(false);
            IconoNoMusica.SetActive(true);
        }

        if (volumeDB > -80)
        {
            IconoMusica.SetActive(true);
            IconoNoMusica.SetActive(false);
        }

        audioMixer.SetFloat("MusicVolume", volumeDB);
    }

    public void SetEffectsVolume(float volume)
    {
        // Convertir el valor del slider (0-1) a decibelios
        float volumeDB = volume <= 0.0001f
            ? -80f
            : Mathf.Log10(volume) * 20f;

        if (volumeDB <= -80)
        {
            IconoSonido.SetActive(false);
            IconoNoSonido.SetActive(true);
        }

        if (volumeDB > -80)
        {
            IconoSonido.SetActive(true);
            IconoNoSonido.SetActive(false);
        }

        audioMixer.SetFloat("EffectsVolume", volumeDB);
    }
}
