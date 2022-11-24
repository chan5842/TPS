using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// 이 오브젝트에는 NavMeshAgent 컴포넌트가 없으면
// 안되며, 없을 시 경고를 보냄(속성)
[RequireComponent(typeof(NavMeshAgent))]

public class MoveAgent : MonoBehaviour
{
    public List<Transform> wayPoints;   // 도착 지점 리스트
    public int nextIdx = 0;             // 배열 인덱스 값
    [SerializeField]
    NavMeshAgent agent;
    [SerializeField]
    Animator animator;

    readonly float patrolSpeed = 1.5f;
    readonly float traceSpeed = 5.0f;
    private bool _patrolling;           // 순찰 상태인지 확인

    public bool patrolling              // 프로퍼티
    {
        get { return _patrolling; }     // 읽기만 가능 수정은 불가능
        set { 
            _patrolling = value;
            agent.speed = patrolSpeed;
            MoveWayPoints();
        }    // 쓰기(수정) 가능
    }

    // 추적 대상의 위치를 저장하는 변수
    Vector3 _traceTarget;
    public Vector3 traceTarget
    {
        get { return _traceTarget; }
        set
        {
            _traceTarget = value;
            agent.speed = traceSpeed;
            TraceTarget(_traceTarget);
        }
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.autoBraking = false;      // 옵션 비활성화
        var group = GameObject.Find("PatrolPoint");
        // 유효성 검사
        if(group!=null)
        {
            // PatrolPoint의 하위 개체 Transform 정보를 wayPoints 리스트에 담음
            group.GetComponentsInChildren<Transform>(wayPoints);
            wayPoints.RemoveAt(0);  // PatrolPoint 정보는 제거
        }
        MoveWayPoints();

        animator.SetBool("IsMove", false);
    }

    // 목표지점으로 이동하는 함수
    void MoveWayPoints()
    {
        // 경로계산이 안되거나 최단 경로가 잡히지 않으면 함수 종료
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

    // NavMeshAgent의 이동 속도에 대한 프로퍼티 정의
    public float speed
    {
        get { return agent.velocity.magnitude; }
    }


    // 순찰 멈춤
    public void Stop()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        _patrolling = false;
    }

    void Update()
    {
        // 순찰 상태가 아니라면 벗어난다.
        if (!_patrolling)
            return;
        // 목표지점 까지 남은거리가 0.5미만인 경우(도착 햇다면)
        if (agent.remainingDistance<= 0.5f)
        {
            nextIdx = ++nextIdx % wayPoints.Count;
            MoveWayPoints();
        }
    }
}
