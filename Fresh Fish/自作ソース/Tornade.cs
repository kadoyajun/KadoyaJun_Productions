using UnityEngine;

public class Tornade : MonoBehaviour
{
    [SerializeField]
    [Tooltip("0:固定 1:回転 2:移動　3:回転移動")]
    private int tornadeNo = 0;

    [SerializeField]
    [Tooltip("ふっとばす強さ")]
    private float tornadeForce = 1;

    [SerializeField]
    [Tooltip("回転速度")]
    private float tornadeRotSpeed = 90;

    [SerializeField]
    [Tooltip("移動周期")]
    private float tornadeCycle = 90;

    [SerializeField]
    [Tooltip("移動向き")]
    private Vector2 tornadeMove = Vector2.zero;

    Rigidbody2D rb;

    //ふっとばす向き
    Vector3 tornadeVector = Vector3.zero;

    //向き
    float RotAngle = 0;
    float MoveCycle = 0;
    private void Update()
    {
        //向きに応じてベクトルの向きを変更
        float rad = Mathf.Deg2Rad * transform.eulerAngles.z;
        float sinvalue = Mathf.Sin(rad);
        float cosvalue = Mathf.Cos(rad);
        Vector2 vector2 = new Vector2(cosvalue, sinvalue);
        tornadeVector = vector2 * tornadeForce;

        if (tornadeNo == 1 || tornadeNo == 3)
        {
            //オブジェクトの回転
            RotAngle += Time.deltaTime * tornadeRotSpeed;
            transform.eulerAngles = new Vector3(0, 0, RotAngle);
            if (RotAngle >= 360)
            {
                RotAngle = 0;
            }
        }

        if (tornadeNo == 2 || tornadeNo == 3)
        {
            //オブジェクトの移動
            MoveCycle += Time.deltaTime * tornadeCycle;
            float MovePos = Mathf.Sin(MoveCycle * Mathf.Deg2Rad);
            transform.position += new Vector3((MovePos * tornadeMove.x * Time.deltaTime), (MovePos * tornadeMove.y * Time.deltaTime), 0);
            if (MoveCycle > 360)
            {
                MoveCycle = 0;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //トリガーに入ったオブジェクトに力を加える
        rb = other.gameObject.GetComponent<Rigidbody2D>();
        rb.AddForce(tornadeVector, ForceMode2D.Impulse);
    }
}
