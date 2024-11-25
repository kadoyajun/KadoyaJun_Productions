using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    // �����I���{�^���̐ݒ�
    [SerializeField]
    private Selectable SelectButton;

    // Menu�������Ȃ��悤�ɂƂ肠����InputSysem���w��
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

    // Scene�ڍs
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
            inputActionDisable(); // �����ōēxDisable����
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
