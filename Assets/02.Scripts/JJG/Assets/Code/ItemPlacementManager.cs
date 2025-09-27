using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
namespace JJG{
public class SpecialTileData
{
    public Vector3Int position;
    public int itemPrefabIndex;
    public bool isLooted;
}

public class ItemPlacementManager : MonoBehaviour
{
    [Header("연결 필수")]
    public Tilemap targetTilemap;
    // --- 변수 이름만 직관적으로 변경 ---
    public GameObject hintLightPrefab;
    public List<GameObject> itemPrefabs;

    [Header("설정")]
    [Range(0, 1)]
    public float itemPlacementChance = 0.1f;

    private Dictionary<Vector3Int, SpecialTileData> specialTilesDict;
    private Dictionary<Vector3Int, GameObject> hintObjects;

    void Start()
    {
        PlaceItemsInTiles();
    }

    public void OnTileDestroyed(Vector3Int cellPosition)
    {
        if (specialTilesDict.ContainsKey(cellPosition))
        {
            SpecialTileData data = specialTilesDict[cellPosition];
            if (!data.isLooted)
            {
                if (data.itemPrefabIndex >= 0 && data.itemPrefabIndex < itemPrefabs.Count)
                {
                    GameObject prefabToDrop = itemPrefabs[data.itemPrefabIndex];
                    Vector3 worldPos = targetTilemap.GetCellCenterWorld(cellPosition);
                    Instantiate(prefabToDrop, worldPos, Quaternion.identity);
                }

                data.isLooted = true;
                specialTilesDict[cellPosition] = data;

                if (hintObjects.ContainsKey(cellPosition))
                {
                    Destroy(hintObjects[cellPosition]);
                    hintObjects.Remove(cellPosition);
                }
            }
        }
    }

    private void PlaceItemsInTiles()
    {
        specialTilesDict = new Dictionary<Vector3Int, SpecialTileData>();
        hintObjects = new Dictionary<Vector3Int, GameObject>();

        BoundsInt bounds = targetTilemap.cellBounds;

        foreach (var pos in bounds.allPositionsWithin)
        {
            if (targetTilemap.GetTile(pos) != null)
            {
                if (Random.value < itemPlacementChance)
                {
                    Vector3Int cellPos = new Vector3Int(pos.x, pos.y, pos.z);
                    int randomIndex = Random.Range(0, itemPrefabs.Count);

                    SpecialTileData newData = new SpecialTileData
                    {
                        position = cellPos,
                        itemPrefabIndex = randomIndex,
                        isLooted = false
                    };

                    specialTilesDict.Add(cellPos, newData);

                    // --- 힌트 빛 프리팹을 생성 ---
                    Vector3 hintPosition = targetTilemap.GetCellCenterWorld(cellPos);
                    GameObject hintObj = Instantiate(hintLightPrefab, hintPosition, Quaternion.identity, this.transform);
                    hintObjects.Add(cellPos, hintObj);
                }
            }
        }
        Debug.Log(specialTilesDict.Count + "개의 특별한 타일 배치 완료.");
    }
}
}