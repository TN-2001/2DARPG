using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    // シングルトンのタイプ
    protected override Type type => Type.DontDestroy;

    [SerializeField] // データベース
    private DataBase dataBase = null;
    public DataBase DataBase => dataBase;
    [SerializeField] // 現在のダンジョン
    private DungeonData currentDungeon = null;
    public DungeonData CurrentDungeon => currentDungeon;
    public enum State // ゲームの状態
    {
        UI,
        Player,
    }
    public State state = State.UI;


    public void InitDungeon(int number)
    {
        currentDungeon = dataBase.DungeonDataList[number];
    }
}
