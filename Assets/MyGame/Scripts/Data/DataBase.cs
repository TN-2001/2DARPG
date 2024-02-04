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
    private int money = 1000;
    public int Money => money;
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
    private List<bool> isFindEnemyList = new List<bool>();
    public List<bool> IsFindEnemyList => isFindEnemyList;
    [SerializeField]
    private int currentDungeonNumber = 0;
    public int CurrentDungeonNumber => currentDungeonNumber;


    public SaveData(DataBase data)
    {
        player = new Player(data);
        while(isFindEnemyList.Count < data.EnemyDataList.Count){
            isFindEnemyList.Add(false);
        }
    }

    public void Init(DataBase data)
    {
        player.Init(data);
        for(int i = 0; i < itemList.Count; i++){
            itemList[i].Init(data.ItemDataList.Find(x => x.Number == itemList[i].Number));
        }
        for(int i = 0; i < armorList.Count; i++){
            armorList[i].Init(data.ArmorDataList.Find(x => x.Number == armorList[i].Number));
        }
        for(int i = 0; i < weaponList.Count; i++){
            weaponList[i].Init(data.WeaponDataList.Find(x => x.Number == weaponList[i].Number));
        }
        while(isFindEnemyList.Count < data.EnemyDataList.Count){
            isFindEnemyList.Add(false);
        }
    }

    public void UpdateMoney(int money)
    {
        this.money += money;
    }

    public void UpdateWeapon(Weapon weapon, int playerNumber)
    {
        if(Player.WeaponList[playerNumber].Data)
            weaponList.Add(Player.WeaponList[playerNumber]);
        
        if(weapon != null){
            Player.UpdateWeapon(weapon, playerNumber);
            weaponList.Remove(weapon);
        }
        else{
            Player.UpdateWeapon(new Weapon(null), playerNumber);
        }
    }

    public void UpdateArmor(Armor armor, int playerNumber)
    {
        if(Player.ArmorList[playerNumber].Data)
            armorList.Add(Player.ArmorList[playerNumber]);
        
        if(armor != null){
            Player.UpdateArmor(armor, playerNumber);
            armorList.Remove(armor);
        }
        else{
            Player.UpdateArmor(new Armor(null), playerNumber);
        }
    }

    public void UpdateFindEnemy(int number)
    {
        isFindEnemyList[number] = true;
    }

    public void UpdateDungeonNumber()
    {
        currentDungeonNumber ++;
    }
}
