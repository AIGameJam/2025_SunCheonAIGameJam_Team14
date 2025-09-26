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

    // ������ �̹��� ���� ����
    public void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }

    // �κ��丮�� ���ο� ������ ���� �߰�
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

    //  �ش� ������ ������ ���� ������Ʈ
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        countText.text = itemCount.ToString();

        if (itemCount <= 0)
            ClearSlot();
    }

    // �ش� ���� �ϳ� ����
    public void ClearSlot()
    {
        item = null;
        itemCount = 0;
        itemImage.sprite = null;
        SetColor(0);

        countText.text = "0";
        countObject.SetActive(false);
    }

    // �ش� ���� �ڸ� ����
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

    // ���콺 �巡�װ� �������� �� ȣ��
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            DragSlot.instance.dragSlot = this;
            DragSlot.instance.SetDragImage(itemImage);
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    // ���콺 �巡�� �ϴ� ���� ��� ȣ�� 
    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
            DragSlot.instance.transform.position = eventData.position;
    }

    // ���콺 �巡�� �ϴ� ���� ������ �� ȣ��
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
