using UnityEngine;
using System.IO; // 파일 시스템에 접근하기 위해 필요
using UnityEngine.SceneManagement; // 씬을 관리하기 위해 필요
namespace JJG
{
    public class MapResetter : MonoBehaviour
    {
        // 버튼을 누르면 호출될 공개 함수
        public void ResetMapData()
        {
            // Bricks.cs에 있는 저장 경로와 ★완벽히 동일하게★ 작성해야 합니다.
            string savePath = Path.Combine(Application.persistentDataPath, "tilemap_data.json");

            // 파일이 실제로 존재하는지 확인
            if (File.Exists(savePath))
            {
                // 파일 삭제
                File.Delete(savePath);
                Debug.Log("<color=orange>맵 데이터 파일 삭제 완료!</color> 씬을 다시 시작합니다.");
            }
            else
            {
                Debug.Log("삭제할 맵 데이터 파일이 없습니다. 씬을 다시 시작합니다.");
            }

            // 현재 씬을 다시 로드하여 변경사항을 즉시 적용
            // SceneManager.GetActiveScene().name은 현재 씬의 이름을 가져옵니다.
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}