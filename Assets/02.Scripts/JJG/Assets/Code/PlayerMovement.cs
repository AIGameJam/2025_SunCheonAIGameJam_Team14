using UnityEngine;

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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.linearVelocity += new Vector2(0, jumpPower);
        }
    }

    private void FixedUpdate()
    {
        float verticalDir = Input.GetAxisRaw("Vertical");
        bool hasDug = Dig(moveDir, verticalDir);

        if (!hasDug)
        {
            rb.AddForce(new Vector2(moveDir * Time.fixedDeltaTime * moveSpeed, 0));
        }
    }

    public bool Dig(float directionX, float directionY)
    {
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
}