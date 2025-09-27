// GameManager.cs
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class GameManager : MonoBehaviour
{
    // === Input System 필드 ===
    private PlayerControls playerControls;

    // 모든 시스템 매니저를 참조 (Inspector에서 할당)
    public TimeManager timeManager;
    public PointManager pointManager;
    public FishingSystem fishingSystem;
    public FishingController fishingController;
    public PlayerStats playerStats;

    [Header("UI 표시")]
    public TextMeshProUGUI phaseDisplayUI;
    // 🚨 [핵심] Time Display UI 필드 🚨
    public TextMeshProUGUI timeDisplayUI;

    void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Player.Fish.performed += OnFishPerformed;
    }

    void OnEnable()
    {
        if (playerControls != null)
            playerControls.Player.Enable();
    }

    void OnDisable()
    {
        if (playerControls != null)
            playerControls.Player.Disable();
    }

    private void OnFishPerformed(InputAction.CallbackContext context)
    {
        if (!context.performed || fishingSystem == null) return;

        FishingPhase currentPhase = fishingSystem.currentPhase;

        if (currentPhase == FishingPhase.IDLE)
        {
            Debug.Log("📢 [GM] Input Success! Requesting StartFishingReady.");
            fishingSystem.StartFishingReady();
        }
        else if (currentPhase == FishingPhase.FISHING_READY)
        {
            Debug.Log("[GM/Input] 🎣 낚시 준비 완료 상태에서 입력 감지: 찌 던지기 시도.");
            fishingSystem.StartFishingAttempt();
        }
        else if (currentPhase == FishingPhase.FISHING_ACTIVE)
        {
            Debug.Log("[GM/Input] 🎣 낚시 진행 중 입력 감지: 챔질 시도.");
            fishingSystem.ConfirmCatch();
        }
    }

    void Start()
    {
        if (timeManager != null)
        {
            timeManager.OnTurnEnd += HandleTurnEnd;
            timeManager.SetTimeFlow(true); // 시간 자동 흐름 기능은 현재 비활성화
        }

        if (playerStats != null)
        {
            playerStats.InitializeHealth();
        }

        if (fishingSystem != null)
            Debug.Log($"[GM/START] 시스템 시작 완료. 현재 단계: {fishingSystem.currentPhase.ToString()}");
    }

    void Update()
    {
        // 🚨 [추가된 로직] 매 프레임 시간 UI 업데이트 🚨
        if (timeManager != null && timeDisplayUI != null)
        {
            int totalMinutes = timeManager.timeLeftInMinutes;
            int hours = totalMinutes / 60;
            int minutes = totalMinutes % 60;

            // "H시간 M분" 형식으로 UI 텍스트 업데이트
            timeDisplayUI.text = $"Time: {hours}H {minutes:D2}Min";
        }

        if (fishingSystem == null) return;
        HandleInput();
    }

    void HandleInput()
    {
        // 현재 시스템은 자유 이동만 허용합니다.
    }

    private void HandleTurnEnd()
    {
        Debug.Log("[GM] 만조 턴이 종료되었습니다.");
        timeManager.StartNewTurn();

        fishingSystem.SetPhase(FishingPhase.IDLE);

        if (fishingController != null)
            fishingController.currentState = FishingController.State.Idle;
    }
}