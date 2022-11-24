using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RayserBeam : MonoBehaviour
{
    Transform tr;
    LineRenderer line;
    RaycastHit hit;

    void Start()
    {
        tr = this.transform;
        line = GetComponent<LineRenderer>();

        line.useWorldSpace = false;
        line.enabled = false;
        //line.SetWidth(0.3f, 0.01f);
    }

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        // 광선 동적할당
        Ray ray = new Ray(tr.position + (Vector3.up * 0.02f), tr.forward);
        //Debug.DrawRay(ray.origin, ray.direction * 20f, Color.green);
        
        if(Input.GetButtonDown("Fire1"))
        {
            // LineRenderer의 첫번째 점의 위치(월드 좌표를 로컬좌표로 변환)
            line.SetPosition(0, tr.InverseTransformPoint(ray.origin));
            // 어떤 물체에 맞았을 때 위치를 LineRenderer 끝점으로 설정
            if(Physics.Raycast(ray, out hit, 100f))
            {
                line.SetPosition(1, tr.InverseTransformPoint(hit.point));
            }
            // 맞지 않았을 때 끝점을 현재 위치에서 100f 뒤로 설정
            else
            {
                line.SetPosition(1, tr.InverseTransformPoint(ray.GetPoint(100f)));
            }
            StartCoroutine(ShowLaserBeam());
        }
    }
    IEnumerator ShowLaserBeam()
    {
        line.enabled = true;
        yield return new WaitForSeconds(Random.Range(0.01f, 0.2f));
        line.enabled = false;
    }
}
