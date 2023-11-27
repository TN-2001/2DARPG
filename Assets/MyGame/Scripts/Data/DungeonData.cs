using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "ScriptableObject/DungeonData")]
public class DungeonData : ScriptableObject
{
    [SerializeField] // 名前
    private new string name = null;
    public string Name => name;
    [SerializeField] // 敵
    private List<EnemyData> enemyDataList = new List<EnemyData>();
    public List<EnemyData> EnemyDataList => enemyDataList;
    [SerializeField] // 地面タイル
    private Tile groundTile = null;
    public Tile GroundTile => groundTile;
    [SerializeField] // 壁タイル
    private Tile wallTile = null;
    public Tile WallTile => wallTile;
}
