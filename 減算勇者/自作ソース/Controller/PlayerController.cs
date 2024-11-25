using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Genzan
{
    public class PlayerController : MonoBehaviour
    {
        GameManager gM;

        [SerializeField]
        Vector2Int FirstPosition;
        public Vector2Int Position {  get; private set; }

        [Tooltip("移動に掛ける時間")]
        [SerializeField]
        private float MoveTime = 0.5f;

        [SerializeField]
        Transform[] playerObject;

        [SerializeField]
        Animator[] animator;

        private bool isAnimating = false;

        //入力を受け付ける時にtrue
        bool inputAllow = true;

        bool[] inputDirection = new bool[4] {false,false,false,false};

        private void Awake()
        {
            SetPosition(FirstPosition);
            transform.position = new(Position.x, 0, Position.y);
        }

        private void Start()
        {
            gM = transform.root.GetComponent<GameManager>();

            ChangeDirection(CNum.UP);
        }
        //入力の受付
        public void OnMoveUp(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                inputDirection[CNum.UP] = true;
            }
            else if(context.canceled)
            {
                inputDirection[CNum.UP] = false;
            }
        }
        public void OnMoveDown(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                inputDirection[CNum.DOWN] = true;
            }
            else if (context.canceled)
            {
                inputDirection[CNum.DOWN] = false;
            }
        }
        public void OnMoveLeft(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                inputDirection[CNum.LEFT] = true;
            }
            else if (context.canceled)
            {
                inputDirection[CNum.LEFT] = false;
            }
        }
        public void OnMoveRight(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                inputDirection[CNum.RIGHT] = true;
            }
            else if (context.canceled)
            {
                inputDirection[CNum.RIGHT] = false;
            }
        }

        private void FixedUpdate()
        {
            if(inputAllow && gM.AllowPlayerActions())
            {
                for(int i = 0; i < 4; i++)
                {
                    if (inputDirection[i])
                    {
                        PlayerMove(i);
                        break;
                    }
                }
            }
        }

        //移動可能かチェック
        private bool MoveCheck(int direction)
        {
            //引数の方向に移動した場合の移動先
            Vector2Int nextPosition = gM.player.Position;
            switch (direction)
            {
                case CNum.UP:
                    nextPosition.y++;
                    break;
                case CNum.DOWN:
                    nextPosition.y--;
                    break;
                case CNum.LEFT:
                    nextPosition.x--;
                    break;
                case CNum.RIGHT:
                    nextPosition.x++;
                    break;
                default:
                    break;
            }
            if(gM.AreaNumber != gM.ReturnAreaNumber(nextPosition))
            {
                bool isMatched = false;
                Vector2Int position = new (Position.x % 10, Position.y % 10);
                for(int i = 0; i < gM.MovablePosition.Length; i++)
                {
                    if (Array.Exists(gM.ReturnMovablePositionNumber(), n => n == i))
                    {
                        for (int j = 0; j < gM.MovablePosition[i].position.Length; j++)
                        {
                            if (position == gM.MovablePosition[i].position[j])
                            {
                                isMatched = true;
                            }
                        }
                    }                
                }
                if (!isMatched)
                {
                    return false;
                }
            }
            //移動先がオブジェクトの場合falseを返す
            for (int i = 0; i < gM.fieldObject.Count; i++)
            {
                if (nextPosition == gM.fieldObject[i].position)
                {
                    return false;
                }
            }
            return true;
        }

        //プレイヤーの移動
        private void PlayerMove(int inputValue)
        {
            if (MoveCheck(inputValue) == false)
            {
                StopAnimation();
                return;
            }
            inputAllow = false;
            {
                // アニメーションが再生中でない場合にのみ再生します
                if (!isAnimating)
                {
                    isAnimating = true;
                    animator[0].SetBool("IsWalking", true);
                    animator[1].SetBool("IsWalking", true);
                    animator[2].SetBool("IsWalking", true);
                    animator[3].SetBool("IsWalking", true);
                    //Invoke(nameof(StopAnimation), animator4.GetCurrentAnimatorClipInfo(0)[0].clip.length);
                }
            }

            switch (inputValue)
            {
                case CNum.UP:
                    Position = new(Position.x, Position.y + 1);
                    ChangeDirection(CNum.UP);
                    break;
                case CNum.DOWN:
                    Position = new(Position.x, Position.y - 1);

                    ChangeDirection(CNum.DOWN);
                    break;
                case CNum.LEFT:
                    Position = new(Position.x - 1, Position.y);
                    ChangeDirection(CNum.LEFT);
                    break;
                case CNum.RIGHT:
                    Position = new(Position.x + 1, Position.y);
                    ChangeDirection(CNum.RIGHT);
                    break;
                default:
                    break;
            }
            AudioManager.Instance.PlaySE("PlayerMoveSE");
            StartCoroutine(PlayerMoving(true));
        }

        void StopAnimation()
        {
            // アニメーションを停止し、制御を元に戻します
            animator[0].SetBool("IsWalking", false);
            animator[1].SetBool("IsWalking", false);
            animator[2].SetBool("IsWalking", false);
            animator[3].SetBool("IsWalking", false);

            animator[0].SetBool("IsAttack", false);
            isAnimating = false;
        }

        //プレイヤーの座標をMoveTime秒で移動を実行
        public IEnumerator PlayerMoving(bool oneself)
        {
            float MoveTimer = 0;

            float xDiff = Position.x - transform.position.x;
            float yDiff = Position.y - transform.position.z;

            //MoveTimerで時間を計測し、決められた移動時間MoveTime秒を超えるまで移動
            while (MoveTime > MoveTimer)
            {
                Vector3 nowPosition = transform.position;
                MoveTimer += Time.deltaTime;
                nowPosition.x += xDiff * Time.deltaTime / MoveTime;
                nowPosition.z += yDiff * Time.deltaTime / MoveTime;
                transform.position = nowPosition;
                //1フレームごとにループさせる
                yield return null;
            }
            //最後に整数値で整える
            transform.position = new(Position.x, 0, Position.y);

            //プレイヤー自身の操作で移動した場合、ターン終了処理を開始
            if (oneself)
            {
                gM.PlayerTurnEnd();
                inputAllow = true;
            }

            StartCoroutine(HoldCheck());
        }

        //移動終了時ボタンを押し続けているかチェック
        IEnumerator HoldCheck()
        {
            yield return null;
            //1フレーム後にプレイヤーのターンであるかチェック
            if(gM.turnState == GameManager.TurnState.PlayerTurn)
            {
                bool holding = false;
                for (int i = 0; i < inputDirection.Length; i++)
                {
                    if (inputDirection[i])
                    {
                        holding = true;
                    }
                }
                //移動のボタンを押していなかったらアニメーション停止
                if (holding == false)
                {
                    StopAnimation();
                }
            }
            else
            {
                StopAnimation();
            }
        }

        public void Attacked()
        {
            ChangeDirection(CNum.UP);

            animator[0].SetBool("IsAttack", true);
        }

        private void ChangeDirection(int direction)
        {
            for(int i = 0; i < 4; i++)
            {
                if(i == direction)
                {
                    Vector3 position = playerObject[i].localPosition;
                    position.x = 0;
                    playerObject[i].localPosition = position;
                }
                else
                {
                    Vector3 position = playerObject[i].localPosition;
                    position.x = 10000;
                    playerObject[i].localPosition = position;
                }
            }
        }

        public void SetPosition(Vector2Int p)
        {
            Position = p;
        }

        #region テストのビルドデータ用
        public void ReroadScene(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                string sceneName = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene(sceneName);
            }
        }
        public void ExitGame(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                Application.Quit();
            }
        }
        #endregion 
    }
}


