using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatUI : Singleton<ChatUI>
{
    // シングルトンのタイプ
    protected override Type type => Type.Destroy;

    [SerializeField] // 名前ウィンドウ
    private GameObject nameWindow = null;
    [SerializeField] // チャットウィンドウ
    private GameObject chatWindow = null;
    [SerializeField] // 名前テキスト
    private TextMeshProUGUI nameText = null;
    [SerializeField] // チャットテキスト
    private TextMeshProUGUI chatText = null;


    public void Chat(List<(string name, string chat)> textList)
    {
        StartCoroutine(IChat(textList));
    }
    private IEnumerator IChat(List<(string name, string chat)> textList)
    {
        GameManager.I.state = GameManager.State.UI;
        // 初期化
        HomeUI.I?.gameObject.SetActive(false);
        DungeonUI.I?.gameObject.SetActive(false);
        nameWindow.SetActive(true);
        chatWindow.SetActive(true);

        // UIの更新
        for(int i = 0; i < textList.Count; i++)
        {
            nameText.text = textList[i].name;
            chatText.text = textList[i].chat;
        }

        // 初期化
        HomeUI.I?.gameObject.SetActive(true);
        DungeonUI.I?.gameObject.SetActive(true);
        nameWindow.SetActive(false);
        chatWindow.SetActive(false);

        GameManager.I.state = GameManager.State.Player;

        yield break;
    }
}
