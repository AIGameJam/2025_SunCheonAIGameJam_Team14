// FishingSystem.cs
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
using System.Linq;

public enum FishingPhase
{
    IDLE,               // 평상시
    FISHING_READY,      // 준비 상태
    FISHING_ACTIVE      // 낚시 진행 중
}

public class FishingSystem : MonoBehaviour
{
    public TimeManager timeManager;
    public PlayerStats playerStats;
    public PointManager pointManager;
    public FishingController fishingController;

    public FishingPhase currentPhase { get; private set; } = FishingPhase.IDLE;

    // ✅ 낚시 후 Indicator를 표시할 수 있는 시간(초)
    public bool indicatorAvailable = false;
    public float indicatorTimer = 0f;

    public void SetPhase(FishingPhase newPhase)
    {
        currentPhase = newPhase;
    }

    private const int ROD_TIME_COST = 30;
    private const int ROD_STAMINA_COST = 5;

    void Update()
    {
        // Indicator가 켜져 있으면 2초 카운트
        if (indicatorAvailable && indicatorTimer > 0f)
        {
            indicatorTimer -= Time.deltaTime;
            if (indicatorTimer <= 0f)
            {
                indicatorAvailable = false; // 자동 종료
                if (fishingController != null)
                    fishingController.UpdateIndicatorForce(); // 즉시 반영
            }
        }
    }

    // === 준비 단계(IDLE -> READY) ===
    public void StartFishingReady()
    {
        if (currentPhase != FishingPhase.IDLE) return;

        currentPhase = FishingPhase.FISHING_READY;
        indicatorAvailable = false;
        indicatorTimer = 0f;

        if (fishingController != null)
            fishingController.currentState = FishingController.State.FishingReady;

        Debug.Log("✅ READY 상태 진입 (Indicator 꺼짐)");
    }

    // === READY -> ACTIVE ===
    public void StartFishingAttempt()
    {
        if (currentPhase != FishingPhase.FISHING_READY)
        {
            Debug.LogWarning($"⚠️ 낚시 시작 실패: 현재 페이즈 {currentPhase}");
            return;
        }

        if (playerStats == null || !playerStats.CanAffordStamina(ROD_STAMINA_COST))
        {
            Debug.LogWarning($"⚠️ 낚시 시작 실패: 체력 부족");
            return;
        }

        if (timeManager != null) timeManager.SpendTime(ROD_TIME_COST);
        playerStats.ConsumeStamina(ROD_STAMINA_COST);

        currentPhase = FishingPhase.FISHING_ACTIVE;

        if (fishingController != null)
            fishingController.currentState = FishingController.State.FishingStart;

        Debug.Log("✅ 낚시 실행! ACTIVE 상태");

        float timeUntilSignal = UnityEngine.Random.Range(0.5f, 5.0f);
        StartCoroutine(WaitForSignal(timeUntilSignal));
    }

    private IEnumerator WaitForSignal(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        bool isSignalTime = true;
        Debug.Log("🔴 신호 감지!");

        yield return new WaitForSeconds(1.0f);

        if (currentPhase == FishingPhase.FISHING_ACTIVE && isSignalTime)
            FailFishing("신호 시간 초과");
    }

    public void ConfirmCatch()
    {
        if (currentPhase != FishingPhase.FISHING_ACTIVE) return;
        SuccessFishing();
    }

    private void SuccessFishing()
    {
        int currentDepth = (fishingController != null) ? fishingController.CurrentDepthLevel : 1;
        GameObject[] creatures = (pointManager != null) ? pointManager.GetCreaturesByDepth(currentDepth) : new GameObject[0];
        GameObject[] validCreatures = creatures.Where(c => c != null).ToArray();

        if (validCreatures.Length > 0)
        {
            int randomIndex = Random.Range(0, validCreatures.Length);
            GameObject caughtCreature = validCreatures[randomIndex];
            if (playerStats != null)
                playerStats.DisplayCaughtCreature(caughtCreature);
            Debug.Log($"🎉 낚시 성공! 레벨 {currentDepth}, 생물: {caughtCreature.name}");
        }
        else
        {
            Debug.LogWarning($"⚠️ 낚시 성공! 하지만 레벨 {currentDepth}에는 생물이 없음");
        }

        currentPhase = FishingPhase.FISHING_READY;
        indicatorAvailable = true;
        indicatorTimer = 2f; // ✅ 2초 동안만 표시
        if (fishingController != null)
            fishingController.currentState = FishingController.State.FishingReady;

        StopAllCoroutines();
        Debug.Log("🎉 READY 복귀 (Indicator 2초 표시)");
    }

    private void FailFishing(string reason)
    {
        currentPhase = FishingPhase.FISHING_READY;
        indicatorAvailable = true;
        indicatorTimer = 2f; // ✅ 2초 동안만 표시
        if (fishingController != null)
            fishingController.currentState = FishingController.State.FishingReady;

        StopAllCoroutines();
        Debug.Log($"😭 낚시 실패! ({reason}) — Indicator 2초 표시");
    }

    public void UseTrapOrNet(int timeCost, string itemType)
    {
        if (currentPhase != FishingPhase.IDLE && currentPhase != FishingPhase.FISHING_READY) return;
        if (timeManager != null) timeManager.SpendTime(timeCost);
        Debug.Log($"[System] {itemType} 사용. {timeCost}분 소모.");
    }
}
