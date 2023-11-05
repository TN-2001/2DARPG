using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] // データベース
    private DataBase dataBase = null;
    public DataBase DataBase => dataBase;

    // シングルトンのタイプ
    protected override Type type => Type.DontDestroy;
}
