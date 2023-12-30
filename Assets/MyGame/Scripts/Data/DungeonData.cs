using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/DungeonData")]
public class DungeonData : ScriptableObject
{
    [SerializeField] // 名前
    private new string name = null;
    public string Name => name;
    [SerializeField] // 階数
    private int floorNumber = 0;
    public int FloorNumber => floorNumber;
    [SerializeField] // 敵
    private List<EnemyData> enemyDataList = new List<EnemyData>();
    public List<EnemyData> EnemyDataList => enemyDataList;
    [SerializeField] // 階段画像
    private Sprite stairSprite = null;
    public Sprite StairSprite => stairSprite;
    [SerializeField] // 地面タイル
    private RuleTile groundTile = null;
    public RuleTile GroundTile => groundTile;
    [SerializeField] // 壁タイル
    private RuleTile wallTile = null;
    public RuleTile WallTile => wallTile;
    [SerializeField] // 壁上タイル
    private RuleTile wallUpTile = null;
    public RuleTile WallUpTile => wallUpTile;
}
