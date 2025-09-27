using UnityEngine;
namespace JJG
{
    public class WaterSystem : MonoBehaviour
    {
        // 싱글톤으로 만들어 어디서든 쉽게 접근
        public static WaterSystem instance;

        [Header("수위 설정")]
        [Tooltip("물이 차오르기 시작하는 Y 좌표")]
        public float startWaterY = -100f;
        [Tooltip("초당 물이 차오르는 속도")]
        public float riseSpeed = 0.5f;

        // 현재 물의 Y좌표 (읽기 전용)
        public float CurrentWaterLevel { get; private set; }

        void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(gameObject);

            CurrentWaterLevel = startWaterY;
        }

        void Update()
        {
            // 매 프레임마다 시간에 비례하여 수위를 올림
            CurrentWaterLevel += riseSpeed * Time.deltaTime;
        }

        // (선택 사항) 플레이어의 현재 위치와 수위를 비교하는 함수
        public bool IsPlayerSubmerged(Vector3 playerPosition)
        {
            return playerPosition.y < CurrentWaterLevel;
        }
    }
}