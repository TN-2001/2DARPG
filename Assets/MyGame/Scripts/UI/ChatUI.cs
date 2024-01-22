using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatUI : Singleton<ChatUI>
{
    // シングルトンのタイプ
    protected override Type type => Type.Destroy;

    [SerializeField] // チャットウィンドウ
    private View chatView = null;
    [SerializeField] // 次へボタン
    private Button nextBtn = null;

    // チャットリスト
    private bool isNext = false;


    private void Start()
    {
        nextBtn.onClick.AddListener(delegate{isNext = true;});
    }

    public void Chat(List<(string name, string text)> chatList)
    {
        IChat(chatList);
    }
    private IEnumerator IChat(List<(string name, string text)> chatList)
    {
        GameManager.I.state = GameManager.State.UI;

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

        GameManager.I.state = GameManager.State.Player;
    }
}
