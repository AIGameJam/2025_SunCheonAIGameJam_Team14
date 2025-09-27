using EnumTypes;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace LYJ
{
    public class Slot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        private Rect baseRect;
        private Inventory inventory;

        public ItemScriptableObject item;
        public int itemCount;
        public Image itemImage;

        private GameObject countObject;
        private Text countText;

        private void Start()
        {
            baseRect = transform.parent.parent.GetComponent<RectTransform>().rect;
            inventory = transform.parent.parent.parent.GetComponent<Inventory>();
        }

        public void Init()
        {
            itemImage = transform.GetChild(0).GetComponentInChildren<Image>(true);
            countObject = transform.GetChild(0).GetChild(0).gameObject;
            countText = countObject.GetComponentInChildren<Text>(true);
        }

        public void SetColor(float _alpha)
        {
            Color color = itemImage.color;
            color.a = _alpha;
            itemImage.color = color;
        }

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

        public void SetSlotCount(int _count)
        {
            if(itemCount + _count >= 20)
                itemCount = 20;
            else
                itemCount += _count;

            countText.text = itemCount.ToString();

            if (itemCount <= 0)
                ClearSlot();
        }

        public void ClearSlot()
        {
            item = null;
            itemCount = 0;
            itemImage.sprite = null;
            SetColor(0);

            countText.text = "0";
            countObject.SetActive(false);
        }

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

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (item != null)
            {
                DragSlot.instance.dragSlot = this;
                DragSlot.instance.SetDragImage(itemImage);
                DragSlot.instance.transform.position = eventData.position;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (item != null)
                DragSlot.instance.transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (DragSlot.instance.transform.localPosition.x < baseRect.xMin
        || DragSlot.instance.transform.localPosition.x > baseRect.xMax
        || DragSlot.instance.transform.localPosition.y < baseRect.yMin
        || DragSlot.instance.transform.localPosition.y > baseRect.yMax)
            {
                GameObject _dropped = eventData.pointerCurrentRaycast.gameObject;

                if (_dropped != null && _dropped.CompareTag("Display"))
                {
                    DisplayController _disply = _dropped.GetComponent<DisplayController>();
                    _disply.item = DragSlot.instance.dragSlot.item;
                    _disply.OnDisplay();
                    DragSlot.instance.dragSlot.ClearSlot();
                }
            }

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
}
