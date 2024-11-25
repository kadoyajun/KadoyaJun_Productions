using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.EventSystems;

public class Fish : MonoBehaviour
{
    // 通常時の泳ぐ速度
    [SerializeField]
    private float[] swimspeed = new float[3];
    // 現在の泳ぐ速度
    float speed = 4;

    // 泳いでいる時間
    [SerializeField]
    private float swimtimer = 0;
    // 泳ぐ向きが変わる時間
    [SerializeField]
    private float returntime = 2;

    // 表示時間
    [SerializeField]
    private float gaugetimer = 0;
    // 表示終了時間
    [SerializeField]
    private float endtime = 15;

    [SerializeField]
    public GaugeUI gaugeUI;
    [SerializeField]
    public Brock brock;
    [SerializeField]
    private FishingScene fishingScene;


    private bool swim = true;

    private bool isHit = false;

    new Rigidbody2D rigidbody;

    //魚のスプライト
    [SerializeField]
    [Tooltip("魚のスプライト")]
    private Sprite[] fishSprite = null;

    SpriteRenderer spriteRenderer;
    int fishType;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();

        isHit = false;

        //3種類の中からランダムでスプライトが変更される
        spriteRenderer = GetComponent<SpriteRenderer>();
        fishType = UnityEngine.Random.Range(0, 3);
        for(int i = 0;  i < fishSprite.Length; i++)
        {
            if(i == fishType)
            {
                spriteRenderer.sprite = fishSprite[i];
                speed = swimspeed[i];
            }
        }


    }

    // Update is called once per frame
    void Update()
    {
        if (swim == true)
        {
            // 等速度運動
            var velocity = rigidbody.velocity;
            velocity.x = speed;
            rigidbody.velocity = velocity;
        }
        else if (swim == false)
        {
            rigidbody.velocity = new Vector3(0, 2, 0);
        }

        if (isHit == true)
        {
            // 表示している時間
            gaugetimer += Time.deltaTime;

            if (gaugetimer >= endtime)
            {
                Flying();
                fishingScene.FishingEnd(fishType);

            }
        }
    }

    private void FixedUpdate()
    {
        // 泳いでいる時間
        swimtimer += Time.deltaTime;

        // 指定した時間を超えると反対方向に移動
        if (swimtimer >= returntime && swim == true)
        {
            // 方向を反転させる
            transform.Rotate(new Vector2(0, 180));

            speed = speed * -1;
            var velocity = rigidbody.velocity;
            velocity.x = speed;
            rigidbody.velocity = velocity;

            // 泳いでいる時間をリセット
            swimtimer = 0;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (collision2D.gameObject.tag == "Feed")
        {
            swim = false;
            isHit = true;

            gaugeUI.Show();
            brock.Show1();
        }
    }

    public void Flying()
    {
            //Debug.Log("飛べ");
            rigidbody.velocity = new Vector3(5, 10, 0);
    }
}
