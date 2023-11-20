using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CreateStage : MonoBehaviour
{
    [SerializeField, Button("Start")]
    private bool start;

    [SerializeField] // マップサイズ
    private Vector2Int mapSize = Vector2Int.zero;
    [SerializeField] // 部屋の最小サイズ
    private Vector2Int miniSize = Vector2Int.zero;
    [SerializeField] // 部屋の最大サイズ
    private Vector2Int maxSize = Vector2Int.zero;
    [SerializeField] // 壁幅
    private int wallWidth = 0;
    [SerializeField] // 道幅
    private int roadWidth = 0;
    [SerializeField, ReadOnly] // ステージ情報
    private CreateStageData data = null;

    [SerializeField] // 地面タイルマップ
    private Tilemap groundMap = null;
    [SerializeField] // 壁タイルマップ
    private Tilemap wallMap = null;
    [SerializeField] // 地面タイル
    private Tile groundTile = null;
    [SerializeField] // 壁タイル
    private Tile wallTile = null;


    public void Start()
    {
        data = new CreateStageData(mapSize, miniSize, maxSize, wallWidth, roadWidth);

        groundMap.ClearAllTiles();
        wallMap.ClearAllTiles();
        for(int y = 0; y < mapSize.y; y++)
        {
            for(int x = 0; x < mapSize.x; x++)
            {
                if(data.Data[x,y] == 0)
                {
                    groundMap.SetTile(new Vector3Int(x, y, 0), groundTile);
                }
                else if(data.Data[x,y] == 1)
                {
                    wallMap.SetTile(new Vector3Int(x, y, 0), wallTile);
                }
            }
        }
    }
}