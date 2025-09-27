using UnityEngine;
using UnityEngine.EventSystems;
namespace JJG
{
    public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public ItemData itemData;
        public int itemCount;

        // 찾은 InventoryUI를 저장할 변수
        private InventoryUI inventoryUI;

        void Start()
        {
            // 씬에서 InventoryUI 타입의 스크립트를 가진 오브젝트를 찾아서 변수에 저장
            inventoryUI = FindObjectOfType<InventoryUI>();
            if (inventoryUI != null)
            {
                // 콘솔에 '1'을 출력합니다.
                Debug.Log("1");
            }
            else
            {
                // 만약 찾지 못했다면 경고 메시지를 출력합니다.
                Debug.LogWarning("ItemSlot이 InventoryUI를 찾지 못했습니다!");
            }
        }

        // 마우스가 슬롯 위에 올라왔을 때 호출
        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("마우스가 슬롯 위에 올라왔습니다!");
            // inventoryUI 변수가 null이 아닌지 확인 (씬에서 찾았는지)
            if (itemData != null && inventoryUI != null)
            {
                // 찾은 inventoryUI의 툴팁 표시 함수를 호출
                inventoryUI.ShowTooltip(itemData, itemCount);
            }
        }

        // 마우스가 슬롯에서 나갔을 때 호출
        public void OnPointerExit(PointerEventData eventData)
        {
            if (inventoryUI != null)
            {
                inventoryUI.HideTooltip();
            }
        }
    }
}
