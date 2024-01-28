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
    private List<WeaponData> weaponDataList = new List<WeaponData>();
    public List<WeaponData> WeaponDataList => weaponDataList;
    [SerializeField]
    private List<ArmorData> armorData = new List<ArmorData>();
    public List<ArmorData> ArmorDataList => armorData;
    [SerializeField]
    private List<EventData> eventDataList = new List<EventData>();
    public List<EventData> EventDataList => eventDataList;
    [SerializeField]
    private List<DungeonData> dungeonDataList = new List<DungeonData>();
    public List<DungeonData> DungeonDataList => dungeonDataList;
}

[System.Serializable]
public class SaveData
{
    [SerializeField]
    private Player player = null;
    public Player Player => player;
    [SerializeField]
    private List<Item> itemList = new List<Item>();
    public List<Item> ItemList => itemList;
    [SerializeField]
    private List<Armor> armorList = new List<Armor>();
    public List<Armor> ArmorList => armorList;
    [SerializeField]
    private List<Weapon> weaponList = new List<Weapon>();
    public List<Weapon> WeaponList => weaponList;

    [SerializeField]
    private List<EnemyData> enemyDataList = new List<EnemyData>();
    public List<EnemyData> EnemyDataList => enemyDataList;
    [SerializeField]
    private List<DungeonData> dungeonDataList = new List<DungeonData>();
    public List<DungeonData> DungeonDataList => dungeonDataList;


    public SaveData(DataBase data)
    {
        player = new Player(data.PlayerData, data);
        dungeonDataList.Add(data.DungeonDataList[0]);
    }

    public void UpdatePlayer(Player player)
    {
        this.player = player;
    }

    public void UpdateEnemyData(EnemyData enemyData)
    {
        for(int i = 0; i < enemyDataList.Count; i++)
        {
            if(enemyDataList[i] == enemyData) return;
        }

        enemyDataList.Add(enemyData);
    }
}
