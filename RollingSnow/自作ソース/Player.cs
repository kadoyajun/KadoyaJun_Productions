using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    /*
    
    // 事前にScenerootを指定。
    [SerializeField]
    private GameObject sceneRoot;
    // 事前にカメラを指定。
    public Transform mainCamera;
    public GameObject freeLookCamera;
    // 事前にプレイヤーを指定。
    public GameObject player;
    // 事前に頭を指定。
    public GameObject head;
    public GameObject modelhead;
    // 事前に体を指定。
    public GameObject body;
    public GameObject modelbody;
    // 事前にマフラーを指定。
    public GameObject scarfLeft;
    public GameObject scarfRight;
    // 事前に手袋を指定。
    public GameObject handLeft;
    public GameObject handRight;

    //事前にエフェクトを参照
    [SerializeField]
    private ParticleSystem[] smoke = new ParticleSystem[3];
    //エフェクトのアクティブ状況
    bool smokeEffectActive = true;
    //エフェクト切り替え用の値　触れている地面
    private int smokeNumber = 0;

    // コンポーネントを事前に参照。
    GameScene gameScene;
    public CinemachineInputProvider cameraInputProvider;
    AudioSource audioSource;
    Animator animator;
    // AnimatorパラメーターID
    static readonly int StoppingId = Animator.StringToHash("Stopping");
    static readonly int MovingId = Animator.StringToHash("Moving");
    static readonly int RunId = Animator.StringToHash("Run");
    static readonly int CollisionId = Animator.StringToHash("Collision");
    static readonly int BrakeStartId = Animator.StringToHash("BrakeStart");
    static readonly int BrakeEndId = Animator.StringToHash("BrakeEnd");
    static readonly int Brake2StartId = Animator.StringToHash("Brake2Start");
    static readonly int Brake2EndId = Animator.StringToHash("Brake2End");
    // headのrigiddbody参照
    Rigidbody headrigidbody;
    // State状態によってheadとbodyのmoveBehaviourを切り替える変数を参照
    MoveBehaviour headmoveBehaviour;
    MoveBehaviour bodymoveBehaviour;

    // 現在のMove入力値
    Vector2 moveInput;

    // 
    //public Vector3 saveHeadPosition;

    // 速度の上限を指定します。
    [SerializeField]
    public float speedLimit = 30;
    // 現在速度
    public Vector3 velocity;
    // Dashするときに何秒間押したか測る
    private float dashTimer = 0;

    // 絶対値の返し変数
    private double absolutely = 0;

    // サウンドの制御のための変数
    private bool footStepsPlaying = false;
    // PlayerのMoveに対する制御の変数
    public bool playerMove = false;

    // 1フレーム前の位置
    private Vector3 prevPosition;
    // プレイヤーのポジション
    private Vector3 playerPos; 
    // ブレーキを行った際秒速を測る
    private Vector3 playerPositionDiff;
    // Bodyのポジション
    public Vector3 bodyPos;

    // 初期のBodyScale
    private Vector3 firstBody;
    // 初期のScarfRotation
    [SerializeField]
    public Vector3 firstScarfrotation_L;
    private Vector3 firstScarfrotation_R;

    // Scarfを回す際のFloat値
    [SerializeField]
    private float scarfRotationSpeed = 18f;

    // Headとbodyの状態を表します。
    public enum HeadOnlyState
    {
        // headのみ
        headOnly,
        // headとbody一つ
        bodyOne,
    }
    // 現在のプレイヤーの切り離し状況
    public HeadOnlyState headcurrentState = HeadOnlyState.bodyOne;

    // プレイヤーの状態を表します。
    public enum PlayerState
    {
        // 待機状態
        Idol,
        // 通常の走行状態
        Walk,
        // ダッシュ
        Dash,
        // ブレーキ動作
        Brake,
        // 動作、投げ
        Throwing,
    }
    // 現在のプレイヤー状態
    public PlayerState currentState = PlayerState.Idol;

    // Start is called before the first frame update
    void Awake()
    {
        // Maincameraの子オブジェクトに付いているAudioSouceを参照
        audioSource = GameObject.Find("FootStepsAudio").GetComponent<AudioSource>();

        // headとbodyのmoveBehaviourをそれぞれ参照
        headmoveBehaviour = head.GetComponent<MoveBehaviour>();
        bodymoveBehaviour = body.GetComponent<MoveBehaviour>();
        // コンポーネントを参照
        gameScene = sceneRoot.GetComponent<GameScene>();
        cameraInputProvider = freeLookCamera.GetComponent<CinemachineInputProvider>();
        cameraInputProvider.enabled = false;
        headrigidbody = head.GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // 最初の段階でBodyのScaleを参照
        firstBody = body.transform.localScale;
        // 最初の段階でScarfのrotationを参照
        firstScarfrotation_L = scarfLeft.transform.localEulerAngles;
        firstScarfrotation_R = scarfRight.transform.localEulerAngles;

        // 最初の時点でのプレイヤーのポジションを取得
        playerPos = head.GetComponent<Transform>().position;
        // 初期位置を保持
        prevPosition = transform.position;

        smoke[0].Stop();
        smoke[1].Stop();
        smoke[2].Stop();
    }

    // Update is called once per frame
    void Update()
    {

        switch (currentState)
        {
            case PlayerState.Idol:
                break;
            case PlayerState.Walk:
                FixedUpdateForWalk();
                break;
            case PlayerState.Dash:
                FixedUpdateForDash();
                break;
            case PlayerState.Brake:
                FixedUpdateForBrake();
                break;
            case PlayerState.Throwing:
                break;
        }
        #region プレイヤーの向き決定
        // プレイヤーがどの方向に進んでいるかがわかるように、初期位置と現在地の座標差分を取得
        Vector3 diff = head.transform.position - playerPos;

        // ベクトルの長さが0.01fより大きい場合にプレイヤーの向きを変える処理を入れる (0では入れない)
        if (diff.magnitude > 0.01f)
        {
            // ベクトルの情報をQuaternion.LookRotationに引き渡し回転量を取得しプレイヤーを回転させる
            head.transform.rotation = Quaternion.LookRotation(diff);
            body.transform.rotation = Quaternion.LookRotation(diff);
        }
        // プレイヤーの位置を更新
        playerPos = head.transform.position;
        #endregion

        #region プレイヤーの速さを求め、制御する。
        // deltaTimeが0の場合は何もしない
        if (Mathf.Approximately(Time.deltaTime, 0))
            return;
        // 現在位置取得
        var position = head.transform.position;
        // 現在速度をログ出力
        //print($"velocity = {headrigidbody.velocity.magnitude}");
        // 現在速度計算
        velocity = (position - prevPosition) / Time.deltaTime;
        // 前フレーム位置を更新
        prevPosition = position;
        #endregion

        */

        #region rigidbody.velocityが3以上だと動いているものとする
        if (headrigidbody.velocity.magnitude >= 3)
        {
            //Debug.Log(smokeEffectActive);
            // Stoppingアニメーションに切り替える
            animator.SetTrigger(MovingId);

            //エフェクトを再生
            if (smokeEffectActive == false)
            {
                if(smokeNumber == 0)
                {
                    smoke[0].Play();
                    smoke[1].Stop();
                    smoke[2].Stop();
                }else if(smokeNumber == 1)
                {
                    smoke[0].Stop();
                    smoke[1].Play();
                    smoke[2].Stop();
                }else if(smokeNumber == 2)
                {
                    smoke[0].Stop();
                    smoke[1].Stop();
                    smoke[2].Play();
                }
                smokeEffectActive = true;
            }

            /*

            var model = new Vector3(5, 0, 0);
            if (model.x < 180 && headcurrentState == HeadOnlyState.bodyOne)
            {
                model.x += 5;
                modelbody.transform.Rotate(model);              
            }
            // オブジェクトが動いていてかつfalseの場合、足音を再生
            if (footStepsPlaying == false)
            {
                footStepsPlaying = true;

                //audioSource.clip = footSteps;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else if (headrigidbody.velocity.magnitude < 3)
        {
            if (currentState == PlayerState.Idol)
            {
                // Stoppingアニメーションに切り替える
                animator.SetTrigger(StoppingId);
            }

            //エフェクトを停止
            if (smokeEffectActive == true)
            {
                smoke[smokeNumber].Stop();
                smokeEffectActive = false;
            }

            // オブジェクトが止まってたら、再生をストップ
            if (audioSource.isPlaying)
            {
                footStepsPlaying = false;

                audioSource.loop = false;
                audioSource.Stop();
            }
        }
        //Debug.Log(smokeNumber);
        #endregion
    }

    // 他のスクリプトから呼び出すための関数
    public void SetAnimation()
    {
        // Colisionアニメーションに切り替える
        animator.SetTrigger(CollisionId);
    }
    // 呼び出すと絶対値を出してくれる。
    public double AbsoluteValue(double numerical)
    {
        if (numerical < 0)
        {
            absolutely = -(numerical);
        }
        return absolutely;
    }

    void FixedUpdateForWalk()
    {
        if (playerMove == true)
        {
            // カメラを正面にキャラクターの移動をさせる
            var speed = new Vector3(moveInput.x, 0, moveInput.y);
            speed = mainCamera.transform.TransformDirection(speed);
            speed.y = 0;

            if (headcurrentState == HeadOnlyState.bodyOne)
            {
                bodymoveBehaviour.Move(speed);
            }
            else if (headcurrentState == HeadOnlyState.headOnly)
            {
                headmoveBehaviour.Move(speed);
            }
            SetScarf();
        }
    }

    void SetScarf()
    {
        // 最初の大きさから現在の大きさを引く
        var bodyScalediff = firstBody - body.transform.localScale;
        // 大きさの違いが0より下の場合のみ行う
        if (bodyScalediff.x < 0)
        {
            // LeftとRightはxの値が違うのそれぞれ行う
            var Scarfrotation_L = firstScarfrotation_L;
            var Scarfrotation_R = firstScarfrotation_R;
            // Bodyの大きさの違いによって、値を出す。
            Scarfrotation_L.z = Scarfrotation_L.z + (scarfRotationSpeed * bodyScalediff.x);
            Scarfrotation_R.z = Scarfrotation_L.z;
            if (Scarfrotation_L.z > 0)
            {
            // 代入
            scarfLeft.transform.localEulerAngles = Scarfrotation_L;
            scarfRight.transform.localEulerAngles = Scarfrotation_R;
            }
        }
    }
    private void FixedUpdateForDash()
    {
        // ダッシュ準備中は移動不可
        moveInput = Vector2.zero;
        // チャージ時間を測る(１秒以上は切り捨て)
        dashTimer += Time.deltaTime;
        if (dashTimer > 1)
        {
            dashTimer = 1;
        }
    }
    void FixedUpdateForBrake()
    {
        // プレイヤーを正面とする処理
        velocity = player.transform.TransformDirection(headrigidbody.velocity);
        if (headcurrentState == HeadOnlyState.bodyOne)
        {
            bodymoveBehaviour.Brake(velocity);
        }
        else if (headcurrentState == HeadOnlyState.headOnly)
        {
            headmoveBehaviour.Brake(velocity);
        }

        // プレイヤーが完全に止まったら、Brakeの二つ目のアニメーションに切り替える。
        if (headrigidbody.velocity.magnitude == 0)
        {
            animator.SetTrigger(Brake2StartId);
        }
    }
    #region CallBack変数を扱うもの
    // Moveアクションによって呼び出されます。
    public void OnMove(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                // Move前にSetTriggerされたanimatorをResetする。
                animator.ResetTrigger(StoppingId);
                animator.ResetTrigger(BrakeStartId);
                animator.ResetTrigger(Brake2StartId);
                animator.ResetTrigger(BrakeEndId);
                animator.ResetTrigger(Brake2EndId);

                // Runアニメーションに切り替える
                animator.SetTrigger(RunId);
                if (currentState == PlayerState.Idol)
                {
                    // 現在のプレイヤー状態変化
                    currentState = PlayerState.Walk;
                }
                break;
            case InputActionPhase.Performed:
                moveInput = context.ReadValue<Vector2>();
                break;
            case InputActionPhase.Canceled:
                // Runアニメーションに切り替える
                animator.ResetTrigger(RunId);
                if (currentState == PlayerState.Walk)
                {
                    // 現在のプレイヤー状態変化
                    currentState = PlayerState.Idol;
                    moveInput = Vector2.zero;
                }
                break;
        }      
    }
    // Brakeアクションによって呼び出されます。
    public void OnBrake(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                // Brake前にSetTriggerされたanimatorをResetする。
                animator.ResetTrigger(StoppingId);
                animator.ResetTrigger(BrakeStartId);
                animator.ResetTrigger(Brake2StartId);
                if (gameScene.IsPaused == false)
                {
                    // 現在のプレイヤー状態変化
                    currentState = PlayerState.Brake;
                    // Brake(First)アニメーションに切り替える
                    animator.SetTrigger(BrakeStartId);
                }
                break;
            case InputActionPhase.Canceled:
                if (headrigidbody.velocity.magnitude > 0)
                {
                    // Brake(Final)アニメーションに切り替える
                    animator.SetTrigger(BrakeEndId);
                    if (currentState == PlayerState.Brake)
                    {
                        // 現在のプレイヤー状態変化
                        currentState = PlayerState.Walk;
                    }
                }
                else if (headrigidbody.velocity.magnitude == 0)
                {
                    // Brake(Final)アニメーションに切り替える
                    animator.SetTrigger(Brake2EndId);
                    if (currentState == PlayerState.Brake)
                    {
                        // 現在のプレイヤー状態変化
                        currentState = PlayerState.Idol;
                    }                    
                }
                break;
        }
    }

    // Dashアクションによって呼び出されます。
    public void OnDash(InputAction.CallbackContext context)
    {
        if (speedLimit == 30)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    // 現在のプレイヤー状態変化
                    currentState = PlayerState.Dash;
                    break;
                case InputActionPhase.Canceled:
                    // 速度上限を上げる
                    speedLimit += (speedLimit * dashTimer) / 2;
                    // Dashの力を指定し、カメラの向きに合わせる
                    var Dashpower = new Vector3(0, 0, 60);
                    Dashpower = mainCamera.transform.TransformDirection(Dashpower);
                    // Dashする
                    if (headcurrentState == HeadOnlyState.bodyOne)
                    {
                        bodymoveBehaviour.Dash(Dashpower, speedLimit);
                    }
                    else if (headcurrentState == HeadOnlyState.headOnly)
                    {
                        headmoveBehaviour.Dash(Dashpower, speedLimit);
                    }
                    // チャージ時間をもとに戻す。
                    dashTimer = 0;
                    // 現在のプレイヤー状態変化
                    currentState = PlayerState.Walk;
                    break;
            }
        }
    }
    // Throwアクションによって呼び出されます。
    public void OnThrow(InputAction.CallbackContext context)
    {
        if (!(headcurrentState == HeadOnlyState.headOnly))
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    break;
                case InputActionPhase.Performed:
                    break;
                case InputActionPhase.Canceled:
                    // プレイヤーの状態変化
                    currentState = PlayerState.Throwing;
                    StartCoroutine(OnThrowing());
                    break;
            }
        }
    }
    public IEnumerator OnThrowing()
    {
        //投げる前にheadのポジションを記憶する
        //var model = modelhead.transform.position;
        //var head2 = head.transform.position;
        //saveHeadPosition = new Vector3 (head.transform.position.x, head.transform.position.y, head.transform.position.z);
        // 投げる前にmodelheadのポジションを直しておく
        modelhead.transform.position = head.transform.position;
        // 投げる前にPlayerからすべてを引きはがす
        transform.DetachChildren();
        // 引き入れてもらう
        head.transform.parent = player.transform;
        // bodyに付いているJointの削除
        Destroy(body.GetComponent<CharacterJoint>());
        // 投げます
        headmoveBehaviour.Throw();
        // 最初に投げたらそのままずっとtrue
        //firstThrow = true;
        // 0.1秒間待機
        yield return new WaitForSeconds((float)0.1);
        // 頭だけになりまりました。
        headcurrentState = HeadOnlyState.headOnly;
        // modelのポジションを同期させる
        modelhead.transform.position = head.transform.position;
        // 2秒間待機
        yield return new WaitForSeconds(2);
        // プレイヤーの状態変化
        currentState = PlayerState.Idol;
    }
    #endregion

    public void ReferToTheNewBody(GameObject Nextbody, GameObject NextModelBody)
    {
        body = Nextbody;
        bodymoveBehaviour = Nextbody.GetComponent<MoveBehaviour>();
        modelbody = NextModelBody;
    }*/

    //再生するエフェクトを変更
    public void changeSmokeNumber(int n)
    {
        smokeNumber = n;
        smokeEffectActive = false;
        Debug.Log(smokeNumber);
    }

    /*public void stopSmoke(int n)
    {
        smoke[n].Stop();
        smokeEffectActive = false;
    }*/
}