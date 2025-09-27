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
        private ItemPlacementManager itemManager;
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
        damagedTilesDict[cellPosition] = currentHealth;

        if (currentHealth <= 0)
        {
            tilemap.SetTile(cellPosition, null);
            damagedTilesDict.Remove(cellPosition);

            if (itemManager != null)
            {
                itemManager.OnTileDestroyed(cellPosition);
            }
        }
        else
        {
            // --- 색상 변경 대신 '타일 교체' 로직 ---
            if (crackTiles.Length > 0)
            {
                // 손상도를 0.0 ~ 1.0 비율로 계산
                float damagePercentage = 1f - ((float)currentHealth / maxHealth);
                // 비율에 맞는 금 간 타일 인덱스 계산
                int crackIndex = Mathf.FloorToInt(damagePercentage * crackTiles.Length);
                // 인덱스가 배열 범위를 벗어나지 않도록 보정
                crackIndex = Mathf.Clamp(crackIndex, 0, crackTiles.Length - 1);

                // 계산된 인덱스의 금 간 타일로 교체
                tilemap.SetTile(cellPosition, crackTiles[crackIndex]);
            }
        }
        
        SaveTiles();
    }
    
    private void LoadTiles()
    {
        if (!File.Exists(savePath)) return;

        string json = File.ReadAllText(savePath);
        TileSaveData loadedData = JsonUtility.FromJson<TileSaveData>(json);
        
        damagedTilesDict.Clear();
        foreach (var tileData in loadedData.damagedTiles)
        {
            damagedTilesDict.Add(tileData.position, tileData.remainingHealth);
            
            // --- 저장된 손상 상태에 맞춰 금 간 타일 복원 ---
            if (crackTiles.Length > 0)
            {
                int maxHealth = baseHealth - (tileData.position.y / depthMultiplier);
                if (maxHealth < 1) maxHealth = 1;
                
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
