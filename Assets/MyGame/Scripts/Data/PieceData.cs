using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/PieceData")]
public class PieceData : ScriptableObject
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
    [SerializeField] // アイテムのタイプ
    private ItemType itemType = ItemType.Damage;
    public ItemType ItemType => itemType;
    [SerializeField] // 値
    private int value = 0;
    public int Value => value;
}

public class Piece
{
    private PieceData data = null;
    public PieceData Data => data;

    public Piece(PieceData data)
    {
        this.data = data;
    }
}
