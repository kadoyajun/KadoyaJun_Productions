using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHPUI : MonoBehaviour
{
    [SerializeField]
    Image image;
    [SerializeField]
    TextMeshProUGUI textMeshProUGUI;
    public void ChangeBossHPBar(float p)
    {
        if(p > 0)
        {
            image.fillAmount = p;
            int parcent = Mathf.CeilToInt(p * 100);
            textMeshProUGUI.text = parcent.ToString() + "%";
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
