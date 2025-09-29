// PointManager.cs
using UnityEngine;
using System.Linq;

[System.Serializable]
public struct FishingPointData
{
    public Transform position;
    public int depthLevel;
}

// 🚨 [추가] 단계별 획득물 목록을 저장하는 구조체 🚨
[System.Serializable]
public struct CatchableCreatureSet
{
    public int depthLevel; // 이 목록이 적용될 단계 (예: 1, 2, 3)
    public ItemScriptableObject[] creaturePrefabs; // 이 단계에서 획득 가능한 생물 프리팹 목록 (사각형 프리팹 연결)
}

public class PointManager : MonoBehaviour
{
    public FishingPointData[] points = new FishingPointData[3];
    public int currentPointIndex { get; private set; } = 0;

    // 🚨 [추가] Inspector에서 설정할 획득물 데이터베이스 🚨
    [Header("단계별 획득물 목록")]
    public CatchableCreatureSet[] creatureSetsByDepth;


    /// <summary>
    /// 지정된 단계 레벨에 해당하는 획득물 프리팹 배열을 반환합니다.
    /// </summary>
    public ItemScriptableObject[] GetCreaturesByDepth(int targetDepthLevel)
    {
        // Linq를 사용하여 targetDepthLevel과 일치하는 획득물 세트를 찾습니다.
        CatchableCreatureSet set = creatureSetsByDepth
            .FirstOrDefault(s => s.depthLevel == targetDepthLevel);

        if (set.creaturePrefabs == null)
        {
            return new ItemScriptableObject[0];
        }

        return set.creaturePrefabs;
    }

    // ... (SelectPoint, GetCurrentPointDepth, GetCurrentPointPosition 함수 유지) ...
    public void SelectPoint(int direction)
    {
        int newIndex = currentPointIndex + direction;
        if (newIndex >= 0 && newIndex < points.Length)
        {
            currentPointIndex = newIndex;
            Debug.Log($"[PointManager] 포인트 {currentPointIndex + 1} 선택됨.");
        }
    }

    public int GetCurrentPointDepth()
    {
        return points[currentPointIndex].depthLevel;
    }

    public Vector3 GetCurrentPointPosition()
    {
        return points[currentPointIndex].position.position;
    }
}