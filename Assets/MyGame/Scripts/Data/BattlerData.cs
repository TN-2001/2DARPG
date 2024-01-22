using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlerData : BaseData
{
    [SerializeField] // hp
    private int hp = 0;
    public int Hp => hp;
    [SerializeField] // 攻撃力
    private int atk = 0;
    public int Atk => atk;
}

public class Battler : Base
{
    private BattlerData data = null;

    // hp
    public int Hp => data.Hp;
    // 攻撃力
    public int Atk => data.Atk;

    // 現在のhp
    private int currentHp = 0;
    public int CurrentHp => currentHp;


    public Battler(BattlerData data) : base(data)
    {
        this.data = data;
        currentHp = Hp;
    }

    public int UpdateHp(int para)
    {
        currentHp += para;

        if(currentHp < 0) currentHp = 0;
        else if(currentHp > Hp) currentHp = Hp;

        return para;
    }
}
