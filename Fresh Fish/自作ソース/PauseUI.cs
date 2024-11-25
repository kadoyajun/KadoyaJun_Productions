using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [SerializeField]
    private Selectable selectButton = null;
    void Start()
    {
        selectButton.Select();
    }

    public void Back()
    {
        gameObject.SetActive(false);
    }

    public void Retry()
    {
        SceneManager.LoadScene("FishingScene");
    }

    public void Title()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
