using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class CreateStage : MonoBehaviour
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

    [SerializeField, Button("Initialize")]
    private bool initialize;

    [SerializeField] // ダンジョンデータ
    private DungeonData dungeonData = null;
    [SerializeField, ReadOnly] // ステージ情報
    private CreateStageData stageData = null;
    // ダンジョン階数
    private int floorNumber = 0;


    private void Start()
    {
        stairDetector.GetComponent<SpriteRenderer>().sprite = dungeonData.StairSprite;

        Initialize();
    }

    private void Update()
    {
        if(stairDetector.IsCollosion & GameManager.I.Input.actions["Attack"].WasPressedThisFrame())
        {
            if(floorNumber < dungeonData.FloorNumber)
            {
                GameManager.I.Fade(Initialize, true, null);
            }
            else
            {
                GameManager.I.Fade(delegate{SceneManager.LoadScene("Home");}, true, "Normal");
            }
        }
    }

    public void Initialize()
    {
        if(GameManager.I)
        {
            dungeonData = GameManager.I.CurrentDungeon;
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
        while(enemyParent.childCount > 0)
        {
            GameObject.DestroyImmediate(enemyParent.GetChild(0).gameObject);
        }
        int playerRoomNumber = Random.Range(0, stageData.RectList.Count);
        for(int i = 0; i < stageData.RectList.Count; i++)
        {
            CreateStageData.Rect.Room room = stageData.RectList[i]._Room;
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
        CreateStageData.Rect.Room sRoom = stageData.RectList[Random.Range(0, stageData.RectList.Count)]._Room;
        int xPos = Random.Range(sRoom.Left, sRoom.Right + 1);
        int yPos = Random.Range(sRoom.Down, sRoom.Up + 1);
        stairDetector.transform.position = new Vector2(xPos + 0.5f, yPos + 0.5f);

        // UI
        floorNumber ++;
        DungeonUI.I?.UpdateDungeonText(dungeonData.Name, floorNumber);
    }
}