using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    Rigidbody rb;
    Transform tr;
    TrailRenderer trail;
    public float Speed = 1500f;             // 총알 속도
    public float damage = 0f; //34f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        tr = GetComponent<Transform>();
        trail = GetComponent<TrailRenderer>();
        damage = GameManager.instance.gameData.damage;
    }

    private void OnEnable()
    {
        rb.AddForce(tr.forward * Speed);
        Invoke("BulletDeActive", 3f);
    }
    void BulletDeActive()
    {
        gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        rb.mass = 1;
        CancelInvoke("BulletDeActive");
        trail.Clear();  // 발사 효과 투명하게
        tr.position = Vector3.zero;
        tr.rotation = Quaternion.identity;
        rb.Sleep(); // Rigidbody 정지
    }
}
