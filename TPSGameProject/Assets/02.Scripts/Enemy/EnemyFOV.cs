using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFOV : MonoBehaviour
{
    // 적 캐릭터의 추적 사정거리의 범위
    public float viewRange = 15f;
    [Range(0, 360)]
    public float viewAngle = 120f;     // 시야각

    Transform enemyTr;
    Transform playerTr;
    int playerLayer;
    int obstacleLayer;
    int layerMask;

    void Start()
    {
        enemyTr = GetComponent<Transform>();
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        playerLayer = LayerMask.NameToLayer("Player");
        obstacleLayer = LayerMask.NameToLayer("Obstacle");
        layerMask = 1 << playerLayer | 1 << obstacleLayer;
    }

    public Vector3 CirclePoint(float angle) // 주어진 각도에 의해 원주 위의 점이 좌표값을 계산하는 함수
    {
        // 로컬 좌표계를 기준으로 설정하기 위한 적캐릭터의 y회전 값을 더함
        angle = transform.eulerAngles.y;
        return new Vector3(Mathf.Sign(angle * Mathf.Rad2Deg), 0,Mathf.Cos(angle * Mathf.Deg2Rad));
        // Mathf.Rad2Deg π / 180값을 가진다
        // 일반 각도에 Mathf.Deg2Rad 를 곱하면 라디안으로 변환한 값을 구할 수 있다.
        // 원주의 점의 3차원 좌표는 (sin, 0, cos)으로 계산할 수 있다.
    }
    public bool isTracePlayer()      // 플레이어 추적 여부 함수
    {
        bool isTrace = false;
        // 120도 시야각에 플레이어가 있는지 확인
        Collider[] cols = Physics.OverlapSphere(enemyTr.position, viewRange, 1 << playerLayer);
        if(cols.Length == 1)    // 플레이어가 범위안에 있다면
        {
            // 에너미와 플레이어 사이의 방향 벡터를 계산
            Vector3 dir = (playerTr.position - enemyTr.position).normalized;
            // 에너미 시야각에 들어왔는지 판단
            if(Vector3.Angle(enemyTr.forward, dir) < viewAngle * 0.5f)
            {
                isTrace = true;
            }
        }
        return isTrace;
    }
    public bool isViewPlayer()  // 플레이 발견 여부 함수
    {
        bool isView = false;

        RaycastHit hit;
        Vector3 dir = (playerTr.position - enemyTr.position).normalized;
        // 캐릭터가 시야에 보였다면
        if(Physics.Raycast(enemyTr.position, dir, out hit, viewRange, layerMask))
        {
            isView = (hit.collider.CompareTag("Player"));
        }

        return isView;
    }


}
