using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    //�ŏ��ɑI�������{�^��
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
        // GameScene��ǂݍ���
        SceneManager.LoadScene("FishingScene");
    }

    public GameObject optionPanel; // �I�v�V������ʂ̃p�l��
�@�@public GameObject instructionObject;
    public GameObject keyboardPanel;
    public GameObject Credit;
   
    private void Start()
    {
        // �ŏ��̓I�v�V������ʂ��\���ɂ���
        optionPanel.SetActive(false);
        instructionObject.SetActive(false);
        keyboardPanel.SetActive(false);
        Credit.SetActive(false);    

        //�{�^�����Z���N�g
        Selectable.Select();
    }

    public void ToggleOptionPanel()
    {
        // �I�v�V�����{�^���������ꂽ�Ƃ��ɌĂ΂��֐�
        // �I�v�V������ʂ̕\���E��\����؂�ւ���
        optionPanel.SetActive(!optionPanel.activeSelf);
        Selectable1.Select();
    }

    public void ShowInstructionPanel()
    {
        // Instruction�p�l����\������
        instructionObject.SetActive(true);
        Selectable2.Select();
    }

    public void ShowKeyboardPanel() 
    { 
        // keyboard��ʂ�\��������B
        keyboardPanel.SetActive(true);
        Selectable3.Select();
    }

    public void Backcontroller()
    {
        // instruction��ʂɖ߂�B
        keyboardPanel.SetActive(false);
        instructionObject.SetActive(true);
        Selectable2.Select();
    }

    public void instructionBackOption()
    {
        //option��ʂɖ߂�B
        instructionObject.SetActive(!instructionObject.activeSelf);
        Selectable1.Select();
    }
    public string titleSceneName = "TitleScene"; // �^�C�g����ʂ̃V�[����

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
        // �^�C�g����ʂ̃V�[���ɖ߂�
        SceneManager.LoadScene(titleSceneName);
    }

    public void ExitGame()
    {
        // �Q�[�����I������
        Application.Quit();
    }
}
