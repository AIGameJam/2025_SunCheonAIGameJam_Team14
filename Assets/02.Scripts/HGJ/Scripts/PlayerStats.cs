// PlayerStats.cs
using UnityEngine;
using UnityEngine.UI;
using LYJ;

public class PlayerStats : MonoBehaviour
{
    // Slider 컴포넌트를 직접 참조합니다.
    public Slider hpBarSlider;

    // 데이터 (float으로 통일)
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    public int fishingRodLevel = 1;

    public Inventory inventory;
    private Image creatureImage;

    // 현재 표시되고 있는 획득물 오브젝트
    private GameObject currentCaughtCreature = null;

    private void Awake()
    {
        inventory = GameObject.Find("Canvas").transform.GetChild(2).GetComponent<Inventory>();
        creatureImage = transform.GetChild(2).GetChild(0).GetComponent<Image>();
    }

    public int PriceCalculate()
    {
        int _totalPrice = 0;

        for (int i = 0; i <  inventory.slots.Length; i++)
        {
            if (inventory.slots[i] != null)
            {
                _totalPrice += (inventory.slots[i].item.ItemCost) * (inventory.slots[i].itemCount);
            }
        }

        return _totalPrice;
    }

    public void InitializeHealth()
    {
        currentHealth = maxHealth;
        if (hpBarSlider != null)
        {
            hpBarSlider.maxValue = maxHealth;
            hpBarSlider.value = currentHealth;
        }
    }

    public void ConsumeStamina(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        if (hpBarSlider != null && maxHealth > 0)
        {
            hpBarSlider.value = currentHealth;
        }

        Debug.Log($"PlayerStats: 체력 {amount} 소모. 현재 체력 데이터: {currentHealth}");
    }

    public bool CanAffordStamina(int cost)
    {
        return currentHealth >= cost;
    }

    /// <summary>
    /// 획득한 생물 프리팹을 캐릭터 위에 표시하고, 일정 시간 후 제거합니다.
    /// </summary>
    public void DisplayCaughtCreature(ItemScriptableObject creaturePrefab)
    {
        // 이전 획득물이 있다면 제거하고 새 획득물 표시
        if (currentCaughtCreature != null)
        {
            Destroy(currentCaughtCreature);
        }

        // 1. 캐릭터(PlayerRoot) 위에 획득물 인스턴스 생성
        //currentCaughtCreature = Instantiate(creaturePrefab, transform);
        creatureImage.sprite = creaturePrefab.ItemImage;
        creatureImage.gameObject.SetActive(true);
        inventory.AcquireItem(creaturePrefab);

        for(int i = 0; i < EncyclopediaManager.Instance.encyclopediaList.Count; i++)
        {
            if (creaturePrefab.ItemName != EncyclopediaManager.Instance.encyclopediaList[i].ItemName)
                EncyclopediaManager.Instance.encyclopediaList.Add(creaturePrefab);

        }

        // 2. 캐릭터 머리 위로 적절히 배치 (위치는 조정 가능)
        //currentCaughtCreature.transform.localPosition = new Vector3(0, 3.0f, 0);

        // 3. 2초 후 오브젝트를 제거하는 코루틴 시작
        StartCoroutine(HideCreatureAfterDelay(2f));
    }

    private System.Collections.IEnumerator HideCreatureAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        creatureImage.gameObject.SetActive(false);
    }

    //private System.Collections.IEnumerator HideCreatureAfterDelay(GameObject creature, float delay)
    //{
    //    yield return new WaitForSeconds(delay);
    //    if (creature != null)
    //    {
    //        Destroy(creature);
    //        currentCaughtCreature = null;
    //    }
    //}
}