using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum State { PATROL, TRACE, ATTACK, DIE };

    public State state = State.PATROL;

    public float attackDist = 5f;
    public float traceDist = 10f;
    public bool isDie = false;

    [SerializeField]
    Transform playerTr;
    Transform EnemyrTr;
    Animator animator;
    NavMeshAgent agent;
    EnemyAgent enemyAgent;
    EnemyFire enemyFire;
    EnemyFOV enemyFOV;
    WaitForSeconds ws;
    Rigidbody rb;
    

    readonly int hashMove = Animator.StringToHash("IsMove");
    readonly int hashSpeed = Animator.StringToHash("Speed");
    readonly int hashDie = Animator.StringToHash("Die");
    readonly int hashDieIdx = Animator.StringToHash("DieIdx");
    readonly int hashOffset = Animator.StringToHash("Offset");
    readonly int hashWalkSpeed = Animator.StringToHash("WalkSpeed");
    readonly int hashExpDie = Animator.StringToHash("Explosion");
    readonly int hashPlayerDie = Animator.StringToHash("PlayerDie");
    EnemyDamage damage;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        damage = GetComponent<EnemyDamage>();
        var player = GameObject.FindGameObjectWithTag("Player");
        if(player != null)      // 유효성 검사
        {
            playerTr = player.GetComponent<Transform>();
        }
        EnemyrTr = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        enemyAgent = GetComponent<EnemyAgent>();
        enemyFire = GetComponent<EnemyFire>();
        enemyFOV = GetComponent<EnemyFOV>();
        ws = new WaitForSeconds(0.3f);
        // cycle Offset 값을 불규칙하게
        animator.SetFloat(hashOffset, Random.Range(0f, 1f));
        animator.SetFloat(hashWalkSpeed, Random.Range(1f, 1.2f));
        rb = GetComponent<Rigidbody>();
    }
    private void OnEnable()     // 오브젝트가 활성화 되었을 때 자동으로 호출되는 콜백 함수(Start보다 빠름)
    {
        Damage.OnPlayerDie += this.OnPlayerDie;
        BarrelCtrl.OnEnemyDie += this.expDie; 
        //agent.isStopped = false;
        rb.mass = 70;
        StartCoroutine(CheckState());
        StartCoroutine(EnemyAction());
    }

    IEnumerator CheckState()
    {
        while (!isDie)
        {
            yield return ws;    // 1초에 3번 호출

            // 두 대상의 거리
            float dist = (playerTr.position - EnemyrTr.position).magnitude;
            // float dist = Vector3.Distance(playerTr.position, EnemyrTr.position);
            if (dist <= attackDist)
            {
                // 플레어와 거리에 장애물 여부 판단
                if(enemyFOV.isViewPlayer())
                    state = State.ATTACK;
            }
            else if(enemyFOV.isTracePlayer())
            {
                state = State.TRACE;
            }
            //else if (dist <= traceDist)
            //{
            //    state = State.TRACE;
            //}
            else
            {
                state = State.PATROL;
            }   
        }
    }

    IEnumerator EnemyAction()
    {
        while (!isDie)
        {
            yield return ws;

            switch (state)
            {
                case State.PATROL:
                    enemyFire.isFire = false;
                    enemyAgent.patrolling = true;
                    animator.SetBool(hashMove, true);
                    break;
                case State.TRACE:
                    enemyFire.isFire = false;
                    enemyAgent.traceTarget = playerTr.position;
                    animator.SetBool(hashMove, true);
                    break;
                case State.ATTACK:
                    if (enemyFire.isFire == false)
                    {
                        EnemyrTr.LookAt(playerTr);         // 플레이어를 바라보며 공격
                        enemyFire.isFire = true;
                    }
                    enemyAgent.Stop();
                    animator.SetBool(hashMove, false);
                    break;
                case State.DIE:
                    Die();
                    break;
            }  
        }
    }

    private void Update()
    {
        animator.SetFloat(hashSpeed, enemyAgent.speed);
    }
    public void Die()
    {
        enemyFire.isFire = false;
        isDie = true;
        enemyAgent.Stop();
        animator.SetInteger(hashDieIdx, Random.Range(0, 2));
        animator.SetTrigger(hashDie);
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<CapsuleCollider>().enabled = false;
        enemyAgent.patrolling = false;

        GameManager.instance.InkillCount();
        damage.hpText.text = "HP: 0";
        damage.hp = 0f;
        StopAllCoroutines();
        StartCoroutine(PushPool());
    }

    public void expDie()
    {
        if (isDie) return;
        enemyFire.isFire = false;
        isDie = true;
        enemyAgent.Stop();
        animator.SetTrigger(hashExpDie);
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<CapsuleCollider>().enabled = false;
        damage.hp = 0f;
        damage.hpBar.fillAmount = 0f;
        damage.hpText.text = "HP: 0";
        //UIManager.uiManager.InkillCount();
        GameManager.instance.InkillCount();
        //damage.hpBar.enabled = true;
        StopAllCoroutines();
        StartCoroutine(PushPool());
    }
    IEnumerator PushPool()
    {
        yield return new WaitForSeconds(3f);
        this.gameObject.SetActive(false);
        enemyAgent.patrolling = true;
        isDie = false;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<CapsuleCollider>().enabled = true;
        //damage.hpBar.gameObject.SetActive(true);
       
    }

    public void OnPlayerDie()
    {
        if (isDie)
            return;
        enemyAgent.Stop();          // 순찰 중지
        enemyFire.isFire = false;   // 총 발사 금지
        StopAllCoroutines();        // 모든 코루틴 정지
        animator.SetTrigger(hashPlayerDie);
    }

    private void OnDisable()
    {
        // 이벤트 연결 해제
        Damage.OnPlayerDie -= this.OnPlayerDie;
        BarrelCtrl.OnEnemyDie -= this.expDie;
    }
}
