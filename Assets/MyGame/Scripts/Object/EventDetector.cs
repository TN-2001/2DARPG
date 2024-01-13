using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventController : MonoBehaviour
{
    public UnityEvent onDo = null;

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


    public void Do()
    {
        onDo?.Invoke();

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
