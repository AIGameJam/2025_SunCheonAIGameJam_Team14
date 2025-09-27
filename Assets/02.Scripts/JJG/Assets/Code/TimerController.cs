using UnityEngine;
using UnityEngine.UI; // UI.Image를 사용하기 위해 필요
using TMPro;
namespace JJG
{

    public class TimerController : MonoBehaviour
    {
        [Header("연결 필수")]
        public Image timerCircle;
        public TextMeshProUGUI timerText;

        [Header("설정")]
        [Tooltip("전체 시간 (분 단위)")]
        public int startMinutes = 11;

        // 현재 남은 분과 초
        private int currentMinutes;
        private float currentSeconds;

        void Start()
        {
            // 타이머 초기화
            ResetTimer();
        }

        void Update()
        {
            // 남은 시간이 있다면 (분 또는 초가 0보다 크면)
            if (currentMinutes > 0 || currentSeconds > 0)
            {
                // 초를 감소시킴
                currentSeconds -= Time.deltaTime;

                // 원형 이미지는 현재 '초'의 진행 상태를 표시 (60초 -> 0초)
                timerCircle.fillAmount = currentSeconds / 60f;

                // 만약 초가 0보다 작아졌다면 (1분이 지났다면)
                if (currentSeconds <= 0)
                {
                    // 분을 1 감소시키고
                    currentMinutes--;
                    // 텍스트를 새로운 '분'으로 업데이트
                    timerText.text = currentMinutes.ToString();
                    
                    // 만약 아직 시간이 남았다면, 초를 다시 60으로 리셋
                    if (currentMinutes > 0)
                    {
                        currentSeconds = 60f;
                    }
                    else // 시간이 모두 종료되었다면
                    {
                        currentSeconds = 0;
                        Debug.Log("타이머 종료!");
                    }
                }
            }
        }

        // 타이머를 리셋하는 함수
        public void ResetTimer()
        {
            currentMinutes = startMinutes;
            currentSeconds = 60f; // 60초부터 시작
            timerText.text = currentMinutes.ToString();
        }
    }
}