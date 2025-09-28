// FishingController.cs
using UnityEngine;
using System.Collections;

public class FishingController : MonoBehaviour
{
    // === 기존 필드 ===
    [Header("연결할 컴포넌트")]
    public SpriteRenderer characterSpriteRenderer;

    [Header("스프라이트")]
    public Sprite idleSprite;
    public Sprite walkSprite;
    public Sprite fishingReadySprite;
    public Sprite fishingStartSprite;

    public Animator animator;

    [Header("캐릭터 설정")]
    public float moveSpeed = 5f;

    // === 시스템 연동 필드 ===
    [Header("시스템 연동")]
    public FishingSystem fishingSystem;

    // 🚨 [핵심] GameManager가 캐릭터 동작을 완전히 멈추기 위해 사용하는 플래그 🚨
    public bool isMovementLocked { get; set; } = false;

    // 상태 관리 (FishingSystem에서 설정)
    public enum State { Idle, FishingReady, FishingStart }
    public State currentState
    {
        get => _currentState;
        set
        {
            if (_currentState != value)
            {
                _currentState = value;
                UpdateVisual();
                Debug.Log($"[Controller] State Changed: {value}.");
            }
        }
    }
    private State _currentState = State.Idle;

    // 현재 깊이 레벨 저장 및 외부 접근용 속성
    private int currentDepthLevel = 1;
    public int CurrentDepthLevel => currentDepthLevel;

    private Vector3 baseScale;

    void Awake()
    {
        baseScale = transform.localScale;
        if (characterSpriteRenderer == null)
            characterSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        HandleInput();
        UpdateVisual();
    }

    private void HandleInput()
    {
        // 🚨 [핵심] isMovementLocked가 true이면 모든 입력을 즉시 차단 🚨
        if (isMovementLocked) 
            return;

        // Idle 상태일 때만 좌우 이동 실행 (경계 제한 없음)
        if (currentState == State.Idle)
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            transform.position += new Vector3(moveX, 0f, 0f) * moveSpeed * Time.deltaTime;

            if (moveX > 0)
                characterSpriteRenderer.flipX = false;
            else
                characterSpriteRenderer.flipX = true;

            //if (Mathf.Abs(moveX) > 0.01f)
            //{
            //    transform.position += new Vector3(moveX, 0f, 0f) * moveSpeed * Time.deltaTime;
            //    characterSpriteRenderer.flipX = moveX > 0f;
            //}
        }

        // 🚨 챔질 입력 확인 및 지시 로직이 남아있다면 여기서 제거되어야 합니다. 🚨
        // if (fishingSystem != null && fishingSystem.indicatorAvailable) { ... } 등
    }

    private void UpdateVisual()
    {
        // 🚨 [핵심] 동작이 멈췄다면 시각 효과도 Idle/정지 상태로 고정 🚨
        if (isMovementLocked)
        {
            characterSpriteRenderer.sprite = idleSprite;
            return;
        }

        // --- 기존 시각 로직 유지 ---
        if (transform.localScale != baseScale)
            transform.localScale = baseScale;

        switch (currentState)
        {
            case State.Idle:
                float moveX = Input.GetAxisRaw("Horizontal");
                characterSpriteRenderer.sprite = Mathf.Abs(moveX) > 0.01f ? walkSprite : idleSprite;
                break;
            case State.FishingReady:
                characterSpriteRenderer.sprite = fishingReadySprite;
                break;
            case State.FishingStart:
                characterSpriteRenderer.sprite = fishingStartSprite;
                break;
            default:
                characterSpriteRenderer.sprite = idleSprite;
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PointDataHolder pointData = other.GetComponent<PointDataHolder>();

        if (pointData != null)
        {
            currentDepthLevel = pointData.depthLevel;
            Debug.Log($"[Controller/Trigger] 선박이 포인트 레벨 {currentDepthLevel} 지역에 진입했습니다!");
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (characterSpriteRenderer == null)
            characterSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
#endif
}