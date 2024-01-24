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

public class Player
{
    private PlayerData data = null;
    public PlayerData Data => data;

    // hp
    public int Hp => data.Hp;
    // 攻撃力
    public int Atk => data.Atk;

    // 現在のhp
    private int currentHp = 0;
    public int CurrentHp => currentHp;


    public Player(PlayerData data)
    {
        this.data = data;
    }

    public int UpdateHp(int para)
    {
        currentHp += para;

        if(currentHp < 0) currentHp = 0;
        else if(currentHp > Hp) currentHp = Hp;

        return para;
    }
}
