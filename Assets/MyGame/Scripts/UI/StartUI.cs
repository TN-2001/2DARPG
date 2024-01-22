using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class StartUI : MonoBehaviour
{
    [SerializeField] // スタートボタン
    private Button startButton = null;
    [SerializeField] // スタートテキスト
    private TextMeshProUGUI startText = null;


    private void Start()
    {
        startButton.onClick.AddListener(delegate{
            startButton.interactable = false;
            startText.text = "読み込み中";
            FadeUI.I.FadeIn(delegate{SceneManager.LoadScene("Home");});
        });
    }
}
