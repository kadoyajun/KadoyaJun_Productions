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
        #region �ύX�\�ȃX�e�[�^�X��
        [SerializeField]
        EnemyData enemyData = null;

        //damageOfDamegageArea[i].damage[j]��Attack[i]��j�Ԗڂ̓�Փx�̃_���[�W
        [SerializeField]
        DamageOfDamageArea[] damageOfDamageArea;

        [SerializeField]
        int maxHP = 0;

        //�v���C���[�ɍU�����ꂽ�ۂ̐�����΂��}�X��
        [SerializeField]
        Vector2Int blowPower;

        [SerializeField]
        GameObject summonEnemy;

        [SerializeField]
        ParticleSystem explosion;
        #endregion

        #region �X�e�[�^�X��
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

        //���j���̃p�[�e�B�N���Đ��̎��̂�true
        public bool DefeatParticle { get; private set; } = false;

        private void Awake()
        {
            //�U���͈͂�v���C���[�̍U���\�͈͂��擾
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

        //�P�t���[���҂��Ă���̂́AGameManager���œ�Փx���Z�b�g�����̂�҂��Ȃ��Ă͂Ȃ�Ȃ�����
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

        //�{�X�̍s��
        public void BossMove()
        {
            switch (bossState)
            {
                //�U���͈͂�\�����A���^�[���ōU��
                case BossState.Charge:
                    attackNumber = UnityEngine.Random.Range(0, attackTypeCount);
                    if (attackNumber == 0)
                    {
                        attackArea[attackNumber].transform.GetChild(0).gameObject.transform.position = new Vector3(gM.player.Position.x, 0.01f, gM.player.Position.y + 1);
                    }
                    attackArea[attackNumber].SetActive(true);
                    bossState = BossState.Attack;
                    break;
                //�U��
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
                        // �A�j���[�V�������Đ����łȂ��ꍇ�ɂ̂ݍĐ����܂�
                        animator.SetBool("IsAtack", true);
                        Invoke(nameof(StopAnimation), animator.GetCurrentAnimatorClipInfo(0).Length);
                    }
                    break;
                //�v���C���[�𐁂���΂�
                case BossState.Blow:
                    attackArea[attackNumber].SetActive(false);
                    gM.ChangePlayerPosition(blowPower);
                    bossState = BossState.Charge;
                    {
                        // �A�j���[�V�������Đ����łȂ��ꍇ�ɂ̂ݍĐ����܂�
                        animator.SetBool("IsAtack", true);
                        Invoke(nameof(StopAnimation), animator.GetCurrentAnimatorClipInfo(0).Length);
                    }
                    break;
                default:
                    break;
            }
        }

        //�v���C���[�̈ʒu���󂯎��A�U���\�͈͂ɂ�����{�X�Ƀ_���[�W
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

                    //�̗͂�0�ɂȂ������A�N�e�B�u�ɂ��A�G�t�F�N�g���Đ�����
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
            // �A�j���[�V�������~���A��������ɖ߂��܂�
            animator.SetBool("IsAtack", false);
        }

        //�U���͈͂̕\����EnemyMoveTime�b��ɔ�\����
        IEnumerator WaitMoveTime()
        {
            yield return new WaitForSeconds(gM.EnemyMoveTime);
            attackArea[attackNumber].SetActive(false);
        }

        //���݂̗̑͂̊�����Ԃ�
        public float HPPercentage()
        {
            return (float)hP / maxHP;
        }

    }
}

