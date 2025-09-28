using EnumTypes;
using UnityEngine;

namespace LYJ
{
    public class Inventory : MonoBehaviour
    {
        public static bool inventoryActivated = false;

        private GameObject inventoryBase;
        private GameObject slotParent;

        public Slot[] slots;

        private void Start()
        {
            inventoryBase = transform.GetChild(0).gameObject;
            slotParent = inventoryBase.transform.GetChild(0).gameObject;
            slots = slotParent.GetComponentsInChildren<Slot>();

            for (int i = 0; i < slots.Length; i++)
                slots[i].Init();
        }

        private void Update()
        {
            InventoryHandler();
        }

        public void InventoryHandler()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                inventoryActivated = !inventoryActivated;

                if (inventoryActivated)
                    inventoryBase.SetActive(true);
                else
                    inventoryBase.SetActive(false);
            }
        }

        public void AcquireItem(ItemScriptableObject _item, int _count = 1)
        {
            if (_item.ItemType != ItemType.Equipment)
            {
                for (int i = 0; i < slots.Length; i++)
                {
                    if (slots[i].item != null && slots[i].item.ItemID == _item.ItemID)
                    {
                        slots[i].SetSlotCount(_count);
                        return;
                    }
                }
            }

            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item == null)
                {
                    slots[i].AddItem(_item, _count);
                    return;
                }
            }
        }

        public void UseItem(int _itemID, int _count)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item != null && slots[i].item.ItemID == _itemID)
                {
                    slots[i].SetSlotCount(-_count);
                    return;
                }
            }
        }

        public int ChechQuantityItem(int _item)
        {
            int _num = 0;

            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item != null && slots[i].item.ItemID == _item)
                {
                    _num += slots[i].itemCount;
                }
            }

            return _num;
        }
    }
}
