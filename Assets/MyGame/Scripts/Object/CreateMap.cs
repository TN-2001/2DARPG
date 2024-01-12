using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public partial class CreateMap : MonoBehaviour
{
    [SerializeField] // プレイヤーオブジェクト
    private Transform playerTra = null;
    [SerializeField] // 敵の親
    private Transform enemyParent = null;
    [SerializeField] // 階段
    private CollisionDetector stairDetector = null;
    [SerializeField] // 地面タイルマップ
    private Tilemap groundMap = null;
    [SerializeField] // 壁タイルマップ
    private Tilemap wallMap = null;
    [SerializeField] // オブジェクトタイルマップ
    private Tilemap objectMap = null;
    [SerializeField] // ボスグリッド
    private GameObject bossGrid = null;

    [SerializeField] // ダンジョンデータ
    private DungeonData dungeonData = null;
    // ダンジョン階数
    private int floorNumber = 0;
    // クリアフラグ
    private bool isClear = false;

    [SerializeField, Button("InitializeRandomMap")]
    private bool initializeRandomMap;


    private void Start()
    {
        if(dungeonData.FloorNumber > 0)
        {
            InitializeRandomMap();
        }
        else
        {
            InitializeBossMap();
        }

        stairDetector.GetComponent<SpriteRenderer>().sprite = dungeonData.StairSprite;
    }

    private void Update()
    {
        if(stairDetector.IsCollosion & GameManager.I.Input.actions["Attack"].WasPressedThisFrame())
        {
            if(floorNumber < dungeonData.FloorNumber)
            {
                GameManager.I.Fade(InitializeRandomMap, true, null);
            }
            else if(!isClear)
            {
                GameManager.I.Fade(InitializeBossMap, true, null);
            }
            else
            {
                GameManager.I.Fade(delegate{SceneManager.LoadScene("Home");}, true, "Normal");
            }
        }
    }

    public void InitializeRandomMap()
    {
        if(GameManager.I)
        {
            dungeonData = GameManager.I.CurrentDungeon;
        }
        CreateMapData();

        // タイルマップ
        groundMap.ClearAllTiles();
        wallMap.ClearAllTiles();
        objectMap.ClearAllTiles();
        for(int y = 0; y < mapSize.y; y++)
        {
            for(int x = 0; x < mapSize.x; x++)
            {
                groundMap.SetTile(new Vector3Int(x, y, 0), dungeonData.GroundTile);
                if(data[x,y] == 1)
                {
                    wallMap.SetTile(new Vector3Int(x, y, 0), dungeonData.WallTile);

                    if(y > 2 & x > 2 & x < mapSize.x - 2)
                    {
                        if(data[x,y-1] == 0 | data[x,y-2] == 0)
                        {
                            wallMap.SetTile(new Vector3Int(x, y, 0), null);
                        }

                        if(dungeonData.WallUpTile.m_TilingRules.Count == 9)
                        {
                            if(data[x,y-1] == 0 | data[x,y-2] == 0 | data[x,y-3] == 0)
                            {
                                objectMap.SetTile(new Vector3Int(x, y, 0), dungeonData.WallUpTile);
                                if(data[x-1,y] == 1)
                                {
                                    objectMap.SetTile(new Vector3Int(x-1, y, 0), dungeonData.WallUpTile);
                                }
                                if(data[x+1,y] == 1)
                                {
                                    objectMap.SetTile(new Vector3Int(x+1, y, 0), dungeonData.WallUpTile);
                                }
                            }
                        }
                        else if(dungeonData.WallUpTile.m_TilingRules.Count == 6)
                        {
                            if(data[x,y-1] == 0 | data[x,y-2] == 0)
                            {
                                objectMap.SetTile(new Vector3Int(x, y, 0), dungeonData.WallUpTile);
                                if(data[x-1,y] == 1)
                                {
                                    objectMap.SetTile(new Vector3Int(x-1, y, 0), dungeonData.WallUpTile);
                                }
                                if(data[x+1,y] == 1)
                                {
                                    objectMap.SetTile(new Vector3Int(x+1, y, 0), dungeonData.WallUpTile);
                                }
                            }
                        }

                    }
                }
            }
        }

        // キャラクター配置
        while(enemyParent.childCount > 0)
        {
            GameObject.DestroyImmediate(enemyParent.GetChild(0).gameObject);
        }
        int playerRoomNumber = Random.Range(0, rectList.Count);
        for(int i = 0; i < rectList.Count; i++)
        {
            Rect.Room room = rectList[i]._Room;
            if(i == playerRoomNumber)
            {
                int x = Random.Range(room.Left, room.Right + 1);
                int y = Random.Range(room.Down, room.Up + 1);
                playerTra.position = new Vector2(x + 0.5f, y + 0.5f);
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

        // 階段配置
        Rect.Room sRoom = rectList[Random.Range(0, rectList.Count)]._Room;
        int xPos = Random.Range(sRoom.Left, sRoom.Right + 1);
        int yPos = Random.Range(sRoom.Down, sRoom.Up + 1);
        stairDetector.transform.position = new Vector2(xPos + 0.5f, yPos + 0.5f);

        // UI
        floorNumber ++;
        DungeonUI.I?.UpdateDungeonText($"{dungeonData.Name} {floorNumber}F");
    }

    private void InitializeBossMap()
    {
        // タイルマップ
        groundMap.gameObject.SetActive(false);
        wallMap.gameObject.SetActive(false);
        objectMap.gameObject.SetActive(false);
        Instantiate(dungeonData.BossMap, dungeonData.BossMap.transform.position, Quaternion.identity, bossGrid.transform);

        // キャラクター配置
        while(enemyParent.childCount > 0)
        {
            GameObject.DestroyImmediate(enemyParent.GetChild(0).gameObject);
        }
        Instantiate(dungeonData.BossEnemyData.Prefab, new Vector2(0f, 3f), Quaternion.identity, enemyParent);
        playerTra.position = Vector2.zero;

        // 階段配置
        stairDetector.gameObject.SetActive(false);

        // UI
        DungeonUI.I?.UpdateDungeonText($"{dungeonData.Name} 深層");

        StartCoroutine(EBossEvent());
    }
    private IEnumerator EBossEvent()
    {
        yield return new WaitUntil(() => enemyParent.childCount == 0);

        isClear = true;

        // 階段配置
        stairDetector.transform.position = new Vector2(0.5f, 3.5f);
        stairDetector.gameObject.SetActive(true);

        yield break;
    }
}