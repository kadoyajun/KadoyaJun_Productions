using System.Collections;
using UnityEngine;

public class Rock : MonoBehaviour
{
    // bodyを指定
    private GameObject body;

    [SerializeField]
    [Tooltip("壊せるようになる大きさ")]
    private float breakSnowScale = 4;

    [SerializeField]
    private Vector3 backPower = new Vector3(0, 0, -100);

    // 壊れた時に出るエフェクトと位置と音を指定
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
        // Bodyを探し参照
        body = GameObject.Find("snowman_body");
        moveBehaviour = body.GetComponent<MoveBehaviour>();
        // rb,cdそれぞれを参照
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
