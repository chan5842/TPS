using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Animations
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

public class PaladinCtrl : MonoBehaviour
{
    float h, v, r;
    public float moveSpeed = 5f;
    public float rotSpeed = 90f;

    Transform tr;
    public Animations anims;
    Animation animation;
    void Start()
    {
        tr = GetComponent<Transform>();
        animation = GetComponent<Animation>();

        animation.Play(anims.idle.name);
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

        tr.Translate(moveDir.normalized * moveSpeed * Time.deltaTime);
        tr.Rotate(r * Vector3.up * rotSpeed * Time.deltaTime);

        UpdateAnim();
        Sprint();
    }

    private void UpdateAnim()
    {
        // 애니메이션 연동
        if (h > 0.1f)
            animation.CrossFade(anims.Right.name, 0.3f);
        else if (h < -0.1f)
            animation.CrossFade(anims.Left.name, 0.3f);
        else if (v > 0.1f)
            animation.CrossFade(anims.Forward.name, 0.3f);
        else if (v < -0.1f)
            animation.CrossFade(anims.Backward.name, 0.3f);
        else
            animation.CrossFade(anims.idle.name, 0.3f);
    }

    void Jump()
    {
        #region idle jump
        if (Input.GetKey(KeyCode.Space) && h == 0 && v == 0)
            animation.Play(anims.idleJump.name);
        #endregion
        #region run jump
        if (Input.GetKey(KeyCode.Space) && v > 0.1f)
            animation.Play(anims.RunJump.name);
        #endregion
    }
    
    void Sprint()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
        {
            moveSpeed = 8.5f;
            animation.CrossFade(anims.Sprint.name, 0.3f);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            moveSpeed = 5f;
        }
    }

    void Attack()
    {
        #region Slash
        if (Input.GetButton("Fire1") && v == 0 && h == 0)
            animation.Play(anims.Slash.name);
        #endregion
        #region Kick
        if (Input.GetButton("Fire2") && v == 0 && h == 0)
            animation.Play(anims.Kick.name);
        #endregion
    }
}
