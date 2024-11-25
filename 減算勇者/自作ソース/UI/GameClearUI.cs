using Genzan;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameClearUI : MonoBehaviour
{
    // �����I���{�^���̐ݒ�
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

    // Menu�������Ȃ��悤�ɂƂ肠����InputSysem���w��
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

    // Scene�ڍs
    public void GoTitle(string sceneName)
    {
        inputActionEnable();
        SceneManager.LoadScene(sceneName);
     }

    // UI�\�����Ăяo��
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
        levelUpText.text = sumPt + "���x��Up�I";
        PlayerPrefs.SetInt("Level", (int)(PlayerPrefs.GetInt("Level") + sumPt));
        inputActionDisable();
        animator.SetTrigger(showId);
        StartCoroutine(OnShow());
    }

    IEnumerator OnShow()
    {
        // 4�b�ҋ@
        yield return new WaitForSeconds(4);
        // �{�^���I��
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
            inputActionDisable(); // �����ōēxDisable����
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
