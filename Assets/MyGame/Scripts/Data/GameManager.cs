using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : Singleton<GameManager>
{
    // 入力コンポーネント
    public PlayerInput Input { get; private set; } = null;
    [SerializeField] // データベース
    private DataBase dataBase = null;
    public DataBase DataBase => dataBase;

    // シングルトンのタイプ
    protected override Type type => Type.Destroy;

    protected override void OnAwake ()
    {
        Input = GetComponent<PlayerInput>();
    }
}
