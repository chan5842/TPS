using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class EnemyAgent : MonoBehaviour
{
    public List<Transform> wayPoints;
    public int nextIdx = 0;

    readonly float patrolSpeed2 = 1.5f;
    readonly float traceSpeed2 = 5f;
    // 회전 할때 속도를 조절하는 계수(부드럽게 하는 용도)
    float damping = 1.0f;
    bool isPatrol;

    [SerializeField]
    NavMeshAgent agent;
    [SerializeField]
    Animator animator;
    [SerializeField]
    Transform enemyTr;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        enemyTr = GetComponent<Transform>();
        agent.autoBraking = false;
        agent.updateRotation = false;
        
        

        var group = GameObject.Find("PatrolPoint");

        if (group != null)
        {
            // PatrolPoint의 하위 개체 Transform 정보를 wayPoints 리스트에 담음
            group.GetComponentsInChildren<Transform>(wayPoints);
            wayPoints.RemoveAt(0);  // PatrolPoint 정보는 제거
        }

        nextIdx = Random.Range(0, wayPoints.Count);
        //MoveWayPoints();

        animator.SetBool("IsMove", false);
    }

    public bool patrolling
    {
        get { return isPatrol; }
        set
        {
            isPatrol = value;
            if (isPatrol)
            {
                agent.speed = patrolSpeed2;
                damping = 1.0f;
                MoveWayPoints();
            }
        }
    }

    Vector3 _traceTarget;
    public Vector3 traceTarget
    {
        get { return _traceTarget; }
        set
        {
            _traceTarget = value;
            agent.speed = traceSpeed2;
            damping = 7.0f;
            TraceTarget(_traceTarget);
        }
    }


    void MoveWayPoints()
    {
        if (agent.isPathStale) return;

        // 목표 경로 설정
        agent.destination = wayPoints[nextIdx].position;
        agent.isStopped = false;

        animator.SetBool("IsMove", true);
        
    }

    void TraceTarget(Vector3 pos)
    {
        if (agent.isPathStale) return;  // 최단 경로가 아니면 빠져나감
        agent.destination = pos;
        agent.isStopped = false;
    }

    public float speed
    {
        get { return agent.velocity.magnitude; }
    }

    public void Stop()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        isPatrol = false;
    }
    void Update()
    {
        // 순찰 중이라면
        if(!agent.isStopped)
        {
            // NavMeshAgent가 가야할 방향 벡터를 Quaternion 타입의 각도로 변경
            Quaternion rot = Quaternion.LookRotation(agent.desiredVelocity);
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);
        }
        // 순찰 상태가 아니라면 벗어난다.
        if (!isPatrol)
            return;
        // 목표지점 까지 남은거리가 0.5미만인 경우(도착 햇다면)
        if (agent.remainingDistance <= 0.5f)
        {
            nextIdx = ++nextIdx % wayPoints.Count;
            MoveWayPoints();
        }
    }
}
