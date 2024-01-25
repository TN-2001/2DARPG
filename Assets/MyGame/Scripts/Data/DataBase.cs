using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/DataBase")]
public class DataBase : ScriptableObject
{
    [SerializeField]
    private PlayerData playerData = null;
    public PlayerData PlayerData => playerData;
    [SerializeField]
    private List<EnemyData> enemyDataList = new List<EnemyData>();
    public List<EnemyData> EnemyDataList => enemyDataList;
    [SerializeField]
    private List<NpcData> npcDataList = new List<NpcData>();
    public List<NpcData> NpcDataList => npcDataList;
    [SerializeField]
    private List<ItemData> itemDataList = new List<ItemData>();
    public List<ItemData> ItemDataList => itemDataList;
    [SerializeField]
    private List<PieceData> pieceDataList = new List<PieceData>();
    public List<PieceData> PieceDataList => pieceDataList;
    [SerializeField]
    private List<WeaponData> weaponDataList = new List<WeaponData>();
    public List<WeaponData> WeaponDataList => weaponDataList;
    [SerializeField]
    private List<EventData> eventDataList = new List<EventData>();
    public List<EventData> EventDataList => eventDataList;
    [SerializeField]
    private List<DungeonData> dungeonDataList = new List<DungeonData>();
    public List<DungeonData> DungeonDataList => dungeonDataList;
}
