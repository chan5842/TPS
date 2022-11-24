using System.Collections.Generic;   // 리스트 사용 가능
using System.Collections;           // 배열 사용 가능

namespace DataInfo
{
    [System.Serializable]

    public class GameData
    {
        public int killCount = 0;                           // 사망한 적 수
        public float hp = 120f;                             // 초기 체력
        public float damage = 25f;                          // 총알 데미지
        public float speed = 6f;                            // 이동 속도
        public List<Item> equipItem = new List<Item>();
    }
    [System.Serializable]
    public class Item
    {
        public enum ItemType { HP, SPEED, GRENADE, DAMAGE }  // 아이템 종류 선언
        public enum ItemCalc { INC_VALUE, PERCENT }         // 계산 방식 선언
        public ItemType itemType;   // 아이템 종류
        public ItemCalc itemCalc;   // 계산 방식
        public string name;         // 아이템 명
        public string desc;         // 아이템 소개
        public float value;        // 계산
    }
}

