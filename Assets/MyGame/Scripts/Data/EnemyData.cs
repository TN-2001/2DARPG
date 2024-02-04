using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/EnemyData")]
public class EnemyData : ScriptableObject
{
    [SerializeField] // 番号
    private int number = 0;
    public int Number => number;
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
    [SerializeField] // アイテム
    private List<ItemData> itemDataList = null;
    public List<ItemData> ItemDataList => itemDataList;
    [SerializeField] // プレハブ
    private GameObject prefab = null;
    public GameObject Prefab => prefab;
}

public class Enemy
{
    private EnemyData data = null;
    public EnemyData Data => data;

    // レベル
    private int lev = 1;
    public int Lev => lev;
    // hp
    public int Hp => data.Hp * lev;
    // 攻撃力
    public int Atk => data.Atk * lev;
    // 現在のhp
    private int currentHp = 0;
    public int CurrentHp => currentHp;


    public Enemy(EnemyData data, int lev)
    {
        this.data = data;
        this.lev = lev;
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