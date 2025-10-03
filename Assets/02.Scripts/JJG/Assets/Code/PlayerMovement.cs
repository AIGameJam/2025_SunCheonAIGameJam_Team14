using UnityEngine;
using UnityEngine.Tilemaps;

namespace JJG
{
    public class PlayerMovement : MonoBehaviour
    {
        Rigidbody2D rb;
        private Animator animator;
        private SpriteRenderer spriteRenderer; // ▼▼▼ 스프라이트 좌우 반전을 위한 변수

        [Header("이동 및 점프 설정")]
        public float moveSpeed = 5f;
        public float jumpPower = 5f;

        [Header("땅 & 벽 감지 설정")]
        public Transform[] HitPoint; // [0]: 오른쪽, [1]: 왼쪽, [2]: 아래쪽
        public LayerMask whatisPlatform;
        public float wallCheckDistance = 0.5f; // 벽을 감지할 거리
        private bool isWalled; // 벽에 붙어있는지 확인

        [Header("벽 타기 설정")]
        public float climbingSpeed = 3f; // 벽을 타고 올라가는 속도
        private bool isClimbing; // 현재 벽 타기 중인지 확인

        private float moveDir;
        private float verticalDir; // 위/아래 입력 감지를 위해 추가
        private int lastHorizontalDir = 1;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponentInChildren<Animator>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>(); // 자식 오브젝트에서 SpriteRenderer 찾기
        }

        void Update()
        {
            if (PlayerHealth.instance != null && PlayerHealth.instance.isDead)
            {
                moveDir = 0;
                return;
            }

            // 이동 및 방향키 입력
            moveDir = Input.GetAxisRaw("Horizontal");
            verticalDir = Input.GetAxisRaw("Vertical");

            // ▼▼▼ 스프라이트 좌우 반전 로직 ▼▼▼
            if (moveDir > 0)
            {
                spriteRenderer.flipX = true; // 오른쪽 볼 때
            }
            else if (moveDir < 0)
            {
                spriteRenderer.flipX = false; // 왼쪽 볼 때
            }

            // ▼▼▼ 개선된 점프 로직 (벽에 붙어있을 때도 점프 가능) ▼▼▼
            if (Input.GetKeyDown(KeyCode.Space) && (IsGrounded() || isWalled) && !isClimbing)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
            }

            // 마우스 클릭 시 파괴
            if (Input.GetMouseButtonDown(0))
            {
                Collider2D blockToDig = CheckForBlock();
                if (blockToDig != null)
                {
                    TriggerDigAnimation();
                    Bricks bricksComponent = blockToDig.GetComponent<Bricks>();
                    if (bricksComponent != null)
                    {
                        Transform hitPoint = GetCurrentHitPoint();
                        if (hitPoint != null)
                        {
                            bricksComponent.OnTileHit(hitPoint.position);
                            if (PlayerHealth.instance != null)
                            {
                                PlayerHealth.instance.ConsumeHealthForDigging(hitPoint.position);
                            }
                        }
                    }
                }
            }

            // 벽 타기 상태 업데이트
            UpdateClimbingState();
            // 애니메이션 상태 업데이트
            UpdateAnimationStates();
            LogCurrentAnimationState(); // 현재 애니메이션 상태를 로그로 출력
        }

        void FixedUpdate()
        {
            if (PlayerHealth.instance != null && PlayerHealth.instance.isDead)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            // ▼▼▼ 벽 타기 / 일반 이동 물리 처리 ▼▼▼
            if (isClimbing)
            {
                // 벽 타기: 중력을 무시하고 위/아래로 이동
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, verticalDir * climbingSpeed);
            }
            else
            {
                // 일반 이동
                rb.linearVelocity = new Vector2(moveDir * moveSpeed, rb.linearVelocity.y);
            }
        }

        private void UpdateClimbingState()
        {
            isWalled = IsWalled();

            // 벽에 붙어있고, 땅이 아니며, 위 방향키를 누를 때 => 클라이밍 상태
            if (isWalled && !IsGrounded() && verticalDir > 0)
            {
                isClimbing = true;
            }
            else
            {
                isClimbing = false;
            }
        }

        private void UpdateAnimationStates()
        {
            if (animator == null) return;

            // 벽 타기 애니메이션
            animator.SetBool("IsClimbing", isClimbing);

            // 점프해서 올라갈 때 (swim 애니메이션)
            bool isJumpingUp = rb.linearVelocity.y > 0.1f && !IsGrounded() && !isClimbing;
            animator.SetBool("IsSwimming", isJumpingUp);

            // 좌우로 걸을 때 (walk 애니메이션)
            bool isWalking = moveDir != 0 && IsGrounded();
            animator.SetBool("IsWalking", isWalking);
        }

        private void TriggerDigAnimation()
        {
            float verticalDir = Input.GetAxisRaw("Vertical");
            if (verticalDir < 0)
            {
                animator.SetTrigger("DigBottom");
            }
            else
            {
                animator.SetTrigger("DigSide");
            }
        }

        private Collider2D CheckForBlock()
        {
            Transform currentHitPoint = GetCurrentHitPoint();
            if (currentHitPoint == null) return null;
            return Physics2D.OverlapCircle(currentHitPoint.position, 0.02f, whatisPlatform);
        }

        private Transform GetCurrentHitPoint()
        {
            float verticalDir = Input.GetAxisRaw("Vertical");
            if (verticalDir < 0) return HitPoint[2];
            if (moveDir > 0) return HitPoint[0];
            if (moveDir < 0) return HitPoint[1];
            return (lastHorizontalDir == 1) ? HitPoint[0] : HitPoint[1];
        }

        private bool IsGrounded()
        {
            return Physics2D.Raycast(transform.position, Vector2.down, 0.6f, whatisPlatform);
        }

        private bool IsWalled()
        {
            if (Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, whatisPlatform)) return true;
            if (Physics2D.Raycast(transform.position, Vector2.left, wallCheckDistance, whatisPlatform)) return true;
            return false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Item"))
            {
                ItemPickup itemToPick = other.GetComponent<ItemPickup>();
                if (itemToPick != null)
                {
                    Inventory.instance.AddItem(itemToPick.itemData);
                    Destroy(other.gameObject);
                }
            }
        }
        // ▼▼▼ 더 많은 정보를 보여주도록 강화된 디버그 함수 ▼▼▼
        private void LogCurrentAnimationState()
        {
            if (animator == null) return;

            // 현재 애니메이션 클립 이름 가져오기
            var clipInfo = animator.GetCurrentAnimatorClipInfo(0);
            string currentClipName = "None"; // 클립이 없으면 None으로 표시
            if (clipInfo.Length > 0)
            {
                currentClipName = clipInfo[0].clip.name;
            }

            // 중요한 변수들의 현재 상태를 하나의 줄에 모두 출력
            // F2는 소수점 둘째 자리까지만 표시하라는 의미 (깔끔하게 보기 위함)
            Debug.Log($"Anim: {currentClipName} | moveDir: {moveDir} | IsGrounded: {IsGrounded()} | isClimbing: {isClimbing} | rb.velocity.y: {rb.linearVelocity.y:F2}");
        }
    }
}