using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDetector : CollisionDetector
{
    protected override string TagName => "Player";

    // UIのタイプ
    private enum Type
    {
        None,
        Shop,
        Box,
        Dungeon,
    }
    [SerializeField]
    private Type type = Type.None;


    private void Update()
    {
        if(IsCollosion & GameManager.I.Input.actions["Do"].WasPerformedThisFrame())
        {
            if(type == Type.None)
            {

            }
            else if(type == Type.Shop)
            {
                HomeUI.I.OnShopView();
            }
            else if(type == Type.Box)
            {

            }
            else if(type == Type.Dungeon)
            {
                HomeUI.I.OnDungeonView();
            }
        }
    }
}
