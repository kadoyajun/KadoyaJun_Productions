using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    //最初に選択されるボタン
    [SerializeField]
    private Selectable Selectable = null;
    [SerializeField]
    private Selectable Selectable1 = null;
    [SerializeField]
    private Selectable Selectable2 = null;
    [SerializeField]
    private Selectable Selectable3 = null;
    [SerializeField]
    private Selectable Selectable4 = null;

    public void LoadGameScene()
    {
        // GameSceneを読み込む
        SceneManager.LoadScene("FishingScene");
    }

    public GameObject optionPanel; // オプション画面のパネル
　　public GameObject instructionObject;
    public GameObject keyboardPanel;
    public GameObject Credit;
   
    private void Start()
    {
        // 最初はオプション画面を非表示にする
        optionPanel.SetActive(false);
        instructionObject.SetActive(false);
        keyboardPanel.SetActive(false);
        Credit.SetActive(false);    

        //ボタンをセレクト
        Selectable.Select();
    }

    public void ToggleOptionPanel()
    {
        // オプションボタンが押されたときに呼ばれる関数
        // オプション画面の表示・非表示を切り替える
        optionPanel.SetActive(!optionPanel.activeSelf);
        Selectable1.Select();
    }

    public void ShowInstructionPanel()
    {
        // Instructionパネルを表示する
        instructionObject.SetActive(true);
        Selectable2.Select();
    }

    public void ShowKeyboardPanel() 
    { 
        // keyboard画面を表示させる。
        keyboardPanel.SetActive(true);
        Selectable3.Select();
    }

    public void Backcontroller()
    {
        // instruction画面に戻る。
        keyboardPanel.SetActive(false);
        instructionObject.SetActive(true);
        Selectable2.Select();
    }

    public void instructionBackOption()
    {
        //option画面に戻る。
        instructionObject.SetActive(!instructionObject.activeSelf);
        Selectable1.Select();
    }
    public string titleSceneName = "TitleScene"; // タイトル画面のシーン名

    public void GoCredit()
    {
        Credit.SetActive(true);
        Selectable4.Select();
    }

    public void BackCredit() 
    { 
        Credit.SetActive(!Credit.activeSelf);
        Selectable1.Select();
    }

    public void GoToTitleScene()
    {
        // タイトル画面のシーンに戻る
        SceneManager.LoadScene(titleSceneName);
    }

    public void ExitGame()
    {
        // ゲームを終了する
        Application.Quit();
    }
}
