using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/PlayerData")]
public class PlayerData : BattlerData {}

public class Player : Battler
{
    private PlayerData data = null;


    public Player(PlayerData data) : base(data) {}
}
