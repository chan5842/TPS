using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwatCtrl : MonoBehaviour
{
    Transform Tr;
    Animator animator;
    Rigidbody rb;

    float h, v, r;
    float moveSpeed = 3.5f;
    float turnSpeed = 85f;
    public float smoothBlend = 0.1f;            // 부드럽게 움직이기 위한 Time.deltaTime값과 곱할 값

    void Start()
    {
        Tr = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Move();
        Jump();
        Attack();
    }

    void Move()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        r = Input.GetAxis("Mouse X");


        Tr.Translate(h * Vector3.right.normalized * moveSpeed * Time.deltaTime);
        {
            animator.SetFloat("SpeedX", h, smoothBlend, Time.deltaTime);     // 블렌드 트리의 SpeedX값 변경
        }
        Tr.Translate(v * Vector3.forward.normalized * moveSpeed * Time.deltaTime);
        {
            animator.SetFloat("SpeedY", v, smoothBlend, Time.deltaTime);
        }
        Tr.Rotate(Vector3.up * r * Time.deltaTime * turnSpeed);

        Sprint();
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && h == 0 && v == 0)
        {
            animator.SetTrigger("doJump");
        }
                
        if (Input.GetKeyDown(KeyCode.Space) && v > 0f)
        {
            animator.SetTrigger("doRunJump"); 
        }
    }

    void Attack()
    {
        if(Input.GetButtonDown("Fire1") && h==0 && v==0)
        {
            animator.SetTrigger("doPunch");
        }
        if(Input.GetButtonDown("Fire2") && h == 0 && v == 0)
        {
            animator.SetTrigger("doKick");
        }
    }

    private void Sprint()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
        {
            moveSpeed = 7f;
            animator.SetBool("isSprint", true);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.W))
        {
            moveSpeed = 3.5f;
            animator.SetBool("isSprint", false);
        }
    }
}
