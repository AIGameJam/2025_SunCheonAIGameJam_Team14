using UnityEngine;
namespace JJG
{


    public class PlayerMovement : MonoBehaviour
    {
        Rigidbody2D rb;

        float moveDir;
        public float moveSpeed = 250f;
        public float jumpPower = 5f;
        public Transform[] HitPoint;
        public LayerMask whatisPlatform;

        // --- 추가된 변수 ---
        [Header("파괴 속도 설정")]
        [Tooltip("한 번 타격 후 다음 타격까지의 시간 (초)")]
        public float digSpeed = 0.2f;
        private float nextDigTime = 0f; // 다음 타격이 가능한 시간
                                        // --- 여기까지 ---
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            moveDir = Input.GetAxisRaw("Horizontal");
            if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower); // 기존 속도에 더하는 것보다 Y속도를 직접 지정하는 것이 더 깔끔합니다.
            }
        }

        private void FixedUpdate()
        {
            if (PlayerHealth.instance != null && PlayerHealth.instance.isDead) return;
            float verticalDir = Input.GetAxisRaw("Vertical");
            bool hasDug = Dig(moveDir, verticalDir);

            if (!hasDug)
            {
                rb.AddForce(new Vector2(moveDir * Time.fixedDeltaTime * moveSpeed, 0));

                // ★★★ 이동 시 체력 소모 로직 추가 ★★★
                // 좌우 이동 입력이 있을 때만
                if (moveDir != 0 && PlayerHealth.instance != null)
                {
                    PlayerHealth.instance.ConsumeHealthForMovement();
                }
            }
        }

        public bool Dig(float directionX, float directionY)
        {
            if (PlayerHealth.instance != null && PlayerHealth.instance.isDead) return false;

            if (directionX == 0 && directionY == 0) return false;
            // 방향 입력이 없으면 아무것도 안 함
            if (directionX == 0 && directionY == 0) return false;

            // --- 쿨다운 확인 로직 추가 ---
            if (Time.time >= nextDigTime)
            {
                Transform currentHitPoint = null;

                if (directionX > 0)
                    currentHitPoint = HitPoint[0];
                else if (directionX < 0)
                    currentHitPoint = HitPoint[1];
                else if (directionY < 0)
                    currentHitPoint = HitPoint[2];

                if (currentHitPoint == null) return false;

                Collider2D overCollider2d = Physics2D.OverlapCircle(currentHitPoint.position, 0.02f, whatisPlatform);

                if (overCollider2d != null)
                {
                    // 타격에 성공하면 다음 타격 가능 시간을 미래로 설정
                    nextDigTime = Time.time + digSpeed;

                    Bricks bricksComponent = overCollider2d.transform.GetComponent<Bricks>();
                    if (bricksComponent != null)
                    {
                        bricksComponent.OnTileHit(currentHitPoint.position);
                        if (PlayerHealth.instance != null)
                        {
                            PlayerHealth.instance.ConsumeHealthForDigging(currentHitPoint.position);
                        }
                        if (directionY < 0)
                        {
                            transform.position += Vector3.down * 0.05f;
                            rb.AddForce(Vector2.down * 2f, ForceMode2D.Impulse);
                        }
                        return true;
                    }
                }
            }
            // 쿨다운 중이거나, 감지된 것이 없으면 false 반환
            return false;
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            // 부딪힌 오브젝트의 태그가 "Item"인지 확인
            if (other.CompareTag("Item"))
            {
                // 부딪힌 오브젝트에서 ItemPickup 스크립트를 가져옴
                ItemPickup itemToPick = other.GetComponent<ItemPickup>();
                Debug.Log("여기까진 된다.");
                // ItemPickup 스크립트가 있다면 (즉, 아이템이라면)
                if (itemToPick != null)
                {
                    Debug.Log("이게되네");
                    // 1. 인벤토리에 아이템 추가 요청 (싱글톤 사용)
                    Inventory.instance.AddItem(itemToPick.itemData);

                    // 2. 월드에 있던 아이템 오브젝트 파괴
                    Destroy(other.gameObject);
                }
            }
        }
        private bool IsGrounded()
        {
            float extraHeight = 0.1f; // 약간의 추가 감지 거리
            
            // 1. 아래쪽 확인
            RaycastHit2D groundCheck = Physics2D.Raycast(transform.position, Vector2.down, 0.5f + extraHeight, whatisPlatform);
            if (groundCheck.collider != null)
            {
                return true; // 아래에 블록이 있으면 점프 가능
            }

            // 2. 왼쪽 확인
            RaycastHit2D leftWallCheck = Physics2D.Raycast(transform.position, Vector2.left, 0.5f + extraHeight, whatisPlatform);
            if (leftWallCheck.collider != null)
            {
                return true; // 왼쪽에 블록이 있으면 점프 가능 (벽점프)
            }

            // 3. 오른쪽 확인
            RaycastHit2D rightWallCheck = Physics2D.Raycast(transform.position, Vector2.right, 0.5f + extraHeight, whatisPlatform);
            if (rightWallCheck.collider != null)
            {
                return true; // 오른쪽에 블록이 있으면 점프 가능 (벽점프)
            }
            
            // 위 3가지 경우에 모두 해당하지 않으면 점프 불가능
            return false;
        }
    }
}