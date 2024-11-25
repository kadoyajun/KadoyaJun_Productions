using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Genzan
{
    public class Boss
    {
        public GameObject gameObject;
        public BossController bossController;
    }

    [System.Serializable]
    public class MovablePosition
    {
        public Vector2Int[] position = new Vector2Int[4];
    }

    public class GameManager : MonoBehaviour
    {
        #region �ύX�\�X�e�[�^�X
        [Tooltip("�G�̈ړ�����")]
        [SerializeField]
        float enemyMoveTime = 0;
        public float EnemyMoveTime { get; private set; }

        [Tooltip("�ł̃_���[�W")]
        [SerializeField]
        int poisonDamage;

        public enum Stage { Forest, Castle, Camp };
        public Stage stage;

        [SerializeField]
        private MovablePosition[] movablePosition;
        public MovablePosition[] MovablePosition { get { return movablePosition; } private set { } }
        #endregion
        #region �ύX�s�X�e�[�^�X
        public int Turn { get; private set; } = 1; //���݂̃^�[����
        public uint Level { get; set; }    //���݂̃v���C���[�̃��x��
        public int AreaNumber { get; private set; } //9���Z�[�t�]�[���@10���{�X����

        public enum Movable {None, Enemy,Object}
        public Movable[,] movable = new Movable[10, 10];

        public enum Difficulty { Easy, Normal, Hard }; //��Փx
        public Difficulty difficulty = Difficulty.Normal;

        public enum TurnState { PlayerTurn, EnemyTurn, BossTurn }
        public TurnState turnState = TurnState.PlayerTurn;

        public enum GameState { StartAnimation, Game, GameOver, StageClear, Pause };
        public GameState gameState = GameState.StartAnimation;

        int areaCount = 0;
        #endregion

        #region �Q�Ƃ��K�v�ȃI�u�W�F�N�g
        public PlayerController player;

        [SerializeField]
        GameObject areas;

        [SerializeField]
        GameObject areaWalls;

        [SerializeField]
        BossHPUI bossHPUI;

        [SerializeField]
        CameraController cameraController;

        [SerializeField]
        TurnUI turnUI;

        [SerializeField]
        LogScripts logScripts;

        [SerializeField]
        GameClearUI gameClearUI;

        [SerializeField]
        LevelUI levelUI;

        [SerializeField]
        ChangeDifficultyUI changeDifficultyUI;
        #endregion

        #region �擾�����I�u�W�F�N�g
        public List<EnemyController> enemy = new();

        public List<ObjectController> fieldObject = new();

        public List<GroundController> ground = new();

        public Boss boss = new();
        #endregion

        private void Awake()
        {
            //PlayerPrefs.DeleteAll();
            if (PlayerPrefs.HasKey("Level"))
            {
                Level = (uint)PlayerPrefs.GetInt("Level");
            }
            else
            {
                Level = CNum.STARTLEVEL;
                PlayerPrefs.SetInt("Level", (int)CNum.STARTLEVEL);
            }
        }

        void Start()
        {
            // ��Փx��ǂݍ���
            switch (changeDifficultyUI.currentDifficulty)
            {
                case ChangeDifficultyUI.Difficulty.Easy:
                    difficulty = Difficulty.Easy;
                    break;
                case ChangeDifficultyUI.Difficulty.Normal:
                    difficulty = Difficulty.Normal;
                    break;
                case ChangeDifficultyUI.Difficulty.Hard:
                    difficulty = Difficulty.Hard;
                    break;
                default:
                    break;
            }
            EnemyMoveTime = enemyMoveTime;

            //Areas�z����GameObject���擾���K�v�ȏ����擾
            areaCount = areas.transform.childCount;
            for(int i = 0; i < areaCount; i++)
            {
                GameObject area = areas.transform.GetChild(i).gameObject;
                for(int j = 0; j < area.transform.childCount; j++)
                {
                    switch (j)
                    {
                        case 0:
                            GameObject enemies = area.transform.GetChild(j).gameObject;
                            int enemyCount = enemies.transform.childCount;
                            for (int k = 0; k < enemyCount; k++)
                            {
                                enemy.Add(enemies.transform.GetChild(k).gameObject.GetComponent<EnemyController>());
                                enemy[enemy.Count - 1].areaNumber = i;
                            }
                            break;
                        case 1:
                            GameObject objects = area.transform.GetChild(j).gameObject;
                            int objectCount = objects.transform.childCount;
                            for (int k = 0; k < objectCount; k++)
                            {
                                fieldObject.Add(objects.transform.GetChild(k).gameObject.GetComponent<ObjectController>());
                                fieldObject[fieldObject.Count - 1].areaNumber = i;
                            }
                            break;
                        case 2:
                            ground.Add(area.transform.GetChild(j).gameObject.GetComponent<GroundController>());
                            ground[ground.Count - 1].AreaNumber = i;
                            break;
                        case 3:
                            boss.gameObject = area.transform.GetChild(j).gameObject;
                            boss.bossController = boss.gameObject.GetComponent<BossController>();
                            break;
                        default:
                            break;
                    }
                }
            }
            AreaChange(true);
            StartCoroutine(WaitStageStartAnimation());
        }

        void Update()
        {
            turnUI.DisplayTurnImage(turnState);

            //���x����0�ȉ��ɂȂ�����Q�[���I�[�o�[�Ɉڍs
            if (Level <= 0)
            {
                gameState = GameState.GameOver;
            }

            //�{�X�G���A�ɂ���ԁA�{�X�p��UI��\��
            if (AreaNumber == 10)
            {
                bossHPUI.gameObject.SetActive(true);
                bossHPUI.ChangeBossHPBar(boss.bossController.HPPercentage());
            }
            else
            {
                bossHPUI.gameObject.SetActive(false);
            }
        }


        #region �^�[������
        //�v���C���[�̈ړ���̏������s���^�[�����I�����A�G�̃^�[���Ɉڍs
        public void PlayerTurnEnd()
        {
            AreaChange(false);
            HitCheck();
            PlayerGroundCheck();
            Turn++;
            if(AreaNumber == 10 && boss.bossController.Active)
            {
                AttackToBoss();
                if (boss.bossController.Active)
                {
                    BossTurnStart();
                }
            }
            else
            {
                EnemyTurnStart();
            }
        }

        //�{�X�̃^�[�����J�n
        void BossTurnStart()
        {
            turnState = TurnState.BossTurn;
            StartCoroutine(BossAction());
        }

        //�{�X�̍s�������s
        IEnumerator BossAction()
        {
            if (boss.gameObject.activeSelf)
            {
                boss.bossController.BossMove();
                yield return new WaitForSeconds(enemyMoveTime);
            }
            EnemyTurnStart();
        }

        //�G�̃^�[�����J�n
        private void EnemyTurnStart()
        {
            turnState = TurnState.EnemyTurn;
            StartCoroutine(EnemyAction());
        }

        //���ԂɓG�̈ړ������s
        IEnumerator EnemyAction()
        {
            MovableCheckAreaChange();
            for (int i = 0; i < enemy.Count; i++)
            {
                //�A�N�e�B�u�̓G�݈̂ړ������s
                if (enemy[i].gameObject.activeSelf == true && enemy[i].areaNumber == AreaNumber && gameState == GameState.Game)
                {
                    bool moved = enemy[i].EnemyMove();
                    if (moved)
                    {
                        yield return new WaitForSeconds(enemyMoveTime);
                    }
                    HitCheck();
                    MovableCheckAreaChange();
                    enemy[i].EndMove();
                }
            }
            turnState = TurnState.PlayerTurn;
        }
        #endregion

        #region �v���C���[�Ɋւ�鏈��

        //TurnState��PlayerTurn�̎������A�����o��
        public bool AllowPlayerActions()
        {
            if (turnState == TurnState.PlayerTurn && gameState == GameState.Game)
            {
                if (boss.gameObject != null)
                {
                    if (boss.bossController.DefeatParticle == true)
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        //Player��Enemy�̐ڐG����
        private void HitCheck()
        {
            for (int i = 0; i < enemy.Count; i++)
            {
                //�A�N�e�B�u��Enemy�ƐڐG���Ă�����A���x���������AEnemy���A�N�e�B�u�ɂ���
                if (enemy[i].active && enemy[i].areaNumber == AreaNumber)
                {
                    if (player.Position == enemy[i].Position)
                    {
                        DisplayMessage(LogText.BattleText(enemy[i].Name));
                        LevelUpDown(-enemy[i].Level, true);
                        enemy[i].DefeatEnemy();
                    }
                }
            }
            //�󔠂Ɨאڂ��Ă�����A���������s
            for (int i = 0; i < fieldObject.Count; i++)
            {
                if (fieldObject[i].objectType == ObjectController.ObjectType.TreasureBox && fieldObject[i].areaNumber == AreaNumber)
                {
                    if (((player.Position.x + 1 == fieldObject[i].position.x || player.Position.x - 1 == fieldObject[i].position.x) && player.Position.y == fieldObject[i].position.y) || ((player.Position.y + 1 == fieldObject[i].position.y || player.Position.y - 1 == fieldObject[i].position.y) && player.Position.x == fieldObject[i].position.x))
                    {
                        fieldObject[i].gameObject.GetComponent<TreasureBox>().BoxOpen();
                    }
                }
            }
        }

        //�v���C���[�̂���ʒu�̒n�ʂ̌��ʂ𔭓�
        private void PlayerGroundCheck()
        {
            //������G���A�ł̃v���C���[�̈ʒu���Z�o
            int x = player.Position.x % CNum.ROW;
            int y = player.Position.y % CNum.COLUMN;

            //�v���C���[�̈ʒu���ł̒n�ʂ̏ꍇ
            if (ground[AreaNumber].groundNumber[x, y] == 1)
            {
                DisplayMessage(LogText.LevelDownByPoison);
                LevelUpDown(-poisonDamage, false);
            }//�v���C���[�̈ʒu���S�[���̒n�ʂ̏ꍇ
            if (ground[AreaNumber].groundNumber[x, y] == 2)
            {
                gameState = GameState.StageClear;
                gameClearUI.ShowUI();
                if (!PlayerPrefs.HasKey("Stage1Clear"))
                {
                    PlayerPrefs.SetInt("Stage1Clear", 1);
                }
            }
        }
        public int ReturnAreaNumber(Vector2Int position)
        {
            int x = position.x;
            int y = position.y;
            int a = -1;
            int b = -1;
            if (y >= 30 && y < 50)
            {
                if (x >= 10 && x < 20)
                {
                    a = 3;
                    if (y >= 30 && y < 40)
                    {
                        b = 0;
                    }
                    else
                    {
                        b = 1;
                    }
                }
            }
            else if (x >= 0 && y >= 0)
            {
                a = x / CNum.ROW;
                b = y / CNum.COLUMN;
            }

            return 3 * a + b;
        }
        public int[] ReturnMovablePositionNumber()
        {
            List<int> movablePositionNumber = new();
            if (AreaNumber != 9 && AreaNumber != 10)
            {
                int x = AreaNumber / 3;
                int y = AreaNumber % 3;
                if (x == 0) movablePositionNumber.Add(3);
                else if (x == 1)
                {
                    movablePositionNumber.Add(2);
                    movablePositionNumber.Add(3);
                }
                else if (x == 2) movablePositionNumber.Add(2);

                if (y == 0) movablePositionNumber.Add(0);
                else if (y == 1)
                {
                    movablePositionNumber.Add(0);
                    movablePositionNumber.Add(1);
                }
                else if (y == 2) movablePositionNumber.Add(1);

                if (AreaNumber == 5)
                {
                    movablePositionNumber.Add(0);
                }
            }
            else
            {
                if (AreaNumber == 9)
                {
                    movablePositionNumber.Add(0);
                    movablePositionNumber.Add(1);
                }
            }
            return movablePositionNumber.ToArray();
        }

        //�G���A�ړ�
        private void AreaChange(bool start)
        {
            int nextAreaNumber = ReturnAreaNumber(player.Position);
            //�ړ����s���Ȃ��Ă������A�������̓Q�[���J�n���AareaNumber��ύX���A�I�u�W�F�N�g�̃A�N�e�B�u�󋵂�ύX
            if (AreaNumber != nextAreaNumber || start)
            {
                AreaNumber = nextAreaNumber;
                for (int i = 0; i < areaCount; i++)
                {
                    GameObject gameObject = areas.transform.GetChild(i).gameObject;
                    GameObject gameObject1 = areaWalls.transform.GetChild(i).gameObject;
                    if (i == AreaNumber)
                    {
                        if (AreaNumber == 9)
                        {
                            gameObject.transform.position = new Vector3(10, 0, 30);
                            gameObject1.SetActive(true);
                            AudioManager.Instance.stopBGM();
                        }
                        else if (AreaNumber == 10)
                        {
                            gameObject.transform.position = new Vector3(10, 0, 40);
                            gameObject1.SetActive(true);
                            AudioManager.Instance.bossAreaBGM();
                        }
                        else
                        {
                            gameObject.transform.position = new Vector3(i / 3 * 10, 0, i % 3 * 10);
                            gameObject1.SetActive(true);
                            AudioManager.Instance.restartBGM();
                        }

                    }
                    else
                    {
                        gameObject.transform.position = new Vector3(100, 0, 100);
                        gameObject1.SetActive(false);
                    }
                }
                MovableCheckAreaChange();
                cameraController.CameraPositionChange(AreaNumber);
            }
        }

        //�v���C���[����{�X�ւ̍U���̔���
        private void AttackToBoss()
        {
            PlayerController pC = player.GetComponent<PlayerController>();
            boss.bossController.HitBox(player.Position, Level,pC);
        }

        //�v���C���[���{�X�̍U���}�X�ɂ����ꍇ�_���[�W
        public bool BossHitCheck(Vector2Int damagePoint, int damage)
        {
            if(damagePoint == player.Position)
            {
                DisplayMessage(LogText.LevelDownByBoss(boss.bossController.Name));
                LevelUpDown(-damage,false);
                return true;
            }
            return false;
        }

        //�v���C���[�̈ʒu��ύX
        public void ChangePlayerPosition(Vector2Int vector2Int)
        {
            player.SetPosition(player.Position - vector2Int);
            StartCoroutine(player.PlayerMoving(false));
        }

        private void MovableCheckAreaChange()
        {
            for(int i = 0; i < 10; i++)
            {
                for(int j = 0; j < 10; j++)
                {
                    movable[i, j] = Movable.None;
                }
            }

            for (int i = 0; i < enemy.Count; i++)
            {
                if (enemy[i].areaNumber == AreaNumber && enemy[i].active)
                {
                    Vector2Int relativePosition = new Vector2Int(enemy[i].Position.x % 10, enemy[i].Position.y % 10);
                    movable[relativePosition.x, relativePosition.y] = Movable.Enemy;
                }
            }

            for (int i = 0;i < fieldObject.Count; i++)
            {
                if (fieldObject[i].areaNumber == AreaNumber)
                {
                    Vector2Int relativePosition = new Vector2Int(fieldObject[i].position.x % 10, fieldObject[i].position.y % 10);
                    movable[relativePosition.x, relativePosition.y] = Movable.Object;
                }
            }
        }

        public bool[,] ReturnEnemyMovable()
        {
            bool[,] enemyMovable = new bool[10,10];
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (movable[i,j] == Movable.Enemy || movable[i,j] == Movable.Object)
                    {
                        enemyMovable[i, j] = false;
                    }
                    else
                    {
                        enemyMovable[i, j] = true;
                    }
                }
            }
            return enemyMovable;
        }
        #endregion

        #region ���̑��̏���

        IEnumerator WaitStageStartAnimation()
        {
            if (stage != Stage.Camp)
            {
                yield return new WaitForSeconds(2.5f);
            }
            gameState = GameState.Game;
        }

        public void AddEnemy(GameObject sEnemy)
        {
            enemy.Add(sEnemy.gameObject.GetComponent<EnemyController>());
            enemy[enemy.Count - 1].areaNumber = AreaNumber;
            enemy[enemy.Count - 1].summonedByBoss = true;
        }

        public void LevelUpDown(int l, bool enemyAttack)
        {
            if(l >= 0)
            {
                Level += (uint)l;
            }
            else
            {
                if(Level <= (uint)Mathf.Abs(l))
                {
                    Level = 0;
                }
                else
                {
                    Level -= (uint)Mathf.Abs(l);
                }
            }
            DisplayMessage(LogText.ChangeLevel(l));
            if (!enemyAttack)
            {
                levelUI.OtherDamage(player.gameObject,l);
            }
        }

        public void DisplayMessage(string t)
        {
            logScripts.AddLogText(t, LogScripts.LogType.All);
        }
        #endregion
    }
}