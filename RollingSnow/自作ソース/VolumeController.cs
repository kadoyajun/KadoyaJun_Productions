using UnityEngine;
using UnityEngine.UI;

// "AudioSource"�R���|�[�l���g���A�^�b�`����Ă��Ȃ��ꍇ�A�^�b�`
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
        // ���y�̉��ʂ��X���C�h�o�[�̒l�ɕύX
        audioSource.volume = newSliderValue;
        PlayerPrefs.SetFloat(valueName, newSliderValue);
    }
}