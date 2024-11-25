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
        #region 変更可能ステータス
        [Tooltip("敵の移動時間")]
        [SerializeField]
        float enemyMoveTime = 0;
        public float EnemyMoveTime { get; private set; }

        [Tooltip("毒のダメージ")]
        [SerializeField]
        int poisonDamage;

        public enum Stage { Forest, Castle, Camp };
        public Stage stage;

        [SerializeField]
        private MovablePosition[] movablePosition;
        public MovablePosition[] MovablePosition { get { return movablePosition; } private set { } }
        #endregion
        #region 変更不可ステータス
        public int Turn { get; private set; } = 1; //現在のターン数
        public uint Level { get; set; }    //現在のプレイヤーのレベル
        public int AreaNumber { get; private set; } //9がセーフゾーン　10がボス部屋

        public enum Movable {None, Enemy,Object}
        public Movable[,] movable = new Movable[10, 10];

        public enum Difficulty { Easy, Normal, Hard }; //難易度
        public Difficulty difficulty = Difficulty.Normal;

        public enum TurnState { PlayerTurn, EnemyTurn, BossTurn }
        public TurnState turnState = TurnState.PlayerTurn;

        public enum GameState { StartAnimation, Game, GameOver, StageClear, Pause };
        public GameState gameState = GameState.StartAnimation;

        int areaCount = 0;
        #endregion

        #region 参照が必要なオブジェクト
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

        #region 取得したオブジェクト
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
            // 難易度を読み込み
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

            //Areas配下のGameObjectを取得し必要な情報を取得
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

            //レベルが0以下になったらゲームオーバーに移行
            if (Level <= 0)
            {
                gameState = GameState.GameOver;
            }

            //ボスエリアにいる間、ボス用のUIを表示
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


        #region ターン処理
        //プレイヤーの移動後の処理を行いターンを終了し、敵のターンに移行
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

        //ボスのターンを開始
        void BossTurnStart()
        {
            turnState = TurnState.BossTurn;
            StartCoroutine(BossAction());
        }

        //ボスの行動を実行
        IEnumerator BossAction()
        {
            if (boss.gameObject.activeSelf)
            {
                boss.bossController.BossMove();
                yield return new WaitForSeconds(enemyMoveTime);
            }
            EnemyTurnStart();
        }

        //敵のターンを開始
        private void EnemyTurnStart()
        {
            turnState = TurnState.EnemyTurn;
            StartCoroutine(EnemyAction());
        }

        //順番に敵の移動を実行
        IEnumerator EnemyAction()
        {
            MovableCheckAreaChange();
            for (int i = 0; i < enemy.Count; i++)
            {
                //アクティブの敵のみ移動を実行
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

        #region プレイヤーに関わる処理

        //TurnStateがPlayerTurnの時だけ、許可を出す
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

        //PlayerとEnemyの接触判定
        private void HitCheck()
        {
            for (int i = 0; i < enemy.Count; i++)
            {
                //アクティブなEnemyと接触していたら、レベルを下げ、Enemyを非アクティブにする
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
            //宝箱と隣接していたら、処理を実行
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

        //プレイヤーのいる位置の地面の効果を発動
        private void PlayerGroundCheck()
        {
            //今いるエリアでのプレイヤーの位置を算出
            int x = player.Position.x % CNum.ROW;
            int y = player.Position.y % CNum.COLUMN;

            //プレイヤーの位置が毒の地面の場合
            if (ground[AreaNumber].groundNumber[x, y] == 1)
            {
                DisplayMessage(LogText.LevelDownByPoison);
                LevelUpDown(-poisonDamage, false);
            }//プレイヤーの位置がゴールの地面の場合
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

        //エリア移動
        private void AreaChange(bool start)
        {
            int nextAreaNumber = ReturnAreaNumber(player.Position);
            //移動が行こなわれていたか、もしくはゲーム開始時、areaNumberを変更し、オブジェクトのアクティブ状況を変更
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

        //プレイヤーからボスへの攻撃の判定
        private void AttackToBoss()
        {
            PlayerController pC = player.GetComponent<PlayerController>();
            boss.bossController.HitBox(player.Position, Level,pC);
        }

        //プレイヤーがボスの攻撃マスにいた場合ダメージ
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

        //プレイヤーの位置を変更
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

        #region その他の処理

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