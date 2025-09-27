// FishingController.cs
using UnityEngine;
using System.Collections;

public class FishingController : MonoBehaviour
{
    [Header("연결할 컴포넌트")]
    public SpriteRenderer characterSpriteRenderer;

    [Header("스프라이트")]
    public Sprite idleSprite;
    public Sprite walkSprite;
    public Sprite fishingReadySprite;
    public Sprite fishingStartSprite;

    [Header("캐릭터 설정")]
    public float moveSpeed = 5f;

    [Header("시스템 연동")]
    public FishingSystem fishingSystem;

    // === 머리 위 인디케이터 관련 ===
    [Header("머리 위 표시")]
    public Transform headAnchor;
    public GameObject indicatorPrefab;
    public Vector3 headOffset = new Vector3(0f, 0.4f, 0f);
    public float followLerp = 15f;

    private GameObject indicatorInstance;
    private bool indicatorShown;

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
                UpdateIndicator();
                Debug.Log($"[Controller] State Changed: {value}.");
            }
        }
    }
    private State _currentState = State.Idle;

    private int currentDepthLevel = 1;
    public int CurrentDepthLevel => currentDepthLevel;

    private Vector3 baseScale;

    void Awake()
    {
        baseScale = transform.localScale;
        if (characterSpriteRenderer == null)
            characterSpriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (headAnchor == null) headAnchor = transform;

        if (indicatorPrefab != null)
        {
            indicatorInstance = Instantiate(indicatorPrefab);
            indicatorInstance.SetActive(false);

            SpriteRenderer sr = indicatorInstance.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingLayerName = "Characters";
                sr.sortingOrder = 100;
            }
        }
    }

    void Update()
    {
        HandleInput();
        UpdateVisual();
        UpdateIndicatorPosition();
    }

    private void HandleInput()
    {
        if (_currentState == State.Idle)
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            if (Mathf.Abs(moveX) > 0.01f)
            {
                transform.position += new Vector3(moveX, 0f, 0f) * moveSpeed * Time.deltaTime;
                characterSpriteRenderer.flipX = moveX > 0f;
            }
        }
    }

    private void UpdateVisual()
    {
        if (transform.localScale != baseScale)
            transform.localScale = baseScale;

        switch (_currentState)
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

    private void UpdateIndicator()
    {
        if (indicatorInstance == null) return;

        // ✅ Ready 상태 + indicatorAvailable == true + 시간이 남아있을 때만 표시
        bool shouldShow = (_currentState == State.FishingReady &&
                           fishingSystem != null &&
                           fishingSystem.indicatorAvailable &&
                           fishingSystem.indicatorTimer > 0f);

        if (shouldShow != indicatorShown)
        {
            indicatorShown = shouldShow;
            indicatorInstance.SetActive(indicatorShown);
        }
    }

    // ✅ FishingSystem에서 강제로 갱신할 때 호출
    public void UpdateIndicatorForce()
    {
        UpdateIndicator();
    }

    private void UpdateIndicatorPosition()
    {
        if (indicatorInstance == null) return;
        Vector3 targetPos = headAnchor.position + headOffset;
        indicatorInstance.transform.position = Vector3.Lerp(
            indicatorInstance.transform.position,
            targetPos,
            followLerp * Time.deltaTime
        );
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PointDataHolder pointData = other.GetComponent<PointDataHolder>();
        if (pointData != null)
        {
            currentDepthLevel = pointData.depthLevel;
            Debug.Log($"[Controller/Trigger] 포인트 {currentDepthLevel} 진입");
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
