using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    /*
    
    // ���O��Sceneroot���w��B
    [SerializeField]
    private GameObject sceneRoot;
    // ���O�ɃJ�������w��B
    public Transform mainCamera;
    public GameObject freeLookCamera;
    // ���O�Ƀv���C���[���w��B
    public GameObject player;
    // ���O�ɓ����w��B
    public GameObject head;
    public GameObject modelhead;
    // ���O�ɑ̂��w��B
    public GameObject body;
    public GameObject modelbody;
    // ���O�Ƀ}�t���[���w��B
    public GameObject scarfLeft;
    public GameObject scarfRight;
    // ���O�Ɏ�܂��w��B
    public GameObject handLeft;
    public GameObject handRight;

    //���O�ɃG�t�F�N�g���Q��
    [SerializeField]
    private ParticleSystem[] smoke = new ParticleSystem[3];
    //�G�t�F�N�g�̃A�N�e�B�u��
    bool smokeEffectActive = true;
    //�G�t�F�N�g�؂�ւ��p�̒l�@�G��Ă���n��
    private int smokeNumber = 0;

    // �R���|�[�l���g�����O�ɎQ�ƁB
    GameScene gameScene;
    public CinemachineInputProvider cameraInputProvider;
    AudioSource audioSource;
    Animator animator;
    // Animator�p�����[�^�[ID
    static readonly int StoppingId = Animator.StringToHash("Stopping");
    static readonly int MovingId = Animator.StringToHash("Moving");
    static readonly int RunId = Animator.StringToHash("Run");
    static readonly int CollisionId = Animator.StringToHash("Collision");
    static readonly int BrakeStartId = Animator.StringToHash("BrakeStart");
    static readonly int BrakeEndId = Animator.StringToHash("BrakeEnd");
    static readonly int Brake2StartId = Animator.StringToHash("Brake2Start");
    static readonly int Brake2EndId = Animator.StringToHash("Brake2End");
    // head��rigiddbody�Q��
    Rigidbody headrigidbody;
    // State��Ԃɂ����head��body��moveBehaviour��؂�ւ���ϐ����Q��
    MoveBehaviour headmoveBehaviour;
    MoveBehaviour bodymoveBehaviour;

    // ���݂�Move���͒l
    Vector2 moveInput;

    // 
    //public Vector3 saveHeadPosition;

    // ���x�̏�����w�肵�܂��B
    [SerializeField]
    public float speedLimit = 30;
    // ���ݑ��x
    public Vector3 velocity;
    // Dash����Ƃ��ɉ��b�ԉ�����������
    private float dashTimer = 0;

    // ��Βl�̕Ԃ��ϐ�
    private double absolutely = 0;

    // �T�E���h�̐���̂��߂̕ϐ�
    private bool footStepsPlaying = false;
    // Player��Move�ɑ΂��鐧��̕ϐ�
    public bool playerMove = false;

    // 1�t���[���O�̈ʒu
    private Vector3 prevPosition;
    // �v���C���[�̃|�W�V����
    private Vector3 playerPos; 
    // �u���[�L���s�����ەb���𑪂�
    private Vector3 playerPositionDiff;
    // Body�̃|�W�V����
    public Vector3 bodyPos;

    // ������BodyScale
    private Vector3 firstBody;
    // ������ScarfRotation
    [SerializeField]
    public Vector3 firstScarfrotation_L;
    private Vector3 firstScarfrotation_R;

    // Scarf���񂷍ۂ�Float�l
    [SerializeField]
    private float scarfRotationSpeed = 18f;

    // Head��body�̏�Ԃ�\���܂��B
    public enum HeadOnlyState
    {
        // head�̂�
        headOnly,
        // head��body���
        bodyOne,
    }
    // ���݂̃v���C���[�̐؂藣����
    public HeadOnlyState headcurrentState = HeadOnlyState.bodyOne;

    // �v���C���[�̏�Ԃ�\���܂��B
    public enum PlayerState
    {
        // �ҋ@���
        Idol,
        // �ʏ�̑��s���
        Walk,
        // �_�b�V��
        Dash,
        // �u���[�L����
        Brake,
        // ����A����
        Throwing,
    }
    // ���݂̃v���C���[���
    public PlayerState currentState = PlayerState.Idol;

    // Start is called before the first frame update
    void Awake()
    {
        // Maincamera�̎q�I�u�W�F�N�g�ɕt���Ă���AudioSouce���Q��
        audioSource = GameObject.Find("FootStepsAudio").GetComponent<AudioSource>();

        // head��body��moveBehaviour�����ꂼ��Q��
        headmoveBehaviour = head.GetComponent<MoveBehaviour>();
        bodymoveBehaviour = body.GetComponent<MoveBehaviour>();
        // �R���|�[�l���g���Q��
        gameScene = sceneRoot.GetComponent<GameScene>();
        cameraInputProvider = freeLookCamera.GetComponent<CinemachineInputProvider>();
        cameraInputProvider.enabled = false;
        headrigidbody = head.GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // �ŏ��̒i�K��Body��Scale���Q��
        firstBody = body.transform.localScale;
        // �ŏ��̒i�K��Scarf��rotation���Q��
        firstScarfrotation_L = scarfLeft.transform.localEulerAngles;
        firstScarfrotation_R = scarfRight.transform.localEulerAngles;

        // �ŏ��̎��_�ł̃v���C���[�̃|�W�V�������擾
        playerPos = head.GetComponent<Transform>().position;
        // �����ʒu��ێ�
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
        #region �v���C���[�̌�������
        // �v���C���[���ǂ̕����ɐi��ł��邩���킩��悤�ɁA�����ʒu�ƌ��ݒn�̍��W�������擾
        Vector3 diff = head.transform.position - playerPos;

        // �x�N�g���̒�����0.01f���傫���ꍇ�Ƀv���C���[�̌�����ς��鏈�������� (0�ł͓���Ȃ�)
        if (diff.magnitude > 0.01f)
        {
            // �x�N�g���̏���Quaternion.LookRotation�Ɉ����n����]�ʂ��擾���v���C���[����]������
            head.transform.rotation = Quaternion.LookRotation(diff);
            body.transform.rotation = Quaternion.LookRotation(diff);
        }
        // �v���C���[�̈ʒu���X�V
        playerPos = head.transform.position;
        #endregion

        #region �v���C���[�̑��������߁A���䂷��B
        // deltaTime��0�̏ꍇ�͉������Ȃ�
        if (Mathf.Approximately(Time.deltaTime, 0))
            return;
        // ���݈ʒu�擾
        var position = head.transform.position;
        // ���ݑ��x�����O�o��
        //print($"velocity = {headrigidbody.velocity.magnitude}");
        // ���ݑ��x�v�Z
        velocity = (position - prevPosition) / Time.deltaTime;
        // �O�t���[���ʒu���X�V
        prevPosition = position;
        #endregion

        */

        #region rigidbody.velocity��3�ȏゾ�Ɠ����Ă�����̂Ƃ���
        if (headrigidbody.velocity.magnitude >= 3)
        {
            //Debug.Log(smokeEffectActive);
            // Stopping�A�j���[�V�����ɐ؂�ւ���
            animator.SetTrigger(MovingId);

            //�G�t�F�N�g���Đ�
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
            // �I�u�W�F�N�g�������Ă��Ă���false�̏ꍇ�A�������Đ�
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
                // Stopping�A�j���[�V�����ɐ؂�ւ���
                animator.SetTrigger(StoppingId);
            }

            //�G�t�F�N�g���~
            if (smokeEffectActive == true)
            {
                smoke[smokeNumber].Stop();
                smokeEffectActive = false;
            }

            // �I�u�W�F�N�g���~�܂��Ă���A�Đ����X�g�b�v
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

    // ���̃X�N���v�g����Ăяo�����߂̊֐�
    public void SetAnimation()
    {
        // Colision�A�j���[�V�����ɐ؂�ւ���
        animator.SetTrigger(CollisionId);
    }
    // �Ăяo���Ɛ�Βl���o���Ă����B
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
            // �J�����𐳖ʂɃL�����N�^�[�̈ړ���������
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
        // �ŏ��̑傫�����猻�݂̑傫��������
        var bodyScalediff = firstBody - body.transform.localScale;
        // �傫���̈Ⴂ��0��艺�̏ꍇ�̂ݍs��
        if (bodyScalediff.x < 0)
        {
            // Left��Right��x�̒l���Ⴄ�̂��ꂼ��s��
            var Scarfrotation_L = firstScarfrotation_L;
            var Scarfrotation_R = firstScarfrotation_R;
            // Body�̑傫���̈Ⴂ�ɂ���āA�l���o���B
            Scarfrotation_L.z = Scarfrotation_L.z + (scarfRotationSpeed * bodyScalediff.x);
            Scarfrotation_R.z = Scarfrotation_L.z;
            if (Scarfrotation_L.z > 0)
            {
            // ���
            scarfLeft.transform.localEulerAngles = Scarfrotation_L;
            scarfRight.transform.localEulerAngles = Scarfrotation_R;
            }
        }
    }
    private void FixedUpdateForDash()
    {
        // �_�b�V���������͈ړ��s��
        moveInput = Vector2.zero;
        // �`���[�W���Ԃ𑪂�(�P�b�ȏ�͐؂�̂�)
        dashTimer += Time.deltaTime;
        if (dashTimer > 1)
        {
            dashTimer = 1;
        }
    }
    void FixedUpdateForBrake()
    {
        // �v���C���[�𐳖ʂƂ��鏈��
        velocity = player.transform.TransformDirection(headrigidbody.velocity);
        if (headcurrentState == HeadOnlyState.bodyOne)
        {
            bodymoveBehaviour.Brake(velocity);
        }
        else if (headcurrentState == HeadOnlyState.headOnly)
        {
            headmoveBehaviour.Brake(velocity);
        }

        // �v���C���[�����S�Ɏ~�܂�����ABrake�̓�ڂ̃A�j���[�V�����ɐ؂�ւ���B
        if (headrigidbody.velocity.magnitude == 0)
        {
            animator.SetTrigger(Brake2StartId);
        }
    }
    #region CallBack�ϐ�����������
    // Move�A�N�V�����ɂ���ČĂяo����܂��B
    public void OnMove(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                // Move�O��SetTrigger���ꂽanimator��Reset����B
                animator.ResetTrigger(StoppingId);
                animator.ResetTrigger(BrakeStartId);
                animator.ResetTrigger(Brake2StartId);
                animator.ResetTrigger(BrakeEndId);
                animator.ResetTrigger(Brake2EndId);

                // Run�A�j���[�V�����ɐ؂�ւ���
                animator.SetTrigger(RunId);
                if (currentState == PlayerState.Idol)
                {
                    // ���݂̃v���C���[��ԕω�
                    currentState = PlayerState.Walk;
                }
                break;
            case InputActionPhase.Performed:
                moveInput = context.ReadValue<Vector2>();
                break;
            case InputActionPhase.Canceled:
                // Run�A�j���[�V�����ɐ؂�ւ���
                animator.ResetTrigger(RunId);
                if (currentState == PlayerState.Walk)
                {
                    // ���݂̃v���C���[��ԕω�
                    currentState = PlayerState.Idol;
                    moveInput = Vector2.zero;
                }
                break;
        }      
    }
    // Brake�A�N�V�����ɂ���ČĂяo����܂��B
    public void OnBrake(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                // Brake�O��SetTrigger���ꂽanimator��Reset����B
                animator.ResetTrigger(StoppingId);
                animator.ResetTrigger(BrakeStartId);
                animator.ResetTrigger(Brake2StartId);
                if (gameScene.IsPaused == false)
                {
                    // ���݂̃v���C���[��ԕω�
                    currentState = PlayerState.Brake;
                    // Brake(First)�A�j���[�V�����ɐ؂�ւ���
                    animator.SetTrigger(BrakeStartId);
                }
                break;
            case InputActionPhase.Canceled:
                if (headrigidbody.velocity.magnitude > 0)
                {
                    // Brake(Final)�A�j���[�V�����ɐ؂�ւ���
                    animator.SetTrigger(BrakeEndId);
                    if (currentState == PlayerState.Brake)
                    {
                        // ���݂̃v���C���[��ԕω�
                        currentState = PlayerState.Walk;
                    }
                }
                else if (headrigidbody.velocity.magnitude == 0)
                {
                    // Brake(Final)�A�j���[�V�����ɐ؂�ւ���
                    animator.SetTrigger(Brake2EndId);
                    if (currentState == PlayerState.Brake)
                    {
                        // ���݂̃v���C���[��ԕω�
                        currentState = PlayerState.Idol;
                    }                    
                }
                break;
        }
    }

    // Dash�A�N�V�����ɂ���ČĂяo����܂��B
    public void OnDash(InputAction.CallbackContext context)
    {
        if (speedLimit == 30)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    // ���݂̃v���C���[��ԕω�
                    currentState = PlayerState.Dash;
                    break;
                case InputActionPhase.Canceled:
                    // ���x������グ��
                    speedLimit += (speedLimit * dashTimer) / 2;
                    // Dash�̗͂��w�肵�A�J�����̌����ɍ��킹��
                    var Dashpower = new Vector3(0, 0, 60);
                    Dashpower = mainCamera.transform.TransformDirection(Dashpower);
                    // Dash����
                    if (headcurrentState == HeadOnlyState.bodyOne)
                    {
                        bodymoveBehaviour.Dash(Dashpower, speedLimit);
                    }
                    else if (headcurrentState == HeadOnlyState.headOnly)
                    {
                        headmoveBehaviour.Dash(Dashpower, speedLimit);
                    }
                    // �`���[�W���Ԃ����Ƃɖ߂��B
                    dashTimer = 0;
                    // ���݂̃v���C���[��ԕω�
                    currentState = PlayerState.Walk;
                    break;
            }
        }
    }
    // Throw�A�N�V�����ɂ���ČĂяo����܂��B
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
                    // �v���C���[�̏�ԕω�
                    currentState = PlayerState.Throwing;
                    StartCoroutine(OnThrowing());
                    break;
            }
        }
    }
    public IEnumerator OnThrowing()
    {
        //������O��head�̃|�W�V�������L������
        //var model = modelhead.transform.position;
        //var head2 = head.transform.position;
        //saveHeadPosition = new Vector3 (head.transform.position.x, head.transform.position.y, head.transform.position.z);
        // ������O��modelhead�̃|�W�V�����𒼂��Ă���
        modelhead.transform.position = head.transform.position;
        // ������O��Player���炷�ׂĂ������͂���
        transform.DetachChildren();
        // ��������Ă��炤
        head.transform.parent = player.transform;
        // body�ɕt���Ă���Joint�̍폜
        Destroy(body.GetComponent<CharacterJoint>());
        // �����܂�
        headmoveBehaviour.Throw();
        // �ŏ��ɓ������炻�̂܂܂�����true
        //firstThrow = true;
        // 0.1�b�ԑҋ@
        yield return new WaitForSeconds((float)0.1);
        // �������ɂȂ�܂�܂����B
        headcurrentState = HeadOnlyState.headOnly;
        // model�̃|�W�V�����𓯊�������
        modelhead.transform.position = head.transform.position;
        // 2�b�ԑҋ@
        yield return new WaitForSeconds(2);
        // �v���C���[�̏�ԕω�
        currentState = PlayerState.Idol;
    }
    #endregion

    public void ReferToTheNewBody(GameObject Nextbody, GameObject NextModelBody)
    {
        body = Nextbody;
        bodymoveBehaviour = Nextbody.GetComponent<MoveBehaviour>();
        modelbody = NextModelBody;
    }*/

    //�Đ�����G�t�F�N�g��ύX
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