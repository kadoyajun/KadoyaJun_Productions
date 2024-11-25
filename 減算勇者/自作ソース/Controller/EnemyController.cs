using System;
using System.Collections;
using UnityEngine;

namespace Genzan
{
    public class EnemyController : MonoBehaviour
    {
        public Vector2Int Position {  get; private set; }
        public string Name { get; private set; }
        public int Level { get; private set; }
        [NonSerialized]
        public int areaNumber;
        [NonSerialized]
        public bool active = true;

        private GameManager gM = null;
        [NonSerialized]
        public bool summonedByBoss = false;
        [SerializeField]
        private int liveTurn = 5;

        public int LiveTurn { get { return liveTurn; } private set { } }

        [SerializeField]
        private Animator animator;

        [SerializeField]
        private Transform enemyObject;

        public EnemyData enemyData;

        [SerializeField]
        private GameObject explosion;

        public Vector2Int SetPosition()
        {
            return new Vector2Int((int)transform.position.x, (int)transform.position.z);
        }
        private void Awake()
        {
            Position = SetPosition();
        }

        private void Start()
        {
            gM = transform.root.GetComponent<GameManager>();
            Name = enemyData.enemyName;
            switch (gM.difficulty)
            {
                case GameManager.Difficulty.Easy:
                    Level = enemyData.easyLevel;
                    break;
                case GameManager.Difficulty.Normal:
                    Level = enemyData.nomalLevel;
                    break;
                case GameManager.Difficulty.Hard:
                    Level = enemyData.hardLevel;
                    break;
                default: break;
            }
            enemyData = null;
            Destroy(enemyData);
        }

        //移動可能かチェック
        private bool MoveCheck(int directionNumber)
        {
            Vector2Int nextPosition = Position;
            //次の位置を移動方向から計算
            switch (directionNumber)
            {
                case 0:
                    nextPosition.y++;
                    break;
                case 1:
                    nextPosition.y--;
                    break;
                case 2:
                    nextPosition.x--;
                    break;
                case 3:
                    nextPosition.x++;
                    break;
                default:
                    break;
            }
            
            //移動先にオブジェクトがないかチェック
            for (int i = 0; i < gM.fieldObject.Count; i++)
            {
                if (nextPosition == gM.fieldObject[i].position)
                {
                    return false;
                }
            }
            //移動先に別の敵がいないかチェック
            for(int i = 0; i < gM.enemy.Count; i++)
            {
                if(nextPosition == gM.enemy[i].Position 
                   && gM.enemy[i].active 
                   && gM.enemy[i].gameObject != this.gameObject)
                {
                    return false;
                }
            }
            return true;
        }

        //敵の移動の処理
        public bool EnemyMove()
        {
            if (gameObject.activeSelf == true)
            {
                //ルート探索
                PathFinding pathFinding = new PathFinding();
                int moveDir = pathFinding.FindPath(CalFn.TrimCoordinate(Position), CalFn.TrimCoordinate(gM.player.Position), gM.ReturnEnemyMovable());
                
                //移動が行われたか否か
                bool moved = false;

                if(moveDir != -1)
                {
                    moved = true;
                    MoveDirection(moveDir);
                }
                PlayWalkSE();
                if (moved)
                {
                    StartCoroutine(EnemyMoving());
                    animator.SetBool("IsWalking", true);
                    Invoke(nameof(StopAnimation), animator.GetCurrentAnimatorClipInfo(0).Length);
                }
                return moved;
            }
            return false;
        }

        void MoveDirection(int d)
        {
            switch (d)
            {
                case CNum.UP:
                    Position = new(Position.x, Position.y + 1);
                    Vector3 upRotation = enemyObject.localEulerAngles;
                    upRotation.y = 180;
                    enemyObject.localEulerAngles = upRotation;
                    break; 
                case CNum.DOWN:
                    Position = new(Position.x, Position.y - 1);
                    Vector3 downRotation = enemyObject.localEulerAngles;
                    downRotation.y = 0;
                    enemyObject.localEulerAngles = downRotation;
                    break;
                case CNum.LEFT:
                    Position = new(Position.x - 1, Position.y);
                    Vector3 leftRotation = enemyObject.localEulerAngles;
                    leftRotation.y = 0;
                    enemyObject.localEulerAngles = leftRotation;
                    break;
                case CNum.RIGHT:
                    Position = new(Position.x + 1, Position.y);
                    Vector3 rightRotation = enemyObject.localEulerAngles;
                    rightRotation.y = 180;
                    enemyObject.localEulerAngles = rightRotation;
                    break;
                default:
                    break;
            }
        }

        void StopAnimation()
        {
            // アニメーションを停止し、制御を元に戻します
            animator.SetBool("IsWalking", false);
        }

        //敵の座標をMoveTime秒で移動を実行
        IEnumerator EnemyMoving()
        {
            Vector3Int position = new(Position.x,0, Position.y);
            float moveTimer = 0;
            float moveTime = gM.EnemyMoveTime;

            while (moveTime > moveTimer)
            {
                Vector3 nowPosition = transform.position;
                moveTimer += Time.deltaTime;
                if (transform.position.x < Position.x)
                {
                    nowPosition.x += Time.deltaTime / moveTime;
                }
                else if (transform.position.x > Position.x)
                {
                    nowPosition.x -= Time.deltaTime / moveTime;
                }

                if (transform.position.z < Position.y)
                {
                    nowPosition.z += Time.deltaTime / moveTime;
                }
                else if (transform.position.z > Position.y)
                {
                    nowPosition.z -= Time.deltaTime / moveTime;
                }
                transform.position = nowPosition;
                yield return null;
            }

            transform.position = position;
        }

        private void PlayWalkSE()
        {
            if(Name == "スライム")
            {
                AudioManager.Instance.PlaySE("WalkSlimeSE");
            }
            else if (Name == "ゴブリン")
            {
                AudioManager.Instance.PlaySE("WalkGoblinSE");
            }
        }

        public void EndMove()
        {
            if (summonedByBoss && active)
            {
                liveTurn--;
                if (liveTurn == 0)
                {
                    DefeatEnemy();
                }
            }
        }

        public void DefeatEnemy()
        {
            active = false;
            gameObject.SetActive(false);
            Instantiate(explosion, transform.position,Quaternion.identity);
        }
    }
}