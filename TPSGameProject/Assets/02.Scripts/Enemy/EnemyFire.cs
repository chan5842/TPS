using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFire : MonoBehaviour
{
    //public GameObject bullet;
    public Transform FirePos;
    AudioSource source;
    [SerializeField]
    AudioClip fireSound;
    Animator animator;
    [SerializeField]
    Transform playerTr;
    Transform enemyTr;
    public MeshRenderer renderer;           // 발사 효과 메시렌더러

    readonly int hashFire = Animator.StringToHash("Fire");
    readonly int hashReload = Animator.StringToHash("Reload");
    float nextFire = 0f;                // 발사 시간 계산용 변수
    readonly float fireRate = 0.1f;     // 총알 발사 간격
    readonly float damping = 10f;       // 플레이어를 향해 회전
    public bool isFire;                 // 총 쏘는 상태인지 확인

    // 재장전 관련 변수
    readonly float reloadTime = 2f;     // 재장전 시간
    readonly int maxBullet = 10;        // 탄창에 있는 총알 개수
    int CurrentBullet = 10;             // 현재 장전되어 있는 총알 개수
    bool isReload;                      // 재장전 상태인지 확인
    WaitForSeconds wsReload;            // 재장전 동안 기다릴 변수
    [SerializeField]
    AudioClip reloadSfx;                // 재장전 효과음

    void Start()
    {
        //bullet = Resources.Load("E_Bullet") as GameObject;
        source = GetComponent<AudioSource>();
        fireSound = Resources.Load("Sounds/p_ak_1") as AudioClip;
        animator = GetComponent<Animator>();
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTr = player.GetComponent<Transform>();
        enemyTr = GetComponent<Transform>();
        wsReload = new WaitForSeconds(reloadTime);
        reloadSfx = Resources.Load("Sounds/p_reload 1") as AudioClip;
        renderer.enabled = false;
    }

    
    void Update()
    {
        if(isFire && !isReload)
        {
            if(Time.time >= nextFire)
            {
                Fire();
                nextFire = Time.time + Random.Range(0.1f, 0.3f) + fireRate;
            }
            // 캐릭터를 향해 자연스럽게 회전
            Quaternion rot = Quaternion.LookRotation(playerTr.position - enemyTr.position);
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);
        }
    }

    void Fire()
    {
        animator.SetTrigger(hashFire);
        source.PlayOneShot(fireSound,1f);
        StartCoroutine(ShowMuzzleFlash());
        isReload = (--CurrentBullet % maxBullet == 0);
        // 재장전 상태가 되면 Reloding 코루틴 시작
        if(isReload)
        {
            StartCoroutine(Reloading());
        }
        //Instantiate(bullet, FirePos.position, FirePos.rotation);
        GameObject e_bullet = GameManager.instance.GetE_Bullet();
        if (e_bullet != null)
        {
            e_bullet.transform.position = FirePos.position;
            e_bullet.transform.rotation = FirePos.rotation;
            e_bullet.SetActive(true);
        }
    }

    IEnumerator ShowMuzzleFlash()
    {
        renderer.enabled = true;
        float _scale = Random.Range(1f, 2f);                        // 크기 값 랜덤
        renderer.transform.localScale = Vector3.one * _scale;       // 크기를 랜덤하게 변경
        Quaternion _rot = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        renderer.transform.localRotation = _rot;
        yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));
        renderer.enabled = false;
    }

    IEnumerator Reloading()
    {
        animator.SetTrigger(hashReload);
        source.PlayOneShot(reloadSfx, 1f);
        yield return wsReload;

        CurrentBullet = maxBullet;
        isReload = false;
    }
}
