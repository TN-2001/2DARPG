using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventController : MonoBehaviour
{
    private enum Type // UIのタイプ
    {
        None,
        Shop,
        Box,
        Dungeon,
    }
    [SerializeField]
    private Type type = Type.None;

    // イベント
    public UnityEvent onDo = null;


    public void Do()
    {
        onDo?.Invoke();

        if(type == Type.None)
        {

        }
        else if(type == Type.Shop)
        {
            
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
