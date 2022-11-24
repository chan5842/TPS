using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager2 : MonoBehaviour
{
    public static GameManager2 instance;

    [SerializeField]
    GameObject swat;
    [SerializeField]
    Transform[] points;

    public List<GameObject> swatPool = new List<GameObject>();

    public bool isGameOver = false;
    public int maxCount = 10;
    float CreateTime = 3f;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        swat = Resources.Load("SwatGuy") as GameObject;
        points = GameObject.Find("SpawnPoints").GetComponentsInChildren<Transform>();

        GameObject swatObj = new GameObject("SwatPool");
        for(int i =0; i<maxCount; i++)
        {
            GameObject _swat = Instantiate(swat, swatObj.transform);
            _swat.name = "Swat" + i.ToString();
            _swat.SetActive(false);
            swatPool.Add(_swat);
        }
        if (points.Length > 0)
            StartCoroutine(CreateSwat());
    }

    IEnumerator CreateSwat()
    {
        while(!isGameOver)
        {
            yield return new WaitForSeconds(CreateTime);
            if (isGameOver) yield break;

            foreach (GameObject _swat in swatPool)
            {
                if (_swat.activeSelf == false)
                {
                    int idx = Random.Range(1, points.Length);
                    _swat.transform.position = points[idx].position;
                    _swat.transform.rotation = points[idx].rotation;
                    _swat.SetActive(true);
                    break;
                }
            }
        }        
    }
}
