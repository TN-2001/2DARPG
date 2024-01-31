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

    [SerializeField] // レベル
    private int lev = 1;
    public int Lev => lev;
    // 必要経験値
    public int Exp => lev * 100;
    [SerializeField] // 現在の経験値
    private int currentExp = 0;
    public int CurrentExp => currentExp;
    // hp
    public int Hp{get{
        int hp = data.Hp * lev;;
        foreach(Armor armor in armorList){
            if(armor != null) hp += armor.Hp;
        }
        return hp;
    }}
    // 現在のhp
    private int currentHp = 0;
    public int CurrentHp => currentHp;
    // 攻撃力
    public int Atk{get{
        int atk = data.Atk * lev;
        atk += weapon.Atk;
        return atk;
    }}
    [SerializeField] // 武器
    private Weapon weapon = null;
    public Weapon Weapon => weapon;
    [SerializeField] // 防具
    private List<Armor> armorList = new List<Armor>(){null, null, null, null};
    public List<Armor> ArmorList => armorList;


    public Player(PlayerData data, DataBase dataBase)
    {
        this.data = data;
        weapon = new Weapon(dataBase.WeaponDataList[0]);
        currentHp = Hp;
    }

    public void UpdateExp(int exp)
    {
        currentExp += exp;

        while(currentExp >= Exp)
        {
            currentExp = currentExp - Exp;
            lev ++;
        }
    }

    public int UpdateHp(int para)
    {
        currentHp += para;

        if(currentHp < 0) currentHp = 0;
        else if(currentHp > Hp) currentHp = Hp;

        return para;
    }

    public void UpdateWeapon(Weapon weapon)
    {
        this.weapon = weapon;
    }
}
