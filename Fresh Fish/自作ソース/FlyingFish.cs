using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FlyingFish : MonoBehaviour
{
    //�ŏ��̐���
    [SerializeField]
    [Tooltip("�ŏ��̐���")]
    private Vector3 firstForce = new Vector3 (0, 0, 0);

    //�����̑��x
    [SerializeField]
    [Tooltip("�����̑��x")]
    private float downSpeed = 1;

    //�㏸���̌����̑��x
    [SerializeField]
    [Tooltip("�㏸���̌����̑��x")]
    private float upDownSpeed = 1;

    //�W�����v�p���[�̔{��
    [SerializeField]
    [Tooltip("�W�����v�p���[�̔{��")]
    private float jumpPower = 1;

    //�W�����v�̃N�[���^�C��
    [SerializeField]
    [Tooltip("�W�����v�̃N�[���^�C��")]
    private float jumpCooltime = 1;

    //��]���x
    [SerializeField]
    [Tooltip("��]���x")]
    private float rotationSpeed = 1;

    //���U���g��pointScore�̒l
    [SerializeField]
    [Tooltip("0:��,1:�Ƃ̋߂�,2:���̎��i��,3:���̎��i���̋߂�,4:�n��,5:�r")]
    private float[] pointScore = new float[6];

    //���̋��̃X�v���C�g
    [SerializeField]
    [Tooltip("���̃X�v���C�g,0:�}�O��,1:�J�W�L,2:�g�r�E�I")]
    private Sprite[] fishSprite = new Sprite[3];

    //�^�C�v���Ƃ̃X�R�A
    [SerializeField]
    [Tooltip("�^�C�v���Ƃ̃X�R�A,0:�}�O��,1:�J�W�L,2:�g�r�E�I")]
    private float[] typeScore = new float[3];

    //�W�����v���\����\���A�C�R��
    [SerializeField]
    private GameObject jumpIcon = null;

    //GameScene.cs�Q�Ɨp
    [SerializeField]
    private GameScene gameScene = null;

    //Fish��Rigidbody2D
    Rigidbody2D rb;

    //�W�����v�̌���
    Vector3 jumpVector = new Vector3(0, 0, 0);

    //�W�����v�̃N�[���^�C���p
    float delta = 0;
    bool Icanfly = true;
    bool isGrounded = false;

    //�ڒn���Ԕ���p
    float isGroundedTime = 0;

    //�W�����v���p
    AudioSource jumpSound;

    //FishSpenLeft,Right����������p
    bool pressL = false;
    bool pressR = false;

    //�^�C�v�̃X�R�A
    int fishType = 0;
    float typeScoreThis;

    SpriteRenderer spriteRenderer;


    void Start()
    {
        fishType = PlayerPrefs.GetInt("fishType", 0);
        float smashForce = PlayerPrefs.GetFloat("smashForce", 1);

        //�X�^�[�g
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(firstForce * smashForce, ForceMode2D.Impulse);
        jumpSound = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        //���̋��̃^�C�v�|�C���g���Z�b�g
        for (int i = 0; i < fishSprite.Length; i++)
        {
            if (i == fishType)
            {
                typeScoreThis = typeScore[i];
                spriteRenderer.sprite = fishSprite[i];
            }
        }
    }

    void Update()
    {
        //�p�x�ɉ����ăW�����v�p���[�̃x�N�g����ύX����
        float rad = Mathf.Deg2Rad * transform.eulerAngles.z;
        float sinvalue = Mathf.Sin(rad);
        float cosvalue = Mathf.Cos(rad);
        Vector2 vector2 = new Vector2(cosvalue,sinvalue);
        jumpVector = vector2 * jumpPower;
        
        //�c�ɋ߂��قǌ���
        if(rb.velocity.x > 0)
        {
            Vector3 velocity = rb.velocity;
            velocity.x -= Mathf.Abs(sinvalue) * Time.deltaTime * downSpeed;
            rb.velocity = velocity;
        }

        if(rb.velocity.y > 0)
        {
            Vector3 upVelocity = rb.velocity;
            upVelocity.y -= upDownSpeed * Time.deltaTime;
            rb.velocity = upVelocity;
        }

        //Icanfly��true�̎��W�����v�\�Afalse�̎��s���A�C�R���ŕ\��
        if (Icanfly == true)
        {
            jumpIcon.SetActive(true);
        }
        else
        {
            jumpIcon.SetActive(false);
        }

        //�W�����v�̃N�[���^�C���Bfalse�ɂȂ��Ă���jumpCoolTime�b���true
        if (Icanfly == false && isGrounded == false)
        {
            delta += Time.deltaTime;
            if (delta >= jumpCooltime)
            {
                Icanfly = true;
                delta = 0;
            }
        }

        if(isGrounded == false && isGroundedTime > 0)
        {
            isGroundedTime = 0;
        }

        //��]
        if (pressL == true)
        {
            transform.Rotate(0, 0, Time.deltaTime * rotationSpeed);
        }
        else if(pressR == true)
        {
            transform.Rotate(0, 0, -1 * Time.deltaTime * rotationSpeed);
        }

        if(transform.position.y < -10)
        {
            gameScene.OnResultScene(pointScore[5], typeScoreThis);
        }
    }

    //Icanfly��true�̎��A������jumpVector�̕����ɃW�����v
    //�W�����v�����Icanfly��false��
    public void OnFishJump(InputAction.CallbackContext context)
    {
        if (context.started && Icanfly == true)
        {
            rb.AddForce(jumpVector, ForceMode2D.Impulse);
            Icanfly = false;
            jumpSound.Play();
        }
    }

    //�����ƍ���]
    public void OnFishSpenLeft(InputAction.CallbackContext context)
    {
        //������true�ɂȂ�A������false�i�������j
        if (context.started)
        {
            pressL = true;
        }
        if (context.canceled)
        {
            pressL = false;
        }
    }
    //�����ƉE��]
    public void OnFishSpenRight(InputAction.CallbackContext context)
    {
        //������true�ɂȂ�A������false�i�������j
        if (context.started)
        {
            pressR = true;
        }
        if(context.canceled)
        {
            pressR = false;
        }
    }

    //���x��0�ɋ߂��Ƃ��G��Ă���g���K�[�ɉ�����OnResultScene�����s
    private void OnTriggerStay2D(Collider2D collision2D)
    {
        if (rb.velocity.x <= 0.1 && rb.velocity.y <= 0.1 && rb.velocity.x >= -0.1 && rb.velocity.y >= -0.1)
        {
            if (collision2D.gameObject.tag == "House")
            {
                gameScene.OnResultScene(pointScore[0], typeScoreThis);
            }
            else if (collision2D.gameObject.tag == "HouseArea")
            {
                gameScene.OnResultScene(pointScore[1], typeScoreThis);
            }
            else if (collision2D.gameObject.tag == "GoldHouse")
            {
                gameScene.OnResultScene(pointScore[2], typeScoreThis);
            }
            else if (collision2D.gameObject.tag == "GoldHouseArea")
            {
                gameScene.OnResultScene(pointScore[3], typeScoreThis);
            }
        }
    }

    //�n�ʂɂ��Ă���Ƃ��W�����v�s��
    private void OnCollisionStay2D(Collision2D collision2D)
    {
        if(collision2D.gameObject.tag == "Ground")
        {
            Icanfly = false;
            isGrounded = true;
            pressL = false;
            pressR = false;
            if (rb.velocity.x <= 0.1 && rb.velocity.y <= 0.1 && rb.velocity.x >= -0.1 && rb.velocity.y >= -0.1)
            {
                isGroundedTime += Time.deltaTime;
                if(isGroundedTime > 3)
                {
                    gameScene.OnResultScene(pointScore[4], typeScoreThis);
                }
            }
        }
    }
}
