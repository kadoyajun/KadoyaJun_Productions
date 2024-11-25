using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FishingScene : MonoBehaviour
{
    [SerializeField]
    private GameObject gauge = null;

    [SerializeField]
    private GameObject pauseUI = null;
    void Start()
    {
        PlayerPrefs.DeleteKey("fishType");
        PlayerPrefs.DeleteKey("smashForece");
        PlayerPrefs.DeleteKey("RangeScoreDate");
        PlayerPrefs.DeleteKey("PointScoreDate");
        PlayerPrefs.DeleteKey("TypeScoreDate");
        pauseUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (pauseUI.activeSelf == false)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }
    }

    public void FishingEnd(int fishType)
    {
        float smashForce = gauge.transform.localPosition.y / 200 + 0.5f;
        Debug.Log(gauge.transform.localPosition.y);
        Debug.Log(smashForce);
        PlayerPrefs.SetFloat("smashForce", smashForce);
        PlayerPrefs.SetInt("fishType", fishType);
        StartCoroutine(FishEnd());
    }

    IEnumerator FishEnd()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("GameScene");
    }

    public void PauseUI_HideAndShow()
    {
        if (pauseUI.activeSelf == false)
        {
            pauseUI.SetActive(true);
        }
        else if (pauseUI.activeSelf == true)
        {
            pauseUI.SetActive(false);
        }
    }
}
