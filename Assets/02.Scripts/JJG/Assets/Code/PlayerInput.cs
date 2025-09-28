using UnityEngine;
using UnityEngine.Tilemaps;
namespace JJG
{
    public class PlayerInput : MonoBehaviour
    {
        public LayerMask whatisPlatform;
        public Transform[] HitPoint; // PlayerMovement에서 이쪽으로 이동

        private bool digRequested = false;
        private Vector3Int digTargetPosition;

        void Update()
        {
            // 매 프레임 파괴 요청 상태를 초기화
            digRequested = false;

            // 1. 파괴할 타일 '탐색'
            Transform currentHitPoint = GetCurrentHitPoint();
            if (currentHitPoint != null)
            {
                Collider2D overCollider2d = Physics2D.OverlapCircle(currentHitPoint.position, 0.02f, whatisPlatform);
                if (overCollider2d != null)
                {
                    // 파괴할 위치를 찾음
                    digTargetPosition = overCollider2d.GetComponent<Tilemap>().WorldToCell(currentHitPoint.position);

                    // 2. 파괴 키('C')가 눌리면 '요청' 상태를 true로 변경
                    if (Input.GetKeyDown(KeyCode.C))
                    {
                        digRequested = true;
                    }
                }
            }
        }

        // 총괄 매니저가 호출할 함수들
        public bool IsDigRequested() => digRequested;
        public Vector3Int GetDigTargetPosition() => digTargetPosition;

        private Transform GetCurrentHitPoint()
        {
            float moveDir = Input.GetAxisRaw("Horizontal");
            float verticalDir = Input.GetAxisRaw("Vertical");
            
            if (moveDir > 0) return HitPoint[0];
            if (moveDir < 0) return HitPoint[1];
            if (verticalDir < 0) return HitPoint[2];
            return null;
        }
    }
}
