using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattlerData
{
    // hp
    public int Hp { get; }
    // 攻撃力
    public int Atk { get; }
    // プレハブ
    public GameObject Prefab { get; }
}

public interface IBattler
{
    // hp
    public int Hp { get; }
    // 攻撃力
    public int Atk { get; }

    // 現在のhp
    public int CurrentHp { get; }

    public int OnDamage(int damage);
}
