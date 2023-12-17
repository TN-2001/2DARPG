using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GameManager : Singleton<GameManager>
{
    // シングルトンのタイプ
    protected override Type type => Type.DontDestroy;

    // 入力コンポーネント
    public PlayerInput Input { get; private set; } = null;
    [SerializeField] // データベース
    private DataBase dataBase = null;
    public DataBase DataBase => dataBase;
    [SerializeField] // カメラコントローラー
    private CameraController cameraController = null;
    public CameraController CameraController => cameraController;

    [SerializeField] // フェード画像
    private Image fadeImage = null;
    
    [SerializeField] // 現在のダンジョン
    private DungeonData currentDungeon = null;
    public DungeonData CurrentDungeon => currentDungeon;


    protected override void OnAwake ()
    {
        Input = GetComponent<PlayerInput>();
    }

    public void InitializeDungeon(int number)
    {
        currentDungeon = dataBase.DungeonDataList[number];
    }

    public void Fade(UnityAction action, bool isStop, string inputMapName)
    {
        StartCoroutine(EFade(action, isStop, inputMapName));
    }
    private IEnumerator EFade(UnityAction action, bool isStop, string inputMapName)
    {
        // 時間停止
        if(isStop)
        {
            Time.timeScale = 0f;
        }
        
        // アクションマップ関係
        string newxtMapName = Input.currentActionMap.name;
        if(inputMapName != null)
        {
            newxtMapName = inputMapName;
        }
        Input.SwitchCurrentActionMap("Null");

        fadeImage.enabled = true;

        float alpha = 0;
        float preTime = 0;
        while(alpha < 1)
        {
            preTime = Time.realtimeSinceStartup;
            yield return null;
            alpha += Time.realtimeSinceStartup - preTime;
            fadeImage.color = new Color(0,0,0,alpha);
        }

        // 関数実行
        if(action != null)
        {
            action();
        }

        alpha = 1f;
        while(alpha > 0)
        {
            preTime = Time.realtimeSinceStartup;
            yield return null;
            alpha -= Time.realtimeSinceStartup - preTime;
            fadeImage.color = new Color(0,0,0,alpha);
        }

        fadeImage.enabled = false;

        // アクションマップ関係
        Input.SwitchCurrentActionMap(newxtMapName);

        // 時間再開
        if(isStop)
        {
            Time.timeScale = 1f;
        }

        yield break;
    }
}
