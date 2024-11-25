using System.Collections;
using UnityEngine;

namespace Genzan
{
    [System.Serializable]
    public class DamageOfDamageArea
    {
        public int[] damage = new int[3];
    }

    public class BossController : MonoBehaviour
    {
        #region 変更可能なステータス類
        [SerializeField]
        EnemyData enemyData = null;

        //damageOfDamegageArea[i].damage[j]はAttack[i]のj番目の難易度のダメージ
        [SerializeField]
        DamageOfDamageArea[] damageOfDamageArea;

        [SerializeField]
        int maxHP = 0;

        //プレイヤーに攻撃された際の吹っ飛ばしマス数
        [SerializeField]
        Vector2Int blowPower;

        [SerializeField]
        GameObject summonEnemy;

        [SerializeField]
        ParticleSystem explosion;
        #endregion

        #region ステータス類
        public string Name { get;private set; }
        public int BossLevel { get; private set; }
        public bool Active { get; private set; } = true;
        enum BossState { Charge, Attack, Blow }
        BossState bossState = BossState.Charge;
        int hP = 0;
        int attackNumber = 0;
        int attackTypeCount = 0;
        #endregion

        GameManager gM;

        GameObject[] attackArea;
        GameObject playerAttackPoint;

        [SerializeField]
        GameObject bossSprite;

        [SerializeField]
        private Animator animator;

        //撃破時のパーティクル再生の時のみtrue
        public bool DefeatParticle { get; private set; } = false;

        private void Awake()
        {
            //攻撃範囲やプレイヤーの攻撃可能範囲を取得
            GameObject gameObject = transform.GetChild(0).gameObject;
            attackTypeCount = gameObject.transform.childCount;
            attackArea = new GameObject[attackTypeCount];
            for (byte i = 0; i < attackTypeCount; i++)
            {
                attackArea[i] = gameObject.transform.GetChild(i).gameObject;
            }
            playerAttackPoint = transform.GetChild(1).gameObject;
        }
        private void Start()
        {
            gM = transform.root.GetComponent<GameManager>();
            Name = enemyData.enemyName;
            hP = maxHP;
            StartCoroutine(SetLevel());
        }

        //１フレーム待っているのは、GameManager側で難易度がセットされるのを待たなくてはならないため
        IEnumerator SetLevel()
        {
            yield return null;
            if (gM.difficulty == GameManager.Difficulty.Easy)
            {
                BossLevel = enemyData.easyLevel;
            }
            else if (gM.difficulty == GameManager.Difficulty.Normal)
            {
                BossLevel = enemyData.nomalLevel;
            }
            else if (gM.difficulty == GameManager.Difficulty.Hard)
            {
                BossLevel = enemyData.hardLevel;
            }
        }

        //ボスの行動
        public void BossMove()
        {
            switch (bossState)
            {
                //攻撃範囲を表示し、次ターンで攻撃
                case BossState.Charge:
                    attackNumber = UnityEngine.Random.Range(0, attackTypeCount);
                    if (attackNumber == 0)
                    {
                        attackArea[attackNumber].transform.GetChild(0).gameObject.transform.position = new Vector3(gM.player.Position.x, 0.01f, gM.player.Position.y + 1);
                    }
                    attackArea[attackNumber].SetActive(true);
                    bossState = BossState.Attack;
                    break;
                //攻撃
                case BossState.Attack:
                    int damagePointCount = attackArea[attackNumber].transform.childCount;
                    bool isHit = false;
                    for (int i = 0; i < damagePointCount; i++)
                    {
                        ParticleSystem particleSystem = attackArea[attackNumber].transform.GetChild(i).gameObject.GetComponent<ParticleSystem>();
                        particleSystem.Play();
                        if (!isHit)
                        {
                            Vector3 vector3 = attackArea[attackNumber].transform.GetChild(i).gameObject.transform.position;
                            Vector2Int damagePointPosition = new((int)vector3.x, (int)vector3.z);
                            if (gM.BossHitCheck(damagePointPosition, damageOfDamageArea[attackNumber].damage[(int)gM.difficulty]))
                            {
                                isHit = true;
                            }
                        }
                    }
                    if (attackNumber == 0 && !isHit)
                    {
                        GameObject summonedEnemy = Instantiate(summonEnemy, attackArea[0].transform.GetChild(0).gameObject.transform.position, Quaternion.identity);
                        summonedEnemy.transform.parent = transform.parent.GetChild(0).gameObject.transform;
                        gM.AddEnemy(summonedEnemy);
                    }
                    AudioManager.Instance.PlaySE("BossAttackSE");
                    StartCoroutine(WaitMoveTime());
                    bossState = BossState.Charge;
                    {
                        // アニメーションが再生中でない場合にのみ再生します
                        animator.SetBool("IsAtack", true);
                        Invoke(nameof(StopAnimation), animator.GetCurrentAnimatorClipInfo(0).Length);
                    }
                    break;
                //プレイヤーを吹っ飛ばす
                case BossState.Blow:
                    attackArea[attackNumber].SetActive(false);
                    gM.ChangePlayerPosition(blowPower);
                    bossState = BossState.Charge;
                    {
                        // アニメーションが再生中でない場合にのみ再生します
                        animator.SetBool("IsAtack", true);
                        Invoke(nameof(StopAnimation), animator.GetCurrentAnimatorClipInfo(0).Length);
                    }
                    break;
                default:
                    break;
            }
        }

        //プレイヤーの位置を受け取り、攻撃可能範囲にいたらボスにダメージ
        public bool HitBox(Vector2Int pp,uint pl,PlayerController pC)
        {
            int pAPCount = playerAttackPoint.transform.childCount;
            for(int i = 0; i < pAPCount; i++)
            {
                Transform transform = playerAttackPoint.transform.GetChild(i).gameObject.transform;
                Vector2Int vector2Int = new((int)transform.position.x,(int)transform.position.z);
                if(pp == vector2Int)
                {
                    bossState = BossState.Blow;
                    hP -= (int)pl;
                    pC.Attacked();
                    AudioManager.Instance.PlaySE("AttackToBossSE");
                    gM.DisplayMessage(LogText.DamagetoBoss(Name, gM.Level));

                    //体力が0になったら非アクティブにし、エフェクトを再生する
                    if(hP <= 0)
                    {
                        Active = false;
                        StartCoroutine(Explosion());
                    }
                    return true;
                }
            }
            return false;
        }

        IEnumerator Explosion()
        {
            DefeatParticle = true;
            float duration = explosion.main.duration;
            ParticleSystem.Burst burst = explosion.emission.GetBurst(0);
            float interval = burst.repeatInterval;
            Instantiate(explosion,bossSprite.transform.position,Quaternion.identity);
            for(int i = 0; i < duration / interval; i++)
            {
                AudioManager.Instance.PlaySE("Explosion");
                yield return new WaitForSeconds(interval);
            }
            DefeatParticle = false;
            gM.DisplayMessage(LogText.DefeatBoss(enemyData.enemyName));
            gM.LevelUpDown(-BossLevel, true);
            gameObject.SetActive(false);
        }

        void StopAnimation()
        {
            // アニメーションを停止し、制御を元に戻します
            animator.SetBool("IsAtack", false);
        }

        //攻撃範囲の表示をEnemyMoveTime秒後に非表示に
        IEnumerator WaitMoveTime()
        {
            yield return new WaitForSeconds(gM.EnemyMoveTime);
            attackArea[attackNumber].SetActive(false);
        }

        //現在の体力の割合を返す
        public float HPPercentage()
        {
            return (float)hP / maxHP;
        }

    }
}

