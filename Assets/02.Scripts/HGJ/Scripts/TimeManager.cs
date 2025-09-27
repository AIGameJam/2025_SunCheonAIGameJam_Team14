// TimeManager.cs
using UnityEngine;
using System;
using System.Collections;

public class TimeManager : MonoBehaviour
{
    // 최대 시간: 11시간 = 660분
    private const int MAX_MINUTES = 660;

    // 현재 남은 시간을 저장합니다.
    public int timeLeftInMinutes { get; private set; } = MAX_MINUTES;

    // 턴 종료 이벤트
    public event Action OnTurnEnd;

    private bool isTimeFlowing = false;

    void Start()
    {
        // GameManager가 SetTimeFlow(true)를 호출할 때까지 시간 흐름을 막습니다.
    }

    /// <summary>
    /// 지정된 시간(minutes)만큼 현재 턴의 시간을 소모합니다. (행동 기반 소모)
    /// </summary>
    public void SpendTime(int minutes)
    {
        if (timeLeftInMinutes <= 0) return;

        // 시간 소모
        timeLeftInMinutes -= minutes;

        if (timeLeftInMinutes <= 0)
        {
            timeLeftInMinutes = 0;

            // 시간이 0이 되면 자동 흐름 코루틴을 멈춥니다.
            StopAllCoroutines();

            // 11시간이 모두 지나갔을 경우 자동으로 다음 턴으로 넘어갑니다.
            if (OnTurnEnd != null)
            {
                OnTurnEnd.Invoke();
            }
        }
        Debug.Log($"시간 {minutes}분 소모. 남은 시간: {timeLeftInMinutes}분");
    }

    /// <summary>
    /// 새로운 턴을 시작하며 남은 시간을 최대값으로 초기화하고, 자동 흐름을 시작/재개합니다.
    /// </summary>
    public void StartNewTurn()
    {
        timeLeftInMinutes = MAX_MINUTES;
        Debug.Log("새로운 만조 턴 시작! (시간 초기화 완료)");

        // 자동 흐름이 켜져 있다면, 코루틴을 시작합니다.
        if (isTimeFlowing)
        {
            StartCoroutine(TimeFlowCoroutine());
        }
    }

    /// <summary>
    /// 🚨 [핵심 기능] 1초에 1분씩 시간이 흐르도록 하는 코루틴 로직 🚨
    /// </summary>
    private IEnumerator TimeFlowCoroutine()
    {
        while (isTimeFlowing && timeLeftInMinutes > 0)
        {
            // 1초 대기 (실제 시간 1초)
            yield return new WaitForSeconds(1f);

            // 시간 1분 소모 (게임 시간 1분)
            SpendTime(1);

            Debug.Log("📢 TimeFlowCoroutine running: Spending 1 minute.");
        }
        Debug.Log("📢 TimeFlowCoroutine finished or stopped."); // 코루틴 종료 확인
    }

    /// <summary>
    /// GameManager가 시간 흐름을 제어하는 함수
    /// </summary>
    public void SetTimeFlow(bool flowing)
    {
        if (isTimeFlowing == flowing) return;

        isTimeFlowing = flowing;

        if (flowing)
        {
            // 시간이 흐르도록 설정되었을 때 코루틴 시작
            if (timeLeftInMinutes > 0)
            {
                // 이전에 실행 중이던 코루틴이 있다면 멈추고 새로 시작 (안전성 확보)
                StopAllCoroutines();
                StartCoroutine(TimeFlowCoroutine());
            }
        }
        else
        {
            // 시간이 멈추도록 설정
            StopAllCoroutines();
        }
        Debug.Log($"시간 자동 흐름 설정: {flowing}");
    }
}