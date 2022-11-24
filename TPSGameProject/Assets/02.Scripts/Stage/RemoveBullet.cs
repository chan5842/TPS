using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBullet : MonoBehaviour
{
    [SerializeField]
    GameObject Spark;
    readonly string bulletTag = "BULLET";
    readonly string bulletTag2 = "E_BULLET";
    // source;
    [SerializeField]
    AudioClip hitSound;

    void Start()
    {
        Spark = (GameObject)Resources.Load("FlareMobile");
        //source = GetComponent<AudioSource>();
        hitSound = (AudioClip)Resources.Load("Sounds/bullet_hit_metal_enemy_4");
    }

    private void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.CompareTag(bulletTag) || col.gameObject.CompareTag(bulletTag2))
        {
            //Destroy(col.gameObject);
            // 플레이어 총알은 오브젝트 풀링 상태이므로
            col.gameObject.SetActive(false);
            ShowEffect(col);
            SoundManager.soundManager.PlaySoundFunc(transform.position, hitSound);
            //source.PlayOneShot(hitSound);
        }

        //else if(col.gameObject.CompareTag(bulletTag2))
        //{
        //    //Destroy(col.gameObject);
        //    col.gameObject.SetActive(false);
        //    ShowEffect(col);
        //    source.PlayOneShot(hitSound);
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(bulletTag2))
        {
            other.gameObject.SetActive(false);
            ShowEffect(other);
            //source.PlayOneShot(hitSound);
            SoundManager.soundManager.PlaySoundFunc(transform.position, hitSound);
        }
    }
    private void ShowEffect(Collision col)
    {
        // 충돌 지점을 좌표로 추출
        ContactPoint contact = col.contacts[0];
        // 백터가 이루는 회전 각도를 추출
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, contact.normal);

        GameObject spk = Instantiate(Spark, contact.point, rot);
        Destroy(spk, 2f);
    }

    void OnDamage(object[] _params)
    {
        ShowEffect((Vector3)_params[0]);
        SoundManager.soundManager.PlaySoundFunc(transform.position, hitSound);
    }

    private void ShowEffect(Vector3 pos)
    {
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, pos.normalized);

        GameObject spk = Instantiate(Spark, pos, rot);
        Destroy(spk, 2f);
    }

    private void ShowEffect(Collider other)
    {
        Vector3 pos = other.transform.position;
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, pos);

        GameObject spk = Instantiate(Spark, pos, rot);
        Destroy(spk, 2f);
    }

}
