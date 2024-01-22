using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "ScriptableObject/DungeonData")]
public class DungeonData : BaseData
{
    [SerializeField] // 階数
    private int floorNumber = 0;
    public int FloorNumber => floorNumber;
    [SerializeField] // 敵
    private List<EnemyData> enemyDataList = new List<EnemyData>();
    public List<EnemyData> EnemyDataList => enemyDataList;
    [SerializeField] // ボス敵
    private EnemyData bossEnemyData = null;
    public EnemyData BossEnemyData => bossEnemyData;
    [SerializeField] // 階段画像
    private Sprite stairSprite = null;
    public Sprite StairSprite => stairSprite;
    [SerializeField] // 地面タイル
    private Tile groundTile = null;
    public Tile GroundTile => groundTile;
    [SerializeField] // 壁タイル
    private RuleTile wallTile = null;
    public RuleTile WallTile => wallTile;
    [SerializeField] // 壁上タイル
    private RuleTile wallUpTile = null;
    public RuleTile WallUpTile => wallUpTile;
    [SerializeField] // ボスマップ
    private GameObject bossMap = null;
    public GameObject BossMap => bossMap;
}
