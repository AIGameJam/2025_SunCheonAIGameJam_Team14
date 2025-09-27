using System.Collections.Generic;
using UnityEngine;
namespace JJG{
public class Inventory : MonoBehaviour
{
    // --- ★★★ 이 부분이 싱글톤의 핵심입니다 ★★★ ---

    // 'instance'라는 이름의 대표 주소를 만듭니다. static으로 선언되어 게임 전체에서 공유됩니다.
    public static Inventory instance;
    public Dictionary<ItemData, int> items = new Dictionary<ItemData, int>();

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;
    void Awake()
    {
        // 만약 대표 주소(instance)가 비어있다면
        if (instance == null)
        {
            // 자기 자신을 대표 주소로 등록합니다.
            instance = this;
        }
        else
        {
            // 만약 이미 대표 주소에 누군가 등록되어 있다면 (씬 로드 시 중복 생성 방지)
            // 새로 만들어진 자기 자신은 파괴합니다.
            Destroy(gameObject);
        }
    }

    // --- 여기까지 ---

    // 획득한 아이템들을 저장할 리스트
    // 인벤토리에 아이템을 추가하는 함수
    public void AddItem(ItemData item)
    {
        // 1. 인벤토리에 이미 같은 아이템이 있는지 확인
        if (items.ContainsKey(item))
        {
            // 2. 있다면, 개수만 1 증가
            items[item]++;
        }
        else
        {
            // 3. 없다면, 새로 추가하고 개수는 1로 설정
            items.Add(item, 1);
        }

        Debug.Log(item.itemName + "을(를) 획득했습니다! 현재 " + items[item] + "개 보유 중.");

        // 아이템에 변화가 생겼다고 알림
        if (onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke();
        }
    }
}
}