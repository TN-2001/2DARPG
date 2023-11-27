using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CreateStage : MonoBehaviour
{
    [SerializeField] // プレイヤーの親
    private Transform playerParent = null;
    [SerializeField] // 敵の親
    private Transform enemyParent = null;
    [SerializeField] // 地面タイルマップ
    private Tilemap groundMap = null;
    [SerializeField] // 壁タイルマップ
    private Tilemap wallMap = null;
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

    [SerializeField, Button("Start")]
    private bool start;

    [SerializeField] // ダンジョンデータ
    private DungeonData dungeonData = null;
    [SerializeField] // プレイヤープレハブ
    private GameObject playerPrefab = null;
    [SerializeField, ReadOnly] // ステージ情報
    private CreateStageData stageData = null;


    public void Start()
    {
        if(GameManager.I)
        {
            dungeonData = GameManager.I.DataBase.DungeonDataList[0];
            playerPrefab = GameManager.I.DataBase.PlayerData.Prefab;
        }
        stageData = new CreateStageData(mapSize, miniSize, maxSize, wallWidth, roadWidth);

        // タイルマップ
        groundMap.ClearAllTiles();
        wallMap.ClearAllTiles();
        for(int y = 0; y < mapSize.y; y++)
        {
            for(int x = 0; x < mapSize.x; x++)
            {
                if(stageData.Data[x,y] == 0)
                {
                    groundMap.SetTile(new Vector3Int(x, y, 0), dungeonData.GroundTile);
                }
                else if(stageData.Data[x,y] == 1)
                {
                    wallMap.SetTile(new Vector3Int(x, y, 0), dungeonData.WallTile);
                }
            }
        }

        // キャラクター配置
        foreach(Transform child in playerParent)
        {
            GameObject.DestroyImmediate(child.gameObject);
        }
        Debug.Log(enemyParent.childCount);
        for(int i = 0; i < enemyParent.childCount; i++)
        {
            GameObject.DestroyImmediate(enemyParent.GetChild(enemyParent.childCount - i - 1).gameObject);
        }
        int playerRoomNumber = Random.Range(0, stageData.RectList.Count);
        for(int i = 0; i < stageData.RectList.Count; i++)
        {
            CreateStageData.Rect.Room room = stageData.RectList[i]._Room;
            if(i == playerRoomNumber)
            {
                GameObject obj = Instantiate(playerPrefab);
                obj.transform.SetParent(playerParent);
                int x = Random.Range(room.Left, room.Right + 1);
                int y = Random.Range(room.Down, room.Up + 1);
                obj.transform.position = new Vector2(x + 0.5f, y + 0.5f);
            }
            else
            {
                int enemyNumber = Random.Range(1, 3);
                for(int j = 0; j < enemyNumber; j++)
                {
                    GameObject obj = Instantiate(
                        dungeonData.EnemyDataList[Random.Range(0, dungeonData.EnemyDataList.Count)].Prefab);
                    obj.transform.SetParent(enemyParent);
                    int x = Random.Range(room.Left, room.Right + 1);
                    int y = Random.Range(room.Down, room.Up + 1);
                    obj.transform.position = new Vector2(x + 0.5f, y + 0.5f);
                }
            }
        }
    }
}