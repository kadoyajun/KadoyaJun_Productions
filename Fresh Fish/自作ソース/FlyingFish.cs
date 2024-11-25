using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FlyingFish : MonoBehaviour
{
    //最初の勢い
    [SerializeField]
    [Tooltip("最初の勢い")]
    private Vector3 firstForce = new Vector3 (0, 0, 0);

    //減速の速度
    [SerializeField]
    [Tooltip("減速の速度")]
    private float downSpeed = 1;

    //上昇時の減速の速度
    [SerializeField]
    [Tooltip("上昇時の減速の速度")]
    private float upDownSpeed = 1;

    //ジャンプパワーの倍率
    [SerializeField]
    [Tooltip("ジャンプパワーの倍率")]
    private float jumpPower = 1;

    //ジャンプのクールタイム
    [SerializeField]
    [Tooltip("ジャンプのクールタイム")]
    private float jumpCooltime = 1;

    //回転速度
    [SerializeField]
    [Tooltip("回転速度")]
    private float rotationSpeed = 1;

    //リザルトのpointScoreの値
    [SerializeField]
    [Tooltip("0:家,1:家の近く,2:金の寿司屋,3:金の寿司屋の近く,4:地面,5:池")]
    private float[] pointScore = new float[6];

    //この魚のスプライト
    [SerializeField]
    [Tooltip("魚のスプライト,0:マグロ,1:カジキ,2:トビウオ")]
    private Sprite[] fishSprite = new Sprite[3];

    //タイプごとのスコア
    [SerializeField]
    [Tooltip("タイプごとのスコア,0:マグロ,1:カジキ,2:トビウオ")]
    private float[] typeScore = new float[3];

    //ジャンプが可能かを表すアイコン
    [SerializeField]
    private GameObject jumpIcon = null;

    //GameScene.cs参照用
    [SerializeField]
    private GameScene gameScene = null;

    //FishのRigidbody2D
    Rigidbody2D rb;

    //ジャンプの向き
    Vector3 jumpVector = new Vector3(0, 0, 0);

    //ジャンプのクールタイム用
    float delta = 0;
    bool Icanfly = true;
    bool isGrounded = false;

    //接地時間判定用
    float isGroundedTime = 0;

    //ジャンプ音用
    AudioSource jumpSound;

    //FishSpenLeft,Right長押し判定用
    bool pressL = false;
    bool pressR = false;

    //タイプのスコア
    int fishType = 0;
    float typeScoreThis;

    SpriteRenderer spriteRenderer;


    void Start()
    {
        fishType = PlayerPrefs.GetInt("fishType", 0);
        float smashForce = PlayerPrefs.GetFloat("smashForce", 1);

        //スタート
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(firstForce * smashForce, ForceMode2D.Impulse);
        jumpSound = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        //この魚のタイプポイントをセット
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
        //角度に応じてジャンプパワーのベクトルを変更する
        float rad = Mathf.Deg2Rad * transform.eulerAngles.z;
        float sinvalue = Mathf.Sin(rad);
        float cosvalue = Mathf.Cos(rad);
        Vector2 vector2 = new Vector2(cosvalue,sinvalue);
        jumpVector = vector2 * jumpPower;
        
        //縦に近いほど減速
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

        //Icanflyがtrueの時ジャンプ可能、falseの時不可をアイコンで表現
        if (Icanfly == true)
        {
            jumpIcon.SetActive(true);
        }
        else
        {
            jumpIcon.SetActive(false);
        }

        //ジャンプのクールタイム。falseになってからjumpCoolTime秒後にtrue
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

        //回転
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

    //Icanflyがtrueの時、押すとjumpVectorの方向にジャンプ
    //ジャンプするとIcanflyがfalseに
    public void OnFishJump(InputAction.CallbackContext context)
    {
        if (context.started && Icanfly == true)
        {
            rb.AddForce(jumpVector, ForceMode2D.Impulse);
            Icanfly = false;
            jumpSound.Play();
        }
    }

    //押すと左回転
    public void OnFishSpenLeft(InputAction.CallbackContext context)
    {
        //押すとtrueになり、離すとfalse（長押し）
        if (context.started)
        {
            pressL = true;
        }
        if (context.canceled)
        {
            pressL = false;
        }
    }
    //押すと右回転
    public void OnFishSpenRight(InputAction.CallbackContext context)
    {
        //押すとtrueになり、離すとfalse（長押し）
        if (context.started)
        {
            pressR = true;
        }
        if(context.canceled)
        {
            pressR = false;
        }
    }

    //速度が0に近いとき触れているトリガーに応じてOnResultSceneを実行
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

    //地面についているときジャンプ不可
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
