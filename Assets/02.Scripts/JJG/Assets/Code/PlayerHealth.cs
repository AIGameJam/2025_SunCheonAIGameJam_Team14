using UnityEngine;

namespace JJG
{
    public class PlayerHealth : MonoBehaviour
    {
        public static PlayerHealth instance;

        [Header("체력 설정")]
        public float maxHealth = 500f;
        public float currentHealth;
        public bool isDead = false; // 플레이어 사망 상태 플래그 추가

        [Header("행동 소모량")]
        public float moveCostPerSecond = 10f;
        public float baseDigCost = 5f;
        public float digCostDepthMultiplier = 10f;

        [Header("침수 피해")]
        public float drownDamageInterval = 10f;
        [Range(0, 1)]
        public float drownDamagePercent = 0.3f;
        private float drownTimer = 0f;

        [Header("게임 오버 UI (연결 필수)")]
        public GameObject gameOverPanel; // 1단계에서 만든 게임 오버 이미지 오브젝트

        void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(gameObject);
        }

        void Start()
        {
            currentHealth = maxHealth;
            isDead = false;
            // 게임 시작 시 게임 오버 패널은 비활성화
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }
        }

        void Update()
        {
            // ★★★ 플레이어가 죽은 상태면 더 이상 아무것도 하지 않음 ★★★
            if (isDead) return;

            // ... (기존 침수 피해 로직) ...
            if (WaterSystem.instance != null)
            {
                Vector3 headPosition = transform.position + Vector3.up;
                if (WaterSystem.instance.IsPlayerSubmerged(headPosition))
                {
                    drownTimer += Time.deltaTime;
                    if (drownTimer >= drownDamageInterval)
                    {
                        TakeDamage(maxHealth * drownDamagePercent);
                        drownTimer = 0f;
                        Debug.Log("침수 피해! 남은 체력: " + currentHealth);
                    }
                }
                else
                {
                    drownTimer = 0f;
                }
            }
        }

        public void TakeDamage(float amount)
        {
            // ★★★ 플레이어가 죽은 상태면 더 이상 피해를 받지 않음 ★★★
            if (isDead) return;

            currentHealth -= amount;

            // ★★★ 체력이 0 이하로 떨어지지 않도록 보정 ★★★
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Die(); // 사망 함수 호출
            }
            Debug.Log("피해 입음! 남은 체력: " + currentHealth);
        }

        public void Heal(float amount)
        {
            if (isDead) return;
            currentHealth += amount;
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
        }

        public void ConsumeHealthForMovement()
        {
            if (isDead) return; // 죽은 상태면 소모 안 함
            TakeDamage(moveCostPerSecond * Time.deltaTime);
        }

        public void ConsumeHealthForDigging(Vector3 digPosition)
        {
            if (isDead) return; // 죽은 상태면 소모 안 함
            float depthCost = -digPosition.y / digCostDepthMultiplier;
            float totalCost = baseDigCost + depthCost;
            if (totalCost < baseDigCost) totalCost = baseDigCost; // 최소 기본 소모량 보장

            TakeDamage(totalCost); // TakeDamage 함수를 통해 체력 깎음
        }

        // ★★★ 플레이어 사망 처리 함수 ★★★
        void Die()
        {
            if (isDead) return; // 이미 죽은 상태면 다시 처리하지 않음

            isDead = true;
            Debug.Log("플레이어 사망!");

            // 1. 모든 조작 불가능하게 만들기: 플레이어 오브젝트 비활성화
            // 또는 PlayerMovement 스크립트만 비활성화
            if (GetComponent<PlayerMovement>() != null)
            {
                GetComponent<PlayerMovement>().enabled = false;
            }
            // 이 외의 다른 플레이어 조작 스크립트가 있다면 해당 스크립트들도 비활성화해야 합니다.
            // 예: GetComponent<PlayerAttack>().enabled = false;

            // 2. 게임 오버 UI 띄우기
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }

            // (선택 사항) 게임 시간 정지
            // Time.timeScale = 0f;
        }
    }
}


