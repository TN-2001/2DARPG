using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData
{
    [SerializeField] // 番号
    private int number = 0;
    public int Number => number;
    [SerializeField] // 名前
    private string name = null;
    public string Name => name;
    [SerializeField] // hp
    private int hp = 0;
    public int Hp => hp;
    [SerializeField] // 攻撃力
    private int atk = 0;
    public int Atk => atk;
}

[System.Serializable]
public class Character
{
    [SerializeField]
    private CharacterData data = null;

    public int Number => data.Number;
    public string Name => data.Name;
    public int Hp => data.Hp;
    public int Atk => data.Atk; 

    [SerializeField]
    private int currentHp = 0;
    public int CurrentHp => currentHp;

    public Character(CharacterData _data)
    {
        data = _data;
        currentHp = Hp;
    }

    public void OnRecovery(int recovery)
    {
        currentHp += recovery;
        if(currentHp > Hp)
        {
            currentHp = Hp;
        }
    }

    public void OnDamage(int damage)
    {
        currentHp -= damage;
        if(currentHp < 0)
        {
            currentHp = 0;
        }
    }
}
