using UnityEngine;

public class Tornade : MonoBehaviour
{
    [SerializeField]
    [Tooltip("0:�Œ� 1:��] 2:�ړ��@3:��]�ړ�")]
    private int tornadeNo = 0;

    [SerializeField]
    [Tooltip("�ӂ��Ƃ΂�����")]
    private float tornadeForce = 1;

    [SerializeField]
    [Tooltip("��]���x")]
    private float tornadeRotSpeed = 90;

    [SerializeField]
    [Tooltip("�ړ�����")]
    private float tornadeCycle = 90;

    [SerializeField]
    [Tooltip("�ړ�����")]
    private Vector2 tornadeMove = Vector2.zero;

    Rigidbody2D rb;

    //�ӂ��Ƃ΂�����
    Vector3 tornadeVector = Vector3.zero;

    //����
    float RotAngle = 0;
    float MoveCycle = 0;
    private void Update()
    {
        //�����ɉ����ăx�N�g���̌�����ύX
        float rad = Mathf.Deg2Rad * transform.eulerAngles.z;
        float sinvalue = Mathf.Sin(rad);
        float cosvalue = Mathf.Cos(rad);
        Vector2 vector2 = new Vector2(cosvalue, sinvalue);
        tornadeVector = vector2 * tornadeForce;

        if (tornadeNo == 1 || tornadeNo == 3)
        {
            //�I�u�W�F�N�g�̉�]
            RotAngle += Time.deltaTime * tornadeRotSpeed;
            transform.eulerAngles = new Vector3(0, 0, RotAngle);
            if (RotAngle >= 360)
            {
                RotAngle = 0;
            }
        }

        if (tornadeNo == 2 || tornadeNo == 3)
        {
            //�I�u�W�F�N�g�̈ړ�
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
        //�g���K�[�ɓ������I�u�W�F�N�g�ɗ͂�������
        rb = other.gameObject.GetComponent<Rigidbody2D>();
        rb.AddForce(tornadeVector, ForceMode2D.Impulse);
    }
}
