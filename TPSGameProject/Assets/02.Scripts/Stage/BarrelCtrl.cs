using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour
{
    [SerializeField]
    Texture[] textures;
    MeshRenderer renderer;
    [SerializeField]
    GameObject explosionEffect;
    //AudioSource source;
    [SerializeField]
    AudioClip expSound;
   // CameraShake cameraShake;
    CameraShake2D cameraShake2D;

    public int hitCount = 0;        // 3번 맞으면 터짐

    //string bulletTag = "BULLET";
    public delegate void EnemyDieHandler();
    public static event EnemyDieHandler OnEnemyDie;

    void Start()
    {
        // 배럴 색 랜덤하게 입혀주기
        renderer = GetComponent<MeshRenderer>();
        textures = Resources.LoadAll<Texture>("BarrelTexture");
        explosionEffect = Resources.Load("Effects/BigExplosionEffect") as GameObject;
        //source = GetComponent<AudioSource>();
        expSound = Resources.Load("Sounds/missile_explosion") as AudioClip;
        //cameraShake = Camera.main.GetComponent<CameraShake>();
        cameraShake2D = Camera.main.GetComponent<CameraShake2D>();

        int idx = Random.Range(0, textures.Length);
        renderer.material.mainTexture = textures[idx];       
    }

    private void OnCollisionEnter(Collision col)
    {
        if(col.collider.CompareTag("BULLET"))
        {
            if (++hitCount == 3)
            {
                BarrelExplosion();
            }
        }
    }
    void OnDamage(object[] _params)
    {
        Vector3 hitPos = (Vector3)_params[0];
        Vector3 firePos = (Vector3)_params[1];

        // 입사각(벡터) 구하기
        Vector3 incomeVector = hitPos - firePos;
        // 입사 벡터를 정규화 벡터로 변경
        incomeVector = incomeVector.normalized;
        GetComponent<Rigidbody>().AddForceAtPosition(incomeVector * 5000f, hitPos);

        if (++hitCount == 3)
        {
            BarrelExplosion();
        }
    }


    void BarrelExplosion()
    {
        //Destroy(gameObject);
        GameObject Effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(Effect, 1f);
        //source.PlayOneShot(expSound, 1f);
        SoundManager.soundManager.PlaySoundFunc(transform.position, expSound);

        // 폭파시 위로 튀어오르게 만드는 작업
        // 거리가 20 안에 있는 오브젝트에서 충돌체가 있으면 Cols에 대입
        Collider[] Cols = Physics.OverlapSphere(transform.position, 20f);

        foreach(Collider col in Cols)
        {
            Rigidbody rb = col.GetComponent<Rigidbody>();
            // 충돌체에 Rigidbody가 존재한다면
            if(rb != null)
            {
                // 플레이어를 제외한 오브젝트에만 영향
                if(col.gameObject.tag != "Player")
                {
                    rb.mass = 1f;       // 무게를 가볍게
                    // 폭파하는 힘을 가해줌(폭파력, 위치, 반경, 솟구치는 힘
                    rb.AddExplosionForce(1000f, transform.position, 20f, 1000f);
                    // 충돌체에 있는 함수를 호출 할 수 있는 함수
                    // SendMessageOptions.DontRequireReceiver : 함수가 없거나 오타가 있어도 오류를 내지 않는 옵션
                    //col.gameObject.SendMessage("expDie", SendMessageOptions.DontRequireReceiver);
                    OnEnemyDie();
                }
            }
        }
        //StartCoroutine(cameraShake.ShakeCamera());
        cameraShake2D.TurnOn();

        Invoke("BareelNormalMass", 3f);
        Destroy(gameObject, 3f);
    }
    void BareelNormalMass()
    {
        Collider[] Cols = Physics.OverlapSphere(transform.position, 20f);

        foreach (Collider col in Cols)
        {
            Rigidbody rb = col.GetComponent<Rigidbody>();
            // 충돌체에 Rigidbody가 존재한다면
            if (rb != null)
            {
                // 플레이어를 제외한 오브젝트에만 영향
                if (col.gameObject.tag != "Player")
                {
                    rb.mass = 100f;       // 무게를 가볍게
                }
            }
        }
    }
}
