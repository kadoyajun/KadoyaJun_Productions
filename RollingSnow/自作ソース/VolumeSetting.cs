using UnityEngine;

public class VolumeSetting : MonoBehaviour
{
    [SerializeField]
    private AudioSource BGM;
    [SerializeField]
    private AudioSource SE;
    void Start()
    {
        BGM.volume = PlayerPrefs.GetFloat("BGM");
        SE.volume = PlayerPrefs.GetFloat("SE");
    }

}
