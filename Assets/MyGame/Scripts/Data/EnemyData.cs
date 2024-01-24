using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/EnemyData")]
public class EnemyData : ScriptableObject
{
    [SerializeField] // 名前
    private new string name = null;
    public string Name => name;
    [SerializeField, TextArea] // 情報
    private string info = null;
    public string Info => info;
    [SerializeField] // イメージ
    private Sprite image = null;
    public Sprite Image => image; 
    [SerializeField] // hp
    private int hp = 0;
    public int Hp => hp;
    [SerializeField] // 攻撃力
    private int atk = 0;
    public int Atk => atk;
    [SerializeField] // プレハブ
    private GameObject prefab = null;
    public GameObject Prefab => prefab;
}

public class Enemy
{
    private EnemyData data = null;
    public EnemyData Data => data;

    // hp
    public int Hp => data.Hp;
    // 攻撃力
    public int Atk => data.Atk;

    // 現在のhp
    private int currentHp = 0;
    public int CurrentHp => currentHp;


    public Enemy(EnemyData data)
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