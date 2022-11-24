using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    Transform CanvasTr;
    Transform CamTr;
    void Start()
    {
        CanvasTr = GetComponent<Transform>();
        CamTr = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        CanvasTr.LookAt(CamTr);
    }
}
