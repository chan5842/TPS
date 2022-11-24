using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MutantCtrl : MonoBehaviour
{
    Animator animator;
    NavMeshAgent agent;
    [SerializeField]
    Transform PlayerTr;
    Transform MutantTr;
    MutantDamage m_damage;
    
    public float traceDist = 15f;
    public float attackDist = 3f;


    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        PlayerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        MutantTr = GetComponent<Transform>();
        m_damage = GetComponent<MutantDamage>();
    }

    void Update()
    {
        float dist = Vector3.Distance(PlayerTr.position, MutantTr.position);

        if (!m_damage.isDie)
        {
            if (dist <= attackDist)
            {
                animator.SetBool("isAttack", true);
                agent.isStopped = true;
            }
            else if (dist <= traceDist)
            {
                agent.isStopped = false;
                agent.destination = PlayerTr.position;
                animator.SetBool("isAttack", false);
                animator.SetBool("isTrace", true);
            }
            else
            {
                agent.isStopped = true;
                animator.SetBool("isTrace", false);
            }
        }
    }
}
