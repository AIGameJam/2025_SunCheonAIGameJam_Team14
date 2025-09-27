using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance;

    public GameObject inventoryPanel;
    public Transform slotGrid;
    public GameObject slotPrefab;
    public GameObject tooltip;
    public TextMeshProUGUI tooltipText;

    // --- 이 줄이 추가되었습니다 ---
    List<Image> itemIcons = new List<Image>();

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        inventoryPanel.SetActive(false);
        if (tooltip != null) tooltip.SetActive(false);
        Inventory.instance.onItemChangedCallback += UpdateUI;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
            if (inventoryPanel.activeSelf)
            {
                UpdateUI();
            }
        }

        if (tooltip != null && tooltip.activeSelf)
        {
            tooltip.transform.position = Input.mousePosition;
        }
    }

    void UpdateUI()
    {
        if (Inventory.instance == null) return;

        foreach (Transform child in slotGrid)
        {
            Destroy(child.gameObject);
        }
        itemIcons.Clear();

        foreach (KeyValuePair<ItemData, int> itemEntry in Inventory.instance.items)
        {
            ItemData item = itemEntry.Key; // 'entry' -> 'itemEntry'로 수정
            GameObject slotInstance = Instantiate(slotPrefab, slotGrid);
            
            // ItemIcon 이라는 이름의 자식 오브젝트가 Slot 프리팹 안에 있는지 확인해주세요.
            Transform iconTransform = slotInstance.transform.Find("ItemIcon");
            if (iconTransform != null)
            {
                Image itemIcon = iconTransform.GetComponent<Image>();
                itemIcon.sprite = item.icon;
                itemIcons.Add(itemIcon);
            }

            // Slot 프리팹에 ItemSlot 컴포넌트가 있는지 확인해주세요.
            ItemSlot itemSlot = slotInstance.GetComponent<ItemSlot>();
            if (itemSlot != null)
            {
                itemSlot.itemData = item;
                itemSlot.itemCount = itemEntry.Value;
            }
        }
    }

    public void ShowTooltip(ItemData item, int count)
    {
        Debug.Log("거의 다되가");
        if (tooltip != null && tooltipText != null)
        {
            tooltip.SetActive(true);
            tooltipText.text = $"{item.itemName} ({count}개)";
        }
    }

    public void HideTooltip()
    {
        if (tooltip != null)
        {
            tooltip.SetActive(false);
        }
    }
}