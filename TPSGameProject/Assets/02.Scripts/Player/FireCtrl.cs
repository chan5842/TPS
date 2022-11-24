using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable] // 아래의 구조체가 인스펙터 컴포넌트에 보여지게 됨
public struct PlayerSfx
{
    // 총기류 마다 소리가 다르므로
    public AudioClip[] fire;
    public AudioClip[] reload;
}

public class FireCtrl : MonoBehaviour
{
    public enum WeaponType
    {
        RIFLE = 0,
        SHOTGUN
    }
    public WeaponType curWeapon = WeaponType.RIFLE;

    public PlayerSfx playerSfx;

    Animation anim;
    PlayerCtrl playerCtrl;
    // 발사 위치
    [SerializeField]
    Transform firePos;
    [SerializeField]
    // 탄피 파티클
    ParticleSystem CartridgeEjectEffect;
    [SerializeField]
    // 발사 효과 파티클
    ParticleSystem MuzzleFlashEffect;

    AudioSource source;
    //[SerializeField]
    //AudioClip fireSound;

    [Header("MegazineUI")]
    // 탄창 텍스트
    [SerializeField]
    Text MegaText;
    // 탄창 이미지
    [SerializeField]
    Image MegaImage;
    // 재장전 사운드
    //[SerializeField]
    //AudioClip reloadSound;
    public int maxBullet = 10;          // 최대 총알수
    public int remainingBullet = 10;    // 남은 총알수
    public float reloadTime = 2f;       // 재장전 시간
    bool isReloding;                    // 재장전 여부

    readonly string enemyTag = "Enemy";
    readonly string barrelTag = "BARREL";
    readonly string wallTag = "WALL";

    public Sprite[] weaponIcons;
    public Image weaponImage;

    public float damage;        // 공격력

    [Header("Raycast Auto Fire")]
    [SerializeField]
    int enemyLayer;     // 에너미 레이어
    [SerializeField]
    int obstacleLayer;  // 장애물 레이어
    [SerializeField]
    int layerMask;
    [SerializeField]
    bool isFire;        // 발사중인 상태 확인
    [SerializeField]
    float nextFire;     // 다음 발사 시간을 저장 할 변수
    [SerializeField]
    public float fireRate = 0.1f;       // 총알의 발사 간격

    void Awake()
    {
        firePos = GameObject.Find("Rweaponholders").transform.
            GetChild(0).GetChild(0).GetComponent<Transform>();
        #region 최적화
        //firePos = gameObject.transform.GetChild(0).GetChild(2).
        //GetChild(0).GetChild(0).GetChild(2).
        //GetChild(0).GetChild(0).GetChild(0).GetChild(2).
        //GetChild(0).GetChild(0).GetComponent<Transform>();
        #endregion

        CartridgeEjectEffect = GameObject.Find("Rweaponholders").transform.
            GetChild(0).GetChild(1).GetComponent<ParticleSystem>();

        MuzzleFlashEffect = GameObject.Find("Rweaponholders").transform.
            GetChild(0).GetChild(0).GetChild(0).GetComponent<ParticleSystem>();

        source = GetComponent<AudioSource>();
        //playerSfx.fire[(int)curWeapon] = Resources.Load("Sounds/p_ak_1") as AudioClip;
        //playerSfx.reload[(int)curWeapon] = Resources.Load("Sounds/p_ak_1") as AudioClip;

        MegaText = GameObject.Find("Canvas-UI").transform.GetChild(2).GetChild(0).GetComponent<Text>();
        MegaImage = GameObject.Find("Canvas-UI").transform.GetChild(2).GetChild(2).GetComponent<Image>();

        MegaImage.fillAmount = 1f;

        playerCtrl = GetComponent<PlayerCtrl>();
        anim = GetComponent<Animation>();

        enemyLayer = LayerMask.NameToLayer("Enemy");
        obstacleLayer = LayerMask.NameToLayer("Obstacle");
        layerMask = 1 << obstacleLayer | 1 << enemyLayer;   // 두 레이어 추출
    }

    private void OnEnable()
    {
        GameManager.OnItemChange += UpdateSetUp;
    }
    void UpdateSetUp()
    {
        damage = GameManager.instance.gameData.damage;
    }

    // Update is called once per frame
    void Update()
    {
        // 광선 디버그
        Debug.DrawRay(firePos.position, firePos.forward * 20, Color.red);

        // 현재 마우스 포인터가 어떤 오브젝트 위에 올려져있다면
        //if (MouseHover.instance.isUIHover) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;

        RaycastHit hit;
        if (Physics.Raycast(firePos.position, firePos.forward, out hit, 20f, layerMask))
        {
            isFire = (hit.collider.CompareTag(enemyTag));

        }
        else
            isFire = false;

        //if (!isReloding && Input.GetButtonDown("Fire1"))
        if(!isReloding && isFire)
        {
            if(Time.time > nextFire)
            {
                remainingBullet--;
                Fire();
                if (remainingBullet <= 0)
                {
                    StartCoroutine("Reloading");
                }
                // 0.1초 마다 발사
                nextFire = Time.time + fireRate;
            }            
        }
    }

    private void Fire()
    {
        if (playerCtrl.isSprint)
            return;

        #region 오브젝트 풀링(Projectile방식)

        ////Instantiate(bulletPrefab, firePos.position, firePos.rotation);
        //GameObject _bullet = GameManager.instance.GetBullet();
        //if (_bullet != null)
        //{
        //    _bullet.transform.position = firePos.position;
        //    _bullet.transform.rotation = firePos.rotation;
        //    _bullet.SetActive(true);
        //}
        #endregion

        #region Raycast 방식

        RaycastHit hit;

        // 광선을 발사해서 맞았다면
        if (Physics.Raycast(firePos.position, firePos.forward, out hit, 20f))
        {
            if(hit.collider.CompareTag(enemyTag))
            {
                object[] _params = new object[2];
                _params[0] = damage;    // 배열의 첫번째에 데미지 전달
                _params[1] = hit.point;     // 배열의 두번째에 맞은 위치 전달
                hit.collider.gameObject.SendMessage("OnDamage", _params,
                    SendMessageOptions.DontRequireReceiver);
            }
            if(hit.collider.CompareTag(barrelTag) || hit.collider.CompareTag(wallTag))
            {
                object[] _params = new object[2];
                _params[0] = hit.point;     // 배열의 두번째에 맞은 위치 전달
                _params[1] = firePos.position;  // 발사 위치 전달
                hit.collider.gameObject.SendMessage("OnDamage", _params,
                    SendMessageOptions.DontRequireReceiver);
            }
        }

        #endregion
        var _sfx = playerSfx.fire[(int)curWeapon];
        source.PlayOneShot(_sfx, 1f);
        //source.PlayOneShot(fireSound, 1f);
        CartridgeEjectEffect.Play();
        MuzzleFlashEffect.Play();

        // 탄창 UI
        MegaImage.fillAmount = (float)remainingBullet / maxBullet;
        UpdateBulletText();

    }

    IEnumerator Reloading()
    {
        anim.Play("IdleReloadShotgun");
        isReloding = true;
        var _reload = playerSfx.reload[(int)curWeapon];
        source.PlayOneShot(_reload, 1f);
        yield return new WaitForSeconds(_reload.length + 0.3f);
        //source.PlayOneShot(reloadSound, 1f);
        //yield return new WaitForSeconds(reloadSound.length + 0.3f);
        isReloding = false;
        MegaImage.fillAmount = 1f;
        remainingBullet = maxBullet;
        MegaImage.fillAmount = (float)remainingBullet / maxBullet;
        UpdateBulletText();
    }

    void UpdateBulletText()
    {
        MegaText.text = string.Format("<color=#ff0000>{0}</color>/{1}",
            remainingBullet, maxBullet);
    }

    public void OnChangeWeapon()
    {
        curWeapon = (WeaponType)((int)++curWeapon % 2);
        weaponImage.sprite = weaponIcons[(int)curWeapon];
    }
}
