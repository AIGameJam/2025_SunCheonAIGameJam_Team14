using EnumTypes;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public ItemScriptableObject item;
    public int itemCount;
    public Image itemImage;

    private GameObject countObject;
    private Text countText;

    public void Init()
    {
        itemImage = transform.GetChild(0).GetComponentInChildren<Image>(true);
        countObject = transform.GetChild(0).GetChild(0).gameObject;
        countText = countObject.GetComponentInChildren<Text>(true);
    }

    // 아이템 이미지 투명도 조절
    public void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }

    // 인벤토리에 새로운 아이템 슬롯 추가
    public void AddItem(ItemScriptableObject _item, int _count = 1)
    {
        item = _item;
        itemCount = _count;
        itemImage.sprite = item.ItemImage;

        if (item.ItemType != ItemType.Equipment)
        {
            countObject.SetActive(true);
            countText.text = itemCount.ToString();
        }
        else
        {
            countText.text = "0";
            countObject.SetActive(false);
        }

        SetColor(1);
    }

    //  해당 슬롯의 아이템 갯수 업데이트
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        countText.text = itemCount.ToString();

        if (itemCount <= 0)
            ClearSlot();
    }

    // 해당 슬롯 하나 삭제
    public void ClearSlot()
    {
        item = null;
        itemCount = 0;
        itemImage.sprite = null;
        SetColor(0);

        countText.text = "0";
        countObject.SetActive(false);
    }

    // 해당 슬롯 자리 변경
    private void ChangeSlot()
    {
        ItemScriptableObject tempItem = item;
        int tempItemCount = itemCount;

        AddItem(DragSlot.instance.dragSlot.item, DragSlot.instance.dragSlot.itemCount);

        if (tempItem != null)
            DragSlot.instance.dragSlot.AddItem(tempItem, tempItemCount);
        else
            DragSlot.instance.dragSlot.ClearSlot();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (item != null)
            {

            }
        }
    }

    // 마우스 드래그가 시작했을 때 호출
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            DragSlot.instance.dragSlot = this;
            DragSlot.instance.SetDragImage(itemImage);
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    // 마우스 드래그 하는 동안 계속 호출 
    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
            DragSlot.instance.transform.position = eventData.position;
    }

    // 마우스 드래그 하는 것이 끝냈을 때 호출
    public void OnEndDrag(PointerEventData eventData)
    {
        DragSlot.instance.SetColor(0);
        DragSlot.instance.dragSlot = null;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (DragSlot.instance.dragSlot != null)
        {
            ChangeSlot();
        }
    }
}
