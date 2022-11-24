using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDamage : MonoBehaviour
{
    //readonly string bulletTag = "BULLET";

    public float maxHp = 100f;
    public float hp = 0f;
    [SerializeField]
    GameObject bloodEffect;

    EnemyAI enemyAI;

    public Image hpBar;
    public Text hpText;

    void Start()
    {
        enemyAI = GetComponent<EnemyAI>();
        hp = maxHp;
        bloodEffect = Resources.Load("BloodSprayEffect") as GameObject;
        hpBar.color = Color.green;
        hpText.text = "HP: " + hp.ToString();
    }

    private void OnEnable()
    {
        hp = maxHp;
        hpBar.fillAmount = 1f;
        hpBar.color = Color.green;
        hpText.text = "HP: " + ((int)hp).ToString();
    }
    private void OnDisable()
    {
    }

    #region 오브젝트 풀링 방식
    //private void OnCollisionEnter(Collision col)
    //{
    //    if (col.collider.CompareTag(bulletTag))
    //    {
    //        // 총알 삭제
    //        //Destroy(col.gameObject);

    //        ShowBloodEffect(col);
    //        col.gameObject.SetActive(false);
    //        hp -= col.gameObject.GetComponent<BulletCtrl>().damage;
    //        hpBar.fillAmount = (float)hp / (float)maxHp;
    //        hpText.text = "HP: " + hp.ToString();
    //        if (hpBar.fillAmount <= 0.3f)
    //            hpBar.color = Color.red;
    //        else if (hpBar.fillAmount <= 0.5f)
    //            hpBar.color = Color.yellow;

    //        // 체력이 0이면 Enemy의 상태를 사망 상태로 변경
    //        if (hp<=0)
    //        {
    //            //enemyAI.state = EnemyAI.State.DIE;
    //            enemyAI.Die();
    //        }
    //    }
    //}
    #endregion

    void OnDamage(object[] _params)
    {
        Vector3 pos = (Vector3)_params[1];
        ShowBloodEffect(pos);
        hp -= (float)_params[0];
        hpBar.fillAmount = (float)hp / (float)maxHp;
        hpText.text = "HP: " + hp.ToString();
        if (hpBar.fillAmount <= 0.3f)
            hpBar.color = Color.red;
        else if (hpBar.fillAmount <= 0.5f)
            hpBar.color = Color.yellow;

        // 체력이 0이면 Enemy의 상태를 사망 상태로 변경
        if (hp <= 0)
        {
            //enemyAI.state = EnemyAI.State.DIE;
            enemyAI.Die();
            
        }
    }
    void ShowBloodEffect(Vector3 pos)
    {
        Quaternion rot = Quaternion.LookRotation(-pos.normalized);  // 바라보는 반대쪽으로 혈흔 발생

        var hitEffect = Instantiate(bloodEffect, pos, rot);
        Destroy(hitEffect.gameObject, 1f);
    }

    //private void ShowBloodEffect(Collision col)
    //{
    //    // 피격 위치에 혈흔 효과 
    //    ContactPoint contact = col.contacts[0]; // 피격 위치 정보 저장
    //    Quaternion rot = Quaternion.LookRotation(-contact.normal);  // 바라보는 반대쪽으로 혈흔 발생
    //    var hitEffect = Instantiate<GameObject>(bloodEffect, contact.point, rot);

    //    Destroy(hitEffect.gameObject, 1f);
    //}
}
    