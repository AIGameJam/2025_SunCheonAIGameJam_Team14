using UnityEngine;
namespace JJG{
// 이 스크립트는 유니티 에디터의 Create 메뉴에 새로운 항목을 만들어줍니다.
[CreateAssetMenu(fileName = "New Item Data", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject // MonoBehaviour가 아닌 ScriptableObject 임에 주의!
{
    public string itemName;
    public Sprite icon;
    // (나중에 설명, 가격 등 추가 가능)
}
}