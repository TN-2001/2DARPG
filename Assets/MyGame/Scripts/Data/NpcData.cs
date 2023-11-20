using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/NpcData")]
public class NpcData : ScriptableObject
{
    [SerializeField] // 名前
    private new string name = null;
    public string Name => name;
}
