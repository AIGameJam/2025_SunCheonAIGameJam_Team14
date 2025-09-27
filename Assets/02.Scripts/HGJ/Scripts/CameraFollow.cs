// CameraFollow.cs
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Inspector에서 PlayerRoot의 Transform을 연결할 필드
    [Header("팔로우할 대상")]
    public Transform target;

    [Header("카메라 고정 값")]
    public float yOffset = 0f;
    private float zOffset;

    void Awake()
    {
        // 카메라의 초기 Z축 위치를 고정값으로 설정
        zOffset = transform.position.z;
    }

    // FixedUpdate는 부드러운 카메라 이동을 위해 사용합니다.
    void FixedUpdate()
    {
        if (target == null) return;

        // 1. 캐릭터의 현재 X축 위치를 가져옵니다.
        float targetX = target.position.x;

        // 2. 카메라의 새로운 위치를 계산합니다.
        Vector3 newPosition = new Vector3(
            targetX,           // X축은 캐릭터를 따라갑니다.
            transform.position.y + yOffset,
            zOffset            // Z축은 고정값을 유지합니다.
        );

        // 3. 카메라 위치 업데이트
        transform.position = newPosition;
    }
}