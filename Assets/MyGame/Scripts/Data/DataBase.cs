using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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

#if UNITY_EDITOR
public class DataBaseWindow : EditorWindow
{
    [MenuItem("Window/DataBase")]
    private static void Open()
    {
        var window = GetWindow<DataBaseWindow>("DataBase");
    }

    private void OnGUI()
    {
        
    }
}
#endif
