using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/WeaponData")]
public class WeaponData : ScriptableObject
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
    [SerializeField] // 価値
    private int price = 0;
    public int Price => price;
    [SerializeField] // 作製に必要なアイテム
    private List<ItemData> itemDataList = new List<ItemData>();
    public List<ItemData> ItemDataList => itemDataList;
    [SerializeField] // アイテムのタイプ
    private WeaponType weaponType = WeaponType.Sword;
    public WeaponType WeaponType => weaponType;
    [SerializeField] // 値
    private int value = 0;
    public int Value => value;
}

public class Weapon
{
    private WeaponData data = null;
    public WeaponData Data => data;

    public Weapon(WeaponData data)
    {
        this.data = data;
    }
}

public enum WeaponType
{
    Sword,
}
