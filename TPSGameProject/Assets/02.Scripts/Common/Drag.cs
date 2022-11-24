using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Drag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    Transform itemTr;
    [SerializeField]
    Transform inventoryTr;
    public static GameObject draggingItem = null;
    [SerializeField]
    Transform itemListTr;
    [SerializeField]
    CanvasGroup canvasGroup;
     void Start()
    {
        itemTr = GetComponent<Transform>();
        inventoryTr = GameObject.Find("Inventory").GetComponent<Transform>();
        itemListTr = GameObject.Find("ItemList").GetComponent<Transform>();
        canvasGroup = GetComponent<CanvasGroup>();

    }

    // 드래그 이벤트
    public void OnDrag(PointerEventData eventData)
    {
        itemTr.position = Input.mousePosition;
        
    }

    // 드래그 시작시 이벤트
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 부모가 인벤토리가 됨
        this.transform.SetParent(inventoryTr);
        // 드래그 되는 아이템 정보를 저장
        draggingItem = this.gameObject;
        canvasGroup.blocksRaycasts = false;
    }

    // 드래그 종료시 이벤트
    public void OnEndDrag(PointerEventData eventData)
    {
        // 드래그 종료시 드래그 아이템 정보를 초기화
        draggingItem = null;
        canvasGroup.blocksRaycasts = true;
        // 드래그 종료 후 원래 대로 돌아온다면
        if (itemTr.parent == inventoryTr)
        {
            itemTr.SetParent(itemListTr.transform);
            GameManager.instance.RemoveItem(GetComponent<ItemInfo>().itemData);
        }
        
    }
}
