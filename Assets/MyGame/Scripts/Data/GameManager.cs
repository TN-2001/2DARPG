using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    // シングルトンのタイプ
    protected override Type type => Type.DontDestroy;

    [SerializeField] // データベース
    private DataBase dataBase = null;
    [SerializeField] // セーブデータ
    private SaveData data = null;
    public SaveData Data => data;
    [SerializeField] // 現在のダンジョン
    private DungeonData currentDungeon = null;
    public DungeonData CurrentDungeon => currentDungeon;


    protected override void OnAwake()
    {
        Load();
    }

    public void InitDungeon(int number)
    {
        currentDungeon = dataBase.DungeonDataList[number];
    }

    private void Load()
    {
        try{
            StreamReader rd = new StreamReader(Application.dataPath + "/SaveData.json");
            string json = rd.ReadToEnd();
            rd.Close();
            data = JsonUtility.FromJson<SaveData>(json);
        }catch{}
    }

    private void Save()
    {
        string json = JsonUtility.ToJson(data);
        StreamWriter wr = new StreamWriter(Application.dataPath + "/SaveData.json", false);
        wr.WriteLine(json);
        wr.Close(); 
    }

    public void Init()
    {
        data = new SaveData(dataBase);
        Save();
    }

    private void OnDestroy()
    {
        if(data.Player.Data != null) Save();
    }
}
