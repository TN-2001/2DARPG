using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : Singleton<UIManager>
{
    // 入力コンポーネント
    public PlayerInput Input { get; private set; } = null;
    [SerializeField] // GameUI
    private GameUI gameUI = null;
    public GameUI GameUI => gameUI;
    [SerializeField] // StartUI
    private StartUI startUI = null;
    public StartUI StartUI => startUI;

    // シングルトンのタイプ
    protected override Type type => Type.Destroy;


    protected override void OnAwake ()
    {
        Input = GetComponent<PlayerInput>();
    }

    public void ChangeUI(string type)
    {
        gameUI?.gameObject.SetActive(false);
        startUI?.gameObject.SetActive(false);

        if(type == "GameUI")
        {
            Input.SwitchCurrentActionMap("Player");
            gameUI.gameObject.SetActive(true);
        }
        else if(type == "StartUI")
        {
            Input.SwitchCurrentActionMap("UI");
            startUI.gameObject.SetActive(true);
        }
    }
}
