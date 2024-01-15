using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GameManager : Singleton<GameManager>
{
    // シングルトンのタイプ
    protected override Type type => Type.DontDestroy;

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

    // ゲームの状態
    public enum State
    {
        UI,
        Player,
    }
    public State state = State.UI;

    [SerializeField, ReadOnly] // 現在の選択状態
    private GameObject currentSelectObject = null;


    public void InitializeDungeon(int number)
    {
        currentDungeon = dataBase.DungeonDataList[number];
    }

    public void Fade(UnityAction action, bool isStop)
    {
        StartCoroutine(EFade(action, isStop));
    }
    private IEnumerator EFade(UnityAction action, bool isStop)
    {
        state = State.UI;

        // 時間停止
        if(isStop)
        {
            Time.timeScale = 0f;
        }

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

        // 時間再開
        if(isStop)
        {
            Time.timeScale = 1f;
        }

        state = State.Player;

        yield break;
    }


    private void Update()
    {
        currentSelectObject = EventSystem.current.currentSelectedGameObject;
    }
}
