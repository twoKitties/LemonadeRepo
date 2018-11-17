using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Slider effectsSlider;
    public Slider musicSlider;

    private void Start()
    {
        effectsSlider.value = SoundPlayer.SoundVolume;
        musicSlider.value = MusicPlayer.MusicVolume;
    }

    public void ChangeEffectsVolume()
    {
        SoundPlayer.SoundVolume = effectsSlider.value;
    }

    public void ChangeMusicVolume()
    {
        MusicPlayer.MusicVolume = musicSlider.value;
    }

    public void SelectLanguage(int index)
    {
    }
}
