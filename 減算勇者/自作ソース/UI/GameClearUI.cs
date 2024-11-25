using Genzan;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameClearUI : MonoBehaviour
{
    // 初期選択ボタンの設定
    [SerializeField]
    private Selectable SelectButton;

    [SerializeField]
    TextMeshProUGUI stageClearPointText;

    [SerializeField]
    TextMeshProUGUI stageClearPoint;

    [SerializeField]
    TextMeshProUGUI bonusPoint;

    [SerializeField]
    TextMeshProUGUI sumPoint;

    [SerializeField]
    TextMeshProUGUI levelUpText;

    [SerializeField]
    uint[] stageClearPointValue;

    // Menu中動けないようにとりあえずInputSysemを指定
    [SerializeField]
    public InputActionProperty[] _inputActions;

    private InputAction[] _inputActionArray;

    Animator animator;

    static readonly int showId = Animator.StringToHash("Show");

    private void Awake()
    {
        _inputActionArray = new InputAction[_inputActions.Length];
    }

    void Start()
    {
    }

    // Scene移行
    public void GoTitle(string sceneName)
    {
        inputActionEnable();
        SceneManager.LoadScene(sceneName);
     }

    // UI表示時呼び出し
    public void ShowUI()
    {
        animator = GetComponent<Animator>();
        GameManager gM = transform.root.GetComponent<GameManager>();
        uint stageClearPt = stageClearPointValue[(int)gM.stage];
        stageClearPointText.text = LogText.StageClearText[(int)gM.stage];
        stageClearPoint.text = stageClearPt + "pt";
        uint bonusPt = (uint)(gM.Level / Mathf.CeilToInt((float)gM.Turn / 100));
        bonusPoint.text = bonusPt + "pt";
        uint sumPt = stageClearPt + bonusPt;
        sumPoint.text = sumPt.ToString() + "pt";
        levelUpText.text = sumPt + "レベルUp！";
        PlayerPrefs.SetInt("Level", (int)(PlayerPrefs.GetInt("Level") + sumPt));
        inputActionDisable();
        animator.SetTrigger(showId);
        StartCoroutine(OnShow());
    }

    IEnumerator OnShow()
    {
        // 4秒待機
        yield return new WaitForSeconds(4);
        // ボタン選択
        SelectButton.Select();
    }

    void inputActionDisable()
    {
        if (_inputActionArray == null)
        {
            _inputActionArray = new InputAction[_inputActions.Length];
        }

        for (int i = 0; i < _inputActions.Length; i++)
        {
            _inputActionArray[i] = _inputActions[i].action;
            if (_inputActionArray[i] != null)
            {
                _inputActionArray[i].Disable();
            }
        }
    }

    void inputActionEnable()
    {
        if (_inputActionArray == null)
        {
            _inputActionArray = new InputAction[_inputActions.Length];
            inputActionDisable(); // ここで再度Disableする
        }

        foreach (var inputAction in _inputActionArray)
        {
            if (inputAction != null)
            {
                inputAction.Enable();
                Debug.Log("Enable");
            }
        }
    }
}
