using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MutantDamage : MonoBehaviour
{
    Animator animator;
    NavMeshAgent agent;
    [SerializeField]
    GameObject bloodEffect;
    [SerializeField]
    int hp = 0;    
    int hpMax = 100;
    public bool isDie = false;
    public Image hpBar;
    public Canvas hpCanvas;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        bloodEffect = (GameObject)Resources.Load("BloodSprayEffect");
        hp = hpMax;
        hpBar.color = Color.green;
    }

    private void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.CompareTag("BULLET"))
        {
            HitAniEffect(col);
            hp -= 25;
            hpBar.fillAmount = (float)hp / (float)hpMax;

            if (hpBar.fillAmount <= 0.5f)
                hpBar.color = Color.yellow;
            if (hpBar.fillAmount <= 0.25f)
                hpBar.color = Color.red;

            if (hp <= 0)
            {
                hp = 0;
                Die();
            }
                
        }
    }

    void HitAniEffect(Collision col)
    {
        Destroy(col.gameObject);
        animator.SetTrigger("doHit");
        agent.isStopped = true;

        // 충돌 지점을 좌표로 추출
        ContactPoint contact = col.contacts[0];
        // 백터가 이루는 회전 각도를 추출
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, contact.normal);

        GameObject blood = Instantiate(bloodEffect,
            contact.point, rot);
        Destroy(blood, 0.5f);
    }

    void Die()
    {
        Debug.Log("사망");
        animator.SetTrigger("doDie");
        GetComponent<CapsuleCollider>().enabled = false;
        isDie = true;
        hpCanvas.enabled = false;
        Destroy(gameObject, 3f);

        //G_Manager.g_Manager.KillCount(1);
    }

}
