using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DataInfo;

public class Drop : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if(transform.childCount == 0)
        {
            Drag.draggingItem.transform.SetParent(this.transform);
            // 드래깅한 아이템의 정보를 가져옴
            Item item = Drag.draggingItem.GetComponent<ItemInfo>().itemData;
            GameManager.instance.AddItem(item);     // 인벤토리에 아이템 추가
        }
    }
}
