// FishingSystem.cs
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
using System.Linq;

public enum FishingPhase
{
    IDLE,               // 평상시: 이동 가능, 낚시 준비 대기
    FISHING_READY,      // 준비 상태: 낚싯대 준비 동작 (우클릭 -> FISHING_ACTIVE)
    FISHING_ACTIVE      // 낚시 진행 중: 찌 던짐, 챔질 대기 (우클릭 -> 챔질/READY 복귀)
}

public class FishingSystem : MonoBehaviour
{
    // 필요한 시스템 참조 (Inspector에서 할당)
    public TimeManager timeManager;
    public PlayerStats playerStats;
    public PointManager pointManager;
    public FishingController fishingController;

    public FishingPhase currentPhase { get; private set; } = FishingPhase.IDLE;

    public void SetPhase(FishingPhase newPhase)
    {
        currentPhase = newPhase;
    }

    // 낚시 상수
    private const int ROD_TIME_COST = 30;
    private const int ROD_STAMINA_COST = 5;

    // === 1. 낚시 준비 단계 진입 (IDLE -> FISHING_READY) ===
    public void StartFishingReady()
    {
        if (currentPhase != FishingPhase.IDLE) return;

        currentPhase = FishingPhase.FISHING_READY;

        if (fishingController != null)
            fishingController.currentState = FishingController.State.FishingReady;

        Debug.Log("✅ [FS] StartFishingReady 함수 호출 성공! 상태 READY로 전환 완료.");
    }

    // === 2. 낚시 실행 로직 (FISHING_READY -> FISHING_ACTIVE) ===
    public void StartFishingAttempt()
    {
        if (currentPhase != FishingPhase.FISHING_READY)
        {
            Debug.LogWarning($"⚠️ 낚시 시작 실패: 현재 페이즈가 FISHING_READY가 아닌 {currentPhase}입니다.");
            return;
        }

        if (playerStats == null || !playerStats.CanAffordStamina(ROD_STAMINA_COST))
        {
            Debug.LogWarning($"⚠️ 낚시 시작 실패: 체력({(playerStats != null ? playerStats.currentHealth.ToString() : "N/A")})이 부족합니다.");
            return;
        }

        // --- 모든 체크를 통과하면 낚시 실행 ---
        timeManager.SpendTime(ROD_TIME_COST);
        playerStats.ConsumeStamina(ROD_STAMINA_COST);

        currentPhase = FishingPhase.FISHING_ACTIVE;

        if (fishingController != null)
            fishingController.currentState = FishingController.State.FishingStart;

        Debug.Log("✅ 낚시 실행! FishingPhase: FISHING_ACTIVE. (찌 던짐)");

        float timeUntilSignal = UnityEngine.Random.Range(0.5f, 5.0f);
        StartCoroutine(WaitForSignal(timeUntilSignal));
        Debug.Log("신호 대기 중...");
    }

    private IEnumerator WaitForSignal(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        bool isSignalTime = true;
        Debug.Log("🔴 신호 감지! 우클릭/Fish 액션 사용!");

        yield return new WaitForSeconds(1.0f);

        if (currentPhase == FishingPhase.FISHING_ACTIVE && isSignalTime)
        {
            FailFishing("신호 시간 초과");
        }
    }

    public void ConfirmCatch()
    {
        if (currentPhase != FishingPhase.FISHING_ACTIVE) return;
        SuccessFishing();
    }

    private void SuccessFishing()
    {
        // 1. 현재 블럭의 단계 레벨을 가져옵니다. 
        int currentDepth = (fishingController != null) ? fishingController.CurrentDepthLevel : 1;

        // 2. PointManager에게 해당 레벨의 획득물 목록을 요청합니다.
        GameObject[] creatures = (pointManager != null) ? pointManager.GetCreaturesByDepth(currentDepth) : new GameObject[0];
        Debug.Log(creatures.Length);

        // 🚨 3. [핵심] Null 요소를 필터링하여 유효한 프리팹만 남깁니다. 🚨
        GameObject[] validCreatures = creatures.Where(c => c != null).ToArray();

        if (validCreatures.Length > 0)
        {
            // 4. 유효한 목록에서만 생물을 무작위로 선택
            int randomIndex = Random.Range(0, validCreatures.Length);
            GameObject caughtCreature = validCreatures[randomIndex];

            // 5. PlayerStats에게 화면에 표시하도록 명령
            playerStats.DisplayCaughtCreature(caughtCreature);
            Debug.Log($"🎉 낚시 성공! 획득 지점: {currentDepth}단계. 획득 생물: {caughtCreature.name}");
        }

        else
        {
            // 이 경고가 나온다면 Inspector의 획득물 슬롯이 비어있다는 뜻입니다.
            Debug.LogWarning($"⚠️ 낚시 성공! 하지만 {currentDepth}단계에서 획득 가능한 생물이 설정되지 않았습니다. 빈 손!");
        }

        // --- 상태 복구 ---
        currentPhase = FishingPhase.FISHING_READY;
        if (fishingController != null)
            fishingController.currentState = FishingController.State.FishingReady;
        StopAllCoroutines();
        Debug.Log("🎉 낚시 성공! 다시 찌를 던질 수 있습니다.");
    }

    private void FailFishing(string reason)
    {
        currentPhase = FishingPhase.FISHING_READY;
        if (fishingController != null)
            fishingController.currentState = FishingController.State.FishingReady;
        StopAllCoroutines();
        Debug.Log($"😭 낚시 실패! ({reason})");
    }

    // 기타 로직 유지
    public void UseTrapOrNet(int timeCost, string itemType)
    {
        if (currentPhase != FishingPhase.IDLE && currentPhase != FishingPhase.FISHING_READY) return;
        timeManager.SpendTime(timeCost);
        Debug.Log($"[System] {itemType} 사용. {timeCost}분 소모.");
    }
}