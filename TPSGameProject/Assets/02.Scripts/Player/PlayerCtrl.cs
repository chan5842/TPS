using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    Animation anim;

    [SerializeField]
    float moveSpeed = 6f;
    public float turnSpeed = 80f;
    public float JumpForce = 5f;
    public bool isSprint;

    float h, v, r;

    private void OnEnable()
    {
        GameManager.OnItemChange += UpdateSetUp;
    }
    void UpdateSetUp()
    {
        moveSpeed = GameManager.instance.gameData.speed;
    }

    private void Start()
    {
        anim = GetComponent<Animation>();
        anim.Play("Idle");
        //moveSpeed = GameManager.instance.gameData.speed;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        Vector3 moveDir;

        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        r = Input.GetAxis("Mouse X");

        moveDir = Vector3.right * h + Vector3.forward * v;

        transform.Translate(moveDir.normalized * moveSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up * r * Time.deltaTime * turnSpeed);

        UpdateAnim();
        // 달리는 경우
        Sprint();
    }

    private void Sprint()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
        {
            isSprint = true;
            moveSpeed = 6.5f;
            anim.CrossFade("SprintF", 0.3f);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isSprint = false;
            moveSpeed = 3.5f;
        }
    }

    private void UpdateAnim()
    {
        // 애니메이션 연동(블렌드 애니메이션)
        //  CrossFade() : 부드러운 전환을 위해 사용
        if (h > 0.1f)
            anim.CrossFade("RunR", 0.3f);
        else if (h < -0.1f)
            anim.CrossFade("RunL", 0.3f);
        else if (v > 0.1f)
            anim.CrossFade("RunF", 0.3f);
        else if (v < -0.1f)
            anim.CrossFade("RunB", 0.3f);
        else
            anim.CrossFade("Idle", 0.3f);
    }
}
