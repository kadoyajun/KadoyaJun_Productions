using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Genzan
{
    public class LevelUI : MonoBehaviour
    {
        //�\������e�L�X�g�̃Y��
        [SerializeField]
        Vector2 shift = Vector2.zero;
        //�\������{�X�̃e�L�X�g�̃Y��
        [SerializeField]
        Vector2 bossShift = Vector2.zero;
        [SerializeField]
        new Camera camera;

        GameManager gM;

        [SerializeField]
        GameObject enemyLevel;
        readonly List<GameObject> enemyLevelText = new();
        readonly List<int> activeEnemyNumber = new();
        readonly List<float>moveTimer = new();

        GameObject bossLevelText = null;

        bool bossActive = false;

        //areaNumber
        private int aN = 0;

        [SerializeField]
        private float textMoveTime = 0;

        [SerializeField]
        private TextMeshProUGUI playerLevelUI;

        [SerializeField]
        private Vector2 playerLevelPosition = Vector2.zero;

        [SerializeField]
        private GameObject GameOverUI;

        private void Start()
        {
            gM = transform.root.GetComponent<GameManager>();
            playerLevelUI.text = "Lv." + gM.Level;
        }
        void Update()
        {

            //�G���A�ړ����A���ݕ\�����Ă���e�L�X�g�������A�V������������
            if (aN != gM.AreaNumber)
            {
                aN = gM.AreaNumber;
                for (int i = 0; i < enemyLevelText.Count; i++)
                {
                    Destroy(enemyLevelText[i]);
                }
                enemyLevelText.Clear();
                activeEnemyNumber.Clear();
                moveTimer.Clear();
                CreateEnemyLevelText();
            }
            //�{�X�G���A�̏ꍇ
            if (aN == 10)
            {
                BossEnemyLevelText();
            }
            for (int i = 0; i < activeEnemyNumber.Count; i++)
            {
                //�A�N�e�B�u�Ȃ�X�V�A��A�N�e�B�u�Ȃ�폜
                if (gM.enemy[activeEnemyNumber[i]].active)
                {
                    //�e�L�X�g�ʒu�̍X�V
                    Transform transform = gM.enemy[activeEnemyNumber[i]].gameObject.transform;
                    Vector3 worldPosition = transform.position;
                    Vector2 screenPosition = camera.WorldToScreenPoint(worldPosition);
                    screenPosition = new Vector2(screenPosition.x * (1920.0f / Screen.width), screenPosition.y * (1080.0f / Screen.height));
                    screenPosition += shift;
                    RectTransform rectTransform = enemyLevelText[i].GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = screenPosition;
                }
                else
                {
                    if (moveTimer[i] == 0)
                    {
                        StartCoroutine(MoveText(i));
                    }
                }
            }
        }

        IEnumerator MoveText(int eLTNumber)
        {
            if (!(gM.enemy[activeEnemyNumber[eLTNumber]].summonedByBoss && gM.enemy[activeEnemyNumber[eLTNumber]].LiveTurn == 0))
            {
                RectTransform rectTransform = enemyLevelText[eLTNumber].GetComponent<RectTransform>();
                Vector2 nowPosition = rectTransform.anchoredPosition;
                while (textMoveTime > moveTimer[eLTNumber])
                {
                    moveTimer[eLTNumber] += Time.deltaTime;
                    Vector2 nextMoveRange = (playerLevelPosition - nowPosition) / (textMoveTime / Time.deltaTime);
                    rectTransform.localPosition += (Vector3)nextMoveRange;

                    yield return null;
                }
                PlayerLevelTextChange(gM.Level, false);
            }
            Destroy(enemyLevelText[eLTNumber]);
            enemyLevelText.Remove(enemyLevelText[eLTNumber]);
            activeEnemyNumber.Remove(activeEnemyNumber[eLTNumber]);
            moveTimer.Remove(moveTimer[eLTNumber]);
        }

        //�e�L�X�g�̐���
        void CreateEnemyLevelText()
        {
            for (int i = 0; i < gM.enemy.Count; i++)
            {
                if (gM.enemy[i].active && gM.AreaNumber == gM.enemy[i].areaNumber)
                {
                    //����
                    GameObject eL = Instantiate(enemyLevel);
                    eL.transform.SetParent(this.transform, false);
                    enemyLevelText.Add(eL);
                    activeEnemyNumber.Add(i);
                    //�G�̃��[���h���W����ʏ�̍��W�ɕϊ�
                    Transform transform = gM.enemy[i].gameObject.transform;
                    Vector3 worldPosition = transform.position;
                    Vector2 screenPosition = camera.WorldToScreenPoint(worldPosition);
                    screenPosition = new Vector2(screenPosition.x * (1920.0f / Screen.width), screenPosition.y * (1080.0f / Screen.height));
                    screenPosition += shift;
                    RectTransform rectTransform = eL.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = screenPosition;
                    TextMeshProUGUI textMeshProUGUI = eL.GetComponent<TextMeshProUGUI>();
                    textMeshProUGUI.text = "Lv." + gM.enemy[i].Level.ToString();

                    moveTimer.Add(0);
                }
            }
        }
        //�{�X�̃e�L�X�g�̐���
        void BossEnemyLevelText()
        {
            if (!bossActive && gM.boss.gameObject.activeSelf)
            {
                bossActive = true;
                //����
                bossLevelText = Instantiate(enemyLevel);
                bossLevelText.transform.SetParent(this.transform, false);
                //�{�X�̃��[���h���W����ʏ�̍��W�ɕϊ�
                Transform transform = gM.boss.gameObject.transform;
                Vector3 worldPosition = transform.position;
                Vector2 screenPosition = camera.WorldToScreenPoint(worldPosition);
                screenPosition = new Vector2(screenPosition.x * (1920.0f / Screen.width), screenPosition.y * (1080.0f / Screen.height));
                screenPosition += bossShift;
                RectTransform rectTransform = bossLevelText.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = screenPosition;
                TextMeshProUGUI textMeshProUGUI = bossLevelText.GetComponent<TextMeshProUGUI>();
                textMeshProUGUI.text = "Lv." +  gM.boss.bossController.BossLevel.ToString();
            }
            for (int i = 0; i < gM.enemy.Count; i++)
            {
                if (gM.enemy[i].active && gM.AreaNumber == gM.enemy[i].areaNumber && !activeEnemyNumber.Contains(i))
                {
                    //����
                    GameObject eL = Instantiate(enemyLevel);
                    eL.transform.SetParent(this.transform, false);
                    enemyLevelText.Add(eL);
                    activeEnemyNumber.Add(i);
                    //�G�̃��[���h���W����ʏ�̍��W�ɕϊ�
                    Transform transform = gM.enemy[i].gameObject.transform;
                    Vector3 worldPosition = transform.position;
                    Vector2 screenPosition = camera.WorldToScreenPoint(worldPosition);
                    screenPosition = new Vector2(screenPosition.x * (1920.0f / Screen.width), screenPosition.y * (1080.0f / Screen.height));
                    screenPosition += shift;
                    RectTransform rectTransform = eL.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = screenPosition;
                    TextMeshProUGUI textMeshProUGUI = eL.GetComponent<TextMeshProUGUI>();
                    textMeshProUGUI.text = "Lv." + gM.enemy[i].Level.ToString();
                    moveTimer.Add(0);
                }
            }
            if(bossActive && !gM.boss.gameObject.activeSelf)
            {
                bossActive = false;
                StartCoroutine(BossMoveText());
            }
        }

        IEnumerator BossMoveText()
        {
            float moveTimer = 0;
            RectTransform rectTransform = bossLevelText.GetComponent<RectTransform>();
            Vector2 nowPosition = rectTransform.anchoredPosition;
            while (textMoveTime > moveTimer)
            {
                moveTimer += Time.deltaTime;
            Vector2 nextMoveRange = (playerLevelPosition - nowPosition) / (textMoveTime / Time.deltaTime);
                rectTransform.localPosition += (Vector3)nextMoveRange;

                yield return null;
            }
            Destroy(bossLevelText);
            PlayerLevelTextChange(gM.Level,false);
        }

        void PlayerLevelTextChange(uint level,bool p)
        {
            playerLevelUI.text = "Lv." + level;
            if (p)
            {
                AudioManager.Instance.PlaySE("LevelUpSE");
            }
            else
            {
                AudioManager.Instance.PlaySE("LevelDownSE");
            }

            if(level <= 0)
            {
                ShowGameOverUI();
            }
        }

        private void ShowGameOverUI()
        {
            GameOverUI.SetActive(true);
            gM.gameState = GameManager.GameState.GameOver;
        }

        public void OtherDamage(GameObject player, int level)
        {
            //����
            GameObject oL = Instantiate(enemyLevel);
            oL.transform.SetParent(this.transform, false);
            //�v���C���[�̃��[���h���W����ʏ�̍��W�ɕϊ�
            Transform transform = player.transform;
            Vector3 worldPosition = transform.position;
            Vector2 screenPosition = camera.WorldToScreenPoint(worldPosition);
            screenPosition = new Vector2(screenPosition.x * (1920.0f / Screen.width), screenPosition.y * (1080.0f / Screen.height));
            screenPosition += shift;
            RectTransform rectTransform = oL.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = screenPosition;
            TextMeshProUGUI textMeshProUGUI = oL.GetComponent<TextMeshProUGUI>();
            textMeshProUGUI.text = "Lv." +Mathf.Abs(level).ToString();
            bool overZero = false;
            if(level >= 0)
            {
                textMeshProUGUI.color = Color.yellow;
                overZero = true;
            }
            StartCoroutine(OtherMoveText(oL,overZero));
        }

        IEnumerator OtherMoveText(GameObject oL,bool p)
        {
            float moveTimer = 0;
            RectTransform rectTransform = oL.GetComponent<RectTransform>();
            Vector2 nowPosition = rectTransform.anchoredPosition;
            while (textMoveTime > moveTimer)
            {
                moveTimer += Time.deltaTime;
                Vector2 nextMoveRange = (playerLevelPosition - nowPosition) / (textMoveTime / Time.deltaTime);
                rectTransform.localPosition += (Vector3)nextMoveRange;

                yield return null;
            }
            Destroy(oL);
            PlayerLevelTextChange(gM.Level,p);
        }
    }
}

