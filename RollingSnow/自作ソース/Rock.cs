using System.Collections;
using UnityEngine;

public class Rock : MonoBehaviour
{
    // body���w��
    private GameObject body;

    [SerializeField]
    [Tooltip("�󂹂�悤�ɂȂ�傫��")]
    private float breakSnowScale = 4;

    [SerializeField]
    private Vector3 backPower = new Vector3(0, 0, -100);

    // ��ꂽ���ɏo��G�t�F�N�g�ƈʒu�Ɖ����w��
    [SerializeField]
    private GameObject breakEffect = null;

    [SerializeField]
    private Transform effectPoint = null;

    [SerializeField]
    private AudioClip soundOnBreak = null;

    Collider cd;

    MoveBehaviour moveBehaviour;
    void Awake()
    {
        // Body��T���Q��
        body = GameObject.Find("snowman_body");
        moveBehaviour = body.GetComponent<MoveBehaviour>();
        // rb,cd���ꂼ����Q��
        cd = GetComponent<Collider>();
    }
    void Break()
    {
        Instantiate(breakEffect, effectPoint.transform.position, breakEffect.transform.rotation);
        AudioSource.PlayClipAtPoint(soundOnBreak, effectPoint.transform.position);
        Destroy(gameObject);
    }
    
    private void FixedUpdate()
    {
        var bodyScale = body.transform.localScale;
        if (bodyScale.x >= breakSnowScale)
        {
            cd.isTrigger = true;
        }
        else if (bodyScale.x < breakSnowScale)
        {
            cd.isTrigger = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        var bodyScale = body.transform.localScale;
        if (bodyScale.x >= breakSnowScale)
        {
            Break();
        }
    }
}
