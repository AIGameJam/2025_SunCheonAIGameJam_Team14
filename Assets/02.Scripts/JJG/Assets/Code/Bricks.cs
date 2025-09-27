using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.IO;

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
        // effectManager = FindObjectOfType<DestructionEffectManager>(); // (향후 추가)
        
        LoadTiles();
    }

    public void OnTileHit(Vector3 worldPos)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(worldPos);
        if (tilemap.GetTile(cellPosition) == null) return;

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
            // 타일 파괴
            tilemap.SetTile(cellPosition, null); 
            damagedTilesDict.Remove(cellPosition); 

            // (향후 추가) effectManager?.ClearEffect(cellPosition);

            if (itemManager != null)
            {
                itemManager.OnTileDestroyed(cellPosition);
            }
        }
        else 
        {
            // (향후 추가) effectManager?.UpdateEffect(cellPosition, (float)currentHealth / maxHealth);
        }
        
        SaveTiles();
    }
    
    // (이하 SaveTiles, LoadTiles, OnApplicationQuit 함수는 이전과 동일하며 색상 관련 코드는 없습니다.)
    
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

    private void LoadTiles()
    {
        if (!File.Exists(savePath)) return;

        string json = File.ReadAllText(savePath);
        TileSaveData loadedData = JsonUtility.FromJson<TileSaveData>(json);
        
        damagedTilesDict.Clear();
        foreach (var tileData in loadedData.damagedTiles)
        {
            damagedTilesDict.Add(tileData.position, tileData.remainingHealth);
            // (향후 추가) 불러온 데이터를 바탕으로 파괴 효과도 복원해야 함
            // int maxHealth = baseHealth - (tileData.position.y / depthMultiplier);
            // if (maxHealth < 1) maxHealth = 1;
            // effectManager?.UpdateEffect(tileData.position, (float)tileData.remainingHealth / maxHealth);
        }
    }

    private void OnApplicationQuit()
    {
        SaveTiles();
    }
}