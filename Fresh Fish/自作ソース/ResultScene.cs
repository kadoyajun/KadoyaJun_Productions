using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ResultScene : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI totalScore = new TextMeshProUGUI();
    [SerializeField]
    private TextMeshProUGUI rangeScore = new TextMeshProUGUI();
    [SerializeField]
    private TextMeshProUGUI pointScore = new TextMeshProUGUI();
    [SerializeField]
    private TextMeshProUGUI typeScore = new TextMeshProUGUI();

    //�X�R�A�ɉ����ă����N��t���邽�߂̒l
    [SerializeField]
    [Tooltip("0�`1:D,1�`2:C,2�`3:B,3�`4:A,4�`5:S,5�`6:SS,6�`:SSS")]
    private float[] rankScore = new float[7];

    [SerializeField]
    private Sprite[] rankSprite = null;
    [SerializeField]
    private Image[] rank = null;

    [SerializeField]
    private GameObject buttons = null;

    [SerializeField]
    private Selectable selectButton = null;

    //SE
    [SerializeField]
    private AudioSource audioSource = null;
    [SerializeField]
    private AudioClip den = null;
    [SerializeField]
    private AudioClip yeee = null;

    //���ꂼ��̃X�R�A
    float totalScoreValue = 0;
    float rangeScoreValue = 0;
    float pointScoreValue = 0;
    float typeScoreValue = 0;

    //���Ԍv���p
    float delta = 0;
    //�ŏ��̒l���\�������܂ł̎���
    float rollTime = 2;
    void Start()
    {
        //��\��
        buttons.SetActive(false);
        rank[0].transform.position = new Vector3(0, 1500, 0);

        StartCoroutine(VisibleScore());
    }

    void Update()
    {
        //�����𑫂��Ă����h�������[�����Č�
        delta += Time.deltaTime;
        if (delta < rollTime)
        {
            rangeScoreValue += 55;
            rangeScore.SetText("{00000000:0}", rangeScoreValue);
        }
        if(delta < rollTime + 1)
        {
            pointScoreValue += 44;
            pointScore.SetText("{00000000:0}", pointScoreValue);
        } 
        if(delta < rollTime + 2) 
        {
            typeScoreValue += 33;
            typeScore.SetText("{00000000:0}", typeScoreValue);
        }
    }

    IEnumerator VisibleScore()
    {
        //���ԂɃX�R�A��\��
        yield return new WaitForSeconds(rollTime);
        rangeScoreValue = PlayerPrefs.GetFloat("RangeScoreDate", 250000);
        rangeScore.SetText("{00000000:0}", rangeScoreValue);
        SEden();
        yield return new WaitForSeconds(1.0f);
        pointScoreValue = PlayerPrefs.GetFloat("PointScoreDate", 0);
        pointScore.SetText("{00000000:0}", pointScoreValue);
        SEden();
        yield return new WaitForSeconds(1.0f);
        typeScoreValue = PlayerPrefs.GetFloat("TypeScoreDate", 0);
        typeScore.SetText("{00000000:0}", typeScoreValue);
        SEden();
        yield return new WaitForSeconds(1.0f);
        SEyeee();
        totalScoreValue = rangeScoreValue + pointScoreValue + typeScoreValue;
        totalScore.SetText("{00000000:0}", totalScoreValue);
        //rank�̈ʒu��߂��\��
        rank[0].transform.position = new Vector3(1600, 850, 0);

        //�g�[�^���X�R�A�ɉ�����rank�̃X�v���C�g��ύX
        for(int i = 0; i < rankScore.Length; i++)
        {
            if (i == rankScore.Length - 1)
            {
                rank[0].sprite = rankSprite[rankSprite.Length - 1];
                break;
            }
            else if (totalScoreValue >= rankScore[i] && totalScoreValue < rankScore[i + 1])
            {
                rank[0].sprite = rankSprite[i];
                break;
            }
        }
        //�{�^����\��
        buttons.SetActive(true);
        selectButton.Select();
    }
    //�^�C�g���V�[����ǂݍ���
    public void OnBack()
    {
        SceneManager.LoadScene("TitleScene");
    }

    //�Q�[���V�[����ǂݍ���
    public void OnRetry()
    {
        SceneManager.LoadScene("FishingScene");
    }

    public void SEden() 
    {
        audioSource.PlayOneShot(den);
    }
    public void SEyeee()
    {
        audioSource.PlayOneShot(yeee);
    }
}
