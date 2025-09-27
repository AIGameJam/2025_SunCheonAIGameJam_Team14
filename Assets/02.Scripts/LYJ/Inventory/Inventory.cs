using EnumTypes;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static bool inventoryActivated = false;
    public ItemScriptableObject diplays;

    private GameObject inventoryBase;
    private GameObject slotParent;

    private Slot[] slots;

    private void Start()
    {
        inventoryBase = transform.GetChild(0).gameObject;
        slotParent = inventoryBase.transform.GetChild(0).gameObject;
        slots = slotParent.GetComponentsInChildren<Slot>();

        for (int i = 0; i < slots.Length; i++)
            slots[i].Init();

        AcquireItem(diplays, 4);
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

    // 아이템 획득
    public void AcquireItem(ItemScriptableObject _item, int _count = 1)
    {
        // 같은 종류의 아이템이 있다면 갯수 더해주기
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

        // 같은 종류의 아이템이 없다면 저장할 새로운 슬롯 생성
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].AddItem(_item, _count);
                return;
            }
        }
    }

    // 아이템 사용
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

    // 특정 아이템 갯수 확인
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
