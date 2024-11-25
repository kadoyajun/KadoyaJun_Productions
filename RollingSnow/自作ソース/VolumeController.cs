using UnityEngine;
using UnityEngine.UI;

// "AudioSource"コンポーネントがアタッチされていない場合アタッチ
//[RequireComponent(typeof(AudioSource))]
public class VolumeController : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private string valueName;

    private void Start()
    {
        Slider volumeSlider = GetComponent<Slider>();
        volumeSlider.value = PlayerPrefs.GetFloat(valueName);
    }
    public void SoundSliderOnValueChange(float newSliderValue)
    {
        // 音楽の音量をスライドバーの値に変更
        audioSource.volume = newSliderValue;
        PlayerPrefs.SetFloat(valueName, newSliderValue);
    }
}