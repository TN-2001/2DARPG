using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ChatUI : MonoBehaviour
{
    public static ChatUI I = null;

    [SerializeField] // チャットウィンドウ
    private View chatView = null;
    [SerializeField] // 次へボタン
    private Button nextBtn = null;

    // チャットリスト
    private bool isNext = false;


    private void Awake()
    {
        I = this;
    }

    private void Start()
    {
        nextBtn.onClick.AddListener(delegate{isNext = true;});
    }

    public void Chat(List<(string name, string text)> chatList, UnityAction action)
    {
        IChat(chatList, action);
    }
    private IEnumerator IChat(List<(string name, string text)> chatList, UnityAction action)
    {
        // 初期化
        chatView.gameObject.SetActive(true);

        // UIの更新
        for(int i = 0; i < chatList.Count; i++)
        {
            chatView.UpdateUI(new List<string>(){chatList[i].name, chatList[i].text});
            yield return new WaitUntil(() => isNext);
            isNext = false;
        }

        // 初期化
        chatView.gameObject.SetActive(false);

        // イベント実行
        if(action != null) action();
    }
}
