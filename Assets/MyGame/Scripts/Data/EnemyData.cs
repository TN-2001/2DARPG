using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/EnemyData")]
public class EnemyData : ScriptableObject
{
    [SerializeField] // 名前
    private new string name = null;
    public string Name => name;
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

[System.Serializable]
public class Enemy
{
    [SerializeField]
    private EnemyData data = null;

    public string Name => data.Name;
    public int Hp => data.Hp;
    public int Atk => data.Atk; 

    [SerializeField]
    private int currentHp = 0;
    public int CurrentHp => currentHp;

    public Enemy(EnemyData _data)
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

    public int OnDamage(int damage)
    {
        currentHp -= damage;
        if(currentHp < 0)
        {
            currentHp = 0;
        }

        return damage;
    }
}