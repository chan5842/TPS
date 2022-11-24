using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPoint : MonoBehaviour
{
    public Color lineColor;         // 그려줄 라인의 색
    [SerializeField]
    Transform[] lineTrs;            // 선 트랜스폼 배열
    public List<Transform> Nodes;   // 트랜스폼 노드 리스트

    // 기즈모(좌표)에 색상이나 선을 그려주는 콜백 함수
    private void OnDrawGizmos() 
    {
        Gizmos.color = lineColor;
        lineTrs = GetComponentsInChildren<Transform>();
        Nodes = new List<Transform>();      // 동적 할당
        for (int i = 0; i < lineTrs.Length; i++)
        {
            // 자기 자신 오브젝트가 아니라면 리스트에 담는다
            if (lineTrs[i] != this.transform)
            {
                Nodes.Add(lineTrs[i]);
            }
        }
        for (int i = 0; i < Nodes.Count; i++)
        {
            Vector3 CurrentNode = Nodes[i].position;    // 현재 노드 위치
            Vector3 PreviousNode = Vector3.zero;        // 이전 노드 위치

            if(i>0)
            {
                PreviousNode = Nodes[i-1].position;
            }
            if(i==0 && Nodes.Count>1)
            {
                // 한바퀴 돌앗거나, 0이면 이전 노드는 0번째 노드
                PreviousNode = Nodes[Nodes.Count - 1].position;
            }
            // 현재 노드에 구체를 그려줌
            Gizmos.DrawSphere(CurrentNode, 0.5f);
            // 이전 노드에서 현재 노드로 선을 그려줌
            Gizmos.DrawLine(PreviousNode, CurrentNode);
        }
    }
}
