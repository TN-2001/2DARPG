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
    private EventController stairController = null;
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

    [SerializeField, Button("InitRandomMap")]
    private bool initRandomMap;
    [SerializeField, Button("InitBossMap")]
    private bool initBossMap;


    private void Start()
    {
        if(dungeonData.FloorNumber > 0) InitRandomMap();
        else InitBossMap();

        stairController.GetComponent<SpriteRenderer>().sprite = dungeonData.StairSprite;
        stairController.onDo.AddListener(delegate{
            if(floorNumber < dungeonData.FloorNumber) FadeUI.I.Fade(InitRandomMap);
            else if(!isClear) FadeUI.I.Fade(InitBossMap);
            else FadeUI.I.FadeIn(delegate{SceneManager.LoadScene("Home");});
        });
    }

    public void InitRandomMap()
    {
        if(GameManager.I) dungeonData = GameManager.I.CurrentDungeon;
        CreateMapData();

        // タイルマップ
        groundMap.ClearAllTiles();
        wallMap.ClearAllTiles();
        objectMap.ClearAllTiles();
        for(int y = 0; y < mapSize.y; y++){
            for(int x = 0; x < mapSize.x; x++){
                groundMap.SetTile(new Vector3Int(x, y, 0), dungeonData.GroundTile);
                if(data[x,y] == 1){
                    wallMap.SetTile(new Vector3Int(x, y, 0), dungeonData.WallTile);

                    if(y > 2 & x > 2 & x < mapSize.x - 2){
                        if(data[x,y-1] == 0 | data[x,y-2] == 0)
                            wallMap.SetTile(new Vector3Int(x, y, 0), null);

                        if(dungeonData.WallUpTile.m_TilingRules.Count == 9){
                            if(data[x,y-1] == 0 | data[x,y-2] == 0 | data[x,y-3] == 0){
                                objectMap.SetTile(new Vector3Int(x, y, 0), dungeonData.WallUpTile);
                                if(data[x-1,y] == 1)
                                    objectMap.SetTile(new Vector3Int(x-1, y, 0), dungeonData.WallUpTile);
                                if(data[x+1,y] == 1)
                                    objectMap.SetTile(new Vector3Int(x+1, y, 0), dungeonData.WallUpTile);
                            }
                        }
                        else if(dungeonData.WallUpTile.m_TilingRules.Count == 6){
                            if(data[x,y-1] == 0 | data[x,y-2] == 0){
                                objectMap.SetTile(new Vector3Int(x, y, 0), dungeonData.WallUpTile);
                                if(data[x-1,y] == 1)
                                    objectMap.SetTile(new Vector3Int(x-1, y, 0), dungeonData.WallUpTile);
                                if(data[x+1,y] == 1)
                                    objectMap.SetTile(new Vector3Int(x+1, y, 0), dungeonData.WallUpTile);
                            }
                        }

                    }
                }
            }
        }

        // キャラクター配置
        while(enemyParent.childCount > 0)
            GameObject.DestroyImmediate(enemyParent.GetChild(0).gameObject);
        int playerRoomNumber = Random.Range(0, rectList.Count);
        for(int i = 0; i < rectList.Count; i++){
            Rect.Room room = rectList[i]._Room;
            if(i == playerRoomNumber){
                int x = Random.Range(room.Left, room.Right + 1);
                int y = Random.Range(room.Down, room.Up + 1);
                playerTra.position = new Vector2(x + 0.5f, y + 0.5f);
            }
            else{
                int enemyNumber = Random.Range(1, 3);
                for(int j = 0; j < enemyNumber; j++){
                    EnemyData data = dungeonData.EnemyDataList[Random.Range(0, dungeonData.EnemyDataList.Count)];
                    GameObject obj = Instantiate(data.Prefab);
                    obj.transform.SetParent(enemyParent);
                    int x = Random.Range(room.Left, room.Right + 1);
                    int y = Random.Range(room.Down, room.Up + 1);
                    obj.transform.position = new Vector2(x + 0.5f, y + 0.5f);
                    obj.GetComponent<EnemyController>().Init(new Enemy(data));
                }
            }
        }

        // 階段配置
        Rect.Room sRoom = rectList[Random.Range(0, rectList.Count)]._Room;
        int xPos = Random.Range(sRoom.Left, sRoom.Right + 1);
        int yPos = Random.Range(sRoom.Down, sRoom.Up + 1);
        stairController.transform.position = new Vector2(xPos + 0.5f, yPos + 0.5f);

        // UI
        floorNumber ++;
    }

    public void InitBossMap()
    {
        Vector2Int mapSize = new Vector2Int(25, 18);
        int[,] map = new int[mapSize.x,mapSize.y];
        for(int y = 0; y < mapSize.y; y++){
            for(int x = 0; x < mapSize.x; x++){
                map[x,y] = 1;
                if(5 <= x & x < mapSize.x - 5 & 5 <= y & y < mapSize.y - 5)
                    map[x,y] = 0;
            }
        }
        // タイルマップ
        groundMap.ClearAllTiles();
        wallMap.ClearAllTiles();
        objectMap.ClearAllTiles();
        for(int y = 0; y < mapSize.y; y++){
            for(int x = 0; x < mapSize.x; x++){
                groundMap.SetTile(new Vector3Int(x, y, 0), dungeonData.GroundTile);
                if(map[x,y] == 1)
                    wallMap.SetTile(new Vector3Int(x, y, 0), dungeonData.WallTile);
            }
        }
        for(int y = 0; y < mapSize.y; y++){
            for(int x = 0; x < mapSize.x; x++){
                if(map[x,y] == 0){
                    if(map[x,y+1] == 1){
                        wallMap.SetTile(new Vector3Int(x, y+1, 0), null);
                        wallMap.SetTile(new Vector3Int(x, y+2, 0), null);
                        objectMap.SetTile(new Vector3Int(x, y+1, 0), dungeonData.WallUpTile);
                        objectMap.SetTile(new Vector3Int(x, y+2, 0), dungeonData.WallUpTile);
                        if(dungeonData.WallUpTile.m_TilingRules.Count == 9)
                            objectMap.SetTile(new Vector3Int(x, y+3, 0), dungeonData.WallUpTile);
                        if(map[x+1,y] == 1){
                            objectMap.SetTile(new Vector3Int(x+1, y+1, 0), dungeonData.WallUpTile);
                            objectMap.SetTile(new Vector3Int(x+1, y+2, 0), dungeonData.WallUpTile);
                            if(dungeonData.WallUpTile.m_TilingRules.Count == 9)
                                objectMap.SetTile(new Vector3Int(x+1, y+3, 0), dungeonData.WallUpTile);
                        }
                        if(map[x-1,y] == 1){
                            objectMap.SetTile(new Vector3Int(x-1, y+1, 0), dungeonData.WallUpTile);
                            objectMap.SetTile(new Vector3Int(x-1, y+2, 0), dungeonData.WallUpTile);
                            if(dungeonData.WallUpTile.m_TilingRules.Count == 9)
                                objectMap.SetTile(new Vector3Int(x-1, y+3, 0), dungeonData.WallUpTile);
                        }
                    }
                }
            }
        }

        // キャラクター配置
        while(enemyParent.childCount > 0)
            GameObject.DestroyImmediate(enemyParent.GetChild(0).gameObject);
        GameObject obj = Instantiate(dungeonData.BossEnemyData.Prefab, new Vector2(12.5f, 12.5f), Quaternion.identity, enemyParent);
        obj.GetComponent<EnemyController>().Init(new Enemy(dungeonData.BossEnemyData));
        playerTra.position = new Vector2(12.5f, 9.5f);

        // 階段配置
        stairController.gameObject.SetActive(false);

        if(GameManager.I) StartCoroutine(EBossEvent());
    }
    private IEnumerator EBossEvent()
    {
        yield return new WaitUntil(() => enemyParent.childCount == 0);

        isClear = true;

        // 階段配置
        stairController.transform.position = new Vector2(12.5f, 12.5f);
        stairController.gameObject.SetActive(true);

        yield break;
    }
}