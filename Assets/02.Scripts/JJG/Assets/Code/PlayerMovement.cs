using UnityEngine;
using UnityEngine.Tilemaps;
namespace JJG
{
    public class PlayerMovement : MonoBehaviour
    {
        Rigidbody2D rb;
        private Animator animator;

        float moveDir;
        public float moveSpeed = 5f;
        public float jumpPower = 5f;
        public Transform[] HitPoint; // [0]: 오른쪽, [1]: 왼쪽, [2]: 아래쪽
        public LayerMask whatisPlatform;

        [Header("파괴 속도 설정")]
        public float digSpeed = 0.2f;
        private float nextDigTime = 0f;
        
        private int lastHorizontalDir = 1;
        // 현재 파괴 중인지 상태를 기억하는 변수
        private bool isDigging = false;
        private bool isJumping = false;
        private bool isWalking = false;
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponentInChildren<Animator>(); 
            Debug.Log($"[Start] Animator 연결됨? {(animator != null ? "YES" : "NO")}");
        }

        void Update()
        {
            if (PlayerHealth.instance != null && PlayerHealth.instance.isDead)
            {
                moveDir = 0;
                return;
            }

            moveDir = Input.GetAxisRaw("Horizontal");

            if (moveDir != 0)
            {
                lastHorizontalDir = (int)Mathf.Sign(moveDir);
            }
            
            if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                 Debug.Log("[Update] 점프 입력 감지");
            }
            
            UpdateAnimator();
        }

        void FixedUpdate()
        {
            if (PlayerHealth.instance != null && PlayerHealth.instance.isDead)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            // 1. 파괴 가능한 블록이 있는지 먼저 확인
            Collider2D blockToDig = CheckForBlock();
            
            // 2. 블록이 있다면 파괴 로직 실행
            if (blockToDig != null)
            {
                Debug.Log("[Dig] 파괴 시작");
                // 파괴를 '시작'하는 순간에만 애니메이션 실행
                if (!isDigging)
                {
                    isDigging = true;
                    TriggerDigAnimation();
                }

                // 쿨다운에 맞춰 타격
                if (Time.time >= nextDigTime)
                {
                    nextDigTime = Time.time + digSpeed;
                    Bricks bricksComponent = blockToDig.GetComponent<Bricks>();
                    if (bricksComponent != null)
                    {
                        // HitPoint의 위치를 다시 계산해서 전달
                        Transform hitPoint = GetCurrentHitPoint();
                        if(hitPoint != null)
                        {
                            Debug.Log($"[Dig] 타일 피격: {hitPoint.position}");
                            bricksComponent.OnTileHit(hitPoint.position);
                            if (PlayerHealth.instance != null)
                            {
                                PlayerHealth.instance.ConsumeHealthForDigging(hitPoint.position);
                            }
                        }
                    }
                }
            }
            // 3. 블록이 없다면 이동 로직 실행
            else
            {
                Debug.Log("[Dig] 파괴 중단");
                isDigging = false;
                Move();
            }
        }

        private void Move()
        {
            rb.linearVelocity = new Vector2(moveDir * moveSpeed, rb.linearVelocity.y);
        }

        private void UpdateAnimator()
        {
            if (animator == null)
            {
                Debug.LogWarning("[Animator] animator가 null입니다!");
                return;
            }

            isWalking = moveDir != 0 && !isDigging;
            isJumping = !IsGrounded();
        }

        // 파괴할 블록을 '탐색'만 하는 함수
        private Collider2D CheckForBlock()
        {
            Transform currentHitPoint = GetCurrentHitPoint();
            if (currentHitPoint == null) return null;
            Collider2D col = Physics2D.OverlapCircle(currentHitPoint.position, 0.02f, whatisPlatform);
            if (col != null)
            {
                Debug.Log($"[CheckForBlock] 블록 감지 at {currentHitPoint.position}");
            }
            return col;
        }

        // 현재 입력에 맞는 HitPoint를 반환하는 함수
        private Transform GetCurrentHitPoint()
        {
            float verticalDir = Input.GetAxisRaw("Vertical");
            if (verticalDir < 0) return HitPoint[2];

            // 수평 이동 입력이 있을 때만 좌우를 판단
            if (moveDir > 0) return HitPoint[0];
            if (moveDir < 0) return HitPoint[1];
            
            // 가만히 서있을 때는 이전에 봤던 방향을 기준으로 판단
            return (lastHorizontalDir == 1) ? HitPoint[0] : HitPoint[1];
        }

        // 현재 입력에 맞는 애니메이션 트리거를 발동시키는 함수
        private void TriggerDigAnimation()
        {
            float verticalDir = Input.GetAxisRaw("Vertical");
            if (verticalDir < 0)
            {
                animator.SetTrigger("Dig_Bottom");
            }
            else
            {
                animator.SetTrigger("Dig_Side");
            }
        }

        private bool IsGrounded()
        {
            float extraHeight = 0.1f;
            if (Physics2D.Raycast(transform.position, Vector2.down, 0.5f + extraHeight, whatisPlatform)) return true;
            if (Physics2D.Raycast(transform.position, Vector2.left, 0.5f + extraHeight, whatisPlatform)) return true;
            if (Physics2D.Raycast(transform.position, Vector2.right, 0.5f + extraHeight, whatisPlatform)) return true;
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
    }
}
