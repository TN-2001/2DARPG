using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatUI : Singleton<ChatUI>
{
    // シングルトンのタイプ
    protected override Type type => Type.Destroy;

    [SerializeField] // 名前テキスト
    private TextMeshProUGUI nameText = null;
    [SerializeField] // チャットテキスト
    private TextMeshProUGUI chatText = null;

    public void UpdateText(string name, string chat)
    {
        nameText.text = name;
        chatText.text = chat;
    }
}
