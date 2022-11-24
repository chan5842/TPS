using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILookAt : MonoBehaviour
{
    Transform tr;
    [SerializeField]
    Transform CamTr;

    void Awake()
    {
        tr = GetComponent<Transform>();
        CamTr = Camera.main.GetComponent<Transform>();
        //CamTr = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    void Update()
    {
        tr.LookAt(CamTr.position);
    }
}
