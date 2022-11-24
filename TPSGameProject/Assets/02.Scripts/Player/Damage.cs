using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damage : MonoBehaviour
{
    // 태그를 미리 읽어옴
    readonly string bulletTag = "E_BULLET";

    [SerializeField]
    GameObject bloodEffect;
    [SerializeField]
    Image bloodScreen;
    [SerializeField]
    Image HPBar;
    Transform tr;
    [SerializeField]
    float hp = 0;
    float maxHp = 100;

    readonly Color initColor = new Vector4(0f, 1f, 0f, 1f);

    // 델리게이트 및 이벤트 선언
    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnPlayerDie;

    void Awake()
    {
        tr = GetComponent<Transform>();
        bloodScreen = GameObject.Find("Canvas-UI").transform.GetChild(0).GetComponent<Image>();
        HPBar = GameObject.Find("Canvas-UI").transform.GetChild(1).GetChild(1).GetComponent<Image>();
        bloodEffect = Resources.Load("BloodSprayEffect") as GameObject;
        hp = GameManager.instance.gameData.hp;
        //HPBar.color = Color.green;
        HPBar.color = initColor;

    }

    private void OnEnable()
    {
        GameManager.OnItemChange += UpdateSetUp;
    }

    void UpdateSetUp()
    {
        hp = GameManager.instance.gameData.hp;
        maxHp = GameManager.instance.gameData.hp - maxHp;
    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag(bulletTag))
        {
            //Destroy(col.gameObject);
            ShowBloodEffect(col);
            col.gameObject.SetActive(false);
            StartCoroutine("ShowBloodScreen");
            
            HpUIManager();

            if (hp <= 0)
                PlayerDie();
        }
    }

    private void HpUIManager()
    {
        hp -= 15f;
        hp = Mathf.Clamp(hp, 0f, maxHp);
        HPBar.fillAmount = hp / maxHp;
        if (HPBar.fillAmount <= 0.3f)
        {
            HPBar.color = Color.red;
        }
        else if (HPBar.fillAmount <= 0.6f)
        {
            HPBar.color = Color.yellow;
        }
    }

    IEnumerator ShowBloodScreen()
    {
        bloodScreen.enabled = true;
        bloodScreen.color = new Color(1, 0, 0, Random.Range(0.5f, 0.8f));
        yield return new WaitForSeconds(0.1f);
        bloodScreen.color = Color.clear;    // 모든 색상을 0으로 변경
    }

    void PlayerDie()
    {
        GameManager.instance.isGameOver = true;
        OnPlayerDie();
        Invoke("OnPaused", 3f);
        //GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        #region SendMessage : 대규모 게임에는 부적합
        //foreach (GameObject _enemy in enemies)
        //{
        //    // 에너미들에게 함수를 실행시키라고 메시지를 전달(오류 발생 X)
        //    _enemy.SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
            
        //}
        ////for(int i=0; i< enemies.Length; i++)
        ////{
        ////    enemies[i].SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
        ////}
        #endregion
    }

    void OnPaused()
    {
        Time.timeScale = 0f;
    }

    private void ShowBloodEffect(Collider col)
    {
        // ContactPoint의 contacts 사용 불가
        Vector3 pos = col.transform.position;
        Vector3 normal = col.transform.localEulerAngles.normalized;
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, normal);
        GameObject blood = Instantiate(bloodEffect, tr.position + new Vector3(0,1.5f,0), rot);
        Destroy(blood, Random.Range(0.5f, 0.8f));
    }
}
