using UnityEngine;
namespace JJG
{
    public class WaterVisual : MonoBehaviour
    {
        void Update()
        {
            // WaterSystem에 접근하여 현재 수위 정보를 가져옴
            if (WaterSystem.instance != null)
            {
                float waterLevel = WaterSystem.instance.CurrentWaterLevel;

                // 현재 오브젝트의 Y 위치를 수위와 동기화
                transform.position = new Vector3(transform.position.x, waterLevel, transform.position.z);
            }
        }
    }
}