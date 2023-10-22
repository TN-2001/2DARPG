using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/DataBase")]
public class DataBase : ScriptableObject
{
    [SerializeField]
    private CharacterData[] characterDatas = null;
    public CharacterData[] CharacterDatas => characterDatas;
}
