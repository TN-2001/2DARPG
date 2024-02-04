using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/ItemData")]
public class ItemData : ScriptableObject
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
    [SerializeField] // 価値
    private int price = 0;
    public int Price => price;
    [SerializeField] // アイテムのタイプ
    private ItemType itemType = ItemType.Damage;
    public ItemType ItemType => itemType;
    [SerializeField] // 値
    private int value = 0;
    public int Value => value;
}

[System.Serializable]
public class Item
{
    private ItemData data = null;
    public ItemData Data => data;

    [SerializeField] // 番号
    private int number = 0;
    public int Number => number;
    [SerializeField] // 数
    private int count = 0;
    public int Count => count;


    public Item(ItemData data)
    {
        this.data = data;
        this.number = data.Number;
    }

    public void Init(ItemData data)
    {
        this.data = data;
    }

    public void UpadateCount(int para)
    {
        count += para;
    }
}

public enum ItemType
{
    Damage,
    Recovery,
}