using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameScene : MonoBehaviour
{
    [SerializeField]
    private GameObject fish = null;

    [SerializeField]
    private GameObject[] ground = new GameObject[5];

    [SerializeField]
    private TextMeshProUGUI textUI = null;

    [SerializeField]
    private GameObject pauseUI = null;

    float number = 0;
    private void Start()
    {
        pauseUI.SetActive(false);
        for (int i = 0; i < 30; i++)
        {
            int groundNumber = UnityEngine.Random.Range(0, ground.Length);
            Vector3 setGroundPos = new Vector3(200 + 90 * i, -0.5f, 0);
            Instantiate(ground[groundNumber], setGroundPos, Quaternion.identity);
        }
    }
    void Update()
    {
        //fish‚ÌxÀ•W‚ð¬”‘æˆê‚Ü‚Å•\Ž¦
        number = fish.transform.position.x;
        textUI.text = number.ToString("f1") + "m";

        if(pauseUI.activeSelf == false)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }
    }
    public void OnResultScene(float point,float type)
    {
        PlayerPrefs.SetFloat("RangeScoreDate", number * 100);
        PlayerPrefs.SetFloat("PointScoreDate", point);
        PlayerPrefs.SetFloat("TypeScoreDate", type);
        SceneManager.LoadScene("ResultScene");
    }

    public void PauseUI_HideAndShow()
    {
        if(pauseUI.activeSelf == false)
        {
            pauseUI.SetActive(true);
        }
        else if(pauseUI.activeSelf == true)
        {
            pauseUI.SetActive(false);
        }
    }
}
