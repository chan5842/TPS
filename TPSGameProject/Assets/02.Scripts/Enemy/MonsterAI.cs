using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    public enum State { PATROL, TRACE, ATTACK, DIE };

    // 기본 상태 PATROL
    public State state = State.PATROL;
 
    public float attackDistance = 3.5f;
    public float traceDistance = 10f;
    public bool isDie = false;
   

    [SerializeField]
    MoveAgent moveAgent;
    [SerializeField]
    Transform playerTr;
    [SerializeField]
    Transform monsterTr;
    [SerializeField]
    Animator animator;
    readonly int hashMove = Animator.StringToHash("IsMove");
    readonly int hashAttack = Animator.StringToHash("IsAttack");
    readonly int hashSpeed = Animator.StringToHash("ForwardSpeed");
    WaitForSeconds ws;

    void Awake()
    {
        moveAgent = GetComponent<MoveAgent>();
        playerTr = GameObject.FindWithTag("Player").transform;
        monsterTr = gameObject.transform;
        animator = GetComponent<Animator>();
        ws = new WaitForSeconds(0.2f);
    }

    // 오브젝트가 활성화 되었을 때 호출되는 콜백함수
    // Awake - OnEnable - Start - Update - FixedUpdate
    private void OnEnable()
    {
        StartCoroutine(CheckState());
        StartCoroutine(MonsterAction());
    }

    // 오브젝트가 비활성화 되었을 때 호출되는 콜백함수
    private void OnDisable()
    {
       
    }

    IEnumerator CheckState()
    {
        while(!isDie)
        {
            // 두 대상의 거리
            float dist = (playerTr.position - monsterTr.position).magnitude;
            if(dist <= attackDistance)
            {
                state = State.ATTACK;
            }
            else if(dist <= traceDistance)
            {
                state = State.TRACE;
            }
            else 
            {
                state = State.PATROL;
            }
            yield return ws;    // 1초에 5번 호출
        }
        yield return ws;
    }

    IEnumerator MonsterAction()
    {
        while (isDie == false)
        {
            yield return ws;
            switch (state)
            {
                case State.PATROL:
                    moveAgent.patrolling = true;
                    animator.SetBool(hashMove, true);
                    break;
                case State.TRACE:
                    moveAgent.traceTarget = playerTr.position;
                    animator.SetBool(hashMove, true);
                    break;
                case State.ATTACK:
                    monsterTr.LookAt(playerTr);         // 플레이어를 바라보며 공격
                    moveAgent.Stop();
                    animator.SetBool(hashMove, false);
                    animator.SetBool(hashAttack, true);
                    break;
                case State.DIE:
                    moveAgent.Stop();
                    animator.SetBool(hashMove, false);
                    break;
            }
        }        
    }

    private void Update()
    {
        animator.SetFloat(hashSpeed, moveAgent.speed);
    }
}
