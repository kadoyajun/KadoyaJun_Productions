using UnityEngine;
using UnityEngine.SceneManagement;

public class AppControll : MonoBehaviour
{
    [System.Obsolete]
    private void Start()
    {
        Physics2D.autoSimulation = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("MainScene");
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Application.Quit();
        }
    }
}
