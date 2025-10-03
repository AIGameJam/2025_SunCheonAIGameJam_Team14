using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.IO;
namespace JJG
{
    // 저장용 데이터 구조: 이제 위치와 '남은 체력'을 함께 저장합니다.
    [System.Serializable]
    public class DamagedTileData
    {
        public Vector3Int position;
        public int remainingHealth;
    }

    [System.Serializable]
    public class TileSaveData
    {
        public List<DamagedTileData> damagedTiles;
    }



    public class Bricks : MonoBehaviour
    {
        [Header("연결 필수")]
        public Tilemap tilemap;
        public ItemPlacementManager itemManager;
        // (향후 추가) 파괴 효과를 관리할 매니저
        // private DestructionEffectManager effectManager; 

        [Header("블록 내구도 설정")]
        public int baseHealth = 3;
        public int depthMultiplier = 5;
        [Header("파괴 효과 설정")]
    [Tooltip("손상 단계별 타일 에셋 (가장 약한 손상부터 순서대로)")]
    public Tile[] crackTiles; 
        private Dictionary<Vector3Int, int> damagedTilesDict = new Dictionary<Vector3Int, int>();
        private string savePath;

 void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        savePath = Path.Combine(Application.persistentDataPath, "tilemap_data.json");
    }

    void Start()
    {
        itemManager = FindObjectOfType<ItemPlacementManager>();
        LoadTiles();
    }

        // Bricks.cs

        public void OnTileHit(Vector3 worldPos)
        {
            Vector3Int cellPosition = tilemap.WorldToCell(worldPos);
            TileBase currentTile = tilemap.GetTile(cellPosition);
            if (currentTile == null) return;

            int maxHealth = baseHealth - (cellPosition.y / depthMultiplier);
            if (maxHealth < 1) maxHealth = 1;

            int currentHealth;
            if (!damagedTilesDict.ContainsKey(cellPosition))
            {
                currentHealth = maxHealth;
            }
            else
            {
                currentHealth = damagedTilesDict[cellPosition];
            }

            currentHealth--;
            damagedTilesDict[cellPosition] = currentHealth; // 체력이 0이 되어도 일단 기록합니다.

            if (currentHealth <= 0)
            {
                tilemap.SetTile(cellPosition, null);
                // damagedTilesDict.Remove(cellPosition); // <<< 이 줄을 주석 처리하거나 삭제하세요!

                if (itemManager != null)
                {
                    itemManager.OnTileDestroyed(cellPosition);
                }
            }
            else
            {
                if (crackTiles.Length > 0)
                {
                    float damagePercentage = 1f - ((float)currentHealth / maxHealth);
                    int crackIndex = Mathf.FloorToInt(damagePercentage * crackTiles.Length);
                    crackIndex = Mathf.Clamp(crackIndex, 0, crackTiles.Length - 1);
                    tilemap.SetTile(cellPosition, crackTiles[crackIndex]);
                }
            }

            SaveTiles();
        }

        // Bricks.cs

        public void LoadTiles()
        {
            if (!File.Exists(savePath)) return;

            string json = File.ReadAllText(savePath);
            TileSaveData loadedData = JsonUtility.FromJson<TileSaveData>(json);

            damagedTilesDict.Clear();
            foreach (var tileData in loadedData.damagedTiles)
            {
                damagedTilesDict.Add(tileData.position, tileData.remainingHealth);

                // ▼▼▼ 로드 로직 수정 ▼▼▼
                if (tileData.remainingHealth <= 0)
                {
                    // 체력이 0 이하면 파괴된 상태이므로 타일을 제거합니다.
                    tilemap.SetTile(tileData.position, null);
                }
                else if (crackTiles.Length > 0)
                {
                    // 체력이 0보다 크면 기존 로직대로 금 간 타일을 복원합니다.
                    int maxHealth = baseHealth - (tileData.position.y / depthMultiplier);
                    if (maxHealth < 1) maxHealth = 2;

                    float damagePercentage = 1f - ((float)tileData.remainingHealth / maxHealth);
                    int crackIndex = Mathf.FloorToInt(damagePercentage * crackTiles.Length);
                    crackIndex = Mathf.Clamp(crackIndex, 0, crackTiles.Length - 1);
                    tilemap.SetTile(tileData.position, crackTiles[crackIndex]);
                }
            }
        }
        // Bricks.cs 클래스 내부

        // (LoadTiles 함수 아래에 추가)
        private void SaveTiles()
    {
        TileSaveData saveData = new TileSaveData();
        saveData.damagedTiles = new List<DamagedTileData>();
        foreach (var tile in damagedTilesDict)
        {
            saveData.damagedTiles.Add(new DamagedTileData { position = tile.Key, remainingHealth = tile.Value });
        }
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, json);
    }
        private void OnApplicationQuit()
        {
            SaveTiles();
        }
    }
}
