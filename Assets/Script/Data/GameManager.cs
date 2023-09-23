using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : Singleton<GameManager>
{
    // コンポーネント
    public PlayerInput Input { get; private set; } = null;

    protected override void Awake ()
    {
        base.Awake();

        // コンポーネントの入手
        Input = GetComponent<PlayerInput>();
    }
}
