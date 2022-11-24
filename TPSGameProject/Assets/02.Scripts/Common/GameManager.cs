using System.Collections;   // 배열 사용 가능
using System.Collections.Generic;   // List 사용 가능
using UnityEngine;
using DataInfo;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField]
    GameObject enemy;
    // 총알 프리펩
    //GameObject bulletPrefab;
    // 에너미 총알 프리펩
    GameObject e_bulletPrefab;
    [SerializeField]
    Transform[] points;
    // 스폰되는 에너미를 저장할 리스트
    public List<GameObject> enemyPool = new List<GameObject>();
    public List<GameObject> bulletPool = new List<GameObject>();
    public List<GameObject> e_bulletPool = new List<GameObject>();
   
    public bool isGameOver = false;
    public int maxCount = 10;
    float CreateTime = 3f;

    public CanvasGroup inventoryCG;

    [Header("GameData")]
    public Text killCountTxt;
    public DataManager dataManager;
    //public GameData gameData;
    public GameDataObject gameData;

    // 인벤토리 아이템이 변경 되었을 때 발생 시킬 이벤트
    public delegate void ItemChangeDelegate();
    public static event ItemChangeDelegate OnItemChange;

    // SlotList 게임 오브젝트에 저장 할 변수
    GameObject slotList;
    // ItemList 하위에 있는 4개의 아이템을 저장할 배열
    public GameObject[] itemObjects;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject);

        dataManager = GetComponent<DataManager>();
        dataManager.Initialize();

        // Resources 폴더에 있는 프리펩 정보를 가져옴
        //bulletPrefab = (GameObject)Resources.Load("Bullet");
        enemy = Resources.Load<GameObject>("Enemy");
        e_bulletPrefab = (GameObject)Resources.Load("E_Bullet");
        points = GameObject.Find("SpawnPoints").GetComponentsInChildren<Transform>();
        // 인벤토리에 추가된 아이템을 검색하기 위해 SlotList를 추출
        slotList = inventoryCG.transform.Find("SlotList").gameObject;
        
        LoadGameData();
    }
    void LoadGameData()
    {
        //GameData data = dataManager.Load();
        //gameData.hp = data.hp;
        //gameData.damage = data.damage;
        //gameData.speed = data.speed;
        //gameData.killCount = data.killCount;
        //gameData.equipItem = data.equipItem;

        // 보유한 아이템이 있는 경우에만 호출
        if (gameData.equipItem.Count > 0)
        {
            InventorySetup();
        }

        killCountTxt.text = "KILL: " + "<color=#ff0000>" + gameData.killCount.ToString("0000") + "</color>";
    }

    // 로드한 데이터를 기준으로 인벤토리 아이템을 추가하는 함수
    void InventorySetup()
    {
        // SlotList 하위에 오브젝트 Transform 정보를 추출
        var slots = slotList.GetComponentsInChildren<Transform>();
        // 보유한 아이템의 개수만큼 반복
        for(int i=0; i<gameData.equipItem.Count; i++)
        {
            // 인벤토리 UI에 있는 Slot개수 만큼반복
            for(int j=1; j<slots.Length; j++)
            {
                // Slot 하위에 다른 아이템이 있으면 다음 인덱스로 넘어감
                if (slots[j].childCount > 0) continue;

                // 보유한 아이템 종류에 따라 인덱스를 추출
                int itemIdx = (int)gameData.equipItem[i].itemType;
                // 아이템의 부모를 Slot으로 변경
                itemObjects[itemIdx].GetComponent<Transform>().SetParent(slots[j]);
                // 아이템의 정보를 itemData에 로드한 데이터 값을 저장
                itemObjects[itemIdx].GetComponent<ItemInfo>().itemData = gameData.equipItem[i];
                // 아이템을 Slot에 추가하면 바깥 for문으로 빠져나감
                break;
            }
        }
    }

    void Start()
    {
        OnInventoryOpen(false);
        CreateEnemyPool();
        //CreateBulletPool();
        CreateEBulletPool();

        //CreateSwatPool();

        //if (points.Length > 0)
        //    StartCoroutine(CreateEnemy());

        // 2초 간격으로 3초후에 반복 호출
        InvokeRepeating("RepeatingEnemy", 2f, 3f);
    }

    private void CreateSwatPool()
    {
        //GameObject swatObj = new GameObject("SwatPool");
        //for (int i = 0; i < maxCount; i++)
        //{
        //    GameObject _swat = Instantiate(swat, swatObj.transform);
        //    _swat.name = "Swat" + i.ToString();
        //    _swat.SetActive(false);
        //    swatPool.Add(_swat);
        //}
    }

    private void CreateEnemyPool()
    {
        GameObject enemyObj = new GameObject("EnemyPool");  // 풀링 오브젝트 담아둘 오브젝트
        for (int i = 0; i < maxCount; i++)
        {
            GameObject _enemy = Instantiate(enemy, enemyObj.transform);
            _enemy.name = "Enemy" + i.ToString();   // 오브젝트 이름 부여
            _enemy.SetActive(false);                // 담아두는 용도로 비활성화 시켜둠
            enemyPool.Add(_enemy);                  // 리스트에 담음
        }
    }

    //private void CreateBulletPool()
    //{
    //    GameObject bulletPools = new GameObject("BulletPool");
    //    for (int i = 0; i < maxCount; i++)
    //    {
    //        GameObject _bullet = Instantiate(bulletPrefab, bulletPools.transform);
    //        _bullet.name = "Bullet" + i.ToString("00"); // 두자릿수로 표현
    //        _bullet.SetActive(false);
    //        bulletPool.Add(_bullet);
    //    }
    //}

    void CreateEBulletPool()
    {
        GameObject ebulletPools = new GameObject("E_BulletPool");
        for (int i = 0; i < 20; i++)
        {
            GameObject _bullet = Instantiate(e_bulletPrefab, ebulletPools.transform);
            _bullet.name = "Bullet" + i.ToString("00"); // 두자릿수로 표현
            _bullet.SetActive(false);
            e_bulletPool.Add(_bullet);
        }
    }

    IEnumerator CreateEnemy()
    {
        while (!isGameOver)
        {
            #region 오브젝트 풀링이 아닐 경우
            //int enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
            //if (enemyCount < maxCount)
            //{
            //    yield return new WaitForSeconds(CreateTime);
            //    int idx = Random.Range(1, points.Length);
            //    Instantiate(enemy, points[idx].position, points[idx].rotation);
            //}
            //else
            //{
            //    yield return null;  // 한 프레임 쉰후 다시 시작
            //}
            #endregion
            yield return new WaitForSeconds(CreateTime);
            

            //foreach (GameObject _swat in swatPool)
            //{
            //    if (_swat.activeSelf == false)
            //    {
            //        int idx = Random.Range(1, points.Length);
            //        _swat.transform.position = points[idx].position;
            //        _swat.transform.rotation = points[idx].rotation;
            //        _swat.SetActive(true);
            //        break;
            //    }
            //}
        }
    }

    public GameObject GetBullet()
    {
        for(int i=0; i<bulletPool.Count; i++)
        {
            // 총알이 비활성화 상태면 총알 오브젝트를 반환
            if(bulletPool[i].activeSelf == false)
            {
                return bulletPool[i];
            }
        }
        return null;
    }

    public GameObject GetE_Bullet()
    {
        for (int i = 0; i < e_bulletPool.Count; i++)
        {
            // 총알이 비활성화 상태면 총알 오브젝트를 반환
            if (e_bulletPool[i].activeSelf == false)
            {
                return e_bulletPool[i];
            }
        }
        return null;
    }

    void RepeatingEnemy()
    {
        if (isGameOver) return;
        // enemyPool에 담겨있는 _enemy만큼 돌림
        foreach (GameObject _enemy in enemyPool)
        {
            if (_enemy.activeSelf == false)
            {
                int idx = Random.Range(1, points.Length);
                _enemy.transform.position = points[idx].position;
                _enemy.transform.rotation = points[idx].rotation;
                _enemy.SetActive(true);
                break;
            }
        }
    }
    bool isPaused;  // 일시 정지 여부

    public void OnPausedClick()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        MonoBehaviour[] scripts = playerObj.GetComponents<MonoBehaviour>();

        // 일시정지 눌렸을때 플레이어의 스크립트 활성화 여부
        foreach(var script in scripts)
        {
            script.enabled = !isPaused;
        }
        var canvasGroup = GameObject.Find("Panel-Weapon").GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = !isPaused;
    }

    // 인벤토리 활성화 유무 함수
    public void OnInventoryOpen(bool isOpend)
    {
        inventoryCG.alpha = isOpend ? 1.0f : 0f;
        inventoryCG.interactable = isOpend;
        inventoryCG.blocksRaycasts = isOpend;
    }

    public void InkillCount()
    {
        // ++killCount;
        ++gameData.killCount;
        killCountTxt.text = "KILL: " + "<color=#ff0000>" + gameData.killCount.ToString("0000") + "</color>";
       // PlayerPrefs.SetInt("KILL_COUNT", killCount);
    }

    // 게임 데이터 저장
    void SaveGameData()
    {
        //dataManager.Save(gameData);
        //.asset 파일에 데이터 저장
        UnityEditor.EditorUtility.SetDirty(gameData);
    }

    // 인벤토리 아이템을 추가 했을 때 데이터의 정보를 갱신하는 함수
    public void AddItem(Item item)
    {
        // 장착 아이템과 같은 아이템을 얻었다면 추가하지 않음
        if (gameData.equipItem.Contains(item)) return;
        // 아이템을 GameData.equipItem 배열에 추가
        gameData.equipItem.Add(item);
        switch(item.itemType)
        {
            case Item.ItemType.HP:
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                    gameData.hp += item.value;
                else
                    gameData.hp += gameData.hp / (1f +  item.value);
                break;
            case Item.ItemType.SPEED:
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                    gameData.speed += item.value;
                else
                    gameData.speed += gameData.speed / (1f + item.value);
                break;
            case Item.ItemType.DAMAGE:
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                    gameData.damage += item.value;
                else
                    gameData.damage += gameData.damage / (1f + item.value);
                break;
            case Item.ItemType.GRENADE:

                break;
        }
        //.aseet 파일에 데이터 저장
        UnityEditor.EditorUtility.SetDirty(gameData);

        // 아이템이 변경 된 것을 실시간으로 적용하기 위해 이벤트를 발생 시킴
        OnItemChange();
    }

    // 인벤토리에서 아이템을 제거 했을 때 데이터를 갱신 하는 함수
    public void RemoveItem(Item item)
    {
        // 아이템을 GameData.equipItem 배열에서 제거
        gameData.equipItem.Remove(item);
        switch (item.itemType)
        {
            case Item.ItemType.HP:
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                    gameData.hp -= item.value;
                else
                    gameData.hp -= gameData.hp / (1f + item.value);
                break;
            case Item.ItemType.SPEED:
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                    gameData.speed -= item.value;
                else
                    gameData.speed -= gameData.speed / (1f + item.value);
                break;
            case Item.ItemType.DAMAGE:
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                    gameData.damage -= item.value;
                else
                    gameData.damage -= gameData.damage / (1f + item.value);
                break;
            case Item.ItemType.GRENADE:
                break;
        }

        //.aseet 파일에 데이터 저장
        UnityEditor.EditorUtility.SetDirty(gameData);

        // 아이템이 변경 된 것을 실시간으로 적용하기 위해 이벤트를 발생 시킴
        OnItemChange();
    }

    // 게임 종료시 자동으로 게임 저장
    private void OnApplicationQuit()
    {
        SaveGameData();
    }
}
