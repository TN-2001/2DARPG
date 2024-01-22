using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/EnemyData")]
public class EnemyData : BattlerData
{
    [SerializeField] // プレハブ
    private GameObject prefab = null;
    public GameObject Prefab => prefab;
}

public class Enemy : Battler
{
    private EnemyData data = null;


    public Enemy(EnemyData data) : base(data) {}
}