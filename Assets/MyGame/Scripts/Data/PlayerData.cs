using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/PlayerData")]
public class PlayerData : ScriptableObject
{
    [SerializeField] // hp
    private int hp = 0;
    public int Hp => hp;
    [SerializeField] // 攻撃力
    private int atk = 0;
    public int Atk => atk;
}

[System.Serializable]
public class Player
{
    [SerializeField]
    private PlayerData data = null;
    public PlayerData Data => data;

    [SerializeField] // プレイヤーアイテム
    private List<PlayerItem> playerItemList = new List<PlayerItem>(4){
        new PlayerItem(), new PlayerItem(), new PlayerItem(), new PlayerItem()};
    public List<PlayerItem> PlayerItemList => playerItemList;
    [SerializeField] // 武器
    private Weapon weapon = null;
    public Weapon Weapon => weapon;

    // hp
    public int Hp => data.Hp;
    // 攻撃力
    public int Atk => data.Atk;

    // 現在のhp
    private int currentHp = 0;
    public int CurrentHp => currentHp;


    public Player(PlayerData data)
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

[System.Serializable]
public class PlayerItem
{
    // アイテム（10個まで）
    public Item Itemt = null;
    // かけら
    public Piece piece = null;
}
