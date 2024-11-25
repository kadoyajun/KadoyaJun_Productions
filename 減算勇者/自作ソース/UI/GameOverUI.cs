using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    // 初期選択ボタンの設定
    [SerializeField]
    private Selectable SelectButton;

    // Menu中動けないようにとりあえずInputSysemを指定
    [SerializeField]
    public InputActionProperty[] _inputActions;

    private InputAction[] _inputActionArray;

    private void Awake()
    {
        _inputActionArray = new InputAction[_inputActions.Length];
    }

    // Start is called before the first frame update
    void Start()
    {
        SelectButton.Select();
    }

    // Scene移行
    public void GoTitle(string sceneName)
    {
        inputActionEnable();
        SceneManager.LoadScene(sceneName);
    }

    public void Show()
    {
        inputActionDisable();
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
            }
        }
    }
}
