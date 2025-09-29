using UnityEngine;
namespace JJG
{

    public class MapLoadManager : MonoBehaviour
    {
        // 씬에 있는 모든 주요 컴포넌트를 연결
        public Bricks bricksManager;
        public ItemPlacementManager itemPlacementManager;
        public PlayerInput playerInput; // 새로 만들 PlayerInput

        void Start()
        {
            // 1. 지형 로드
            bricksManager.LoadTiles();
            
            // 2. 아이템 배치
            itemPlacementManager.PlaceItemsInTiles();
            
            Debug.Log("모든 맵 준비 완료. 게임 시작!");
        }

        // LateUpdate는 모든 Update 함수가 실행된 후 마지막에 호출됩니다.
        // 따라서 입력과 물리 업데이트가 끝난 뒤 행동을 처리하기에 가장 안정적입니다.
        void LateUpdate()
        {
            // PlayerInput으로부터 파괴 요청이 있었는지 확인
            if (playerInput.IsDigRequested())
            {
                // 파괴할 위치 정보를 받아와서 Bricks에게 파괴 명령
                Vector3Int targetPosition = playerInput.GetDigTargetPosition();
                bricksManager.OnTileHit(targetPosition);
            }
        }
    }
}

