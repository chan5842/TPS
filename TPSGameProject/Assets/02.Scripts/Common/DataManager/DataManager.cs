using System.Collections;
using System.Collections.Generic;
// 실시간으로 데이터 내용을 파일로 입출력을 위해
using System.IO;
// 바이너리 파일 포맷을 위한 namespace
// 실시간으로 이루어진 데이터를 이진데이터로 직렬화(바꾼 사람이 누구인지 알 수 있음)
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using DataInfo;

public class DataManager : MonoBehaviour
{
    // 파일 저장 경로
    string dataPath;
    // 파일 저장 경로와 파일명 지정
    public void Initialize()
    {
        // IOS, 안드로이트 폰의 경우 공용폴더의 경로에 반환
        // 앱이 업그레이드 되어도 삭제되지 않음
        dataPath = Application.persistentDataPath + "/gameData.dat";
    }
    // 데이터 저장 및 파일을 생성하는 함수
    public void Save(GameData gameData)
    {
        // 바이너리 파일 포맷을 위한 BinaryFormatter 생성
        BinaryFormatter bf = new BinaryFormatter();
        // 데이터 저장을 위한 파일 생성
        FileStream file = File.Create(dataPath);

        // 파일에 저장할 클래스에 데이터 할당
        GameData data = new GameData();
        data.killCount = gameData.killCount;
        data.hp = gameData.hp;
        data.speed = gameData.speed;
        data.damage = gameData.damage;
        data.equipItem = gameData.equipItem;

        // BinaryFormatter를 사용해 파일에 데이터 기록
        bf.Serialize(file, data);
        file.Close();
    }
    // 데이터를 불러오는 함수
    public GameData Load()
    {
        // 파일 경로에 파일에 존재한다면
        if(File.Exists(dataPath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(dataPath, FileMode.Open);
            // GameData 클래스에 파일로 부터 읽은 데이터를 기록
            GameData data = (GameData)bf.Deserialize(file);
            file.Close();

            return data;
        }
        // 없을 경우 기본값을 반환
        else
        {
            GameData data = new GameData();
            return data;
        }
    }
}
