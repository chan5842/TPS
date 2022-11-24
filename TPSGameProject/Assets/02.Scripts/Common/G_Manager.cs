using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class G_Manager : MonoBehaviour
{
    public static G_Manager g_Manager;
    public Transform[] Points;
    public GameObject m_Prefab;

    public Text killText;
    public static int score;
    int total = 0;

    float timePreve;
    int maxCount = 10;

    void Start()
    {
        Points = GameObject.Find("SpawnPoint").
            GetComponentsInChildren<Transform>();

        g_Manager = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - timePreve > 3f)
        {
            timePreve = Time.time;
            int enemyCount = (int)GameObject.FindGameObjectsWithTag("Enemy").Length;
            if(enemyCount <= maxCount)
                CreateEnemy();
        }
    }

    void CreateEnemy()
    {
        int count = Random.Range(1, Points.Length);
        Instantiate(m_Prefab, Points[count].position,
            Points[count].rotation);
    }

    public void KillCount(int count)
    {
        total += count;
        killText.text = "Kill : " + "<color=#ff0000>" + total.ToString() + "</color>"; 
    }
}
