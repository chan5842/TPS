using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]   // 클래스를 Inspector 하위에 보여지게 한다.
public class _Animation
{
    public AnimationClip idle;
    public AnimationClip Forward;
    public AnimationClip Backward;
    public AnimationClip Right;
    public AnimationClip Left;
    public AnimationClip Sprint;
    public AnimationClip RunJump;
    public AnimationClip idleJump;
    public AnimationClip Kick;
    public AnimationClip Slash;
}
public class MariaCtrl : MonoBehaviour
{
    float h, v, r;          // 수평, 수직, 회전
    public float moveSpeed = 4.5f;
    public float rotSpeed = 85f;

    [SerializeField]
    Transform tr;           // Transform 정보

    public _Animation anim;
    [SerializeField]
    Animation _anim;

    void Start()
    {
        tr = GetComponent<Transform>();
        _anim = GetComponent<Animation>();

        _anim.Play(anim.idle.name);
    }

    void Update()
    {
        Move();
        Jump();
        Attack();
    }

    private void Move()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        r = Input.GetAxis("Mouse X");

        Vector3 moveDir = (h * Vector3.right) + (v * Vector3.forward);

        tr.Translate(moveDir.normalized * Time.deltaTime * moveSpeed);
        tr.Rotate(Vector3.up * rotSpeed * Time.deltaTime * r);
        UpdateAnim();
        Sprint();
    }

    private void UpdateAnim()
    {
        // 애니메이션 연동
        if (h > 0.1f)
            _anim.CrossFade(anim.Right.name, 0.3f);
        else if (h < -0.1f)
            _anim.CrossFade(anim.Left.name, 0.3f);
        else if (v > 0.1f)
            _anim.CrossFade(anim.Forward.name, 0.3f);
        else if (v < -0.1f)
            _anim.CrossFade(anim.Backward.name, 0.3f);
        else
            _anim.CrossFade(anim.idle.name, 0.3f);            
    }

    void Jump()
    {
        #region idle jump
        // 가만히 있는 경우의 점프
        if (Input.GetKey(KeyCode.Space) && h == 0 && v == 0)
        {
            _anim.Play(anim.idleJump.name);
        }
        #endregion
        #region move jump
        // 앞으로 이동할때만 점프
        if(Input.GetKey(KeyCode.Space) && v > 0.1f)
        {
            _anim.Play(anim.RunJump.name);
        }
        #endregion
    }

    void Sprint()
    {
        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
        {
            moveSpeed = 7.5f;
            _anim.CrossFade(anim.Sprint.name, 0.3f);
        }
        else if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            moveSpeed = 3.5f;
        }
    }

    void Attack()
    {
        #region Slash
        // 마우스 왼쪽 클릭
        if (Input.GetMouseButton(0) && v == 0 && h == 0)
        {
            _anim.Play(anim.Slash.name);
        }
        #endregion
        #region Kick
        // 움직이지 않을 때 마우스 오른쪽 클릭
        if (Input.GetMouseButton(1) && v == 0&& h == 0)
        {
            _anim.Play(anim.Kick.name);
        }
        #endregion
    }
}
