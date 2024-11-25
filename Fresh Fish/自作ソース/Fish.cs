using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.EventSystems;

public class Fish : MonoBehaviour
{
    // �ʏ펞�̉j�����x
    [SerializeField]
    private float[] swimspeed = new float[3];
    // ���݂̉j�����x
    float speed = 4;

    // �j���ł��鎞��
    [SerializeField]
    private float swimtimer = 0;
    // �j���������ς�鎞��
    [SerializeField]
    private float returntime = 2;

    // �\������
    [SerializeField]
    private float gaugetimer = 0;
    // �\���I������
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

    //���̃X�v���C�g
    [SerializeField]
    [Tooltip("���̃X�v���C�g")]
    private Sprite[] fishSprite = null;

    SpriteRenderer spriteRenderer;
    int fishType;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();

        isHit = false;

        //3��ނ̒����烉���_���ŃX�v���C�g���ύX�����
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
            // �����x�^��
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
            // �\�����Ă��鎞��
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
        // �j���ł��鎞��
        swimtimer += Time.deltaTime;

        // �w�肵�����Ԃ𒴂���Ɣ��Ε����Ɉړ�
        if (swimtimer >= returntime && swim == true)
        {
            // �����𔽓]������
            transform.Rotate(new Vector2(0, 180));

            speed = speed * -1;
            var velocity = rigidbody.velocity;
            velocity.x = speed;
            rigidbody.velocity = velocity;

            // �j���ł��鎞�Ԃ����Z�b�g
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
            //Debug.Log("���");
            rigidbody.velocity = new Vector3(5, 10, 0);
    }
}
