using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/PlayerData")]
public class PlayerData : ScriptableObject
{
    [SerializeField] // hp
    private int hp = 0;
    public int Hp => hp;
    [SerializeField] // 攻撃力
    private int atk = 0;
    public int Atk => atk;
}

[System.Serializable]
public class Player
{
    [SerializeField]
    private PlayerData data = null;
    public PlayerData Data => data;

    // hp
    public int Hp{get{
        int hp = data.Hp;
        foreach(Armor armor in armorList){
            if(armor.Data) hp += armor.Hp;
        }
        return hp;
    }}
    // 現在のhp
    private int currentHp = 0;
    public int CurrentHp => currentHp;
    // 攻撃力
    public int Atk{get{
        int atk = data.Atk;
        if(weaponList[weaponNumber].Data)
            atk += weaponList[weaponNumber].Atk;
        return atk;
    }}
    [SerializeField] // 武器リスト
    private List<Weapon> weaponList = new List<Weapon>(){new Weapon(null), new Weapon(null), new Weapon(null), new Weapon(null)};
    public List<Weapon> WeaponList => weaponList.FindAll(x => x.Data);
    // 現在の武器番号
    private int weaponNumber = 0;
    public int WeaponNumber => weaponNumber;
    [SerializeField] // 防具リスト
    private List<Armor> armorList = new List<Armor>(){new Armor(null), new Armor(null), new Armor(null), new Armor(null)};
    public List<Armor> ArmorList => armorList;


    public Player(DataBase dataBase)
    {
        this.data = dataBase.PlayerData;
        weaponList[0] = new Weapon(dataBase.WeaponDataList[0]);
    }

    public void Init(DataBase dataBase)
    {
        this.data = dataBase.PlayerData;
        for(int i = 0; i < 4; i++){
            if(weaponList[i].Number > 0)
                weaponList[i].Init(dataBase.WeaponDataList.Find(x => x.Number == weaponList[i].Number));
        }
        for(int i = 0; i < 4; i++){
            if(armorList[i].Number > 0)
                armorList[i].Init(dataBase.ArmorDataList.Find(x => x.Number == armorList[i].Number));
        }
        currentHp = Hp;
    }

    public int UpdateHp(int para)
    {
        currentHp += para;

        if(currentHp < 0) currentHp = 0;
        else if(currentHp > Hp) currentHp = Hp;

        return para;
    }

    public void UpdateWeapon(Weapon weapon, int number)
    {
        weaponList[number] = weapon;
        weaponList = WeaponList;
        while(weaponList.Count < 4){
            weaponList.Add(new Weapon(null));
        }
    }

    public void UpdateCurrentWeapon(int number)
    {
        weaponNumber = number;
    }

    public void UpdateArmor(Armor armor, int number)
    {
        armorList[number] = armor;
    }
}
