using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using static GameScene;

public class Goal : MonoBehaviour
{
    public TimeManager timeManager;

    // コンポーネントを参照
    GameScene gameScene;
    public Animator goalFadeout;

    private void Start()
    {
        gameScene = GameObject.Find("Sceneroot").GetComponent<GameScene>();
    }
    void OnCollisionStay(UnityEngine.Collision other)
    {
        //Debug.Log("aaa");
        if (other.gameObject.tag == "Body")
        {
            //Debug.Log("Body");
            if (gameScene.gameState == SceneState.Play)
            {
                if (timeManager != null)
                {
                    gameScene.gameState = SceneState.StageClear;
                    goalFadeout.SetTrigger("FadeOut");
                }
                StartCoroutine(goal());
                //SceneManager.LoadScene("StageClearScene");
            }
        }
    }

    IEnumerator goal()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("StageClearScene");
    }


}
