using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData
{
    [SerializeField] // 名前
    private string name = null;
    public string Name => name;
    [SerializeField] // hp
    private int hp = 0;
    public int Hp => hp;
    [SerializeField] // 攻撃力
    private int atk = 0;
    public int Atk => atk;
    [SerializeField] // 攻撃
    private AttackData[] attackDatas = null;
    public AttackData[] AttackDatas => attackDatas;
}

[System.Serializable]
public class AttackData
{
    public enum Type // 攻撃タイプ
    {
        Direct,
        Move,
        Throw,
    }
    [SerializeField]
    private Type type = Type.Direct;
    public Type _Type => type;
    [SerializeField] // 攻撃可能距離
    private float area = 0;
    public float Area => area;
    [SerializeField] // 移動速度
    private float speed = 0;
    public float Speed => speed;
    [SerializeField] // 生存時間
    private float survivalTime = 0;
    public float SurvivalTime => survivalTime;
    [SerializeField] // ダメージ%
    private int damage = 0;
    public int Damage => damage;
}

[System.Serializable]
public class Character
{
    [SerializeField]
    private CharacterData data = null;

    public string Name => data.Name;
    public int Hp => data.Hp;
    public int Atk => data.Atk; 
    public AttackData[] AttackDatas => data.AttackDatas;
    // プレイヤーに接近し停止する距離
    public float CloseDistance
    {
        get
        {
            float closeDistance = 10;
            for(int i = 0; i < data.AttackDatas.Length; i++)
            {
                if(closeDistance > data.AttackDatas[i].Area)
                {
                    closeDistance = data.AttackDatas[i].Area;
                }
            }
            return closeDistance;
        }
    }

    [SerializeField]
    private int currentHp = 0;
    public int CurrentHp => currentHp;

    public Character(CharacterData _data)
    {
        data = _data;
        currentHp = Hp;
    }

    public int GetAttack(int number)
    {
        return Atk * AttackDatas[number].Damage / 100;
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
