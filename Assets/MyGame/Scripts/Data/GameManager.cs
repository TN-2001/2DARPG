using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    // シングルトンのタイプ
    protected override Type type => Type.DontDestroy;

    // 入力コンポーネント
    public PlayerInput Input { get; private set; } = null;
    [SerializeField] // データベース
    private DataBase dataBase = null;
    public DataBase DataBase => dataBase;

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

    public void ChangeScene(string name)
    {
        StartCoroutine(Fade(name));
    }
    public void Fade()
    {
        StartCoroutine(Fade(""));
    }
    private IEnumerator Fade(string name)
    {
        float alpha = 0;

        fadeImage.enabled = true;

        while(alpha < 1)
        {
            alpha += 1f / 100f;
            fadeImage.color = new Color(0,0,0,alpha);
            yield return null;
        }

        if(name != "")
        {
            yield return SceneManager.LoadSceneAsync(name);
        }

        while(alpha > 0)
        {
            alpha -= 1f / 100f;
            fadeImage.color = new Color(0,0,0,alpha);
            yield return null;
        }

        fadeImage.enabled = false;
        yield return null;
    }
}
